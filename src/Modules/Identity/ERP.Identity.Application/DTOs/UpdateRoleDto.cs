using System.ComponentModel.DataAnnotations;

namespace ERP.Identity.Application.DTOs;

public class UpdateRoleDto
{
    [Required(ErrorMessage = "Role name is required")]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }
}