using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Utils;
using FinancialAudit.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OneOf;

namespace FinancialAudit.Application.Services;

public class TransactionImportService : ITransactionImportService
{
    private readonly ITransactionImportStrategyFactory _strategyFactory;
    private readonly IUserRepository _userRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<TransactionImportService> _logger;

    public TransactionImportService(
        ITransactionImportStrategyFactory strategyFactory,
        IUserRepository userRepository, 
        ITransactionRepository transactionRepository,
        ILogger<TransactionImportService> logger)
    {
        _strategyFactory = strategyFactory;
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<OneOf<ImportResultDto, AppError>> ImportTransactionsAsync(IFormFile file, string format)
    {
        try
        {
            var strategy = _strategyFactory.GetStrategy(format);
            
            using var stream = file.OpenReadStream();
            var transactions = await strategy!.ImportAsync(stream);

            var importedCount = 0;
            var ignoredCount = 0;
            
            foreach (var transaction in transactions.AsT0)
            {
                var userExists = await _userRepository.GetByIdAsync(transaction.UserId);
                if (userExists != null)
                {
                    transaction.IsImported = true;
                    await _transactionRepository.AddAsync(transaction);
                    importedCount++;
                }
                else
                {
                    ignoredCount++;
                }
            }
            
            var message = ignoredCount > 0
                ? $"Importação concluída com sucesso. {ignoredCount} transações foram ignoradas pois os usuários não existem no banco de dados."
                : "Importação concluída com sucesso.";
            
            _logger.LogInformation(message);

            return new ImportResultDto
            {
                SuccessCount = importedCount,
                Message = message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao acessar o banco de dados durante a obtenção de transações");
            return new InternalErrorException();
        }
    }
}