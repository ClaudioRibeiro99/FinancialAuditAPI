using System.Globalization;
using System.Text.Json.Serialization;

namespace FinancialAudit.Application.DTOs;

public record TransactionDto
{
    public int Id { get; set; }
    [JsonIgnore]
    public decimal Amount { get; set; }
    
    [JsonPropertyName("Amount")]
    public string FormattedAmount => Amount.ToString("C", new CultureInfo("pt-BR"));
    public string? Type { get; set; }
    public DateTime Date { get; set; }
    public int UserId { get; set; }
}