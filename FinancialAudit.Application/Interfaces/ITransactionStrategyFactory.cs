namespace FinancialAudit.Application.Interfaces;

public interface ITransactionStrategyFactory
{
    ITransactionStrategy? GetStrategy(string transactionType);
}