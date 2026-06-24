using MonoMarket.Domain.Entities;

namespace MonoMarket.Application.Clients.Repositories;

/// <summary>
/// Acceso de solo lectura a la colección de clientes. Application define el contrato,
/// Infrastructure lo implementa (inversión de dependencias).
/// </summary>
public interface IClientRepository
{
    /// <summary>
    /// Busca un cliente por su identificación (NIT/Cédula), única.
    /// </summary>
    Task<Client?> GetByIdentificationAsync(string identification, CancellationToken cancellationToken);
}
