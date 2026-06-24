using MonoMarket.Commons.Enums;

namespace MonoMarket.Application.InvoiceReminders.StageHandlers;

/// <summary>
/// Etapa SegundoRecordatorio: a los 90 días desde la emisión pasa a Vencida/Desactivada
/// y se envía la plantilla de desactivación.
/// </summary>
public class SecondReminderToDeactivatedStageHandler : InvoiceReminderStageHandlerBase
{
    /// <inheritdoc />
    public override InvoiceStatus FromStatus => InvoiceStatus.SecondReminder;

    /// <inheritdoc />
    public override InvoiceStatus NextStatus => InvoiceStatus.Deactivated;

    /// <inheritdoc />
    public override int ThresholdDays => 90;

    /// <inheritdoc />
    protected override SendMailTemplateType? TemplateType => SendMailTemplateType.Deactivation;
}
