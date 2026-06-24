namespace MonoMarket.Commons.Enums;

/// <summary>
/// Valores conocidos para Invoice.Status. El campo se guarda como int porque
/// es abierto más allá de FourthReminder (el status sigue subiendo como NthReminder).
/// </summary>
public enum InvoiceStatus
{
    /// <summary>Factura desactivada.</summary>
    Deactivated = -2,

    /// <summary>Factura pendiente de pago.</summary>
    Pending = -1,

    /// <summary>Factura pagada.</summary>
    Paid = 0,

    /// <summary>Primer recordatorio enviado.</summary>
    FirstReminder = 1,

    /// <summary>Segundo recordatorio enviado.</summary>
    SecondReminder = 2,

    /// <summary>Tercer recordatorio enviado.</summary>
    ThirdReminder = 3,

    /// <summary>Cuarto recordatorio enviado.</summary>
    FourthReminder = 4,
}
