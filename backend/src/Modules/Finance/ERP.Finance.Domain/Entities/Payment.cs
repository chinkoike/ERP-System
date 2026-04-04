using ERP.Shared;

namespace ERP.Finance.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Guid AccountId { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
    public Invoice? Invoice { get; set; }
}
