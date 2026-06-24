using MonoMarket.Commons.Enums;
using MonoMarket.Domain.Common;

namespace MonoMarket.Domain.Entities;

/// <summary>
/// Cabecera (maestro) de un correo a enviar.
/// </summary>
public class SendMail : AuditableEntity
{
    /// <summary>
    /// Número de la factura a la que pertenece este correo.
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Correo del destinatario.
    /// </summary>
    public string ToEmail { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del destinatario.
    /// </summary>
    public string ToName { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de plantilla usada para el correo.
    /// </summary>
    public SendMailTemplateType TemplateType { get; set; }

    /// <summary>
    /// Asunto del correo.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Estado del envío.
    /// </summary>
    public SendMailStatus Status { get; set; }

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
}
