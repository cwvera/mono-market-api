using MediatR;

namespace MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan.Commands;

/// <summary>
/// Maneja el comando de detener el job de escaneo de recordatorios.
/// </summary>
public class StopInvoiceReminderScanCommandHandler(IInvoiceReminderScanJob job) : IRequestHandler<StopInvoiceReminderScanCommand>
{
    /// <summary>
    /// Delega el stop al job singleton.
    /// </summary>
    public Task Handle(StopInvoiceReminderScanCommand request, CancellationToken cancellationToken)
    {
        job.Stop();
        return Task.CompletedTask;
    }
}
