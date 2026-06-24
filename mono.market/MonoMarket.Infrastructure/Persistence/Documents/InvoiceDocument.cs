using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MonoMarket.Infrastructure.Persistence.Documents;

/// <summary>
/// Representación del documento de la colección "Invoices" en MongoDB.
/// Es el único punto del sistema que conoce la forma física del dato; el dominio no sabe de esto.
/// </summary>
public class InvoiceDocument
{
    /// <summary>
    /// Identificador único del documento (ObjectId de MongoDB).
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>
    /// Número de factura, único.
    /// </summary>
    [BsonElement("invoiceNumber")]
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Identificación del cliente al que pertenece la factura.
    /// </summary>
    [BsonElement("clientIdentification")]
    public string ClientIdentification { get; set; } = string.Empty;

    /// <summary>
    /// Monto de la factura.
    /// </summary>
    [BsonElement("amount")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Amount { get; set; }

    /// <summary>
    /// Fecha de emisión de la factura.
    /// </summary>
    [BsonElement("issueDate")]
    public DateTime IssueDate { get; set; }

    /// <summary>
    /// Estado de la factura.
    /// </summary>
    [BsonElement("status")]
    public int Status { get; set; }

    /// <summary>
    /// Fecha del último recordatorio enviado.
    /// </summary>
    [BsonElement("lastReminderSentAt")]
    public DateTime? LastReminderSentAt { get; set; }

    /// <summary>
    /// Número de recordatorios enviados.
    /// </summary>
    [BsonElement("reminderCount")]
    public int ReminderCount { get; set; }

    /// <summary>
    /// Fecha de creación del documento.
    /// </summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Fecha de última actualización del documento.
    /// </summary>
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
