namespace MonoMarket.Application.Common.Email;

/// <summary>
/// Correo a enviar, ya con el contenido armado y vinculado a su registro de SendMail.
/// </summary>
public class EmailMessage
{
    /// <summary>
    /// Identificador del registro SendMail (cabecera) al que pertenece este envío.
    /// </summary>
    public string SendMailId { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del destinatario.
    /// </summary>
    public string ToEmail { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del destinatario.
    /// </summary>
    public string ToName { get; set; } = string.Empty;

    /// <summary>
    /// Asunto del correo.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Cuerpo del correo (HTML).
    /// </summary>
    public string Body { get; set; } = string.Empty;
}
