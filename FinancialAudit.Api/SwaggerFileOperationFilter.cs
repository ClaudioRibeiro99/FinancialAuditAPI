using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FinancialAuditApi;

public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParams = context.ApiDescription.ParameterDescriptions
            .Where(p => p.Type == typeof(IFormFile));

        foreach (var fileParam in fileParams)
        {
            var schema = new OpenApiSchema
            {
                Type = "string",
                Format = "binary"
            };

            operation.RequestBody ??= new OpenApiRequestBody();
            operation.RequestBody.Content["multipart/form-data"] = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties = { [fileParam.Name] = schema },
                    Required = new HashSet<string> { fileParam.Name }
                }
            };
        }
    }
}
