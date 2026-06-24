using FluentValidation;

namespace MonoMarket.Application.SendMails.Queries.GetLogsBySendMailId;

/// <summary>
/// Valida que la consulta venga con el Id de la cabecera SendMail obligatorio.
/// </summary>
public class GetLogsBySendMailIdQueryValidator : AbstractValidator<GetLogsBySendMailIdQuery>
{
    /// <summary>
    /// Define las reglas de validación de la consulta.
    /// </summary>
    public GetLogsBySendMailIdQueryValidator()
    {
        RuleFor(x => x.SendMailId).NotEmpty();
    }
}
