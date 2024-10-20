using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialAudit.Application.Factories;

public class TransactionExportStrategyFactory : ITransactionExportStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TransactionExportStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITransactionExportStrategy? GetStrategy(string format)
    {
        return format.ToLower() switch
        {
            "csv" => _serviceProvider.GetService<CsvTransactionExportStrategy>(),
            "xlsx" => _serviceProvider.GetService<ExcelTransactionExportStrategy>(),
            _ => null
        };
    }
}