namespace MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan;

/// <summary>
/// Job en background (singleton) que escanea, en lotes, las facturas en PrimerRecordatorio o
/// SegundoRecordatorio, envía el correo de recordatorio correspondiente y persiste el avance de estado.
/// </summary>
public interface IInvoiceReminderScanJob
{
    /// <summary>
    /// Inicia una corrida nueva. Si ya hay una corriendo, no hace nada y devuelve false.
    /// </summary>
    bool Start();

    /// <summary>
    /// Detiene la corrida actual, si hay una en curso, entre el lote actual y el siguiente.
    /// </summary>
    void Stop();

    /// <summary>
    /// Devuelve una copia del avance de la corrida actual o la última.
    /// </summary>
    InvoiceReminderScanProgress GetProgress();
}
