namespace MonoMarket.Application.InvoiceReminders.StageHandlers;

/// <summary>
/// Abstract Factory: dado el estado actual de una factura, entrega el handler de etapa correspondiente.
/// </summary>
public interface IInvoiceReminderStageHandlerFactory
{
    /// <summary>
    /// Devuelve el handler para el estado indicado, o null si ese estado no tiene una etapa definida
    /// (por ejemplo, Paid o Deactivated no avanzan a ningún lado).
    /// </summary>
    IInvoiceReminderStageHandler? GetHandler(int status);
}
