using FinancialAudit.Application.DTOs;
using FluentValidation;

namespace FinancialAudit.Application.Validators;

public class ImportTransactionValidator : AbstractValidator<ImportTransactionRequestDto>
{
    public ImportTransactionValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("O arquivo é obrigatório.")
            .Must(f => f.Length > 0).WithMessage("O arquivo não pode estar vazio.");

        RuleFor(x => x.Format)
            .NotEmpty().WithMessage("O formato é obrigatório.")
            .Must(f => f.Equals("csv", StringComparison.OrdinalIgnoreCase) 
                       || f.Equals("xlsx", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Formato inválido. Deve ser 'csv' ou 'xlsx'.");
    }
}