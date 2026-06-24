namespace MonoMarket.Infrastructure.Configuration;

/// <summary>
/// Configuración de conexión a MongoDB, leída desde appsettings.
/// </summary>
public class MongoDbSettings
{
    /// <summary>
    /// Cadena de conexión a MongoDB.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la base de datos a usar.
    /// </summary>
    public string DatabaseName { get; set; } = string.Empty;
}
