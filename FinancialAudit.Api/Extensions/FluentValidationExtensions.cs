using FinancialAudit.Application.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialAuditApi.Extensions
{
    public static class FluentValidationExtensions
    {
        public static void AddFluentValidationConfiguration(this IMvcBuilder builder)
        {
            builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
            
            builder.Services.AddValidatorsFromAssemblyContaining<CreateTransactionValidator>();
        }
    }
}