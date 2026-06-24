namespace MonoMarket.Application.SendMails.Dtos;

/// <summary>
/// Representación de la cabecera de un correo expuesta por la capa de aplicación.
/// </summary>
public class SendMailDto
{
    /// <summary>
    /// Identificador del documento.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Número de la factura a la que pertenece este correo.
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del destinatario.
    /// </summary>
    public string ToEmail { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del destinatario.
    /// </summary>
    public string ToName { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de plantilla usada para el correo.
    /// </summary>
    public int TemplateType { get; set; }

    /// <summary>
    /// Asunto del correo.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Estado del envío.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Número total de intentos realizados.
    /// </summary>
    public int TotalAttempts { get; set; }

    /// <summary>
    /// Fecha del último intento de envío.
    /// </summary>
    public DateTime? LastAttemptAt { get; set; }

    /// <summary>
    /// Información contextual del envío (cliente, factura, cambio de estado).
    /// </summary>
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de creación del registro.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Fecha de última actualización del registro.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
