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
            .NotEmpty()
            .WithMessage("O formato é obrigatório.")
            .Must(BeAValidTransactionType)
            .WithMessage("Formato inválido. Deve ser 'csv' ou 'xlsx'.");
    }
    
    private bool BeAValidTransactionType(string transactionType)
    {
        var allowedTypes = new[] { "csv", "xlsx" };
        return allowedTypes.Contains(transactionType);
    }
}