using FinancialAudit.Application.Interfaces;
using FinancialAudit.Domain.Entities;

namespace FinancialAudit.Application.Strategies;

public abstract class TransactionStrategyBase : ITransactionStrategy
{
    public async Task ExecuteAsync(User user, decimal amount)
    {
        if (!IsValid(user, amount))
        {
            throw new InvalidOperationException("Transação inválida.");
        }

        await ApplyTransactionAsync(user, amount);
    }

    protected virtual bool IsValid(User user, decimal amount)
    {
        return amount > 0;
    }

    protected abstract Task ApplyTransactionAsync(User user, decimal amount);
}