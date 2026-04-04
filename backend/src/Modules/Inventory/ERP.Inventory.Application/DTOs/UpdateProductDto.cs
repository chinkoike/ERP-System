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
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; }

    // เรามักไม่แก้ SKU ที่นี่ เพราะ SKU ควรจะคงที่ 
    // ถ้าจะแก้สต็อก ให้ไปใช้ UpdateProductStockDto แทนครับ
}