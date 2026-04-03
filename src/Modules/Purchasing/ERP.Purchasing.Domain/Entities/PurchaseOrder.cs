using ERP.Shared;

namespace ERP.Purchasing.Domain.Entities;

public class PurchaseOrder : BaseEntity
{
    public required string PurchaseOrderNumber { get; set; }
    public Guid SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public decimal TotalAmount { get; set; }
    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
}
