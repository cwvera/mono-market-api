using Mapster;
using MongoDB.Bson;
using MongoDB.Driver;
using MonoMarket.Application.SendMails.Repositories;
using MonoMarket.Domain.Entities;
using MonoMarket.Infrastructure.Persistence;
using MonoMarket.Infrastructure.Persistence.Documents;

namespace MonoMarket.Infrastructure.Repositories;

/// <summary>
/// Implementación con MongoDB del detalle de intentos de envío de correo.
/// </summary>
public class SendMailLogRepository(MongoDbContext context) : ISendMailLogRepository
{
    private const string CollectionName = "SendMailsLog";

    private readonly IMongoCollection<SendMailLogDocument> _collection = context.GetCollection<SendMailLogDocument>(CollectionName);

    /// <inheritdoc />
    public async Task AddAsync(SendMailLog sendMailLog, CancellationToken cancellationToken)
    {
        SendMailLogDocument document = sendMailLog.Adapt<SendMailLogDocument>();
        document.Id = ObjectId.GenerateNewId().ToString();

        await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<SendMailLog>> GetBySendMailIdAsync(string sendMailId, CancellationToken cancellationToken)
    {
        List<SendMailLogDocument> documents = await _collection
            .Find(d => d.SendMailId == sendMailId)
            .SortBy(d => d.AttemptNumber)
            .ToListAsync(cancellationToken);

        return documents.Adapt<List<SendMailLog>>();
    }
}
