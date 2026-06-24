using Mapster;
using MongoDB.Bson;
using MongoDB.Driver;
using MonoMarket.Application.SendMails.Repositories;
using MonoMarket.Commons.Enums;
using MonoMarket.Domain.Entities;
using MonoMarket.Infrastructure.Persistence;
using MonoMarket.Infrastructure.Persistence.Documents;

namespace MonoMarket.Infrastructure.Repositories;

/// <summary>
/// Implementación con MongoDB de la cabecera de correos a enviar.
/// </summary>
public class SendMailRepository(MongoDbContext context) : ISendMailRepository
{
    private const string CollectionName = "SendMails";

    private readonly IMongoCollection<SendMailDocument> _collection = context.GetCollection<SendMailDocument>(CollectionName);

    /// <inheritdoc />
    public async Task<string> CreateAsync(SendMail sendMail, CancellationToken cancellationToken)
    {
        SendMailDocument document = sendMail.Adapt<SendMailDocument>();
        document.Id = ObjectId.GenerateNewId().ToString();

        await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);

        return document.Id;
    }

    /// <inheritdoc />
    public async Task UpdateStatusAsync(string id, SendMailStatus status, int totalAttempts, DateTime lastAttemptAt, CancellationToken cancellationToken)
    {
        UpdateDefinition<SendMailDocument> update = Builders<SendMailDocument>.Update
            .Set(d => d.Status, (int)status)
            .Set(d => d.TotalAttempts, totalAttempts)
            .Set(d => d.LastAttemptAt, lastAttemptAt)
            .Set(d => d.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(d => d.Id == id, update, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<SendMail>> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken)
    {
        List<SendMailDocument> documents = await _collection
            .Find(d => d.InvoiceNumber == invoiceNumber)
            .SortByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);

        return documents.Adapt<List<SendMail>>();
    }
}
