using MediatR;
using Microsoft.AspNetCore.Mvc;
using MonoMarket.Application.Invoices.Dtos;
using MonoMarket.Application.Invoices.Queries.GetByClientOrStatus;
using MonoMarket.Application.Invoices.Queries.GetByNumber;
using MonoMarket.Commons.Enums;

namespace MonoMarket.WebApi.Controllers;

/// <summary>
/// Endpoints de consulta de facturas.
/// </summary>
[ApiController]
[Route("api/invoices")]
[Produces("application/json")]
public class InvoicesController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Lista facturas, filtrando opcionalmente por cliente y/o estado. Sin parámetros, lista todas.
    /// </summary>
    /// <param name="clientIdentification">Identificación del cliente (NIT/Cédula), opcional; busca por coincidencia parcial.</param>
    /// <param name="status">Estado de la factura, opcional; si se envía, debe ser un valor válido del enum (coincidencia exacta).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <response code="200">Listado de facturas que cumplen los filtros (puede ser vacío).</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByClientOrStatus([FromQuery] string? clientIdentification, [FromQuery] InvoiceStatus? status, CancellationToken cancellationToken)
    {
        List<InvoiceDto> invoices = await sender.Send(new GetByClientOrStatusQuery(clientIdentification, status), cancellationToken);
        return Ok(invoices);
    }

    /// <summary>
    /// Obtiene una factura por su número.
    /// </summary>
    /// <param name="invoiceNumber">Número único de la factura, por ejemplo "FAC-001-2025".</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <response code="200">La factura solicitada.</response>
    /// <response code="404">No existe una factura con ese número.</response>
    [HttpGet("{invoiceNumber}")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByNumber(string invoiceNumber, CancellationToken cancellationToken)
    {
        InvoiceDto? invoice = await sender.Send(new GetByNumberQuery(invoiceNumber), cancellationToken);
        return invoice is null ? NotFound() : Ok(invoice);
    }
}
