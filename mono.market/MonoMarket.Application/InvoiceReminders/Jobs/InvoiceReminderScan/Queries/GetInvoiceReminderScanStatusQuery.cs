using MediatR;

namespace MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan.Queries;

/// <summary>
/// Consulta el avance de la corrida actual o la última del job de escaneo de recordatorios.
/// </summary>
public record GetInvoiceReminderScanStatusQuery : IRequest<InvoiceReminderScanProgress>;
