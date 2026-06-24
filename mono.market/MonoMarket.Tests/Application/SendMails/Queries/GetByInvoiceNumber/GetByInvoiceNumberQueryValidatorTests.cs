using FluentValidation.Results;
using MonoMarket.Application.SendMails.Queries.GetByInvoiceNumber;

namespace MonoMarket.Tests.Application.SendMails.Queries.GetByInvoiceNumber;

/// <summary>
/// Pruebas del validador de GetByInvoiceNumberQuery.
/// </summary>
public class GetByInvoiceNumberQueryValidatorTests
{
    private readonly GetByInvoiceNumberQueryValidator _validator = new();

    /// <summary>
    /// Verifica que un número de factura no vacío sea válido.
    /// </summary>
    [Fact]
    public void Validate_WithInvoiceNumber_IsValid()
    {
        GetByInvoiceNumberQuery query = new("FAC-001-2025");

        ValidationResult result = _validator.Validate(query);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que un número de factura vacío sea inválido.
    /// </summary>
    [Fact]
    public void Validate_WithEmptyInvoiceNumber_IsInvalid()
    {
        GetByInvoiceNumberQuery query = new(string.Empty);

        ValidationResult result = _validator.Validate(query);

        Assert.False(result.IsValid);
    }
}
