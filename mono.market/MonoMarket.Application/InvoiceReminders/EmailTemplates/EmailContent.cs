namespace MonoMarket.Application.InvoiceReminders.EmailTemplates;

/// <summary>
/// Asunto y cuerpo de un correo, ya armados a partir de una plantilla.
/// </summary>
public class EmailContent
{
    /// <summary>
    /// Asunto del correo.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Cuerpo del correo (HTML).
    /// </summary>
    public string Body { get; set; } = string.Empty;
}
