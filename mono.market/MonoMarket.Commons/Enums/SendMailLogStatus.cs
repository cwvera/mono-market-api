namespace MonoMarket.Commons.Enums;

/// <summary>
/// Estado de un intento individual de envío de correo (detalle SendMailsLog).
/// </summary>
public enum SendMailLogStatus
{
    /// <summary>El intento se envió correctamente.</summary>
    Sent = 0,

    /// <summary>El intento falló.</summary>
    Failed = 1,
}
