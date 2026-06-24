using FluentValidation.Results;
using MonoMarket.Application.Invoices.Queries.GetByNumber;

namespace MonoMarket.Tests.Application.Invoices.Queries.GetByNumber;

/// <summary>
/// Pruebas del validador de GetByNumberQuery.
/// </summary>
public class GetByNumberQueryValidatorTests
{
    private readonly GetByNumberQueryValidator _validator = new();

    /// <summary>
    /// Verifica que un número de factura no vacío sea válido.
    /// </summary>
    [Fact]
    public void Validate_WithInvoiceNumber_IsValid()
    {
        GetByNumberQuery query = new GetByNumberQuery("FAC-001-2025");

        ValidationResult result = _validator.Validate(query);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que un número de factura vacío sea inválido.
    /// </summary>
    [Fact]
    public void Validate_WithEmptyInvoiceNumber_IsInvalid()
    {
        GetByNumberQuery query = new GetByNumberQuery(string.Empty);

        ValidationResult result = _validator.Validate(query);

        Assert.False(result.IsValid);
    }
}
