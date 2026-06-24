using Mapster;
using MediatR;
using MonoMarket.Application.SendMails.Dtos;
using MonoMarket.Application.SendMails.Repositories;
using MonoMarket.Domain.Entities;

namespace MonoMarket.Application.SendMails.Queries.GetLogsBySendMailId;

/// <summary>
/// Maneja la consulta de intentos de envío por Id de la cabecera SendMail.
/// </summary>
public class GetLogsBySendMailIdQueryHandler(ISendMailLogRepository sendMailLogRepository)
    : IRequestHandler<GetLogsBySendMailIdQuery, List<SendMailLogDto>>
{
    /// <summary>
    /// Obtiene los intentos del repositorio y los mapea a DTO.
    /// </summary>
    public async Task<List<SendMailLogDto>> Handle(GetLogsBySendMailIdQuery request, CancellationToken cancellationToken)
    {
        List<SendMailLog> logs = await sendMailLogRepository.GetBySendMailIdAsync(request.SendMailId, cancellationToken);

        return logs.Select(l => l.Adapt<SendMailLogDto>()).ToList();
    }
}
