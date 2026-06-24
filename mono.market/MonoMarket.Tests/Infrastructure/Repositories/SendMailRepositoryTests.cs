using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonoMarket.Commons.Enums;
using MonoMarket.Domain.Entities;
using MonoMarket.Infrastructure.Configuration;
using MonoMarket.Infrastructure.Persistence;
using MonoMarket.Infrastructure.Persistence.Documents;
using MonoMarket.Infrastructure.Repositories;
using Testcontainers.MongoDb;

namespace MonoMarket.Tests.Infrastructure.Repositories;

/// <summary>
/// Pruebas de integración de SendMailRepository contra una instancia real de MongoDB levantada con Testcontainers.
/// </summary>
public class SendMailRepositoryTests : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder("mongo:7").Build();

    private SendMailRepository _repository = null!;
    private IMongoCollection<SendMailDocument> _collection = null!;

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

        _collection = context.GetCollection<SendMailDocument>("SendMails");
        _repository = new SendMailRepository(context);
    }

    /// <summary>
    /// Apaga el contenedor de Mongo al terminar la prueba.
    /// </summary>
    public async Task DisposeAsync()
    {
        await _mongoContainer.DisposeAsync();
    }

    /// <summary>
    /// Verifica que CreateAsync inserte el documento y devuelva un Id válido de ObjectId.
    /// </summary>
    [Fact]
    public async Task CreateAsync_InsertsDocumentAndReturnsId()
    {
        SendMail sendMail = new() { InvoiceNumber = "FAC-001-2025", ToEmail = "contacto@alpha.com", ToName = "Empresa Alpha SAS" };

        string id = await _repository.CreateAsync(sendMail, CancellationToken.None);

        SendMailDocument? document = await _collection.Find(d => d.Id == id).FirstOrDefaultAsync();
        Assert.NotNull(document);
        Assert.Equal("FAC-001-2025", document!.InvoiceNumber);
    }

    /// <summary>
    /// Verifica que UpdateStatusAsync persista el estado final y el total de intentos.
    /// </summary>
    [Fact]
    public async Task UpdateStatusAsync_PersistsFinalStatus()
    {
        SendMailDocument document = new() { InvoiceNumber = "FAC-001-2025", ToEmail = "contacto@alpha.com" };
        await _collection.InsertOneAsync(document);
        DateTime lastAttemptAt = DateTime.UtcNow;

        await _repository.UpdateStatusAsync(document.Id, SendMailStatus.Sent, 1, lastAttemptAt, CancellationToken.None);

        SendMailDocument? updated = await _collection.Find(d => d.Id == document.Id).FirstOrDefaultAsync();
        Assert.NotNull(updated);
        Assert.Equal((int)SendMailStatus.Sent, updated!.Status);
        Assert.Equal(1, updated.TotalAttempts);
    }

    /// <summary>
    /// Verifica que GetByInvoiceNumberAsync solo devuelva los correos de la factura indicada.
    /// </summary>
    [Fact]
    public async Task GetByInvoiceNumberAsync_ReturnsOnlyMatchingInvoice()
    {
        await _collection.InsertManyAsync(
        [
            new SendMailDocument { InvoiceNumber = "FAC-001-2025", ToEmail = "uno@alpha.com" },
            new SendMailDocument { InvoiceNumber = "FAC-001-2025", ToEmail = "dos@alpha.com" },
            new SendMailDocument { InvoiceNumber = "FAC-002-2025", ToEmail = "otro@beta.com" },
        ]);

        List<SendMail> result = await _repository.GetByInvoiceNumberAsync("FAC-001-2025", CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.All(result, s => Assert.Equal("FAC-001-2025", s.InvoiceNumber));
    }
}
