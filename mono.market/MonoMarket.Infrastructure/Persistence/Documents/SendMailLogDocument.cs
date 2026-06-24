using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MonoMarket.Infrastructure.Persistence.Documents;

/// <summary>
/// Representación del documento de la colección "SendMailsLog" en MongoDB.
/// </summary>
public class SendMailLogDocument
{
    /// <summary>
    /// Identificador único del documento (ObjectId de MongoDB).
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>
    /// Identificador del SendMail (cabecera) al que pertenece este intento.
    /// </summary>
    [BsonElement("sendMailId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string SendMailId { get; set; } = string.Empty;

    /// <summary>
    /// Número del intento (1, 2, 3...).
    /// </summary>
    [BsonElement("attemptNumber")]
    public int AttemptNumber { get; set; }

    /// <summary>
    /// Estado de este intento.
    /// </summary>
    [BsonElement("status")]
    public int Status { get; set; }

    /// <summary>
    /// Mensaje de error si el intento falló.
    /// </summary>
    [BsonElement("errorMessage")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Duración del intento en milisegundos.
    /// </summary>
    [BsonElement("durationMs")]
    public int DurationMs { get; set; }

    /// <summary>
    /// Fecha en la que se realizó este intento.
    /// </summary>
    [BsonElement("sentAt")]
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Fecha de creación del registro de este intento.
    /// </summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
}
