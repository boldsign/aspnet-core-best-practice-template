﻿namespace DemoApi.Versioning;

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
    /// </summary>
    /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in this.provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    /// <summary>
    /// CreateInfoForApiVersion.
    /// </summary>
    /// <param name="description">description.</param>
    /// <returns>OpenApiInfo.</returns>
    internal static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "Demo API",
            Version = description.ApiVersion.ToString(),
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}
