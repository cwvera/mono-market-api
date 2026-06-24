using Microsoft.Extensions.Logging;
using MonoMarket.Application.Clients.Repositories;
using MonoMarket.Application.InvoiceReminders.StageHandlers;
using MonoMarket.Application.Invoices.Repositories;
using MonoMarket.Domain.Entities;

namespace MonoMarket.Application.InvoiceReminders;

/// <summary>
/// Implementación del procesador de avisos: pide el handler a la factory, valida el plazo,
/// busca el cliente, deja el aviso en el log y despacha el correo. Si la transición no dependía de
/// un correo (silenciosa) o el correo se envió con éxito, persiste el avance de estado de la factura.
/// </summary>
public class InvoiceReminderNoticeProcessor(
    IInvoiceReminderStageHandlerFactory stageHandlerFactory,
    IClientRepository clientRepository,
    IInvoiceReminderEmailDispatcher emailDispatcher,
    IInvoiceRepository invoiceRepository,
    ILogger<InvoiceReminderNoticeProcessor> logger) : IInvoiceReminderNoticeProcessor
{
    /// <inheritdoc />
    public async Task ProcessAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        IInvoiceReminderStageHandler? handler = stageHandlerFactory.GetHandler(invoice.Status);
        if (handler is null)
        {
            return;
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (!handler.IsDue(DateOnly.FromDateTime(invoice.IssueDate), today))
        {
            return;
        }

        Client? client = await clientRepository.GetByIdentificationAsync(invoice.ClientIdentification, cancellationToken);
        if (client is null)
        {
            logger.LogWarning(
                "No se encontró el cliente {ClientIdentification} para la factura {InvoiceNumber}.",
                invoice.ClientIdentification,
                invoice.InvoiceNumber);
            return;
        }

        InvoiceReminderCandidate candidate = new()
        {
            InvoiceNumber = invoice.InvoiceNumber,
            ClientIdentification = invoice.ClientIdentification,
            ClientName = client.Name,
            ClientEmail = client.Email,
            Amount = invoice.Amount,
            IssueDate = invoice.IssueDate,
        };

        InvoiceReminderNotice notice = handler.BuildNotice(candidate);

        logger.LogInformation(
            "[AVISO] Factura {InvoiceNumber} | Cliente {ClientName} <{ClientEmail}> | Monto {Amount} | Emitida {IssueDate:yyyy-MM-dd} | {FromStatus} -> {NextStatus} | Plantilla: {TemplateType}",
            notice.Candidate.InvoiceNumber,
            notice.Candidate.ClientName,
            notice.Candidate.ClientEmail,
            notice.Candidate.Amount,
            notice.Candidate.IssueDate,
            notice.FromStatus,
            notice.NextStatus,
            notice.TemplateType?.ToString() ?? "ninguna (transición silenciosa)");

        bool canAdvance = await emailDispatcher.DispatchAsync(notice, cancellationToken);
        if (canAdvance)
        {
            await invoiceRepository.UpdateStatusAsync(invoice.Id, (int)notice.NextStatus, cancellationToken);
        }
    }
}
