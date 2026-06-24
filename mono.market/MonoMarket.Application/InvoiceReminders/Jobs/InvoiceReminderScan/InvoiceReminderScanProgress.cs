namespace MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan;

/// <summary>
/// Snapshot del avance de una corrida del job de escaneo de recordatorios.
/// </summary>
public class InvoiceReminderScanProgress
{
    /// <summary>
    /// Estado actual de la corrida.
    /// </summary>
    public InvoiceReminderScanStatus Status { get; set; } = InvoiceReminderScanStatus.Idle;

    /// <summary>
    /// Cantidad de lotes procesados en la corrida actual (o la última).
    /// </summary>
    public int BatchesProcessed { get; set; }

    /// <summary>
    /// Cantidad total de facturas extraídas (PrimerRecordatorio o SegundoRecordatorio) hasta el momento.
    /// </summary>
    public int TotalExtracted { get; set; }

    /// <summary>
    /// Fecha de inicio de la corrida actual (o la última).
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Fecha en la que terminó la corrida (por completarse, detenerse o fallar).
    /// </summary>
    public DateTime? FinishedAt { get; set; }

    /// <summary>
    /// Mensaje del último error, si la corrida terminó en Failed.
    /// </summary>
    public string? LastError { get; set; }
}
