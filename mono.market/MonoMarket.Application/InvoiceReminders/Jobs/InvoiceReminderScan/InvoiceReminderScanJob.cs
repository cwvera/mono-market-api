using Microsoft.Extensions.Logging;
using MonoMarket.Application.InvoiceReminders;
using MonoMarket.Application.Invoices.Repositories;
using MonoMarket.Commons.Enums;
using MonoMarket.Domain.Entities;

namespace MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan;

/// <summary>
/// Implementación en memoria del job de escaneo de recordatorios. Vive como singleton:
/// una sola instancia mantiene el estado de la corrida durante toda la vida del proceso.
/// </summary>
public class InvoiceReminderScanJob(
    IInvoiceRepository invoiceRepository,
    IInvoiceReminderNoticeProcessor noticeProcessor,
    InvoiceReminderScanSettings settings,
    ILogger<InvoiceReminderScanJob> logger) : IInvoiceReminderScanJob
{
    private static readonly int[] TargetStatuses =
    [
        (int)InvoiceStatus.Pending,
        (int)InvoiceStatus.FirstReminder,
        (int)InvoiceStatus.SecondReminder,
    ];

    private readonly Lock _lock = new();
    private readonly InvoiceReminderScanProgress _progress = new();

    private CancellationTokenSource? _cancellationTokenSource;

    /// <inheritdoc />
    public bool Start()
    {
        lock (_lock)
        {
            if (_progress.Status == InvoiceReminderScanStatus.Running)
            {
                return false;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _progress.Status = InvoiceReminderScanStatus.Running;
            _progress.BatchesProcessed = 0;
            _progress.TotalExtracted = 0;
            _progress.StartedAt = DateTime.UtcNow;
            _progress.FinishedAt = null;
            _progress.LastError = null;
        }

        _ = RunAsync(_cancellationTokenSource.Token);

        return true;
    }

    /// <inheritdoc />
    public void Stop()
    {
        lock (_lock)
        {
            _cancellationTokenSource?.Cancel();
        }
    }

    /// <inheritdoc />
    public InvoiceReminderScanProgress GetProgress()
    {
        lock (_lock)
        {
            return new InvoiceReminderScanProgress
            {
                Status = _progress.Status,
                BatchesProcessed = _progress.BatchesProcessed,
                TotalExtracted = _progress.TotalExtracted,
                StartedAt = _progress.StartedAt,
                FinishedAt = _progress.FinishedAt,
                LastError = _progress.LastError,
            };
        }
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        string? afterId = null;

        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                List<Invoice> batch = await invoiceRepository.GetBatchByStatusesAsync(TargetStatuses, afterId, settings.BatchSize, cancellationToken);

                if (batch.Count == 0)
                {
                    Finish(InvoiceReminderScanStatus.Completed);
                    return;
                }

                afterId = batch[^1].Id;

                lock (_lock)
                {
                    _progress.BatchesProcessed++;
                    _progress.TotalExtracted += batch.Count;
                }

                logger.LogInformation(
                    "Lote {BatchNumber} extraído: {Count} facturas (acumulado {Total}).",
                    _progress.BatchesProcessed,
                    batch.Count,
                    _progress.TotalExtracted);

                foreach (Invoice invoice in batch)
                {
                    await noticeProcessor.ProcessAsync(invoice, cancellationToken);
                }

                await Task.Delay(settings.DelayBetweenBatches, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            Finish(InvoiceReminderScanStatus.Stopped);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "El job de escaneo de recordatorios terminó con un error.");

            lock (_lock)
            {
                _progress.LastError = ex.Message;
            }

            Finish(InvoiceReminderScanStatus.Failed);
        }
    }

    private void Finish(InvoiceReminderScanStatus status)
    {
        lock (_lock)
        {
            _progress.Status = status;
            _progress.FinishedAt = DateTime.UtcNow;
        }
    }
}
