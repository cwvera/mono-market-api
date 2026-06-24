using MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan;

namespace MonoMarket.WebApi.BackgroundServices;

/// <summary>
/// Arranca automáticamente el job de escaneo de recordatorios cuando la aplicación inicia,
/// y lo vuelve a arrancar cada día: al terminar una corrida, espera a la medianoche siguiente
/// antes de disparar la próxima. El control manual (start/stop/status) sigue disponible vía
/// endpoints sobre el mismo job singleton, en paralelo a este ciclo diario.
/// </summary>
public class InvoiceReminderScanHostedService(IInvoiceReminderScanJob job) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Dispara el job al arrancar el host y luego, cada vez que termina, espera a la medianoche
    /// siguiente para volver a dispararlo, hasta que la aplicación se detenga.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                job.Start();

                await WaitUntilFinishedAsync(stoppingToken);

                DateTime nextRunAt = DateTime.UtcNow.Date.AddDays(1);
                await Task.Delay(nextRunAt - DateTime.UtcNow, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Apagado normal del host: no es un error.
        }
    }

    /// <summary>
    /// Espera, sondeando el progreso, a que la corrida actual deje de estar en Running.
    /// </summary>
    private async Task WaitUntilFinishedAsync(CancellationToken cancellationToken)
    {
        while (job.GetProgress().Status == InvoiceReminderScanStatus.Running)
        {
            await Task.Delay(PollInterval, cancellationToken);
        }
    }

    /// <summary>
    /// Detiene la corrida en curso cuando el host se apaga.
    /// </summary>
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        job.Stop();
        return base.StopAsync(cancellationToken);
    }
}
