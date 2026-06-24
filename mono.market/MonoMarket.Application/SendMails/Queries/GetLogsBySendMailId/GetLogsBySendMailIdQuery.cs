using MediatR;
using MonoMarket.Application.SendMails.Dtos;

namespace MonoMarket.Application.SendMails.Queries.GetLogsBySendMailId;

/// <summary>
/// Lista los intentos de envío (SendMailsLog) de una cabecera SendMail.
/// </summary>
public record GetLogsBySendMailIdQuery(string SendMailId) : IRequest<List<SendMailLogDto>>;
