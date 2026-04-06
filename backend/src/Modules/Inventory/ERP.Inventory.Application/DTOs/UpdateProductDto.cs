using System.ComponentModel.DataAnnotations;

namespace ERP.Inventory.Application.DTOs;

public class UpdateProductDto
{
    [Required(ErrorMessage = "Product Name is required")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal BasePrice { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; }


}