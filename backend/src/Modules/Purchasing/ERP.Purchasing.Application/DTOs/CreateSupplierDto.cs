using System.ComponentModel.DataAnnotations;
namespace ERP.Purchasing.Application.DTOs;

public class CreateSupplierDto
{
    [Required]
    [StringLength(200)]
    public required string Name { get; set; }

    [StringLength(100)]
    public string? ContactName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? Phone { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }
}
