using MediatR;
using MonoMarket.Application.Invoices.Dtos;
using MonoMarket.Commons.Enums;

namespace MonoMarket.Application.Invoices.Queries.GetByClientOrStatus;

/// <summary>
/// Lista facturas, filtrando opcionalmente por cliente y/o estado. Sin filtros, lista todas.
/// </summary>
public record GetByClientOrStatusQuery(string? ClientIdentification, InvoiceStatus? Status) : IRequest<List<InvoiceDto>>;
