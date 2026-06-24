using Mapster;
using MediatR;
using MonoMarket.Application.Invoices.Dtos;
using MonoMarket.Application.Invoices.Repositories;
using MonoMarket.Domain.Entities;

namespace MonoMarket.Application.Invoices.Queries.GetByClientOrStatus;

/// <summary>
/// Maneja la consulta de listado de facturas con filtros opcionales.
/// </summary>
public class GetByClientOrStatusQueryHandler(IInvoiceRepository invoiceRepository) : IRequestHandler<GetByClientOrStatusQuery, List<InvoiceDto>>
{
    /// <summary>
    /// Obtiene las facturas del repositorio y las mapea a DTO.
    /// </summary>
    public async Task<List<InvoiceDto>> Handle(GetByClientOrStatusQuery request, CancellationToken cancellationToken)
    {
        List<Invoice> invoices = await invoiceRepository.GetByClientOrStatusAsync(request.ClientIdentification, (int?)request.Status, cancellationToken);

        return invoices.Select(i => i.Adapt<InvoiceDto>()).ToList();
    }
}
