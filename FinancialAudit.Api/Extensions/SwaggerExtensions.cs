using System.Reflection;
using Microsoft.OpenApi.Models;

namespace FinancialAuditApi.Extensions;

public static class SwaggerExtensions
{
    public static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Financial Audit API - Claudio Marcelo Ribeiro",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Financial Audit API",
                    Email = "contato.claudioribeiro@gmail.com",
                    Url = new Uri("https://github.com/ClaudioRibeiro99/FinancialAuditAPI"),
                }
            });
            //c.OperationFilter<SwaggerFileOperationFilter>();
            
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
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