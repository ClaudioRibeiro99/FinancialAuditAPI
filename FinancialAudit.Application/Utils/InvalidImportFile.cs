namespace FinancialAudit.Application.Utils;

public record InvalidImportFile() : AppError("Falha ao importar os dados.", ErrorType.Exception);