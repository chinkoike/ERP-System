using System.ComponentModel.DataAnnotations;

namespace ERP.Identity.Application.DTOs;

public class UpdateUserDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    // -------------------------
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public bool IsActive { get; set; }

    public Guid? RoleId { get; set; }
}