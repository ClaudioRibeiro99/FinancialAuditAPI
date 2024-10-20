using FinancialAudit.Domain.Entities;

namespace FinancialAudit.Application.Strategies;

public class DepositTransactionStrategy : TransactionStrategyBase
{
    protected override Task ApplyTransactionAsync(User user, decimal amount)
    {
        user.Balance += amount;
        return Task.CompletedTask;
    }
}