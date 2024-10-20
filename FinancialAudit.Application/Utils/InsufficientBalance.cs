namespace FinancialAudit.Application.Utils;

public record InsufficientBalance() : AppError("Saldo insuficiente para a operação.", ErrorType.BusinessRule);