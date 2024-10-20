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
    public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
    }
    
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // Repositórios
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Serviços
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITransactionExportService, TransactionExportService>();

        // Fábrica de Estratégias de Transações
        services.AddScoped<ITransactionStrategyFactory, TransactionStrategyFactory>();

        // Estratégias de Transação registradas com chave
        services.AddKeyedScoped<ITransactionStrategy, DepositTransactionStrategy>("Deposit");
        services.AddKeyedScoped<ITransactionStrategy, WithdrawalTransactionStrategy>("Withdrawal");
        services.AddKeyedScoped<ITransactionStrategy, PurchaseTransactionStrategy>("Purchase");

        // Fábrica de Estratégias de Exportação
        services.AddScoped<ITransactionExportStrategyFactory, TransactionExportStrategyFactory>();
        
        // Registro das estratégias de exportação
        services.AddScoped<CsvTransactionExportStrategy>();
        services.AddScoped<ExcelTransactionExportStrategy>();

        // Estratégias de Exportação de Transações registradas com chave
        services.AddKeyedScoped<ITransactionExportStrategy, CsvTransactionExportStrategy>("CSV");
        services.AddKeyedScoped<ITransactionExportStrategy, ExcelTransactionExportStrategy>("Excel");
    }

}