using Mapster;
using MongoDB.Bson;
using MongoDB.Driver;
using MonoMarket.Application.Invoices.Repositories;
using MonoMarket.Domain.Entities;
using MonoMarket.Infrastructure.Persistence;
using MonoMarket.Infrastructure.Persistence.Documents;
using System.Text.RegularExpressions;

namespace MonoMarket.Infrastructure.Repositories;

/// <summary>
/// Implementación con MongoDB del acceso a facturas.
/// </summary>
public class InvoiceRepository(MongoDbContext context) : IInvoiceRepository
{
    private const string CollectionName = "Invoices";

    private readonly IMongoCollection<InvoiceDocument> _collection = context.GetCollection<InvoiceDocument>(CollectionName);

    /// <inheritdoc />
    public async Task<Invoice?> GetByNumberAsync(string invoiceNumber, CancellationToken cancellationToken)
    {
        InvoiceDocument? document = await _collection
            .Find(d => d.InvoiceNumber == invoiceNumber)
            .FirstOrDefaultAsync(cancellationToken);

        return document?.Adapt<Invoice>();
    }

    /// <inheritdoc />
    public async Task<List<Invoice>> GetByClientOrStatusAsync(string? clientIdentification, int? status, CancellationToken cancellationToken)
    {
        FilterDefinitionBuilder<InvoiceDocument> filterBuilder = Builders<InvoiceDocument>.Filter;
        FilterDefinition<InvoiceDocument> filter = filterBuilder.Empty;

        if (!string.IsNullOrWhiteSpace(clientIdentification))
        {
            BsonRegularExpression pattern = new(Regex.Escape(clientIdentification), "i");
            filter &= filterBuilder.Regex(d => d.ClientIdentification, pattern);
        }

        if (status.HasValue)
        {
            filter &= filterBuilder.Eq(d => d.Status, status.Value);
        }

        List<InvoiceDocument> documents = await _collection.Find(filter).ToListAsync(cancellationToken);

        return documents.Adapt<List<Invoice>>();
    }

    /// <inheritdoc />
    public async Task<List<Invoice>> GetBatchByStatusesAsync(IReadOnlyCollection<int> statuses, string? afterId, int take, CancellationToken cancellationToken)
    {
        FilterDefinitionBuilder<InvoiceDocument> filterBuilder = Builders<InvoiceDocument>.Filter;
        FilterDefinition<InvoiceDocument> filter = filterBuilder.In(d => d.Status, statuses);

        if (!string.IsNullOrWhiteSpace(afterId))
        {
            filter &= filterBuilder.Gt(d => d.Id, afterId);
        }

        List<InvoiceDocument> documents = await _collection
            .Find(filter)
            .SortBy(d => d.Id)
            .Limit(take)
            .ToListAsync(cancellationToken);

        return documents.Adapt<List<Invoice>>();
    }

    /// <inheritdoc />
    public async Task UpdateStatusAsync(string id, int status, CancellationToken cancellationToken)
    {
        UpdateDefinition<InvoiceDocument> update = Builders<InvoiceDocument>.Update
            .Set(d => d.Status, status)
            .Set(d => d.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(d => d.Id == id, update, cancellationToken: cancellationToken);
    }
}
