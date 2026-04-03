namespace ERP.Purchasing.Application.DTOs;

public class CreatePurchaseOrderDto
{
    public Guid SupplierId { get; set; }
    public List<PurchaseOrderItemDto> Items { get; set; } = new List<PurchaseOrderItemDto>();
}
