using ERP.Shared;
namespace ERP.Sales.Domain;

public class Order : BaseEntity
{
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public required string OrderNumber { get; set; } // เช่น INV-20231001
    public decimal TotalAmount { get; set; }
    public Guid CustomerId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public Customer? Customer { get; set; }
    public string? ShippingAddress { get; set; }
    public ICollection<OrderItem> Items { get; set; } = [];
}