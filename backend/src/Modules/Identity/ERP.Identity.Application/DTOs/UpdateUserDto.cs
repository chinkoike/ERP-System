using System.ComponentModel.DataAnnotations;

namespace ERP.Identity.Application.DTOs;

public class UpdateUserDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    // --- เพิ่ม 2 บรรทัดนี้ครับ ---
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    // -------------------------

    public bool IsActive { get; set; }

    public Guid? RoleId { get; set; }
}