namespace FinancialAudit.Application.Utils;

public record UserNotFound() : AppError("Usuário não encontrado", ErrorType.BusinessRule);