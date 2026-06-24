using FluentValidation;
using MediatR;
using MonoMarket.Application.Common.Behaviors;

namespace MonoMarket.Tests.Application.Common.Behaviors;

/// <summary>
/// Request de prueba usado solo para probar ValidationBehavior.
/// </summary>
public record SampleRequest(string Value) : IRequest<string>;

/// <summary>
/// Validador de prueba que exige que Value no esté vacío.
/// </summary>
public class SampleRequestValidator : AbstractValidator<SampleRequest>
{
    /// <summary>
    /// Define la regla de validación de prueba.
    /// </summary>
    public SampleRequestValidator()
    {
        RuleFor(x => x.Value).NotEmpty();
    }
}

/// <summary>
/// Pruebas del comportamiento de validación del pipeline de MediatR.
/// </summary>
public class ValidationBehaviorTests
{
    /// <summary>
    /// Verifica que, sin validadores registrados, el pipeline continúe normalmente.
    /// </summary>
    [Fact]
    public async Task Handle_WithoutValidators_ContinuesPipeline()
    {
        ValidationBehavior<SampleRequest, string> behavior = new([]);
        RequestHandlerDelegate<string> next = _ => Task.FromResult("ok");

        string result = await behavior.Handle(new SampleRequest("algo"), next, CancellationToken.None);

        Assert.Equal("ok", result);
    }

    /// <summary>
    /// Verifica que, con un request válido, el pipeline continúe normalmente.
    /// </summary>
    [Fact]
    public async Task Handle_ValidRequest_ContinuesPipeline()
    {
        IValidator<SampleRequest>[] validators = [new SampleRequestValidator()];
        ValidationBehavior<SampleRequest, string> behavior = new(validators);
        RequestHandlerDelegate<string> next = _ => Task.FromResult("ok");

        string result = await behavior.Handle(new SampleRequest("algo"), next, CancellationToken.None);

        Assert.Equal("ok", result);
    }

    /// <summary>
    /// Verifica que, con un request inválido, se lance ValidationException y no se llame al siguiente paso del pipeline.
    /// </summary>
    [Fact]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        IValidator<SampleRequest>[] validators = [new SampleRequestValidator()];
        ValidationBehavior<SampleRequest, string> behavior = new(validators);
        bool nextWasCalled = false;
        RequestHandlerDelegate<string> next = _ =>
        {
            nextWasCalled = true;
            return Task.FromResult("ok");
        };

        await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(new SampleRequest(string.Empty), next, CancellationToken.None));
        Assert.False(nextWasCalled);
    }
}
