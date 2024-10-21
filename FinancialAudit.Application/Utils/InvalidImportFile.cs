namespace FinancialAudit.Application.Utils;

public record InvalidImportFile() : AppError("Número de página inválido.", ErrorType.Exception);