using FluentValidation;

namespace MonoMarket.Application.Invoices.Queries.GetByClientOrStatus;

/// <summary>
/// Valida que, si se envía un estado, sea un valor válido del enum. Cliente y estado son filtros opcionales.
/// </summary>
public class GetByClientOrStatusQueryValidator : AbstractValidator<GetByClientOrStatusQuery>
{
    /// <summary>
    /// Define las reglas de validación de la consulta.
    /// </summary>
    public GetByClientOrStatusQueryValidator()
    {
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
    }
}
