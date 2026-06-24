using MediatR;

namespace MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan.Commands;

/// <summary>
/// Detiene la corrida actual del job de escaneo de recordatorios, si hay una en curso.
/// </summary>
public record StopInvoiceReminderScanCommand : IRequest;
