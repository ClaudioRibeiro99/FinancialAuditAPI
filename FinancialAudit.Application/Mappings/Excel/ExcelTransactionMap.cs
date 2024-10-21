using System.Globalization;
using FinancialAudit.Domain.Entities;
using OfficeOpenXml;

namespace FinancialAudit.Application.Mappings.Excel;

public class ExcelTransactionMap
{
    public Transaction MapFromExcelRow(ExcelRange cells, int row)
    {
        var transaction = new Transaction
        {
            UserId = Guid.Parse(cells[row, 2].Text),
            Amount = decimal.Parse(cells[row, 3].Text),
            Type = Enum.Parse<TransactionType>(cells[row, 4].Text),
            Date = DateTime.ParseExact(cells[row, 5].Text, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
        };

        return transaction;
    }
}