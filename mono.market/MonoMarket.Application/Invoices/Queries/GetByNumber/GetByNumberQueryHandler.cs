using Mapster;
using MediatR;
using MonoMarket.Application.Invoices.Dtos;
using MonoMarket.Application.Invoices.Repositories;
using MonoMarket.Domain.Entities;

namespace MonoMarket.Application.Invoices.Queries.GetByNumber;

/// <summary>
/// Maneja la consulta de una factura por su número.
/// </summary>
public class GetByNumberQueryHandler(IInvoiceRepository invoiceRepository) : IRequestHandler<GetByNumberQuery, InvoiceDto?>
{
    /// <summary>
    /// Busca la factura en el repositorio y la mapea a DTO. Devuelve null si no existe.
    /// </summary>
    public async Task<InvoiceDto?> Handle(GetByNumberQuery request, CancellationToken cancellationToken)
    {
        Invoice? invoice = await invoiceRepository.GetByNumberAsync(request.InvoiceNumber, cancellationToken);

        return invoice?.Adapt<InvoiceDto>();
    }
}
