using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Utils;
using FinancialAudit.Domain.Entities;

namespace FinancialAudit.Application.Strategies;

public abstract class TransactionStrategyBase : ITransactionStrategy
{
    public async Task<TransactionResult> ExecuteAsync(User user, decimal amount)
    {
        return await ExecuteAsync(user, amount, this);
    }
    
    public async Task<TransactionResult> ExecuteAsync(User user, decimal amount, ITransactionStrategy? transactionStrategy)
    {
        if (user.Balance < amount && transactionStrategy is not DepositTransactionStrategy)
        {
            return TransactionResult.InsufficientBalance;
        }
        
        if (!IsValid(user, amount))
        {
            return TransactionResult.InvalidTransaction;
        }

        await ApplyTransactionAsync(user, amount);
        return TransactionResult.Success;
    }

    protected virtual bool IsValid(User user, decimal amount)
    {
        return amount > 0;
    }

    protected abstract Task ApplyTransactionAsync(User user, decimal amount);
}