using FinancialAudit.Application.DTOs;
using FinancialAudit.Application.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace FinancialAuditApi.Extensions;

public static class FluentValidationExtensions
{
    public static void AddFluentValidationConfiguration(this IMvcBuilder builder)
    {
        builder.Services.AddFluentValidationAutoValidation()
                        .AddFluentValidationClientsideAdapters();
            
        builder.Services.AddValidatorsFromAssemblyContaining<CreateTransactionValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateTransactionValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<ImportTransactionValidator>();
    }
}