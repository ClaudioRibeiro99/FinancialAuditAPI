using CsvHelper.Configuration;
using FinancialAudit.Application.DTOs;

namespace FinancialAudit.Application.Mappings.Csv;

public sealed class TransactionMap : ClassMap<TransactionDto>
{
    public TransactionMap()
    {
        Map(m => m.UserId).Name("UserId");
        Map(m => m.Amount).Name("Amount");
        Map(m => m.Type).Name("Type");
        Map(m => m.Date).TypeConverterOption.Format("dd/MM/yyyy HH:mm:ss");
    }
}