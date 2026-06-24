namespace MonoMarket.Application.SendMails.Dtos;

/// <summary>
/// Representación del detalle de un intento de envío, expuesta por la capa de aplicación.
/// </summary>
public class SendMailLogDto
{
    /// <summary>
    /// Identificador del documento.
    /// </summary>
    public string Id { get; set; } = string.Empty;

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
    public int Status { get; set; }

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
    public DateTime CreatedAt { get; set; }
}
