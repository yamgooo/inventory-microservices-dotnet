using ProductService.API.Middlewares;
using ProductService.Application;
using ProductService.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/product-service-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting ProductService.API...");

    builder.Services.AddControllers();
    
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new()
        {
            Title = "Product Service API",
            Version = "v1",
            Description = "API for managing products in the inventory system",
            Contact = new()
            {
                Name = "Erik Portilla",
                Email = "erik@example.com"
            }
        });
    });

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins(
                    "http://localhost:4200"   // Angular
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    builder.Services.AddApplicationServices(); 
    builder.Services.AddInfrastructureServices(builder.Configuration); 
    
    var app = builder.Build();
    
    // Development only
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Service API v1");
            options.RoutePrefix = string.Empty;
        });
    }

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseMiddleware<RequestLoggingMiddleware>();

    app.UseHttpsRedirection();
    
    app.UseCors("AllowFrontend");
    
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("ProductService.API started successfully on {Url}", 
        app.Configuration["Urls"] ?? "http://localhost:5001");
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}