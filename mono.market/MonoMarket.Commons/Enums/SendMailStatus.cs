namespace MonoMarket.Commons.Enums;

/// <summary>
/// Estado del envío de un correo (cabecera SendMails).
/// </summary>
public enum SendMailStatus
{
    /// <summary>Pendiente de envío.</summary>
    Pending = 0,

    /// <summary>Enviado correctamente.</summary>
    Sent = 1,

    /// <summary>Falló el envío.</summary>
    Failed = 2,

    /// <summary>Se alcanzó el máximo de reintentos.</summary>
    MaxRetries = 3,
}
