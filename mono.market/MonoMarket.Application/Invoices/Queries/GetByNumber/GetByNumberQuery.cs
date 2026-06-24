using MediatR;
using MonoMarket.Application.Invoices.Dtos;

namespace MonoMarket.Application.Invoices.Queries.GetByNumber;

/// <summary>
/// Consulta una factura por su número.
/// </summary>
public record GetByNumberQuery(string InvoiceNumber) : IRequest<InvoiceDto?>;
