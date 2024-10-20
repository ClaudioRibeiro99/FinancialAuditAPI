using System.Globalization;
using System.Text.Json.Serialization;

namespace FinancialAudit.Application.DTOs;

public record UserBalanceDto
{
    public Guid UserId { get; set; }
    [JsonIgnore]
    public decimal Balance { get; init; }
    
    [JsonPropertyName("Balance")]
    public string FormattedBalance => Balance.ToString("C", new CultureInfo("pt-BR"));
}