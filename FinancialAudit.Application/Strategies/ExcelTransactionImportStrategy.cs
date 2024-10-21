using System.Globalization;
using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Utils;
using FinancialAudit.Domain.Entities;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OneOf;

namespace FinancialAudit.Application.Strategies;

public class ExcelTransactionImportStrategy : ITransactionImportStrategy
{
    private readonly ILogger<ExcelTransactionImportStrategy> _logger;
    
    public ExcelTransactionImportStrategy(
        ILogger<ExcelTransactionImportStrategy> logger)
    {
        _logger = logger;
    }
    public async Task<OneOf<IEnumerable<Transaction>, AppError>> ImportAsync(Stream fileStream)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets[0];
        var transactions = new List<Transaction>();

        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
        {
            var cells = worksheet.Cells[row, 1, row, worksheet.Dimension.Columns];
            
            if (Guid.TryParse(cells[row, 1].Text, out Guid userId))
            {
                if (decimal.TryParse(cells[row, 2].Text, NumberStyles.Any, new CultureInfo("pt-BR"), out decimal amount) &&
                    DateTime.TryParseExact(cells[row, 4].Text, "d/M/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    var transaction = new Transaction
                    {
                        UserId = userId,
                        Amount = amount,
                        Type = Enum.Parse<TransactionType>(cells[row, 3].Text),
                        Date = date
                    };

                    transactions.Add(transaction);
                }
                else
                {
                    _logger.LogWarning($"Dados inválidos na linha {row}: Amount ou Date não puderam ser convertidos.");
                    return new InvalidImportFile();
                }
            }
            else
            {
                _logger.LogWarning($"UserId inválido na linha {row}: {cells[row, 1].Text}");
                return new InvalidImportFile();
            }
        }

        return await Task.FromResult(transactions);
    }
}