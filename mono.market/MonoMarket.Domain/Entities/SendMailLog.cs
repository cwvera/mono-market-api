using MonoMarket.Commons.Enums;
using MonoMarket.Domain.Common;

namespace MonoMarket.Domain.Entities;

/// <summary>
/// Detalle de un intento individual de envío de correo.
/// </summary>
public class SendMailLog : BaseEntity
{
    /// <summary>
    /// Identificador del SendMail (cabecera) al que pertenece este intento.
    /// </summary>
    public string SendMailId { get; set; } = string.Empty;

    /// <summary>
    /// Número del intento (1, 2, 3...).
    /// </summary>
    public int AttemptNumber { get; set; }

    /// <summary>
    /// Estado de este intento.
    /// </summary>
    public SendMailLogStatus Status { get; set; }

    /// <summary>
    /// Mensaje de error si el intento falló.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Duración del intento en milisegundos.
    /// </summary>
    public int DurationMs { get; set; }

    /// <summary>
    /// Fecha en la que se realizó este intento.
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Fecha de creación del registro de este intento.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
