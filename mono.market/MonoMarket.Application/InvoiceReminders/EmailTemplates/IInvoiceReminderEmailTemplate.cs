using MonoMarket.Commons.Enums;

namespace MonoMarket.Application.InvoiceReminders.EmailTemplates;

/// <summary>
/// Construye el asunto y el cuerpo del correo para una plantilla de recordatorio.
/// Cada plantilla es un producto de esta Abstract Factory, igual que los handlers de etapa en
/// <see cref="StageHandlers.IInvoiceReminderStageHandler"/>, pero esta familia varía por tipo de plantilla en vez de por estado.
/// </summary>
public interface IInvoiceReminderEmailTemplate
{
    /// <summary>
    /// Tipo de plantilla que construye esta implementación.
    /// </summary>
    SendMailTemplateType TemplateType { get; }

    /// <summary>
    /// Construye el asunto y el cuerpo del correo a partir del aviso.
    /// </summary>
    EmailContent Build(InvoiceReminderNotice notice);
}
