using Mapster;
using MediatR;
using MonoMarket.Application.SendMails.Dtos;
using MonoMarket.Application.SendMails.Repositories;
using MonoMarket.Domain.Entities;

namespace MonoMarket.Application.SendMails.Queries.GetByInvoiceNumber;

/// <summary>
/// Maneja la consulta de correos por número de factura.
/// </summary>
public class GetByInvoiceNumberQueryHandler(ISendMailRepository sendMailRepository)
    : IRequestHandler<GetByInvoiceNumberQuery, List<SendMailDto>>
{
    /// <summary>
    /// Obtiene los correos del repositorio y los mapea a DTO.
    /// </summary>
    public async Task<List<SendMailDto>> Handle(GetByInvoiceNumberQuery request, CancellationToken cancellationToken)
    {
        List<SendMail> sendMails = await sendMailRepository.GetByInvoiceNumberAsync(request.InvoiceNumber, cancellationToken);

        return sendMails.Select(s => s.Adapt<SendMailDto>()).ToList();
    }
}
