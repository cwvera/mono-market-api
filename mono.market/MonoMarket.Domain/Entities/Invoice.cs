using MonoMarket.Domain.Common;

namespace MonoMarket.Domain.Entities;

/// <summary>
/// Factura emitida a un cliente, con seguimiento de recordatorios de pago.
/// </summary>
public class Invoice : AuditableEntity
{
    /// <summary>
    /// Número de factura, único.
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Identificación del cliente al que pertenece la factura.
    /// </summary>
    public string ClientIdentification { get; set; } = string.Empty;

    /// <summary>
    /// Monto de la factura.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Fecha de emisión de la factura.
    /// </summary>
    public DateTime IssueDate { get; set; }

    /// <summary>
    /// Estado de la factura. Ver <see cref="MonoMarket.Commons.Enums.InvoiceStatus"/> para los valores conocidos.
    /// Se guarda como int porque sigue creciendo más allá de FourthReminder (NthReminder).
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
