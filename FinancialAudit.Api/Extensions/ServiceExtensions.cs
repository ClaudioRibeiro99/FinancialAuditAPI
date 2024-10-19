using FinancialAudit.Application.Factories;
using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Services;
using FinancialAudit.Application.Strategies;
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
        // Repositórios
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Serviços
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IUserService, UserService>();

        // Fábrica de Estratégias
        services.AddScoped<ITransactionStrategyFactory, TransactionStrategyFactory>();

        // Estratégias registradas com chave
        services.AddKeyedScoped<ITransactionStrategy, DepositTransactionStrategy>("Deposit");
        services.AddKeyedScoped<ITransactionStrategy, WithdrawalTransactionStrategy>("Withdrawal");
        services.AddKeyedScoped<ITransactionStrategy, PurchaseTransactionStrategy>("Purchase");

        return services;
    }

}