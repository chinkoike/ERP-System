namespace ERP.Purchasing.Application.DTOs;

public class PurchaseOrderDto
{
    public Guid Id { get; set; }
    public string PurchaseOrderNumber { get; set; } = null!;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public List<PurchaseOrderItemDto> Items { get; set; } = new();
}
