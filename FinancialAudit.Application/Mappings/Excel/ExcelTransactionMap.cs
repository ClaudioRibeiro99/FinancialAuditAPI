using System.Globalization;
using FinancialAudit.Domain.Entities;
using OfficeOpenXml;

namespace FinancialAudit.Application.Mappings.Excel
{
    public class ExcelTransactionMap
    {
        public static Transaction MapFromExcelRow(ExcelRange cells, int row)
        {
            var amountStr = cells[row, 2].Text;
            
            if (!decimal.TryParse(amountStr, NumberStyles.Any, new CultureInfo("en-US"), out var amount))
            {
                if (!decimal.TryParse(amountStr, NumberStyles.Any, new CultureInfo("pt-BR"), out amount))
                {
                    throw new FormatException($"O valor '{amountStr}' não está em um formato válido");
                }
            }

            var dateStr = cells[row, 4].Text;
            
            var dateFormats = new[] { 
                "dd/MM/yyyy HH:mm",
                "dd/MM/yyyy HH:mm:ss",
                "MM/dd/yy H:mm",
                "MM/dd/yy H:mm:ss",
                "MM/dd/yyyy H:mm",
                "MM/dd/yyyy H:mm:ss",
                "d/M/yy H:mm",
                "d/M/yy H:mm:ss", 
                "M/d/yyyy H:mm",
                "M/d/yyyy H:mm:ss"
            };
            
            if (!DateTime.TryParseExact(dateStr, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                throw new FormatException($"A data '{dateStr}' não está em um formato válido");
            }
            
            var transaction = new Transaction
            {
                UserId = Guid.Parse(cells[row, 1].Text),
                Amount = amount,
                Type = Enum.Parse<TransactionType>(cells[row, 3].Text),
                Date = date
            };

            return transaction;
        }
    }
}