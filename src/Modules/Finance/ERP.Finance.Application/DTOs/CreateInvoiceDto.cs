namespace ERP.Finance.Application.DTOs;

public class CreateInvoiceDto
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public string? Description { get; set; }
    public decimal AmountDue { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
}
