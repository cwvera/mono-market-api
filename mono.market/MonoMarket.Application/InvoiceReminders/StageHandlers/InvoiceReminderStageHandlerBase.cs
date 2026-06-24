using MonoMarket.Application.InvoiceReminders;
using MonoMarket.Commons.Enums;

namespace MonoMarket.Application.InvoiceReminders.StageHandlers;

/// <summary>
/// Base para los handlers de etapa: comparten la fórmula de "días desde emisión" y el armado
/// del aviso; cada etapa concreta solo aporta sus 4 valores (FromStatus, NextStatus, ThresholdDays, TemplateType).
/// </summary>
public abstract class InvoiceReminderStageHandlerBase : IInvoiceReminderStageHandler
{
    /// <inheritdoc />
    public abstract InvoiceStatus FromStatus { get; }

    /// <inheritdoc />
    public abstract InvoiceStatus NextStatus { get; }

    /// <inheritdoc />
    public abstract int ThresholdDays { get; }

    /// <summary>
    /// Plantilla de correo de esta etapa. Null si la transición es silenciosa (sin correo).
    /// </summary>
    protected abstract SendMailTemplateType? TemplateType { get; }

    /// <inheritdoc />
    public bool IsDue(DateOnly issueDate, DateOnly today)
    {
        int daysSinceIssue = today.DayNumber - issueDate.DayNumber;

        return daysSinceIssue >= ThresholdDays;
    }

    /// <inheritdoc />
    public InvoiceReminderNotice BuildNotice(InvoiceReminderCandidate candidate) => new()
    {
        Candidate = candidate,
        FromStatus = FromStatus,
        NextStatus = NextStatus,
        TemplateType = TemplateType,
    };
}
