using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Utils;
using OneOf;

namespace FinancialAudit.Application.Interfaces;

public interface ITransactionService
{
    Task<OneOf<PaginatedResult<TransactionDto>, AppError>> GetAllTransactionsAsync(int pageNumber, int pageSize);
    Task<OneOf<UserBalanceDto, AppError>> GetUserBalanceAsync(Guid userId);
    Task<OneOf<string, AppError>> CreateTransactionAsync(CreateTransactionDto transactionDto);
}