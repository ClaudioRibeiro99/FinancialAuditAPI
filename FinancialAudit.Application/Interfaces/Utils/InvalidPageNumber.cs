namespace FinancialAudit.Application.Utils;

public record InvalidPageNumber() : AppError("Número de página inválido.", ErrorType.Exception)
{
    
}
