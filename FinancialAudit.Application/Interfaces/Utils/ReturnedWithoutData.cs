namespace FinancialAudit.Application.Utils;

public record ReturnedWithoutData() : AppError("Não possui registros para consulta", ErrorType.BussinessRule)
{
    
}