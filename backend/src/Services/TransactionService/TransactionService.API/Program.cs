using FluentValidation;
using Microsoft.OpenApi;
using Serilog;
using TransactionService.API.Extensions;
using TransactionService.API.Middlewares;
using TransactionService.Application;
using TransactionService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/transactions-service.logs", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // new lib

try
{
    Log.Information("Starting TransactionService.API...");

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Transaction Service API",
            Version = "v1",
            Description = "API to manage transactions"
        });
    });

    builder.Services.ConfigureProductServiceClient(builder.Configuration);
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    
    
    
    var app = builder.Build();
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Service API v1");
            options.RoutePrefix = string.Empty;
        });
    }
    
    app.UseMiddleware<ExceptionHandlingMiddleware>(); // I should centralize

    app.UseMiddleware<RequestLoggingMiddleware>();

    app.UseHttpsRedirection();
    
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("TransactionService.API started successfully");
    
    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application fatal error");
}