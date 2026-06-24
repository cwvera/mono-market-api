namespace MonoMarket.Domain.Common;

/// <summary>
/// Clase base para toda entidad de dominio identificable.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único de la entidad.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
}
