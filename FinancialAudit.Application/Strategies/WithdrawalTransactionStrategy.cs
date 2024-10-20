using FinancialAudit.Domain.Entities;

namespace FinancialAudit.Application.Strategies;

public class WithdrawalTransactionStrategy : TransactionStrategyBase
{
    protected override bool IsValid(User user, decimal amount)
    {
        return base.IsValid(user, amount) && user.Balance >= amount;
    }

    protected override Task ApplyTransactionAsync(User user, decimal amount)
    {
        user.Balance -= amount;
        return Task.CompletedTask;
    }
}