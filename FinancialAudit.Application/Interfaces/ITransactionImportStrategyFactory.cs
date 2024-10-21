namespace FinancialAudit.Application.Interfaces;

public interface ITransactionImportStrategyFactory
{
    ITransactionImportStrategy? GetStrategy(string format);
}