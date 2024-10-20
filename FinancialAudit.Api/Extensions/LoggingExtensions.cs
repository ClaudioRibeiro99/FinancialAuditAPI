using Serilog;

namespace FinancialAuditApi.Extensions;

public static class LoggingExtensions
{
    public static void AddSerilogConfiguration(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration);
        });
    }
}