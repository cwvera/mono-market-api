using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonoMarket.Infrastructure.Configuration;

namespace MonoMarket.Infrastructure.Persistence;

/// <summary>
/// Punto de acceso a la base de datos MongoDB configurada.
/// </summary>
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    /// <summary>
    /// Crea el contexto conectándose con la configuración indicada.
    /// </summary>
    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        MongoClient client = new(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    /// <summary>
    /// Obtiene la colección tipada con el nombre indicado.
    /// </summary>
    public IMongoCollection<T> GetCollection<T>(string collectionName) =>
        _database.GetCollection<T>(collectionName);
}
