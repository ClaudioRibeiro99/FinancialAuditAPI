using FinancialAudit.Application.DTOs;
using FluentValidation;

namespace FinancialAudit.Application.Validators;

public class ExportRequestDtoValidator : AbstractValidator<ExportRequestDto>
{
    public ExportRequestDtoValidator()
    {
        RuleFor(x => x.Format)
            .NotEmpty().WithMessage("O formato é obrigatório.")
            .Must(f => f.Equals("csv", StringComparison.OrdinalIgnoreCase) 
                       || f.Equals("xlsx", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Formato inválido. Deve ser 'csv' ou 'xlsx'.");
    }
}