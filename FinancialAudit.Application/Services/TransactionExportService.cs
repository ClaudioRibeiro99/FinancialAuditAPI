using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Utils;
using FinancialAudit.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using OneOf;

namespace FinancialAudit.Application.Services;

public class TransactionExportService : ITransactionExportService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ITransactionExportStrategyFactory _exportStrategyFactory;
    private readonly ILogger<TransactionExportService> _logger;

    public TransactionExportService(
        ITransactionRepository transactionRepository, 
        ITransactionExportStrategyFactory exportStrategyFactory, 
        ILogger<TransactionExportService> logger)
    {
        _transactionRepository = transactionRepository;
        _exportStrategyFactory = exportStrategyFactory;
        _logger = logger;
    }

    public async Task<OneOf<byte[], AppError>> ExportTransactionsAsync(ExportRequestDto request)
    {
        try
        {
            var transactions = await _transactionRepository.GetAllAsync();

            if (transactions is null)
                return new ReturnedWithoutData();

            var enumerable = transactions.ToList();
            var transactionDtos = enumerable.Select(t => new TransactionDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Amount = t.Amount,
                Type = t.Type.ToString(),
                Date = t.Date
            });

            var strategy = _exportStrategyFactory.GetStrategy(request.Format);

            var fileBytes = await strategy!.ExportAsync(transactionDtos);
            return fileBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao acessar o banco de dados durante a obtenção de transações");
            return new InternalErrorException();
        }
    }
}