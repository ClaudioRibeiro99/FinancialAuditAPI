using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Utils;
using OneOf;

namespace FinancialAudit.Application.Interfaces
{
    public interface ITransactionExportService
    {
        Task<OneOf<byte[], AppError>> ExportTransactionsAsync(ExportRequestDto request);
    }
}