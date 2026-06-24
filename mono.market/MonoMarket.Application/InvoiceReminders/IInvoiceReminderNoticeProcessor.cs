using MonoMarket.Domain.Entities;

namespace MonoMarket.Application.InvoiceReminders;

/// <summary>
/// Evalúa una factura contra su etapa de recordatorio y, si ya venció el plazo, produce el aviso.
/// </summary>
public interface IInvoiceReminderNoticeProcessor
{
    /// <summary>
    /// Procesa una factura. Si no tiene etapa definida o no ha vencido el plazo, no hace nada.
    /// </summary>
    Task ProcessAsync(Invoice invoice, CancellationToken cancellationToken);
}
