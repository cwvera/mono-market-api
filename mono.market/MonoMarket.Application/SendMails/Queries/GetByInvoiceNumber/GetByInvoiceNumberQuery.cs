using MediatR;
using MonoMarket.Application.SendMails.Dtos;

namespace MonoMarket.Application.SendMails.Queries.GetByInvoiceNumber;

/// <summary>
/// Lista los correos (cabeceras SendMails) asociados a una factura.
/// </summary>
public record GetByInvoiceNumberQuery(string InvoiceNumber) : IRequest<List<SendMailDto>>;
