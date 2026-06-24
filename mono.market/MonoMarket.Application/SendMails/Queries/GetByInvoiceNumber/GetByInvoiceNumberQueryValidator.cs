using FluentValidation;

namespace MonoMarket.Application.SendMails.Queries.GetByInvoiceNumber;

/// <summary>
/// Valida que la consulta venga con el número de factura obligatorio.
/// </summary>
public class GetByInvoiceNumberQueryValidator : AbstractValidator<GetByInvoiceNumberQuery>
{
    /// <summary>
    /// Define las reglas de validación de la consulta.
    /// </summary>
    public GetByInvoiceNumberQueryValidator()
    {
        RuleFor(x => x.InvoiceNumber).NotEmpty();
    }
}
