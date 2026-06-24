using FluentValidation;
using System.Net;
using System.Text.Json;

namespace MonoMarket.WebApi.Middleware;

/// <summary>
/// Middleware para manejo centralizado de excepciones y errores de validación.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Ejecuta el siguiente middleware y captura excepciones para responder en JSON.
    /// </summary>
    /// <param name="context">Contexto HTTP de la solicitud actual.</param>
    /// <returns>Tarea que representa la ejecución del pipeline.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException vex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            var payload = new
            {
                message = "Error de validación.",
                errors = vex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error interno inesperado.");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            string[] errors = [ex.Message];
            var payload = new
            {
                message = $"Error interno inesperado.",
                errors = errors.Select(e => new { ErrorMessage = e })
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
