using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using MonoMarket.Application.Invoices.Dtos;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MonoMarket.WebApi.Swagger;

/// <summary>
/// Agrega un ejemplo real al esquema de InvoiceDto en la documentación de Swagger.
/// </summary>
public class InvoiceDtoExampleSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Rellena el ejemplo del esquema cuando el tipo es InvoiceDto.
    /// </summary>
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(InvoiceDto) || schema is not OpenApiSchema concreteSchema)
        {
            return;
        }

        concreteSchema.Example = JsonNode.Parse("""
        {
          "id": "100000000000000000000001",
          "invoiceNumber": "FAC-001-2025",
          "clientIdentification": "900123456-7",
          "amount": 1500000,
          "issueDate": "2025-02-15T00:00:00Z",
          "status": 1,
          "lastReminderSentAt": null,
          "reminderCount": 0
        }
        """);
    }
}
