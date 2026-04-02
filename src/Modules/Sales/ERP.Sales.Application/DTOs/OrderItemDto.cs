namespace ERP.Sales.Application.DTOs;

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; } // เก็บราคา ณ วันที่ขายจริง
}