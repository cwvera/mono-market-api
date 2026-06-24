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
/// Pruebas de integración de InvoiceRepository contra una instancia real de MongoDB levantada con Testcontainers.
/// Cada prueba arranca su propio contenedor para garantizar aislamiento de datos.
/// </summary>
public class InvoiceRepositoryTests : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder("mongo:7").Build();

    private InvoiceRepository _repository = null!;
    private IMongoCollection<InvoiceDocument> _collection = null!;

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

        _collection = context.GetCollection<InvoiceDocument>("Invoices");
        _repository = new InvoiceRepository(context);
    }

    /// <summary>
    /// Apaga el contenedor de Mongo al terminar la prueba.
    /// </summary>
    public async Task DisposeAsync()
    {
        await _mongoContainer.DisposeAsync();
    }

    /// <summary>
    /// Verifica que una factura existente se devuelva mapeada a la entidad de dominio.
    /// </summary>
    [Fact]
    public async Task GetByNumberAsync_ExistingInvoice_ReturnsMappedInvoice()
    {
        await _collection.InsertOneAsync(new InvoiceDocument
        {
            InvoiceNumber = "FAC-001-2025",
            ClientIdentification = "900123456-7",
            Amount = 1500000m,
            IssueDate = new DateTime(2025, 2, 15, 0, 0, 0, DateTimeKind.Utc),
            Status = 1,
        });

        Invoice? invoice = await _repository.GetByNumberAsync("FAC-001-2025", CancellationToken.None);

        Assert.NotNull(invoice);
        Assert.Equal("FAC-001-2025", invoice!.InvoiceNumber);
        Assert.Equal("900123456-7", invoice.ClientIdentification);
        Assert.Equal(1500000m, invoice.Amount);
        Assert.Equal(1, invoice.Status);
    }

    /// <summary>
    /// Verifica que se devuelva null cuando no existe una factura con ese número.
    /// </summary>
    [Fact]
    public async Task GetByNumberAsync_NonExistingInvoice_ReturnsNull()
    {
        Invoice? invoice = await _repository.GetByNumberAsync("NO-EXISTE", CancellationToken.None);

        Assert.Null(invoice);
    }

    /// <summary>
    /// Verifica que el filtro combinado de cliente y estado solo devuelva la factura que cumple ambos.
    /// </summary>
    [Fact]
    public async Task GetByClientOrStatusAsync_FiltersByClientAndStatus()
    {
        await _collection.InsertManyAsync(
        [
            new InvoiceDocument { InvoiceNumber = "FAC-101", ClientIdentification = "111", Status = 1, Amount = 100m, IssueDate = DateTime.UtcNow },
            new InvoiceDocument { InvoiceNumber = "FAC-102", ClientIdentification = "111", Status = 2, Amount = 200m, IssueDate = DateTime.UtcNow },
            new InvoiceDocument { InvoiceNumber = "FAC-103", ClientIdentification = "222", Status = 1, Amount = 300m, IssueDate = DateTime.UtcNow },
        ]);

        List<Invoice> result = await _repository.GetByClientOrStatusAsync("111", 1, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("FAC-101", result[0].InvoiceNumber);
    }

    /// <summary>
    /// Verifica que el filtro de cliente sea una coincidencia parcial (LIKE), no exacta.
    /// </summary>
    [Fact]
    public async Task GetByClientOrStatusAsync_WithPartialClientIdentification_ReturnsMatches()
    {
        await _collection.InsertManyAsync(
        [
            new InvoiceDocument { InvoiceNumber = "FAC-111", ClientIdentification = "900123456-7", Status = 1, Amount = 100m, IssueDate = DateTime.UtcNow },
            new InvoiceDocument { InvoiceNumber = "FAC-112", ClientIdentification = "800999111-2", Status = 1, Amount = 200m, IssueDate = DateTime.UtcNow },
        ]);

        List<Invoice> result = await _repository.GetByClientOrStatusAsync("123456", null, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("FAC-111", result[0].InvoiceNumber);
    }

    /// <summary>
    /// Verifica que sin filtros se devuelvan todas las facturas de la colección.
    /// </summary>
    [Fact]
    public async Task GetByClientOrStatusAsync_WithoutFilters_ReturnsAll()
    {
        await _collection.InsertManyAsync(
        [
            new InvoiceDocument { InvoiceNumber = "FAC-201", ClientIdentification = "333", Status = 1, Amount = 100m, IssueDate = DateTime.UtcNow },
            new InvoiceDocument { InvoiceNumber = "FAC-202", ClientIdentification = "444", Status = 2, Amount = 200m, IssueDate = DateTime.UtcNow },
        ]);

        List<Invoice> result = await _repository.GetByClientOrStatusAsync(null, null, CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    /// <summary>
    /// Verifica que UpdateStatusAsync persista el nuevo estado en el documento.
    /// </summary>
    [Fact]
    public async Task UpdateStatusAsync_PersistsNewStatus()
    {
        InvoiceDocument document = new()
        {
            InvoiceNumber = "FAC-301",
            ClientIdentification = "555",
            Status = 1,
            Amount = 100m,
            IssueDate = DateTime.UtcNow,
        };
        await _collection.InsertOneAsync(document);

        await _repository.UpdateStatusAsync(document.Id, 2, CancellationToken.None);

        InvoiceDocument? updated = await _collection.Find(d => d.Id == document.Id).FirstOrDefaultAsync();
        Assert.NotNull(updated);
        Assert.Equal(2, updated!.Status);
    }
}
