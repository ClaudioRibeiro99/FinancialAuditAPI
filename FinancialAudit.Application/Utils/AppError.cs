namespace FinancialAudit.Application.Utils;

public record AppError(string Detail, ErrorType ErrorType)
{
}