namespace MonoMarket.Commons.Enums;

/// <summary>
/// Tipo de plantilla usada para el envío de un correo.
/// </summary>
public enum SendMailTemplateType
{
    /// <summary>Plantilla de segundo recordatorio.</summary>
    SecondReminder = 0,

    /// <summary>Plantilla de desactivación de factura.</summary>
    Deactivation = 1,
}
