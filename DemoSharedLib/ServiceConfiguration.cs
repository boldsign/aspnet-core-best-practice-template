namespace DemoSharedLib;

using System.Text;
using System.Text.Json;
using AspNetCoreRateLimit;
using DemoSharedLib.Policy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

public static class ServiceConfiguration
{
    public static void AddPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(
            options =>
            {
                options.AddPolicy(
                    AdminRolePolicy.PolicyName,
                    policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.Requirements.Add(new AdminRolePolicy());
                    });
            });
    }

    public static void AddRateLimit(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.Configure<IpRateLimitOptions>(
            x =>
            {
                x.HttpStatusCode = 429;

                x.GeneralRules =
                [
                    new RateLimitRule()
                    {
                        Endpoint = "*",
                        Period = "10s",
                        Limit = 10,
                    },
                ];

                var serialize = JsonSerializer.Serialize(
                    new
                    {
                        message = "You have reached the maximum number of requests allowed",
                        details = "Maximum allowed: {0} requests per {1}. Please try again in {2} second(s).",
                    });

                x.QuotaExceededResponse = new QuotaExceededResponse()
                {
                    StatusCode = 429,
                    ContentType = "application/json",
                    Content = $"{{{serialize}}}",
                };
            });

        // demo in-memory rate limiting
        services.AddInMemoryRateLimiting();
        services.AddMvc();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }

    public static void AddAuth(this IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
            .AddJwtBearer(
                o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = AuthParameters.Issuer,
                        ValidAudience = AuthParameters.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthParameters.SecurityKey)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                    };
                });
    }

    public static void UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
