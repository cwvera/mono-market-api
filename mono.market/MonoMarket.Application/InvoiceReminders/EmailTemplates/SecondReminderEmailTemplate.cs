using System.Globalization;
using MonoMarket.Commons.Enums;

namespace MonoMarket.Application.InvoiceReminders.EmailTemplates;

/// <summary>
/// Plantilla para informar al cliente que su factura pasó de PrimerRecordatorio a SegundoRecordatorio.
/// </summary>
public class SecondReminderEmailTemplate : IInvoiceReminderEmailTemplate
{
    /// <inheritdoc />
    public SendMailTemplateType TemplateType => SendMailTemplateType.SecondReminder;

    /// <inheritdoc />
    public EmailContent Build(InvoiceReminderNotice notice)
    {
        InvoiceReminderCandidate candidate = notice.Candidate;
        string amount = candidate.Amount.ToString("C", CultureInfo.GetCultureInfo("es-CO"));

        return new EmailContent
        {
            Subject = $"Segundo recordatorio de pago - Factura {candidate.InvoiceNumber}",
            Body =
                $"<p>Estimado(a) {candidate.ClientName},</p>" +
                $"<p>Le informamos que la factura <strong>{candidate.InvoiceNumber}</strong> por un valor de {amount}, " +
                $"emitida el {candidate.IssueDate:yyyy-MM-dd}, sigue pendiente de pago y ha pasado a estado de " +
                "<strong>Segundo Recordatorio</strong>.</p>" +
                "<p>Por favor regularice su pago a la brevedad posible.</p>" +
                $"<p><small>Mensaje generado automáticamente el {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC.</small></p>",
        };
    }
}
