using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AndreyevInterview;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options => options.AddPolicy(
            "CorsPolicy",
            builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin())
        );

        //services.AddCors(options =>
        //{
        //    options.AddPolicy("CorsPolicy",
        //                          builder =>
        //                          {
        //                              builder.WithOrigins("*");
        //                          });
        //});

        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddAutoMapper(typeof(Startup));


        // Register DbContext
        var dbPath = Environment.CurrentDirectory + "andreyev_interview.db";
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

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}