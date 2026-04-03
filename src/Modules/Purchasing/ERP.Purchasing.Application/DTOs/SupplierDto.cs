namespace ERP.Purchasing.Application.DTOs;

public class SupplierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
