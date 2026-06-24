using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MonoMarket.Application.Common.Email;
using MonoMarket.Application.InvoiceReminders;
using MonoMarket.Application.InvoiceReminders.EmailTemplates;
using MonoMarket.Application.SendMails.Repositories;
using MonoMarket.Commons.Enums;
using MonoMarket.Domain.Entities;

namespace MonoMarket.Tests.Application.InvoiceReminders;

/// <summary>
/// Pruebas del despachador de correos de recordatorio.
/// </summary>
public class InvoiceReminderEmailDispatcherTests
{
    private readonly Mock<IInvoiceReminderEmailTemplateFactory> _templateFactoryMock = new();
    private readonly Mock<ISendMailRepository> _sendMailRepositoryMock = new();
    private readonly Mock<IEmailSender> _emailSenderMock = new();

    private InvoiceReminderEmailDispatcher CreateDispatcher() => new(
        _templateFactoryMock.Object,
        _sendMailRepositoryMock.Object,
        _emailSenderMock.Object,
        NullLogger<InvoiceReminderEmailDispatcher>.Instance);

    private static InvoiceReminderNotice CreateNotice(SendMailTemplateType? templateType) => new()
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
        TemplateType = templateType,
    };

    /// <summary>
    /// Verifica que, sin plantilla (transición silenciosa), no se cree cabecera ni se envíe nada,
    /// y que igualmente se devuelva true (la transición puede avanzar sin depender de un correo).
    /// </summary>
    [Fact]
    public async Task DispatchAsync_WithoutTemplateType_DoesNothingAndReturnsTrue()
    {
        InvoiceReminderEmailDispatcher dispatcher = CreateDispatcher();

        bool result = await dispatcher.DispatchAsync(CreateNotice(null), CancellationToken.None);

        Assert.True(result);
        _sendMailRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<SendMail>(), It.IsAny<CancellationToken>()), Times.Never);
        _emailSenderMock.Verify(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Verifica que, con plantilla, se cree la cabecera, se envíe el correo con el contenido de la
    /// plantilla y se devuelva true al tener éxito el envío.
    /// </summary>
    [Fact]
    public async Task DispatchAsync_WithTemplateType_CreatesHeaderSendsEmailAndReturnsTrue()
    {
        InvoiceReminderNotice notice = CreateNotice(SendMailTemplateType.SecondReminder);
        EmailContent content = new() { Subject = "Asunto de prueba", Body = "Cuerpo de prueba" };
        Mock<IInvoiceReminderEmailTemplate> templateMock = new();
        templateMock.Setup(t => t.Build(notice)).Returns(content);
        _templateFactoryMock.Setup(f => f.GetTemplate(SendMailTemplateType.SecondReminder)).Returns(templateMock.Object);
        _sendMailRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<SendMail>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("sendmail-id-1");
        InvoiceReminderEmailDispatcher dispatcher = CreateDispatcher();

        bool result = await dispatcher.DispatchAsync(notice, CancellationToken.None);

        Assert.True(result);
        _sendMailRepositoryMock.Verify(
            r => r.CreateAsync(
                It.Is<SendMail>(m =>
                    m.InvoiceNumber == notice.Candidate.InvoiceNumber &&
                    m.ToEmail == notice.Candidate.ClientEmail &&
                    m.Subject == content.Subject),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _emailSenderMock.Verify(
            s => s.SendAsync(
                It.Is<EmailMessage>(m => m.SendMailId == "sendmail-id-1" && m.ToEmail == notice.Candidate.ClientEmail && m.Body == content.Body),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica que, si el envío final falla (reintentos agotados), la excepción no se propague
    /// y se devuelva false (para que no se persista el avance de estado de la factura).
    /// </summary>
    [Fact]
    public async Task DispatchAsync_WhenSendFails_DoesNotThrowAndReturnsFalse()
    {
        InvoiceReminderNotice notice = CreateNotice(SendMailTemplateType.Deactivation);
        Mock<IInvoiceReminderEmailTemplate> templateMock = new();
        templateMock.Setup(t => t.Build(notice)).Returns(new EmailContent());
        _templateFactoryMock.Setup(f => f.GetTemplate(SendMailTemplateType.Deactivation)).Returns(templateMock.Object);
        _sendMailRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<SendMail>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("sendmail-id-2");
        _emailSenderMock
            .Setup(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SMTP no disponible"));
        InvoiceReminderEmailDispatcher dispatcher = CreateDispatcher();

        bool result = await dispatcher.DispatchAsync(notice, CancellationToken.None);

        Assert.False(result);
    }
}
