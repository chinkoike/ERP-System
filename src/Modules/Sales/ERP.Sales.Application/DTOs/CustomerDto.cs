namespace ERP.Sales.Application.DTOs;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Read-only property สำหรับแสดงชื่อเต็ม (สะดวกตอนโชว์ที่หน้าบ้าน)
    public string FullName => $"{FirstName} {LastName}";
}