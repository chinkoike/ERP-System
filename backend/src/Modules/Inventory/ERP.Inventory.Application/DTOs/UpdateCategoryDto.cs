using System.ComponentModel.DataAnnotations;

namespace ERP.Inventory.Application.DTOs;

public class UpdateCategoryDto
{
    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}