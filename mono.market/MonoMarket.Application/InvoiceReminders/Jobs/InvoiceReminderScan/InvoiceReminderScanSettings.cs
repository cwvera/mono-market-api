namespace MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan;

/// <summary>
/// Configuración del job de escaneo de recordatorios (tamaño de lote y pausa entre lotes).
/// </summary>
public class InvoiceReminderScanSettings
{
    /// <summary>
    /// Cantidad máxima de facturas a traer por lote.
    /// </summary>
    public int BatchSize { get; set; } = 100;

    /// <summary>
    /// Pausa entre un lote y el siguiente, para no saturar la base de datos.
    /// </summary>
    public TimeSpan DelayBetweenBatches { get; set; } = TimeSpan.FromSeconds(1);
}
