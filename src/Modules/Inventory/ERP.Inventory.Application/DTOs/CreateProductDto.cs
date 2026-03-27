using System.ComponentModel.DataAnnotations;

namespace ERP.Inventory.Application.DTOs;

public class CreateProductDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "SKU is required")]
    [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
    public string SKU { get; set; } = string.Empty; // เพิ่ม SKU สำหรับระบุรหัสสินค้า
    public string? ImageUrl { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal BasePrice { get; set; } // เปลี่ยนจาก Price เป็น BasePrice ให้ตรงกับ Entity

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Initial stock cannot be negative")]
    public int InitialStock { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public Guid CategoryId { get; set; } // เปลี่ยนจาก int เป็น Guid ให้ตรงกับระบบของคุณ
}