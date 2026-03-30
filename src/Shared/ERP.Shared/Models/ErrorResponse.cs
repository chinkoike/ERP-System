namespace ERP.Shared.Models;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; } // เอาไว้เก็บ Error ละเอียด (เฉพาะตอน Dev)
}