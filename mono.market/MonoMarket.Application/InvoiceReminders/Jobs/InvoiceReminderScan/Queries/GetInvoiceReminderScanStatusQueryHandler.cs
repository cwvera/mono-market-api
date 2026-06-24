using MediatR;

namespace MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan.Queries;

/// <summary>
/// Maneja la consulta de avance del job de escaneo de recordatorios.
/// </summary>
public class GetInvoiceReminderScanStatusQueryHandler(IInvoiceReminderScanJob job)
    : IRequestHandler<GetInvoiceReminderScanStatusQuery, InvoiceReminderScanProgress>
{
    /// <summary>
    /// Delega la consulta de avance al job singleton.
    /// </summary>
    public Task<InvoiceReminderScanProgress> Handle(GetInvoiceReminderScanStatusQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(job.GetProgress());
}
