using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Utils;
using Microsoft.AspNetCore.Http;
using OneOf;

namespace FinancialAudit.Application.Interfaces;

public interface ITransactionImportService
{
    Task<OneOf<ImportResultDto, AppError>> ImportTransactionsAsync(IFormFile file, string format);
}