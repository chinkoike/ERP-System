using System.ComponentModel.DataAnnotations;

namespace ERP.Sales.Application.DTOs;

public class OrderItemDto
{
    [Required]
    public Guid ProductId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; } // เก็บราคา ณ วันที่ขายจริง
}