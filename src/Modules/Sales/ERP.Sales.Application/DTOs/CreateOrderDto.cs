namespace ERP.Sales.Application.DTOs;

public class CreateOrderDto
{
    public Guid CustomerId { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();

    // สามารถเพิ่ม Note หรือที่อยู่จัดส่งได้ที่นี่
    public string? Remarks { get; set; }
}