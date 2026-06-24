using System.Globalization;
using MonoMarket.Commons.Enums;

namespace MonoMarket.Application.InvoiceReminders.EmailTemplates;

/// <summary>
/// Plantilla para informar al cliente que su factura, tras dos recordatorios, será desactivada.
/// </summary>
public class DeactivationEmailTemplate : IInvoiceReminderEmailTemplate
{
    /// <inheritdoc />
    public SendMailTemplateType TemplateType => SendMailTemplateType.Deactivation;

    /// <inheritdoc />
    public EmailContent Build(InvoiceReminderNotice notice)
    {
        InvoiceReminderCandidate candidate = notice.Candidate;
        string amount = candidate.Amount.ToString("C", CultureInfo.GetCultureInfo("es-CO"));

        return new EmailContent
        {
            Subject = $"Aviso de desactivación - Factura {candidate.InvoiceNumber}",
            Body =
                $"<p>Estimado(a) {candidate.ClientName},</p>" +
                $"<p>La factura <strong>{candidate.InvoiceNumber}</strong> por un valor de {amount}, " +
                $"emitida el {candidate.IssueDate:yyyy-MM-dd}, no ha sido pagada después de dos recordatorios y será " +
                "<strong>desactivada</strong>.</p>" +
                "<p>Por favor contáctenos para regularizar su situación.</p>" +
                $"<p><small>Mensaje generado automáticamente el {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC.</small></p>",
        };
    }
}
