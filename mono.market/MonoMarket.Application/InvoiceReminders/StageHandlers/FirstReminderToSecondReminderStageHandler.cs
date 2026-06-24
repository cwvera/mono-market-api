using MonoMarket.Commons.Enums;

namespace MonoMarket.Application.InvoiceReminders.StageHandlers;

/// <summary>
/// Etapa PrimerRecordatorio: a los 60 días desde la emisión pasa a SegundoRecordatorio
/// y se envía la plantilla de segundo recordatorio.
/// </summary>
public class FirstReminderToSecondReminderStageHandler : InvoiceReminderStageHandlerBase
{
    /// <inheritdoc />
    public override InvoiceStatus FromStatus => InvoiceStatus.FirstReminder;

    /// <inheritdoc />
    public override InvoiceStatus NextStatus => InvoiceStatus.SecondReminder;

    /// <inheritdoc />
    public override int ThresholdDays => 60;

    /// <inheritdoc />
    protected override SendMailTemplateType? TemplateType => SendMailTemplateType.SecondReminder;
}
