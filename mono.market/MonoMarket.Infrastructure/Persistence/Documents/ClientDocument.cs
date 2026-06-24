using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MonoMarket.Infrastructure.Persistence.Documents;

/// <summary>
/// Representación del documento de la colección "Clients" en MongoDB.
/// </summary>
public class ClientDocument
{
    /// <summary>
    /// Identificador único del documento (ObjectId de MongoDB).
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>
    /// Identificación del cliente (NIT/Cédula), única.
    /// </summary>
    [BsonElement("identification")]
    public string Identification { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del cliente.
    /// </summary>
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del cliente.
    /// </summary>
    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Teléfono del cliente.
    /// </summary>
    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

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
