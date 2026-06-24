using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonoMarket.Domain.Entities;
using MonoMarket.Infrastructure.Configuration;
using MonoMarket.Infrastructure.Persistence;
using MonoMarket.Infrastructure.Persistence.Documents;
using MonoMarket.Infrastructure.Repositories;
using Testcontainers.MongoDb;

namespace MonoMarket.Tests.Infrastructure.Repositories;

/// <summary>
/// Pruebas de integración de SendMailLogRepository contra una instancia real de MongoDB levantada con Testcontainers.
/// </summary>
public class SendMailLogRepositoryTests : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder("mongo:7").Build();

    private SendMailLogRepository _repository = null!;
    private IMongoCollection<SendMailLogDocument> _collection = null!;

    /// <summary>
    /// Levanta el contenedor de Mongo y prepara el repositorio antes de cada prueba.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();

        MongoDbSettings settings = new()
        {
            ConnectionString = _mongoContainer.GetConnectionString(),
            DatabaseName = "monomarket_test",
        };
        MongoDbContext context = new(Options.Create(settings));

        _collection = context.GetCollection<SendMailLogDocument>("SendMailsLog");
        _repository = new SendMailLogRepository(context);
    }

    /// <summary>
    /// Apaga el contenedor de Mongo al terminar la prueba.
    /// </summary>
    public async Task DisposeAsync()
    {
        await _mongoContainer.DisposeAsync();
    }

    /// <summary>
    /// Verifica que AddAsync inserte el documento del intento.
    /// </summary>
    [Fact]
    public async Task AddAsync_InsertsDocument()
    {
        SendMailLog log = new() { SendMailId = "100000000000000000000099", AttemptNumber = 1 };

        await _repository.AddAsync(log, CancellationToken.None);

        long count = await _collection.CountDocumentsAsync(d => d.SendMailId == "100000000000000000000099");
        Assert.Equal(1, count);
    }

    /// <summary>
    /// Verifica que GetBySendMailIdAsync solo devuelva los intentos de esa cabecera, ordenados por intento.
    /// </summary>
    [Fact]
    public async Task GetBySendMailIdAsync_ReturnsOnlyMatchingHeaderOrderedByAttempt()
    {
        await _collection.InsertManyAsync(
        [
            new SendMailLogDocument { SendMailId = "100000000000000000000001", AttemptNumber = 2 },
            new SendMailLogDocument { SendMailId = "100000000000000000000001", AttemptNumber = 1 },
            new SendMailLogDocument { SendMailId = "100000000000000000000002", AttemptNumber = 1 },
        ]);

        List<SendMailLog> result = await _repository.GetBySendMailIdAsync("100000000000000000000001", CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].AttemptNumber);
        Assert.Equal(2, result[1].AttemptNumber);
    }
}
