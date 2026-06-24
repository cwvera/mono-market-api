using MonoMarket.Commons.Enums;

namespace MonoMarket.Application.InvoiceReminders.EmailTemplates;

/// <summary>
/// Implementación de la factory: indexa las plantillas registradas por su TemplateType.
/// </summary>
public class InvoiceReminderEmailTemplateFactory : IInvoiceReminderEmailTemplateFactory
{
    private readonly Dictionary<SendMailTemplateType, IInvoiceReminderEmailTemplate> _templatesByType;

    /// <summary>
    /// Recibe todas las plantillas registradas en DI y las indexa por tipo.
    /// </summary>
    public InvoiceReminderEmailTemplateFactory(IEnumerable<IInvoiceReminderEmailTemplate> templates)
    {
        _templatesByType = templates.ToDictionary(template => template.TemplateType);
    }

    /// <inheritdoc />
    public IInvoiceReminderEmailTemplate GetTemplate(SendMailTemplateType templateType) =>
        _templatesByType.TryGetValue(templateType, out IInvoiceReminderEmailTemplate? template)
            ? template
            : throw new InvalidOperationException($"No hay plantilla de correo registrada para el tipo {templateType}.");
}
