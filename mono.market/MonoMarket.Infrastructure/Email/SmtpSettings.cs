namespace MonoMarket.Infrastructure.Email;

/// <summary>
/// Configuración del servidor SMTP usado para el envío real de correos.
/// </summary>
public class SmtpSettings
{
    /// <summary>
    /// Host del servidor SMTP.
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Puerto del servidor SMTP.
    /// </summary>
    public int Port { get; set; } = 25;

    /// <summary>
    /// Si se debe usar SSL/TLS al conectar.
    /// </summary>
    public bool EnableSsl { get; set; }

    /// <summary>
    /// Usuario para autenticación, si el servidor la requiere.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Contraseña para autenticación, si el servidor la requiere.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Correo remitente.
    /// </summary>
    public string FromAddress { get; set; } = "no-reply@monomarket.com";

    /// <summary>
    /// Nombre del remitente.
    /// </summary>
    public string FromName { get; set; } = "MonoMarket";
}
