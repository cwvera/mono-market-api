using MonoMarket.Application.InvoiceReminders;
using MonoMarket.Commons.Enums;

namespace MonoMarket.Application.InvoiceReminders.StageHandlers;

/// <summary>
/// Maneja una etapa del flujo de recordatorios: sabe desde qué estado parte, a cuál avanza,
/// cuántos días desde la emisión hacen falta, y cómo armar el aviso correspondiente.
/// Cada etapa es una familia de reglas + plantilla que cambian juntas (por eso Abstract Factory).
/// </summary>
public interface IInvoiceReminderStageHandler
{
    /// <summary>
    /// Estado del que parte esta etapa.
    /// </summary>
    InvoiceStatus FromStatus { get; }

    /// <summary>
    /// Estado al que avanza la factura si ya venció el plazo de esta etapa.
    /// </summary>
    InvoiceStatus NextStatus { get; }

    /// <summary>
    /// Días desde la fecha de emisión (acumulados, no desde que entró a esta etapa) que hacen falta para avanzar.
    /// </summary>
    int ThresholdDays { get; }

    /// <summary>
    /// Indica si la factura ya cumplió el plazo de esta etapa. Compara solo fechas, sin hora.
    /// </summary>
    bool IsDue(DateOnly issueDate, DateOnly today);

    /// <summary>
    /// Construye el aviso a partir de los datos fusionados de factura y cliente.
    /// </summary>
    InvoiceReminderNotice BuildNotice(InvoiceReminderCandidate candidate);
}
