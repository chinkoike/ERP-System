namespace ERP.Purchasing.Application.DTOs;

public class PurchaseOrderItemDto
{
    public Guid ProductId { get; set; }
    public int QuantityOrdered { get; set; }
    public int QuantityReceived { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
