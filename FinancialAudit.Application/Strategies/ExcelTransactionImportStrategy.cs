using System.Globalization;
using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.Utils;
using FinancialAudit.Application.Mappings;
using FinancialAudit.Application.Mappings.Excel;
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
        
            try
            {
                var transaction = ExcelTransactionMap.MapFromExcelRow(cells, row);
                
                transactions.Add(transaction);
            }
            catch (FormatException ex)
            {
                _logger.LogWarning($"Erro de formatação na linha {row}: {ex.Message}");
                return new InvalidImportFile();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao processar a linha {row}: {ex.Message}");
                return new InvalidImportFile();
            }
        }

        return await Task.FromResult(transactions);
    }
}