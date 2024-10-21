using System.Globalization;
using System.Text;
using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Interfaces;

namespace FinancialAudit.Application.Strategies;

public class CsvTransactionExportStrategy : ITransactionExportStrategy
{
    public async Task<byte[]> ExportAsync(IEnumerable<TransactionDto> transactions)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Id,UserId,Amount,Type,Date");
        
        var brtTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        foreach (var transaction in transactions)
        {
            var dateInBrazilTime = TimeZoneInfo.ConvertTimeFromUtc(transaction.Date, brtTimeZone);
            csv.AppendLine($"{transaction.Id},{transaction.UserId},{transaction.Amount},{transaction.Type},{dateInBrazilTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}");
        }

        // Simula uma operação assíncrona com Task.FromResult
        return await Task.FromResult(Encoding.UTF8.GetBytes(csv.ToString()));
    }
}