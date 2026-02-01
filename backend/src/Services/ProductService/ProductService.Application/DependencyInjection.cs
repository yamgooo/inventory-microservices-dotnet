using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Behaviors;
using ProductService.Application.Commands.CreateProduct;

namespace ProductService.Application;


public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var applicationAssembly = typeof(CreateProductCommand).Assembly;

        // mediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
        
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>)); // for use fluent validation TODO: add autommaper 
        });

        // fluent validation
        services.AddValidatorsFromAssembly(applicationAssembly);

        return services;
    }
}