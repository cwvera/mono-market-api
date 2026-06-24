namespace MonoMarket.Infrastructure.Email;

/// <summary>
/// Configuración de reintentos para el envío de correo.
/// </summary>
public class EmailRetrySettings
{
    /// <summary>
    /// Cantidad máxima de intentos (incluyendo el primero).
    /// </summary>
    public int MaxAttempts { get; set; } = 3;

    /// <summary>
    /// Espera base entre intentos; crece linealmente con el número de intento.
    /// </summary>
    public TimeSpan BaseDelay { get; set; } = TimeSpan.FromSeconds(1);
}
