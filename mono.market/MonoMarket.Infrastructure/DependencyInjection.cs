using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MonoMarket.Application.Clients.Repositories;
using MonoMarket.Application.Common.Email;
using MonoMarket.Application.Invoices.Repositories;
using MonoMarket.Application.SendMails.Repositories;
using MonoMarket.Infrastructure.Configuration;
using MonoMarket.Infrastructure.Email;
using MonoMarket.Infrastructure.Persistence;
using MonoMarket.Infrastructure.Repositories;

namespace MonoMarket.Infrastructure;

/// <summary>
/// Registro de servicios de la capa Infrastructure en el contenedor de dependencias.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra la configuración de MongoDB, el contexto, los repositorios y el envío de correo.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
        services.AddSingleton<MongoDbContext>();
        services.AddSingleton<IInvoiceRepository, InvoiceRepository>();
        services.AddSingleton<IClientRepository, ClientRepository>();

        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        services.Configure<EmailRetrySettings>(configuration.GetSection("EmailRetrySettings"));
        services.AddSingleton<ISendMailRepository, SendMailRepository>();
        services.AddSingleton<ISendMailLogRepository, SendMailLogRepository>();
        services.AddSingleton<IEmailSender>(sp => new ResilientEmailSender(
            new SmtpEmailSender(sp.GetRequiredService<IOptions<SmtpSettings>>()),
            sp.GetRequiredService<ISendMailRepository>(),
            sp.GetRequiredService<ISendMailLogRepository>(),
            sp.GetRequiredService<IOptions<EmailRetrySettings>>().Value,
            sp.GetRequiredService<ILogger<ResilientEmailSender>>()));

        return services;
    }
}
