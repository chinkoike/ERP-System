namespace ERP.Finance.Application.DTOs;

public class CreatePaymentDto
{
    public Guid InvoiceId { get; set; }
    public Guid AccountId { get; set; }

    public decimal AmountPaid { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
}
