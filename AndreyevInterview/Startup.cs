using AndreyevInterview.Extensions;
using AndreyevInterview.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AndreyevInterview;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Register Configuration settings
        services.Configure<AppSettings>(Configuration);
        services.AddScoped(cfg => cfg.GetService<IOptions<AppSettings>>()!.Value);

        // Register JavaScript Cross Origin Resource Sharing Policy
        services.AddCors(options => options.AddPolicy(
            "CorsPolicy",
            builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin())
        );

        // Register Services
        services.RegisterServices(Configuration);

        // Register Controllers and Swagger
        services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Register DbContext
        var dbName = Configuration["DatabaseName"];
        var dbPath = Environment.CurrentDirectory + dbName + ".db";
        services.AddDbContext<AIDbContext>(options =>
            options.UseSqlite(@"Data Source=" + dbPath)
        );
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseCors("CorsPolicy");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AndreyvInterview");
                c.RoutePrefix = "swagger";
            });

        }

        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseHttpsRedirection();

        app.UseRouting();

        // Not using Authorization
        //app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}