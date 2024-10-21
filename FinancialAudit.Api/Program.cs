using FinancialAuditApi.Extensions;
using FinancialAudit.Infrastructure;
using FinancialAudit.Infrastructure.Persistence;
using FinancialAuditApi.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddSerilogConfiguration();
builder.Services.AddCorsConfiguration();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddControllers().AddFluentValidationConfiguration();
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();
app.UseMiddleware<ValidationExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    dbContext.Database.Migrate();
    
    await SeedDatabase.SeedAsync(dbContext);
}

app.UseCors("AllowAll");
app.UseSwaggerInDevelopment();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();