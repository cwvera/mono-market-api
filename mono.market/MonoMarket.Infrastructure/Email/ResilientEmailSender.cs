using System.Diagnostics;
using Microsoft.Extensions.Logging;
using MonoMarket.Application.Common.Email;
using MonoMarket.Application.SendMails.Repositories;
using MonoMarket.Commons.Enums;
using MonoMarket.Domain.Entities;
using Polly;
using Polly.Retry;

namespace MonoMarket.Infrastructure.Email;

/// <summary>
/// Decorador transversal sobre <see cref="IEmailSender"/>: aplica reintentos con Polly y registra cada
/// intento en SendMailsLog, sin importar qué adapter de envío esté detrás (SMTP, proveedor externo, etc.).
/// Al terminar, deja la cabecera en SendMails con el estado final (Sent o MaxRetries).
/// </summary>
public class ResilientEmailSender(
    IEmailSender innerSender,
    ISendMailRepository sendMailRepository,
    ISendMailLogRepository sendMailLogRepository,
    EmailRetrySettings settings,
    ILogger<ResilientEmailSender> logger) : IEmailSender
{
    /// <inheritdoc />
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        int attemptNumber = 0;
        long lastAttemptDurationMs = 0;

        AsyncRetryPolicy retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                settings.MaxAttempts - 1,
                attempt => settings.BaseDelay * attempt,
                onRetryAsync: (exception, _, _, _) =>
                    LogAttemptAsync(message, attemptNumber, false, exception.Message, lastAttemptDurationMs, cancellationToken));

        try
        {
            await retryPolicy.ExecuteAsync(async ct =>
            {
                attemptNumber++;
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    await innerSender.SendAsync(message, ct);
                    stopwatch.Stop();
                    lastAttemptDurationMs = stopwatch.ElapsedMilliseconds;
                    await LogAttemptAsync(message, attemptNumber, true, null, lastAttemptDurationMs, ct);
                }
                catch
                {
                    stopwatch.Stop();
                    lastAttemptDurationMs = stopwatch.ElapsedMilliseconds;
                    throw;
                }
            }, cancellationToken);

            await sendMailRepository.UpdateStatusAsync(message.SendMailId, SendMailStatus.Sent, attemptNumber, DateTime.UtcNow, cancellationToken);
        }
        catch (Exception ex)
        {
            await LogAttemptAsync(message, attemptNumber, false, ex.Message, lastAttemptDurationMs, cancellationToken);
            await sendMailRepository.UpdateStatusAsync(message.SendMailId, SendMailStatus.MaxRetries, attemptNumber, DateTime.UtcNow, cancellationToken);

            logger.LogError(
                ex,
                "Se agotaron los {MaxAttempts} intentos de envío a {ToEmail} (SendMailId {SendMailId}).",
                settings.MaxAttempts,
                message.ToEmail,
                message.SendMailId);

            throw;
        }
    }

    /// <summary>
    /// Registra un intento individual de envío en SendMailsLog. Es el punto donde el log queda
    /// desacoplado del adapter de envío: aquí no importa cómo se intentó enviar, solo qué pasó.
    /// </summary>
    private async Task LogAttemptAsync(
        EmailMessage message,
        int attemptNumber,
        bool success,
        string? errorMessage,
        long durationMs,
        CancellationToken cancellationToken)
    {
        SendMailLog log = new()
        {
            SendMailId = message.SendMailId,
            AttemptNumber = attemptNumber,
            Status = success ? SendMailLogStatus.Sent : SendMailLogStatus.Failed,
            ErrorMessage = errorMessage,
            DurationMs = (int)durationMs,
            SentAt = DateTime.UtcNow,
        };

        await sendMailLogRepository.AddAsync(log, cancellationToken);
    }
}
