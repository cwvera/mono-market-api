using Microsoft.Extensions.Logging;
using MonoMarket.Application.Common.Email;
using MonoMarket.Application.InvoiceReminders.EmailTemplates;
using MonoMarket.Application.SendMails.Repositories;
using MonoMarket.Commons.Enums;
using MonoMarket.Domain.Entities;

namespace MonoMarket.Application.InvoiceReminders;

/// <summary>
/// Implementación del despachador: arma el contenido con la Abstract Factory de plantillas, deja la
/// cabecera en SendMails y delega el envío (con reintentos y log de intentos) al adapter de correo.
/// Si el envío final falla, se registra el error pero no se relanza, para no tumbar el resto del lote del job.
/// </summary>
public class InvoiceReminderEmailDispatcher(
    IInvoiceReminderEmailTemplateFactory templateFactory,
    ISendMailRepository sendMailRepository,
    IEmailSender emailSender,
    ILogger<InvoiceReminderEmailDispatcher> logger) : IInvoiceReminderEmailDispatcher
{
    /// <inheritdoc />
    public async Task<bool> DispatchAsync(InvoiceReminderNotice notice, CancellationToken cancellationToken)
    {
        if (notice.TemplateType is null)
        {
            return true;
        }

        InvoiceReminderCandidate candidate = notice.Candidate;
        IInvoiceReminderEmailTemplate template = templateFactory.GetTemplate(notice.TemplateType.Value);
        EmailContent content = template.Build(notice);

        SendMail sendMail = new()
        {
            InvoiceNumber = candidate.InvoiceNumber,
            ToEmail = candidate.ClientEmail,
            ToName = candidate.ClientName,
            TemplateType = notice.TemplateType.Value,
            Subject = content.Subject,
            Status = SendMailStatus.Pending,
            Data = $"Factura {candidate.InvoiceNumber} | {notice.FromStatus} -> {notice.NextStatus}",
        };

        string sendMailId = await sendMailRepository.CreateAsync(sendMail, cancellationToken);

        EmailMessage message = new()
        {
            SendMailId = sendMailId,
            ToEmail = candidate.ClientEmail,
            ToName = candidate.ClientName,
            Subject = content.Subject,
            Body = content.Body,
        };

        try
        {
            await emailSender.SendAsync(message, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "No se pudo enviar el correo de recordatorio para la factura {InvoiceNumber} a {ToEmail}.",
                candidate.InvoiceNumber,
                candidate.ClientEmail);
            return false;
        }
    }
}
