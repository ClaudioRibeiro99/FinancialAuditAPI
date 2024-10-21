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
    // Serviços
    services.AddScoped<ITransactionService, TransactionService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<ITransactionExportService, TransactionExportService>();
    services.AddScoped<ITransactionImportService, TransactionImportService>();

    // Estratégias de Transação registradas com chave
    services.AddKeyedScoped<ITransactionStrategy, DepositTransactionStrategy>("Deposit");
    services.AddKeyedScoped<ITransactionStrategy, WithdrawalTransactionStrategy>("Withdrawal");
    services.AddKeyedScoped<ITransactionStrategy, PurchaseTransactionStrategy>("Purchase");
    
    // Estratégias de Exportação de Transações registradas com chave
    services.AddKeyedScoped<ITransactionExportStrategy, CsvTransactionExportStrategy>("CSV");
    services.AddKeyedScoped<ITransactionExportStrategy, ExcelTransactionExportStrategy>("Excel");
    
    // Estratégias de Importação de Transações registradas com chave
    services.AddKeyedScoped<ITransactionImportStrategy, CsvTransactionImportStrategy>("CSV");
    services.AddKeyedScoped<ITransactionImportStrategy, ExcelTransactionImportStrategy>("Excel");
    
    // Fábrica de Estratégias de Transações
    services.AddScoped<ITransactionStrategyFactory, TransactionStrategyFactory>();

    // Fábrica de Estratégias de Exportação
    services.AddScoped<ITransactionExportStrategyFactory, TransactionExportStrategyFactory>();
    
    // Fábrica de Estratégias de Importação
    services.AddScoped<ITransactionImportStrategyFactory, TransactionImportStrategyFactory>(); 

    // Registro das estratégias de exportação
    services.AddScoped<CsvTransactionExportStrategy>();
    services.AddScoped<ExcelTransactionExportStrategy>();

    // Registro das estratégias de importação
    services.AddScoped<CsvTransactionImportStrategy>();
    services.AddScoped<ExcelTransactionImportStrategy>();
    
    // Repositórios
    services.AddScoped<ITransactionRepository, TransactionRepository>();
    services.AddScoped<IUserRepository, UserRepository>();
    }
}