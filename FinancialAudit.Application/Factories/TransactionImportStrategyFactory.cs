using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialAudit.Application.Factories;

public class TransactionImportStrategyFactory : ITransactionImportStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TransactionImportStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITransactionImportStrategy? GetStrategy(string format)
    {
        return format.ToLower() switch
        {
            "csv" => _serviceProvider.GetService<CsvTransactionImportStrategy>(),
            "xlsx" => _serviceProvider.GetService<ExcelTransactionImportStrategy>(),
            _ => null
        };
    }
}