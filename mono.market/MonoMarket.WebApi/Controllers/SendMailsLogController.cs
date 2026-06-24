using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonoMarket.Application.SendMails.Dtos;
using MonoMarket.Application.SendMails.Queries.GetLogsBySendMailId;

namespace MonoMarket.WebApi.Controllers;

/// <summary>
/// Endpoints de consulta del detalle de intentos de envío (SendMailsLog).
/// </summary>
[ApiController]
[Route("api/send-mails-log")]
[Produces("application/json")]
public class SendMailsLogController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Lista los intentos de envío de una cabecera SendMail.
    /// </summary>
    /// <param name="sendMailId">Id de la cabecera SendMail (su encabezado), obligatorio.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <response code="200">Listado de intentos de esa cabecera (puede ser vacío).</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<SendMailLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBySendMailId([FromQuery] string sendMailId, CancellationToken cancellationToken)
    {
        List<SendMailLogDto> logs = await sender.Send(new GetLogsBySendMailIdQuery(sendMailId), cancellationToken);
        return Ok(logs);
    }
}
