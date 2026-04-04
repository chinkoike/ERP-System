using System.ComponentModel.DataAnnotations;

namespace ERP.Inventory.Application.DTOs;

public class UpdateProductStockDto
{
    [Required(ErrorMessage = "Product ID is required")]
    public Guid ProductId { get; set; } // เปลี่ยนจาก int เป็น Guid ให้ตรงกับ Entity

    [Required(ErrorMessage = "Quantity change is required")]
    // ปริมาณที่เปลี่ยนไป (บวกคือเพิ่มสต็อก, ลบคือลดสต็อก)
    public int QuantityChange { get; set; }

    [StringLength(200, ErrorMessage = "Note cannot exceed 200 characters")]
    public string? Note { get; set; } // สำหรับบันทึกเหตุผล เช่น "Stock Adjustment" หรือ "Damaged"
}