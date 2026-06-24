using MonoMarket.Application.InvoiceReminders;
using MonoMarket.Application.InvoiceReminders.EmailTemplates;
using MonoMarket.Commons.Enums;

namespace MonoMarket.Tests.Application.InvoiceReminders.EmailTemplates;

/// <summary>
/// Pruebas de las plantillas de correo: cada una debe declarar su tipo y mencionar los datos de la factura.
/// </summary>
public class InvoiceReminderEmailTemplateTests
{
    private static InvoiceReminderNotice CreateNotice() => new()
    {
        Candidate = new InvoiceReminderCandidate
        {
            InvoiceNumber = "FAC-001-2025",
            ClientIdentification = "900123456-7",
            ClientName = "Empresa Alpha SAS",
            ClientEmail = "contacto@alpha.com",
            Amount = 1500000m,
            IssueDate = new DateTime(2025, 2, 15, 0, 0, 0, DateTimeKind.Utc),
        },
        FromStatus = InvoiceStatus.FirstReminder,
        NextStatus = InvoiceStatus.SecondReminder,
        TemplateType = SendMailTemplateType.SecondReminder,
    };

    /// <summary>
    /// Verifica que SecondReminderEmailTemplate declare su tipo y mencione la factura y al cliente.
    /// </summary>
    [Fact]
    public void SecondReminderEmailTemplate_Build_MentionsInvoiceAndClient()
    {
        SecondReminderEmailTemplate template = new();
        InvoiceReminderNotice notice = CreateNotice();

        Assert.Equal(SendMailTemplateType.SecondReminder, template.TemplateType);

        EmailContent content = template.Build(notice);

        Assert.Contains(notice.Candidate.InvoiceNumber, content.Subject);
        Assert.Contains(notice.Candidate.ClientName, content.Body);
        Assert.Contains(notice.Candidate.InvoiceNumber, content.Body);
        Assert.Contains("Mensaje generado automáticamente", content.Body);
    }

    /// <summary>
    /// Verifica que DeactivationEmailTemplate declare su tipo y mencione la factura y al cliente.
    /// </summary>
    [Fact]
    public void DeactivationEmailTemplate_Build_MentionsInvoiceAndClient()
    {
        DeactivationEmailTemplate template = new();
        InvoiceReminderNotice notice = CreateNotice();

        Assert.Equal(SendMailTemplateType.Deactivation, template.TemplateType);

        EmailContent content = template.Build(notice);

        Assert.Contains(notice.Candidate.InvoiceNumber, content.Subject);
        Assert.Contains(notice.Candidate.ClientName, content.Body);
        Assert.Contains(notice.Candidate.InvoiceNumber, content.Body);
        Assert.Contains("Mensaje generado automáticamente", content.Body);
    }
}
