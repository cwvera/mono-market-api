using MonoMarket.Domain.Entities;

namespace MonoMarket.Application.SendMails.Repositories;

/// <summary>
/// Acceso al detalle de intentos de envío de correo (colección SendMailsLog).
/// </summary>
public interface ISendMailLogRepository
{
    /// <summary>
    /// Registra un intento individual de envío.
    /// </summary>
    Task AddAsync(SendMailLog sendMailLog, CancellationToken cancellationToken);

    /// <summary>
    /// Lista los intentos registrados para una cabecera de SendMail, ordenados por número de intento.
    /// </summary>
    Task<List<SendMailLog>> GetBySendMailIdAsync(string sendMailId, CancellationToken cancellationToken);
}
