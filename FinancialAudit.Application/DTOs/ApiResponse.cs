namespace FinancialAudit.Application.DTOs;

public record ApiResponse<T>
{
    public ApiResponse(bool success, T data)
    {
        Success = success;
        Data = data;
    }

    public bool Success { get; set; }
    public T Data { get; set; }
}