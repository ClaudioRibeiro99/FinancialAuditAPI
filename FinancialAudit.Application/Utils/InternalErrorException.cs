namespace FinancialAudit.Application.Utils;

public record InternalErrorException() : AppError("Error Interno no servidor", ErrorType.Exception);