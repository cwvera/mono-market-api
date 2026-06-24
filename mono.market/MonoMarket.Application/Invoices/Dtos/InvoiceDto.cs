namespace MonoMarket.Application.Invoices.Dtos;

/// <summary>
/// Representación de una factura expuesta por la capa de aplicación.
/// </summary>
public class InvoiceDto
{
    /// <summary>
    /// Identificador del documento.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Número de factura.
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Identificación del cliente.
    /// </summary>
    public string ClientIdentification { get; set; } = string.Empty;

    /// <summary>
    /// Monto de la factura.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Fecha de emisión.
    /// </summary>
    public DateTime IssueDate { get; set; }

    /// <summary>
    /// Estado actual de la factura.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Fecha del último recordatorio enviado.
    /// </summary>
    public DateTime? LastReminderSentAt { get; set; }

    /// <summary>
    /// Número de recordatorios enviados.
    /// </summary>
    public int ReminderCount { get; set; }
}
