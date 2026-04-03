namespace ERP.Purchasing.Domain.Entities;

public class PurchaseOrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }

    public Guid ProductId { get; set; }
    public int QuantityOrdered { get; set; }
    public int QuantityReceived { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
