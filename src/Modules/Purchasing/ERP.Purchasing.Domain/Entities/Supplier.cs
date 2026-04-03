using ERP.Shared;

namespace ERP.Purchasing.Domain.Entities;

public class Supplier : BaseEntity
{
    public required string Name { get; set; }
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}