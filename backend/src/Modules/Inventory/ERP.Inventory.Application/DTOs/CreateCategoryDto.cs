using System.ComponentModel.DataAnnotations;

namespace ERP.Inventory.Application.DTOs;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}