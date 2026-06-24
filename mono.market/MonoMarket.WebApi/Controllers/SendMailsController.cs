using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonoMarket.Application.SendMails.Dtos;
using MonoMarket.Application.SendMails.Queries.GetByInvoiceNumber;

namespace MonoMarket.WebApi.Controllers;

/// <summary>
/// Endpoints de consulta de los correos de recordatorio enviados (cabecera SendMails).
/// </summary>
[ApiController]
[Route("api/send-mails")]
[Produces("application/json")]
public class SendMailsController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Lista los correos asociados a una factura.
    /// </summary>
    /// <param name="invoiceNumber">Número de la factura, obligatorio.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <response code="200">Listado de correos de la factura (puede ser vacío).</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<SendMailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByInvoiceNumber([FromQuery] string invoiceNumber, CancellationToken cancellationToken)
    {
        List<SendMailDto> sendMails = await sender.Send(new GetByInvoiceNumberQuery(invoiceNumber), cancellationToken);
        return Ok(sendMails);
    }
}
