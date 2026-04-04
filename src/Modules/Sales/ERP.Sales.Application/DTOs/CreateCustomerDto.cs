using System.ComponentModel.DataAnnotations;
namespace ERP.Sales.Application.DTOs;

public class CreateCustomerDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }
}