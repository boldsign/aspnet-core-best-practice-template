namespace DemoApi;

using AspNetCoreRateLimit;
using DemoApi.Middleware;
using DemoApi.Service;
using DemoSharedLib;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddControllers();
        services.AddSwaggerOpenApi();
        services.AddRateLimit();
        services.AddAuth();
        services.AddPolicies();

        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IGrpcClientService, GrpcClientService>();

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });
    }

    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescription)
    {
        app.UseSwaggerOpenApi(env, apiVersionDescription);
        app.UseIpRateLimiting();

        // Configure the HTTP request pipeline.
        if (!env.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        app.UseRouting();
        app.UseAuth();
        app.UseMiddleware<UserEnrichMiddleware>();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
