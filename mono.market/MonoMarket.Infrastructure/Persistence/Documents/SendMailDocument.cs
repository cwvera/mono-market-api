using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MonoMarket.Infrastructure.Persistence.Documents;

/// <summary>
/// Representación del documento de la colección "SendMails" en MongoDB.
/// </summary>
public class SendMailDocument
{
    /// <summary>
    /// Identificador único del documento (ObjectId de MongoDB).
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>
    /// Número de la factura a la que pertenece este correo.
    /// </summary>
    [BsonElement("invoiceNumber")]
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del destinatario.
    /// </summary>
    [BsonElement("toEmail")]
    public string ToEmail { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del destinatario.
    /// </summary>
    [BsonElement("toName")]
    public string ToName { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de plantilla usada para el correo.
    /// </summary>
    [BsonElement("templateType")]
    public int TemplateType { get; set; }

    /// <summary>
    /// Asunto del correo.
    /// </summary>
    [BsonElement("subject")]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Estado del envío.
    /// </summary>
    [BsonElement("status")]
    public int Status { get; set; }

    /// <summary>
    /// Número total de intentos realizados.
    /// </summary>
    [BsonElement("totalAttempts")]
    public int TotalAttempts { get; set; }

    /// <summary>
    /// Fecha del último intento de envío.
    /// </summary>
    [BsonElement("lastAttemptAt")]
    public DateTime? LastAttemptAt { get; set; }

    /// <summary>
    /// Información contextual del envío (cliente, factura, cambio de estado).
    /// </summary>
    [BsonElement("data")]
    public string Data { get; set; } = string.Empty;

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
