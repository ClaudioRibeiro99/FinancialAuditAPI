using FinancialAudit.Application.Utils;
using FinancialAudit.Domain.Entities;

namespace FinancialAudit.Application.Interfaces;

public interface ITransactionStrategy
{
    Task<TransactionResult> ExecuteAsync(User user, decimal amount);
}