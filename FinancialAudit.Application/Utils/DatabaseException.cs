using FinancialAudit.Application.Utils;

namespace FinancialAudit.Application.Error;

public record DatabaseException() : AppError("Error Interno no servidor", ErrorType.Exception);