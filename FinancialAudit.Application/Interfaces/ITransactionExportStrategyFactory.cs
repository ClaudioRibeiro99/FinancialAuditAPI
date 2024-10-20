using FinancialAudit.Application.Interfaces;

namespace FinancialAudit.Application.Interfaces;

public interface ITransactionExportStrategyFactory
{
    ITransactionExportStrategy? GetStrategy(string format);
}