using FinancialAudit.Domain.Entities;

namespace FinancialAudit.Application.Interfaces;

public interface ITransactionStrategy
{
    Task ExecuteAsync(User user, decimal amount);
}