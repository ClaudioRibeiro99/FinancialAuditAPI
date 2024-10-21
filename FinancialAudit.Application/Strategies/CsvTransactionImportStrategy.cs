using CsvHelper;
using CsvHelper.Configuration;
using FinancialAudit.Application.Interfaces;
using FinancialAudit.Domain.Entities;
using System.Globalization;
using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Mappings.Csv;
using FinancialAudit.Application.Utils;
using OneOf;

namespace FinancialAudit.Application.Strategies;

public class CsvTransactionImportStrategy : ITransactionImportStrategy
{
    public async Task<OneOf<IEnumerable<Transaction>, AppError>> ImportAsync(Stream fileStream)
    {
        using var reader = new StreamReader(fileStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HeaderValidated = null,
        });

        csv.Context.RegisterClassMap<TransactionMap>();
        var records = csv.GetRecords<TransactionDto>().ToList();
        
        var transactions = records.Select(dto => new Transaction
        {
            UserId = dto.UserId,
            Amount = dto.Amount,
            Type = Enum.Parse<TransactionType>(dto.Type!), 
            Date = dto.Date
        }).ToList();

        return await Task.FromResult(transactions);
    }
}