using System.Globalization;
using System.Text.Json.Serialization;

namespace FinancialAudit.Application.DTOs;

public record TransactionDto
{
    public Guid Id { get; init; }
    [JsonIgnore]
    public decimal Amount { get; init; }
    
    [JsonPropertyName("Amount")]
    public string FormattedAmount => Amount.ToString("C", new CultureInfo("pt-BR"));
    public string? Type { get; init; }
    public DateTime Date { get; init; }
    public Guid UserId { get; init; }
}