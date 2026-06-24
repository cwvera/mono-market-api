using MonoMarket.Domain.Entities;

namespace MonoMarket.Application.Invoices.Repositories;

/// <summary>
/// Acceso a la colección de facturas. Application define el contrato,
/// Infrastructure lo implementa (inversión de dependencias).
/// </summary>
public interface IInvoiceRepository
{
    /// <summary>
    /// Busca una factura por su número único.
    /// </summary>
    Task<Invoice?> GetByNumberAsync(string invoiceNumber, CancellationToken cancellationToken);

    /// <summary>
    /// Lista facturas, filtrando opcionalmente por coincidencia parcial de identificación de cliente y/o estado exacto.
    /// </summary>
    Task<List<Invoice>> GetByClientOrStatusAsync(string? clientIdentification, int? status, CancellationToken cancellationToken);

    /// <summary>
    /// Trae un lote de facturas cuyo estado esté en <paramref name="statuses"/>, ordenadas por Id ascendente.
    /// Paginación por cursor (no por skip/limit): si se indica <paramref name="afterId"/>, solo trae
    /// facturas con Id mayor a ese valor. Esto hace que cada lote sea estable aunque la colección
    /// reciba escrituras concurrentes mientras se escanea.
    /// </summary>
    Task<List<Invoice>> GetBatchByStatusesAsync(IReadOnlyCollection<int> statuses, string? afterId, int take, CancellationToken cancellationToken);

    /// <summary>
    /// Actualiza el estado de una factura (avance de etapa de recordatorio).
    /// </summary>
    Task UpdateStatusAsync(string id, int status, CancellationToken cancellationToken);
}
