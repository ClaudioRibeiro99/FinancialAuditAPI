using System.Globalization;
using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Error;
using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Utils;
using FinancialAudit.Domain.Entities;
using FinancialAudit.Domain.Interfaces;
using FinancialAuditApi.DTOs;
using Microsoft.Extensions.Logging;
using OneOf;

namespace FinancialAudit.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(ITransactionRepository transactionRepository, IUserRepository userRepository, ILogger<TransactionService> logger)
    {
        _transactionRepository = transactionRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<OneOf<PaginatedResult<TransactionDto>, AppError>> GetAllTransactionsAsync(int pageNumber, int pageSize)
{
    try
    {
        var transactions = await _transactionRepository.GetAllAsync();

        var enumerable = transactions as Transaction[] ?? transactions.ToArray();
        
        if (!enumerable.Any())
            return new ReturnedWithoutData();
        
        var totalCount = enumerable.Length;
        
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        if (pageNumber > totalPages || pageNumber < 1)
            return new InvalidPageNumber();
        
        var pagedTransactions = enumerable
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type.ToString(),
                Date = t.Date,
                UserId = t.UserId
            })
            .ToList();
        
        var paginatedResult = new PaginatedResult<TransactionDto>
        {
            Items = pagedTransactions,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalItems = totalCount
        };

        return paginatedResult;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao acessar o banco de dados durante a obtenção de transações");

        return new DatabaseException();
    }
}


    public async Task<OneOf<UserBalanceDto, AppError>> GetUserBalanceAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
                return new ReturnedWithoutData();

            var userBalanceDto = new UserBalanceDto
            {
                UserId = user.Id,
                Balance = user.Balance
            };

            return userBalanceDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao acessar o banco de dados durante a obtenção de transações");

            return new DatabaseException();
        }
    }

    public async Task<OneOf<string, AppError>> CreateTransactionAsync(CreateTransactionDto transactionDto)
    {
        await using var transaction = await _transactionRepository.BeginTransactionAsync();

        try
        {
            var user = await _userRepository.GetByIdAsync(transactionDto.UserId);

            if (user is null) return new ReturnedWithoutData();

            if (transactionDto.Type is "Withdrawal" or "Purchase" && user.Balance < transactionDto.Amount)
                return new InsufficientBalance();

            var transactionEntity = new Transaction
            {
                Amount = transactionDto.Amount,
                Date = DateTime.UtcNow,
                Type = Enum.Parse<TransactionType>(transactionDto.Type),
                UserId = transactionDto.UserId
            };

            switch (transactionEntity.Type)
            {
                case TransactionType.Deposit:
                    user.Balance += transactionEntity.Amount;
                    break;
                case TransactionType.Withdrawal:
                case TransactionType.Purchase:
                    user.Balance -= transactionEntity.Amount;
                    break;
            }

            await _transactionRepository.AddAsync(transactionEntity);
            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Transação criada e atualizada!");

            await transaction.CommitAsync();

            return "Transação criada com sucesso";
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogInformation("Transação cancelada!");

            _logger.LogError(ex, "Erro ao processar a transação. Erro: BANCO DE DADOS");
            return new DatabaseException();
        }
    }
}