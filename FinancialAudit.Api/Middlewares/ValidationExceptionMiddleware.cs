using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialAuditApi.Middlewares;

public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            var errors = ex.Errors.Select(e => new
            {
                Field = e.PropertyName,
                Error = e.ErrorMessage
            });
            await context.Response.WriteAsJsonAsync(new
            {
                Message = "Houve erro na validação dos campos.",
                Errors = errors
            });
        }
    }
}