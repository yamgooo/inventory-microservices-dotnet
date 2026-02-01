using TransactionService.Infrastructure.HttpClients;

namespace TransactionService.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureProductServiceClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var productServiceUrl = configuration["ServiceUrls:ProductService"];

        if (string.IsNullOrWhiteSpace(productServiceUrl))
        {
            throw new InvalidOperationException(
                "ProductService URL is not configured. " +
                "Please add 'ServiceUrls:ProductService' to appsettings.json");
        }

        if (!Uri.TryCreate(productServiceUrl, UriKind.Absolute, out var productServiceUri))
        {
            throw new InvalidOperationException(
                $"Invalid ProductService URL: {productServiceUrl}");
        }

        services.AddHttpClient<IProductServiceClient, ProductServiceClient>(client =>
            {
                client.BaseAddress = productServiceUri;
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "TransactionService/1.0");
                client.Timeout = TimeSpan.FromSeconds(30);
            });
    }
}