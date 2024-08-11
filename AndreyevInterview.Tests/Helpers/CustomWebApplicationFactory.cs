using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace AndreyevInterview.Tests.Helpers;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AIDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add an in-memory database for testing
            services.AddDbContext<AIDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });
        });

    }

    public HttpClient CreateClientWithBaseAddress(string baseAddress)
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri(baseAddress)
        });
    }
}
