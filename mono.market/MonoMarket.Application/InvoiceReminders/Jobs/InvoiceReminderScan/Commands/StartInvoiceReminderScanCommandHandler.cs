using MediatR;

namespace MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan.Commands;

/// <summary>
/// Maneja el comando de inicio del job de escaneo de recordatorios.
/// </summary>
public class StartInvoiceReminderScanCommandHandler(IInvoiceReminderScanJob job) : IRequestHandler<StartInvoiceReminderScanCommand, bool>
{
    /// <summary>
    /// Delega el inicio al job singleton.
    /// </summary>
    public Task<bool> Handle(StartInvoiceReminderScanCommand request, CancellationToken cancellationToken) => Task.FromResult(job.Start());
}
