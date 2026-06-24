using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MonoMarket.Application.Common.Behaviors;
using MonoMarket.Application.InvoiceReminders;
using MonoMarket.Application.InvoiceReminders.EmailTemplates;
using MonoMarket.Application.InvoiceReminders.Jobs.InvoiceReminderScan;
using MonoMarket.Application.InvoiceReminders.StageHandlers;

namespace MonoMarket.Application;

/// <summary>
/// Registro de servicios de la capa Application en el contenedor de dependencias.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra MediatR (con el comportamiento de validación) y los validadores de FluentValidation.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddSingleton<IInvoiceReminderStageHandler, PendingToFirstReminderStageHandler>();
        services.AddSingleton<IInvoiceReminderStageHandler, FirstReminderToSecondReminderStageHandler>();
        services.AddSingleton<IInvoiceReminderStageHandler, SecondReminderToDeactivatedStageHandler>();
        services.AddSingleton<IInvoiceReminderStageHandlerFactory, InvoiceReminderStageHandlerFactory>();
        services.AddSingleton<IInvoiceReminderNoticeProcessor, InvoiceReminderNoticeProcessor>();

        services.AddSingleton<IInvoiceReminderEmailTemplate, SecondReminderEmailTemplate>();
        services.AddSingleton<IInvoiceReminderEmailTemplate, DeactivationEmailTemplate>();
        services.AddSingleton<IInvoiceReminderEmailTemplateFactory, InvoiceReminderEmailTemplateFactory>();
        services.AddSingleton<IInvoiceReminderEmailDispatcher, InvoiceReminderEmailDispatcher>();

        services.AddSingleton<IInvoiceReminderScanJob, InvoiceReminderScanJob>();

        return services;
    }
}
