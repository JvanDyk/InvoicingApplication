using AndreyevInterview.Exceptions;
using AndreyevInterview.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AndreyevInterview.Extensions;

public static class RegisterServicesExtensions
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Services
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IInvoicesService, InvoicesService>();

        // Middleware
        services.AddScoped<ErrorHandlingMiddleware>();
        services.AddScoped<RequestLoggingMiddleware>();

        // Handlers
        services.AddScoped<ExceptionHandler>();
    }
}
