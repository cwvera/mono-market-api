using MediatR;

namespace MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan.Commands;

/// <summary>
/// Inicia una corrida del job de escaneo de recordatorios. Devuelve false si ya había una corriendo.
/// </summary>
public record StartInvoiceReminderScanCommand : IRequest<bool>;
