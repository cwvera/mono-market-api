using MonoMarket.Commons.Enums;

namespace MonoMarket.Application.InvoiceReminders.EmailTemplates;

/// <summary>
/// Abstract Factory: dado un tipo de plantilla, entrega la plantilla correspondiente.
/// </summary>
public interface IInvoiceReminderEmailTemplateFactory
{
    /// <summary>
    /// Devuelve la plantilla registrada para el tipo indicado, o lanza si no existe ninguna.
    /// </summary>
    IInvoiceReminderEmailTemplate GetTemplate(SendMailTemplateType templateType);
}
