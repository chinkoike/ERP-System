using ERP.Shared;

namespace ERP.Sales.Domain;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
    public Guid ProductId { get; set; } // ID จาก Module Inventory
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}