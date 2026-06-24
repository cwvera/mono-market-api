using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using MonoMarket.Application.Common.Email;

namespace MonoMarket.Infrastructure.Email;

/// <summary>
/// Adapter que envía correos reales por SMTP. Es el único punto del sistema que sabe cómo hablar
/// con un servidor de correo; el resto de la aplicación solo conoce el puerto <see cref="IEmailSender"/>.
/// </summary>
public class SmtpEmailSender(IOptions<SmtpSettings> settings) : IEmailSender
{
    /// <inheritdoc />
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        SmtpSettings smtpSettings = settings.Value;

        using SmtpClient client = new(smtpSettings.Host, smtpSettings.Port)
        {
            EnableSsl = smtpSettings.EnableSsl,
        };

        if (!string.IsNullOrWhiteSpace(smtpSettings.Username))
        {
            client.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
        }

        using MailMessage mailMessage = new(
            new MailAddress(smtpSettings.FromAddress, smtpSettings.FromName),
            new MailAddress(message.ToEmail, message.ToName))
        {
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = true,
        };

        await client.SendMailAsync(mailMessage, cancellationToken);
    }
}
