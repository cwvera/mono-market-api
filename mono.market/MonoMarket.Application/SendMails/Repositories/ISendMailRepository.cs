using MonoMarket.Commons.Enums;
using MonoMarket.Domain.Entities;

namespace MonoMarket.Application.SendMails.Repositories;

/// <summary>
/// Acceso a la cabecera de correos a enviar (colección SendMails).
/// </summary>
public interface ISendMailRepository
{
    /// <summary>
    /// Crea el registro de cabecera (estado Pending) y devuelve su Id.
    /// </summary>
    Task<string> CreateAsync(SendMail sendMail, CancellationToken cancellationToken);

    /// <summary>
    /// Actualiza el estado final, el total de intentos y la fecha del último intento.
    /// </summary>
    Task UpdateStatusAsync(string id, SendMailStatus status, int totalAttempts, DateTime lastAttemptAt, CancellationToken cancellationToken);

    /// <summary>
    /// Lista los correos asociados a una factura.
    /// </summary>
    Task<List<SendMail>> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken);
}
