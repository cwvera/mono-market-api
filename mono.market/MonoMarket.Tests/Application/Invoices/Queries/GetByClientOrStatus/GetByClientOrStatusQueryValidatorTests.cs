using FluentValidation.Results;
using MonoMarket.Application.Invoices.Queries.GetByClientOrStatus;
using MonoMarket.Commons.Enums;

namespace MonoMarket.Tests.Application.Invoices.Queries.GetByClientOrStatus;

/// <summary>
/// Pruebas del validador de GetByClientOrStatusQuery.
/// </summary>
public class GetByClientOrStatusQueryValidatorTests
{
    private readonly GetByClientOrStatusQueryValidator _validator = new();

    /// <summary>
    /// Verifica que cliente y estado válidos pasen la validación.
    /// </summary>
    [Fact]
    public void Validate_WithValidClientAndStatus_IsValid()
    {
        GetByClientOrStatusQuery query = new("900123456-7", InvoiceStatus.SecondReminder);

        ValidationResult result = _validator.Validate(query);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que la consulta sea válida cuando solo se filtra por estado, sin cliente.
    /// </summary>
    [Fact]
    public void Validate_WithoutClientIdentification_IsValid()
    {
        GetByClientOrStatusQuery query = new(string.Empty, InvoiceStatus.SecondReminder);

        ValidationResult result = _validator.Validate(query);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que la consulta sea válida cuando solo se filtra por cliente, sin estado.
    /// </summary>
    [Fact]
    public void Validate_WithoutStatus_IsValid()
    {
        GetByClientOrStatusQuery query = new("900123456-7", null);

        ValidationResult result = _validator.Validate(query);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que la consulta sea válida cuando no se envía ningún filtro (lista todo).
    /// </summary>
    [Fact]
    public void Validate_WithoutAnyFilter_IsValid()
    {
        GetByClientOrStatusQuery query = new(null, null);

        ValidationResult result = _validator.Validate(query);

        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifica que la consulta sea inválida cuando el estado no es un valor definido del enum.
    /// </summary>
    [Fact]
    public void Validate_WithStatusOutsideEnum_IsInvalid()
    {
        GetByClientOrStatusQuery query = new("900123456-7", (InvoiceStatus)99);

        ValidationResult result = _validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetByClientOrStatusQuery.Status));
    }
}
