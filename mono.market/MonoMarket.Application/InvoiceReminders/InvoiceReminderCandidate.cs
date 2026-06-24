namespace MonoMarket.Application.InvoiceReminders;

/// <summary>
/// Fusión de los datos de factura y cliente que realmente necesita el flujo de recordatorios.
/// Evita pasar las entidades completas (<c>Invoice</c>, <c>Client</c>) entre el procesador y los handlers de etapa.
/// </summary>
public class InvoiceReminderCandidate
{
    /// <summary>
    /// Número de la factura.
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Identificación del cliente.
    /// </summary>
    public string ClientIdentification { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del cliente.
    /// </summary>
    public string ClientName { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del cliente.
    /// </summary>
    public string ClientEmail { get; set; } = string.Empty;

    /// <summary>
    /// Monto de la factura.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Fecha de emisión de la factura.
    /// </summary>
    public DateTime IssueDate { get; set; }
}
