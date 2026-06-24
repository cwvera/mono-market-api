namespace MonoMarket.Application.Common.Email;

/// <summary>
/// Puerto para enviar correos. La implementación real (SMTP, proveedor externo, etc.) vive en Infrastructure
/// y se conecta a este contrato como un Adapter.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Envía el correo indicado. Si falla tras los reintentos, lanza la excepción del último intento.
    /// </summary>
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken);
}
