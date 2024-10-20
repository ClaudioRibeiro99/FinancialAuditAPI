using System.Diagnostics;
using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Utils;
using FinancialAudit.Domain.Entities;
using FinancialAudit.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using OneOf;

namespace FinancialAudit.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITransactionStrategyFactory _transactionStrategyFactory;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(
        ITransactionRepository transactionRepository, 
        IUserRepository userRepository, 
        ITransactionStrategyFactory transactionStrategyFactory,
        ILogger<TransactionService> logger)
    {
        _transactionRepository = transactionRepository;
        _userRepository = userRepository;
        _transactionStrategyFactory = transactionStrategyFactory;
        _logger = logger;
    }

    public async Task<OneOf<PaginatedResult<TransactionDto>, AppError>> GetAllTransactionsAsync(int pageNumber, int pageSize)
    {
        try
        {
            var transactions = await _transactionRepository.GetAllAsync();
            var enumerable = transactions as Transaction[] ?? transactions.ToArray();

            if (!enumerable.Any()) return new ReturnedWithoutData();

            var totalCount = enumerable.Length;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (pageNumber > totalPages || pageNumber < 1) return new InvalidPageNumber();

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
            return new InternalErrorException();
        }
    }

    public async Task<OneOf<UserBalanceDto, AppError>> GetUserBalanceAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                _logger.LogInformation($"Usuário não encontrado. Id informado: {userId}");
                return new UserNotFound();
            }
            
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
            return new InternalErrorException();
        }
    }

    public async Task<OneOf<string, AppError>> CreateTransactionAsync(CreateTransactionDto transactionDto)
    {
        await using var transaction = await _transactionRepository.BeginTransactionAsync();

        try
        {
            var user = await _userRepository.GetByIdAsync(transactionDto.UserId);

            if (user is null)
            {
                _logger.LogInformation($"Usuário não encontrado. Id informado: {transactionDto.UserId}");
                return new UserNotFound();
            }

            var transactionStrategy = _transactionStrategyFactory.GetStrategy(transactionDto.Type);

            var result = await transactionStrategy!.ExecuteAsync(user, transactionDto.Amount);

            switch (result)
            {
                case TransactionResult.InsufficientBalance:
                    _logger.LogInformation($"Transação não finalizada, saldo insuficiente para o usuário. Usuário: {user.Name}");
                    return new InsufficientBalance();
                case TransactionResult.InvalidTransaction:
                    _logger.LogInformation($"Transação invalida. Usuário: {user.Name}");
                    return new InternalErrorException();
            }

            var transactionEntity = new Transaction
            {
                Amount = transactionDto.Amount,
                Date = DateTime.UtcNow,
                Type = Enum.Parse<TransactionType>(transactionDto.Type),
                UserId = transactionDto.UserId
            };

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
            return new InternalErrorException();
        }
    }
}