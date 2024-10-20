namespace FinancialAudit.Application.DTOs;

public record PaginatedResult<T>
{
    public IEnumerable<T> Items { get; init; } = new List<T>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; init; }
    public int TotalItems { get; set; }
}