using Microsoft.OpenApi.Models;

namespace FinancialAuditApi.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Financial Audit API",
                Version = "v1"
            });
        });

        return services;
    }

    public static void UseSwaggerInDevelopment(this IApplicationBuilder app)
    {
        var env = app.ApplicationServices.GetService<IWebHostEnvironment>();
        if (env == null || !env.IsDevelopment()) return;
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("v1/swagger.json", "Financial Audit API v1");
        });
    }
}