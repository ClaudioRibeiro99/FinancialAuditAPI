using FinancialAudit.Application.DTOs;

namespace FinancialAudit.Application.Interfaces;

public interface ITransactionExportStrategy
{
    Task<byte[]> ExportAsync(IEnumerable<TransactionDto> transactions);
}