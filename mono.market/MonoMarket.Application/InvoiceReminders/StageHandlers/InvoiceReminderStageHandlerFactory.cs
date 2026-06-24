namespace MonoMarket.Application.InvoiceReminders.StageHandlers;

/// <summary>
/// Implementación de la factory: indexa los handlers registrados por su FromStatus.
/// </summary>
public class InvoiceReminderStageHandlerFactory : IInvoiceReminderStageHandlerFactory
{
    private readonly Dictionary<int, IInvoiceReminderStageHandler> _handlersByStatus;

    /// <summary>
    /// Recibe todos los handlers registrados en DI y los indexa por estado de origen.
    /// </summary>
    public InvoiceReminderStageHandlerFactory(IEnumerable<IInvoiceReminderStageHandler> handlers)
    {
        _handlersByStatus = handlers.ToDictionary(handler => (int)handler.FromStatus);
    }

    /// <inheritdoc />
    public IInvoiceReminderStageHandler? GetHandler(int status) =>
        _handlersByStatus.GetValueOrDefault(status);
}
