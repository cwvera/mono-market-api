using FluentValidation.Results;
using MonoMarket.Application.SendMails.Queries.GetLogsBySendMailId;

namespace MonoMarket.Tests.Application.SendMails.Queries.GetLogsBySendMailId;

/// <summary>
/// Pruebas del validador de GetLogsBySendMailIdQuery.
/// </summary>
public class GetLogsBySendMailIdQueryValidatorTests
{
    private readonly GetLogsBySendMailIdQueryValidator _validator = new();

    /// <summary>
    /// Verifica que un Id de cabecera no vacío sea válido.
    /// </summary>
    [Fact]
    public void Validate_WithSendMailId_IsValid()
    {
        GetLogsBySendMailIdQuery query = new("sendmail-1");

        ValidationResult result = _validator.Validate(query);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que un Id de cabecera vacío sea inválido.
    /// </summary>
    [Fact]
    public void Validate_WithEmptySendMailId_IsInvalid()
    {
        GetLogsBySendMailIdQuery query = new(string.Empty);

        ValidationResult result = _validator.Validate(query);

        Assert.False(result.IsValid);
    }
}
