using FinancialAudit.Application.Interfaces;
using FinancialAudit.Application.DTOs;
using OfficeOpenXml;
using System.Globalization;

namespace FinancialAudit.Application.Strategies;

public class ExcelTransactionExportStrategy : ITransactionExportStrategy
{
    public Task<byte[]> ExportAsync(IEnumerable<TransactionDto> transactions)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Transactions");
        
        worksheet.Cells[1, 1].Value = "Id";
        worksheet.Cells[1, 2].Value = "UserId";
        worksheet.Cells[1, 3].Value = "Amount";
        worksheet.Cells[1, 4].Value = "Type";
        worksheet.Cells[1, 5].Value = "Date";
        
        var brtTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        var row = 2;
        foreach (var transaction in transactions)
        {
            var dateInBrazilTime = TimeZoneInfo.ConvertTimeFromUtc(transaction.Date, brtTimeZone);
            
            worksheet.Cells[row, 1].Value = transaction.Id;
            worksheet.Cells[row, 2].Value = transaction.UserId;
            worksheet.Cells[row, 3].Value = transaction.Amount;
            worksheet.Cells[row, 4].Value = transaction.Type;
            worksheet.Cells[row, 5].Value = dateInBrazilTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            row++;
        }
        
        return Task.FromResult(package.GetAsByteArray());
    }
}