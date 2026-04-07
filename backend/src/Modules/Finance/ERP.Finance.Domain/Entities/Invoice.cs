using ERP.Shared;
using ERP.Purchasing.Domain.Entities;
using ERP.Sales.Domain;

namespace ERP.Finance.Domain.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountDue { get; set; }
    public string? Description { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public ICollection<Payment>? Payments { get; set; }
}
