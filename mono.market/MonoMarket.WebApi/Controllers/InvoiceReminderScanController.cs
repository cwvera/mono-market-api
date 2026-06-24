using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan;
using MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan.Commands;
using MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan.Queries;

namespace MonoMarket.WebApi.Controllers;

/// <summary>
/// Control manual del job en background que escanea facturas en PrimerRecordatorio o SegundoRecordatorio.
/// </summary>
[ApiController]
[Route("api/jobs/invoice-reminder-scan")]
[Produces("application/json")]
public class InvoiceReminderScanController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Inicia una corrida nueva. Si ya hay una corriendo, no hace nada.
    /// </summary>
    /// <response code="200">Se inició una corrida nueva.</response>
    /// <response code="409">Ya había una corrida en curso.</response>
    [HttpPost("start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Start(CancellationToken cancellationToken)
    {
        bool started = await sender.Send(new StartInvoiceReminderScanCommand(), cancellationToken);
        return started ? Ok() : Conflict("Ya hay una corrida en curso.");
    }

    /// <summary>
    /// Detiene la corrida en curso, si hay una.
    /// </summary>
    /// <response code="204">Se envió la señal de detener (o no había nada corriendo).</response>
    [HttpPost("stop")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Stop(CancellationToken cancellationToken)
    {
        await sender.Send(new StopInvoiceReminderScanCommand(), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Consulta el avance de la corrida actual o la última.
    /// </summary>
    /// <response code="200">Avance de la corrida.</response>
    [HttpGet("status")]
    [ProducesResponseType(typeof(InvoiceReminderScanProgress), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatus(CancellationToken cancellationToken)
    {
        InvoiceReminderScanProgress progress = await sender.Send(new GetInvoiceReminderScanStatusQuery(), cancellationToken);
        return Ok(progress);
    }
}
