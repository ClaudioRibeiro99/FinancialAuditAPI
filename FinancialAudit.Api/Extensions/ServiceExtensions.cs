using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Services;
using FinancialAudit.Domain.Interfaces;
using FinancialAudit.Infrastructure.Persistence;
using FinancialAudit.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinancialAuditApi.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }
    
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITransactionService, TransactionService>();

        return services;
    }
}