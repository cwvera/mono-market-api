using MonoMarket.Domain.Common;

namespace MonoMarket.Domain.Entities;

/// <summary>
/// Cliente al que se le emiten facturas.
/// </summary>
public class Client : AuditableEntity
{
    /// <summary>
    /// Identificación del cliente (NIT/Cédula), única.
    /// </summary>
    public string Identification { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del cliente.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del cliente.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Teléfono del cliente.
    /// </summary>
    public string Phone { get; set; } = string.Empty;
}
