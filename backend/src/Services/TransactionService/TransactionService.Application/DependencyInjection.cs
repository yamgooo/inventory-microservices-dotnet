using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Behaviors;
using TransactionService.Application.Commands.CreateTransaction;

namespace TransactionService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var applicationAssembly = typeof(CreateTransactionCommand).Assembly;
        
        // mediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
        
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>)); // for use fluent validation TODO: I should centralize this
        });

        services.AddValidatorsFromAssembly(applicationAssembly);

        return services;
    }
}