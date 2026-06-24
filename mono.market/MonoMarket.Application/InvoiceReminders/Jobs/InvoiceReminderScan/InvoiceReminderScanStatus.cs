namespace MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan;

/// <summary>
/// Estado de una corrida del job de escaneo de recordatorios.
/// </summary>
public enum InvoiceReminderScanStatus
{
    /// <summary>Nunca se ha corrido.</summary>
    Idle,

    /// <summary>Corriendo actualmente.</summary>
    Running,

    /// <summary>Se detuvo manualmente antes de terminar.</summary>
    Stopped,

    /// <summary>Terminó de recorrer todas las facturas pendientes.</summary>
    Completed,

    /// <summary>Terminó por un error no controlado.</summary>
    Failed,
}
