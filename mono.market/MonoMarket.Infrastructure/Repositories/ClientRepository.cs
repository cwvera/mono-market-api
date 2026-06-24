using Mapster;
using MongoDB.Driver;
using MonoMarket.Application.Clients.Repositories;
using MonoMarket.Domain.Entities;
using MonoMarket.Infrastructure.Persistence;
using MonoMarket.Infrastructure.Persistence.Documents;

namespace MonoMarket.Infrastructure.Repositories;

/// <summary>
/// Implementación con MongoDB del acceso de solo lectura a clientes.
/// </summary>
public class ClientRepository(MongoDbContext context) : IClientRepository
{
    private const string CollectionName = "Clients";

    private readonly IMongoCollection<ClientDocument> _collection = context.GetCollection<ClientDocument>(CollectionName);

    /// <inheritdoc />
    public async Task<Client?> GetByIdentificationAsync(string identification, CancellationToken cancellationToken)
    {
        ClientDocument? document = await _collection
            .Find(d => d.Identification == identification)
            .FirstOrDefaultAsync(cancellationToken);

        return document?.Adapt<Client>();
    }
}
