namespace DemoApi;

using System.Reflection;
using DemoApi.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

public static class SwaggerConfiguration
{
    public static void AddSwaggerOpenApi(this IServiceCollection services)
    {
        var basePath = AppContext.BaseDirectory;
        var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";

        services.AddSwaggerGen(
            options =>
            {
                options.EnableAnnotations();
                options.IncludeXmlComments(Path.Combine(basePath, fileName));

                options.AddSecurityDefinition(
                    "Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",
                    });

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                                Scheme = "oauth2",
                                Name = "Token",
                                In = ParameterLocation.Header,
                                BearerFormat = "JWT",
                            },
                            new List<string>()
                        },
                    });
            });

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddApiVersioning(options => options.ReportApiVersions = true);
        services.AddVersionedApiExplorer(
            options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        services.AddMvcCore(options => { options.Conventions.Add(new RoutePrefixConvention(new RouteAttribute("v{version:apiVersion}"))); });
    }

    public static void UseSwaggerOpenApi(this IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescription)
    {
        var scheme = env.IsDevelopment() ? "http" : "https";

        app.UseSwagger(SetupSwagger);
        app.UseSwaggerUI(SetupSwaggerUi);

        void SetupSwagger(SwaggerOptions c)
        {
            c.PreSerializeFilters.Add(
                (swaggerDoc, httpReq) => swaggerDoc.Servers = new List<OpenApiServer>
                {
                    new OpenApiServer
                    {
                        Url = $"{scheme}://{httpReq.Host.Value}",
                    },
                });
        }

        void SetupSwaggerUi(SwaggerUIOptions options)
        {
            // sort the version by its value to display it in the swagger api definition lists.
            var descriptions = apiVersionDescription.ApiVersionDescriptions.GroupBy(x => x.ApiVersion.MajorVersion)
                .SelectMany(x => x.OrderBy(y => y.ApiVersion.Status));

            // build a swagger endpoint for each discovered API version
            foreach (var description in descriptions)
            {
                options.SwaggerEndpoint($"{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }

            options.DefaultModelsExpandDepth(0);
            options.RoutePrefix = "swagger";
        }
    }
}
