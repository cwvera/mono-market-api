using MonoMarket.Commons.Enums;

namespace MonoMarket.Application.InvoiceReminders.StageHandlers;

/// <summary>
/// Etapa Pendiente: a los 30 días desde la emisión pasa a PrimerRecordatorio.
/// Es una transición silenciosa, sin plantilla de correo (no está en el modelo de datos original).
/// </summary>
public class PendingToFirstReminderStageHandler : InvoiceReminderStageHandlerBase
{
    /// <inheritdoc />
    public override InvoiceStatus FromStatus => InvoiceStatus.Pending;

    /// <inheritdoc />
    public override InvoiceStatus NextStatus => InvoiceStatus.FirstReminder;

    /// <inheritdoc />
    public override int ThresholdDays => 30;

    /// <inheritdoc />
    protected override SendMailTemplateType? TemplateType => null;
}
