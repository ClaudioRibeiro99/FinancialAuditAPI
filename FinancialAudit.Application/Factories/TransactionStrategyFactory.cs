using FinancialAudit.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialAudit.Application.Factories;

public class TransactionStrategyFactory : ITransactionStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TransactionStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITransactionStrategy? GetStrategy(string transactionType)
    {
        return _serviceProvider.GetRequiredKeyedService<ITransactionStrategy>(transactionType);
    }
}