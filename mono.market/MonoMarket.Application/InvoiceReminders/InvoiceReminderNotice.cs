using MonoMarket.Commons.Enums;

namespace MonoMarket.Application.InvoiceReminders;

/// <summary>
/// Aviso que resulta de evaluar una factura contra su etapa de recordatorio.
/// Es el "producto" que construye cada <see cref="StageHandlers.IInvoiceReminderStageHandler"/>
/// y el insumo que usa <see cref="IInvoiceReminderEmailDispatcher"/> para armar el correo real.
/// </summary>
public class InvoiceReminderNotice
{
    /// <summary>
    /// Datos de factura y cliente evaluados.
    /// </summary>
    public InvoiceReminderCandidate Candidate { get; set; } = new();

    /// <summary>
    /// Estado del que parte la factura.
    /// </summary>
    public InvoiceStatus FromStatus { get; set; }

    /// <summary>
    /// Estado al que debería avanzar la factura.
    /// </summary>
    public InvoiceStatus NextStatus { get; set; }

    /// <summary>
    /// Plantilla de correo a usar. Null significa transición silenciosa (sin correo),
    /// como pasa hoy al entrar a PrimerRecordatorio según el modelo de datos original.
    /// </summary>
    public SendMailTemplateType? TemplateType { get; set; }
}
