using FinancialAudit.Application.DTOs;
using FluentValidation;

namespace FinancialAudit.Application.Validators;

public class CreateTransactionValidator : AbstractValidator<CreateTransactionDto>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("UserId deve ser maior que zero.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("O valor da transação deve ser maior que zero.");

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("O tipo de transação é obrigatório.")
            .Must(BeAValidTransactionType)
            .WithMessage("Tipo de transação inválido. Permitidos: Deposit, Withdrawal, Purchase.");
    }

    private bool BeAValidTransactionType(string transactionType)
    {
        var allowedTypes = new[] { "Deposit", "Withdrawal", "Purchase" };
        return allowedTypes.Contains(transactionType);
    }
}