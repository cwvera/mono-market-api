using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan;
using MonoMarket.Application.InvoiceReminders;
using MonoMarket.Application.Invoices.Repositories;
using MonoMarket.Domain.Entities;

namespace MonoMarket.Tests.Application.InvoiceReminders.Jobs.InvoiceReminderScan;

/// <summary>
/// Pruebas del job de escaneo de recordatorios (loop, concurrencia y stop).
/// </summary>
public class InvoiceReminderScanJobTests
{
    private readonly Mock<IInvoiceRepository> _repositoryMock = new();
    private readonly Mock<IInvoiceReminderNoticeProcessor> _noticeProcessorMock = new();
    private readonly InvoiceReminderScanSettings _settings = new() { BatchSize = 10, DelayBetweenBatches = TimeSpan.Zero };

    private InvoiceReminderScanJob CreateJob() =>
        new(_repositoryMock.Object, _noticeProcessorMock.Object, _settings, NullLogger<InvoiceReminderScanJob>.Instance);

    /// <summary>
    /// Verifica que, cuando no hay facturas pendientes, el job termine como Completed con cero extraídas.
    /// </summary>
    [Fact]
    public async Task Start_NoPendingInvoices_FinishesCompletedWithZero()
    {
        _repositoryMock
            .Setup(r => r.GetBatchByStatusesAsync(It.IsAny<IReadOnlyCollection<int>>(), null, _settings.BatchSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        InvoiceReminderScanJob job = CreateJob();

        job.Start();
        await WaitUntilAsync(() => job.GetProgress().Status != InvoiceReminderScanStatus.Running);

        InvoiceReminderScanProgress progress = job.GetProgress();
        Assert.Equal(InvoiceReminderScanStatus.Completed, progress.Status);
        Assert.Equal(0, progress.TotalExtracted);
    }

    /// <summary>
    /// Verifica que el job pagine por cursor y acumule el total extraído a través de varios lotes.
    /// </summary>
    [Fact]
    public async Task Start_WithMultipleBatches_AccumulatesTotalAndFinishesCompleted()
    {
        List<Invoice> firstBatch = [new Invoice { Id = "1", InvoiceNumber = "FAC-001" }, new Invoice { Id = "2", InvoiceNumber = "FAC-002" }];
        List<Invoice> secondBatch = [new Invoice { Id = "3", InvoiceNumber = "FAC-003" }];
        _repositoryMock
            .Setup(r => r.GetBatchByStatusesAsync(It.IsAny<IReadOnlyCollection<int>>(), null, _settings.BatchSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(firstBatch);
        _repositoryMock
            .Setup(r => r.GetBatchByStatusesAsync(It.IsAny<IReadOnlyCollection<int>>(), "2", _settings.BatchSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(secondBatch);
        _repositoryMock
            .Setup(r => r.GetBatchByStatusesAsync(It.IsAny<IReadOnlyCollection<int>>(), "3", _settings.BatchSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        InvoiceReminderScanJob job = CreateJob();

        job.Start();
        await WaitUntilAsync(() => job.GetProgress().Status != InvoiceReminderScanStatus.Running);

        InvoiceReminderScanProgress progress = job.GetProgress();
        Assert.Equal(InvoiceReminderScanStatus.Completed, progress.Status);
        Assert.Equal(3, progress.TotalExtracted);
        Assert.Equal(2, progress.BatchesProcessed);
    }

    /// <summary>
    /// Verifica que iniciar el job mientras ya hay una corrida en curso no haga nada y devuelva false.
    /// </summary>
    [Fact]
    public async Task Start_WithRunInProgress_ReturnsFalse()
    {
        TaskCompletionSource gate = new();
        _repositoryMock
            .Setup(r => r.GetBatchByStatusesAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<string?>(), _settings.BatchSize, It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                await gate.Task;
                return [];
            });
        InvoiceReminderScanJob job = CreateJob();

        bool firstStart = job.Start();
        bool secondStart = job.Start();
        gate.SetResult();
        await WaitUntilAsync(() => job.GetProgress().Status != InvoiceReminderScanStatus.Running);

        Assert.True(firstStart);
        Assert.False(secondStart);
    }

    /// <summary>
    /// Verifica que Stop corte el loop entre lotes y deje la corrida en Stopped.
    /// </summary>
    [Fact]
    public async Task Stop_DuringRun_FinishesStopped()
    {
        TaskCompletionSource gate = new();
        _repositoryMock
            .Setup(r => r.GetBatchByStatusesAsync(It.IsAny<IReadOnlyCollection<int>>(), null, _settings.BatchSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Invoice { Id = "1", InvoiceNumber = "FAC-001" }]);
        _repositoryMock
            .Setup(r => r.GetBatchByStatusesAsync(It.IsAny<IReadOnlyCollection<int>>(), "1", _settings.BatchSize, It.IsAny<CancellationToken>()))
            .Returns(async (IReadOnlyCollection<int> _, string? _, int _, CancellationToken ct) =>
            {
                gate.TrySetResult();
                await Task.Delay(Timeout.Infinite, ct);
                return [];
            });
        InvoiceReminderScanJob job = CreateJob();

        job.Start();
        await gate.Task;
        job.Stop();
        await WaitUntilAsync(() => job.GetProgress().Status != InvoiceReminderScanStatus.Running);

        Assert.Equal(InvoiceReminderScanStatus.Stopped, job.GetProgress().Status);
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        DateTime deadline = DateTime.UtcNow.AddSeconds(5);
        while (!condition() && DateTime.UtcNow < deadline)
        {
            await Task.Delay(10);
        }
    }
}
