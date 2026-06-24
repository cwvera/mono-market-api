using MonoMarket.Application.InvoiceReminders;
using MonoMarket.Application.InvoiceReminders.StageHandlers;
using MonoMarket.Commons.Enums;

namespace MonoMarket.Tests.Application.InvoiceReminders.StageHandlers;

/// <summary>
/// Pruebas de los handlers de etapa: umbral de días e identidad de cada etapa.
/// </summary>
public class InvoiceReminderStageHandlerTests
{
    /// <summary>
    /// Verifica los datos de identidad de cada handler (FromStatus, NextStatus, ThresholdDays).
    /// </summary>
    [Theory]
    [InlineData(typeof(PendingToFirstReminderStageHandler), InvoiceStatus.Pending, InvoiceStatus.FirstReminder, 30)]
    [InlineData(typeof(FirstReminderToSecondReminderStageHandler), InvoiceStatus.FirstReminder, InvoiceStatus.SecondReminder, 60)]
    [InlineData(typeof(SecondReminderToDeactivatedStageHandler), InvoiceStatus.SecondReminder, InvoiceStatus.Deactivated, 90)]
    public void Handler_HasCorrectIdentity(Type handlerType, InvoiceStatus fromStatus, InvoiceStatus nextStatus, int thresholdDays)
    {
        IInvoiceReminderStageHandler handler = (IInvoiceReminderStageHandler)Activator.CreateInstance(handlerType)!;

        Assert.Equal(fromStatus, handler.FromStatus);
        Assert.Equal(nextStatus, handler.NextStatus);
        Assert.Equal(thresholdDays, handler.ThresholdDays);
    }

    /// <summary>
    /// Verifica que IsDue compare solo fechas: con la misma fecha pero distinta hora, el resultado no cambia.
    /// </summary>
    [Fact]
    public void IsDue_ComparesDateOnlyIgnoringTime()
    {
        FirstReminderToSecondReminderStageHandler handler = new();
        DateOnly issueDateAtNight = DateOnly.FromDateTime(new DateTime(2025, 1, 1, 23, 59, 0, DateTimeKind.Utc));
        DateOnly exactlySixtyDaysLaterAtDawn = DateOnly.FromDateTime(new DateTime(2025, 3, 2, 0, 1, 0, DateTimeKind.Utc));

        bool isDue = handler.IsDue(issueDateAtNight, exactlySixtyDaysLaterAtDawn);

        Assert.True(isDue);
    }

    /// <summary>
    /// Verifica que antes de cumplirse el umbral, IsDue sea false; al cumplirse, true.
    /// </summary>
    [Fact]
    public void IsDue_BeforeAndAfterThreshold()
    {
        FirstReminderToSecondReminderStageHandler handler = new();
        DateOnly issueDate = new(2025, 1, 1);

        Assert.False(handler.IsDue(issueDate, issueDate.AddDays(59)));
        Assert.True(handler.IsDue(issueDate, issueDate.AddDays(60)));
    }

    /// <summary>
    /// Verifica que BuildNotice copie el candidato y use el TemplateType de la etapa.
    /// </summary>
    [Fact]
    public void BuildNotice_CopiesCandidate()
    {
        SecondReminderToDeactivatedStageHandler handler = new();
        InvoiceReminderCandidate candidate = new()
        {
            InvoiceNumber = "FAC-001-2025",
            ClientIdentification = "900123456-7",
            ClientName = "Empresa Alpha SAS",
            ClientEmail = "contacto@alpha.com",
            Amount = 1500000m,
            IssueDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };

        InvoiceReminderNotice notice = handler.BuildNotice(candidate);

        Assert.Same(candidate, notice.Candidate);
        Assert.Equal(InvoiceStatus.SecondReminder, notice.FromStatus);
        Assert.Equal(InvoiceStatus.Deactivated, notice.NextStatus);
        Assert.Equal(SendMailTemplateType.Deactivation, notice.TemplateType);
    }

    /// <summary>
    /// Verifica que la etapa Pendiente→PrimerRecordatorio no tenga plantilla (transición silenciosa).
    /// </summary>
    [Fact]
    public void BuildNotice_PendingToFirstReminder_HasNoTemplate()
    {
        PendingToFirstReminderStageHandler handler = new();
        InvoiceReminderCandidate candidate = new() { InvoiceNumber = "FAC-001-2025" };

        InvoiceReminderNotice notice = handler.BuildNotice(candidate);

        Assert.Null(notice.TemplateType);
    }
}
