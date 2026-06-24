using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using MonoMarket.Application;
using MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan;
using MonoMarket.Infrastructure;
using MonoMarket.WebApi.BackgroundServices;
using MonoMarket.WebApi.Middleware;
using MonoMarket.WebApi.Swagger;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<InvoiceReminderScanSettings>(builder.Configuration.GetSection("InvoiceReminderScanSettings"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<InvoiceReminderScanSettings>>().Value);
builder.Services.AddHostedService<InvoiceReminderScanHostedService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MonoMarket API",
        Version = "v1",
        Description = "API de facturación con recordatorios automáticos. Persistencia en MongoDB, CQRS con MediatR y validación con FluentValidation.",
    });

    string webApiXmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(webApiXmlPath))
    {
        options.IncludeXmlComments(webApiXmlPath);
    }

    string applicationXmlPath = Path.Combine(AppContext.BaseDirectory, "MonoMarket.Application.xml");
    if (File.Exists(applicationXmlPath))
    {
        options.IncludeXmlComments(applicationXmlPath);
    }

    options.SchemaFilter<InvoiceDtoExampleSchemaFilter>();
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "MonoMarket API v1");
    options.DocumentTitle = "MonoMarket API Docs";
    options.DefaultModelsExpandDepth(-1);
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    options.EnableDeepLinking();
    options.EnableFilter();
    options.EnableTryItOutByDefault();
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
