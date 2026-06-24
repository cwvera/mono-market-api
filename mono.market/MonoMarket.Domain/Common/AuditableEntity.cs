namespace MonoMarket.Domain.Common;

/// <summary>
/// Entidad base con campos de auditoría de creación y actualización.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    /// <summary>
    /// Fecha de creación de la entidad.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de última actualización de la entidad.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
