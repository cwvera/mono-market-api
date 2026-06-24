using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MonoMarket.Application.Common.Email;
using MonoMarket.Application.SendMails.Repositories;
using MonoMarket.Commons.Enums;
using MonoMarket.Domain.Entities;
using MonoMarket.Infrastructure.Email;

namespace MonoMarket.Tests.Infrastructure.Email;

/// <summary>
/// Pruebas del decorador resiliente: reintentos con Polly, log de cada intento y cierre de la cabecera.
/// </summary>
public class ResilientEmailSenderTests
{
    private readonly Mock<IEmailSender> _innerSenderMock = new();
    private readonly Mock<ISendMailRepository> _sendMailRepositoryMock = new();
    private readonly Mock<ISendMailLogRepository> _sendMailLogRepositoryMock = new();
    private readonly EmailRetrySettings _settings = new() { MaxAttempts = 3, BaseDelay = TimeSpan.Zero };

    private ResilientEmailSender CreateSender() => new(
        _innerSenderMock.Object,
        _sendMailRepositoryMock.Object,
        _sendMailLogRepositoryMock.Object,
        _settings,
        NullLogger<ResilientEmailSender>.Instance);

    private static EmailMessage CreateMessage() => new() { SendMailId = "sendmail-1", ToEmail = "contacto@alpha.com" };

    /// <summary>
    /// Verifica que, si el primer intento ya funciona, se registre un solo intento Sent y la cabecera quede Sent.
    /// </summary>
    [Fact]
    public async Task SendAsync_FirstAttemptSucceeds_LogsOneAttemptAndMarksHeaderSent()
    {
        _innerSenderMock.Setup(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        ResilientEmailSender sender = CreateSender();

        await sender.SendAsync(CreateMessage(), CancellationToken.None);

        _sendMailLogRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SendMailLog>(), It.IsAny<CancellationToken>()), Times.Once);
        _sendMailLogRepositoryMock.Verify(
            r => r.AddAsync(It.Is<SendMailLog>(l => l.AttemptNumber == 1 && l.Status == SendMailLogStatus.Sent), It.IsAny<CancellationToken>()),
            Times.Once);
        _sendMailRepositoryMock.Verify(
            r => r.UpdateStatusAsync("sendmail-1", SendMailStatus.Sent, 1, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica que, si fallan los primeros dos intentos y el tercero funciona, se registren 3 intentos
    /// (2 Failed + 1 Sent) y la cabecera quede Sent con 3 intentos totales.
    /// </summary>
    [Fact]
    public async Task SendAsync_FailsTwiceThenSucceeds_LogsThreeAttemptsAndMarksHeaderSent()
    {
        _innerSenderMock
            .SetupSequence(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("falla 1"))
            .ThrowsAsync(new InvalidOperationException("falla 2"))
            .Returns(Task.CompletedTask);
        ResilientEmailSender sender = CreateSender();

        await sender.SendAsync(CreateMessage(), CancellationToken.None);

        _sendMailLogRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SendMailLog>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _sendMailLogRepositoryMock.Verify(
            r => r.AddAsync(It.Is<SendMailLog>(l => l.AttemptNumber == 1 && l.Status == SendMailLogStatus.Failed), It.IsAny<CancellationToken>()),
            Times.Once);
        _sendMailLogRepositoryMock.Verify(
            r => r.AddAsync(It.Is<SendMailLog>(l => l.AttemptNumber == 2 && l.Status == SendMailLogStatus.Failed), It.IsAny<CancellationToken>()),
            Times.Once);
        _sendMailLogRepositoryMock.Verify(
            r => r.AddAsync(It.Is<SendMailLog>(l => l.AttemptNumber == 3 && l.Status == SendMailLogStatus.Sent), It.IsAny<CancellationToken>()),
            Times.Once);
        _sendMailRepositoryMock.Verify(
            r => r.UpdateStatusAsync("sendmail-1", SendMailStatus.Sent, 3, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifica que, si fallan los 3 intentos, se registren 3 Failed, la cabecera quede MaxRetries
    /// y la excepción se relance (para que el dispatcher decida qué hacer).
    /// </summary>
    [Fact]
    public async Task SendAsync_AllAttemptsFail_LogsThreeFailedAttemptsMarksHeaderMaxRetriesAndRethrows()
    {
        _innerSenderMock
            .Setup(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SMTP no disponible"));
        ResilientEmailSender sender = CreateSender();

        await Assert.ThrowsAsync<InvalidOperationException>(() => sender.SendAsync(CreateMessage(), CancellationToken.None));

        _sendMailLogRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SendMailLog>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _sendMailLogRepositoryMock.Verify(
            r => r.AddAsync(It.Is<SendMailLog>(l => l.Status == SendMailLogStatus.Failed), It.IsAny<CancellationToken>()),
            Times.Exactly(3));
        _sendMailRepositoryMock.Verify(
            r => r.UpdateStatusAsync("sendmail-1", SendMailStatus.MaxRetries, 3, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
