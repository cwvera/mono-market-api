using FluentValidation;

namespace MonoMarket.Application.Invoices.Queries.GetByNumber;

/// <summary>
/// Valida que el número de factura consultado no venga vacío.
/// </summary>
public class GetByNumberQueryValidator : AbstractValidator<GetByNumberQuery>
{
    /// <summary>
    /// Define las reglas de validación de la consulta.
    /// </summary>
    public GetByNumberQueryValidator()
    {
        RuleFor(x => x.InvoiceNumber).NotEmpty();
    }
}
