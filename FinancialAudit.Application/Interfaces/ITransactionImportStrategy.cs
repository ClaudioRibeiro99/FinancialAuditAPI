using FinancialAudit.Application.Utils;
using FinancialAudit.Domain.Entities;
using OneOf;

namespace FinancialAudit.Application.Interfaces;

public interface ITransactionImportStrategy
{
    Task<OneOf<IEnumerable<Transaction>, AppError>> ImportAsync(Stream fileStream);
}