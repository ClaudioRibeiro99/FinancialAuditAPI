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

        foreach (var transaction in transactions)
        {
            csv.AppendLine($"{transaction.Id},{transaction.UserId},{transaction.Amount},{transaction.Type},{transaction.Date:yyyy-MM-dd}");
        }

        // Simula uma operação assíncrona com Task.FromResult
        return await Task.FromResult(Encoding.UTF8.GetBytes(csv.ToString()));
    }
}