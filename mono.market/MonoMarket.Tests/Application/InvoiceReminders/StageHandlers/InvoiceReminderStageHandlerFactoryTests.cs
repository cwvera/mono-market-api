using MonoMarket.Application.InvoiceReminders.StageHandlers;
using MonoMarket.Commons.Enums;

namespace MonoMarket.Tests.Application.InvoiceReminders.StageHandlers;

/// <summary>
/// Pruebas de la factory: debe devolver el handler correcto por estado, o null si no hay etapa definida.
/// </summary>
public class InvoiceReminderStageHandlerFactoryTests
{
    private readonly InvoiceReminderStageHandlerFactory _factory = new(
    [
        new PendingToFirstReminderStageHandler(),
        new FirstReminderToSecondReminderStageHandler(),
        new SecondReminderToDeactivatedStageHandler(),
    ]);

    /// <summary>
    /// Verifica que cada estado con etapa definida devuelva el handler correcto.
    /// </summary>
    [Theory]
    [InlineData(InvoiceStatus.Pending, typeof(PendingToFirstReminderStageHandler))]
    [InlineData(InvoiceStatus.FirstReminder, typeof(FirstReminderToSecondReminderStageHandler))]
    [InlineData(InvoiceStatus.SecondReminder, typeof(SecondReminderToDeactivatedStageHandler))]
    public void GetHandler_WithStatusHavingStage_ReturnsCorrectHandler(InvoiceStatus status, Type expectedHandlerType)
    {
        IInvoiceReminderStageHandler? handler = _factory.GetHandler((int)status);

        Assert.NotNull(handler);
        Assert.IsType(expectedHandlerType, handler);
    }

    /// <summary>
    /// Verifica que un estado sin etapa definida (Paid, Deactivated, etc.) devuelva null.
    /// </summary>
    [Fact]
    public void GetHandler_WithStatusHavingNoStage_ReturnsNull()
    {
        IInvoiceReminderStageHandler? handler = _factory.GetHandler((int)InvoiceStatus.Paid);

        Assert.Null(handler);
    }
}
