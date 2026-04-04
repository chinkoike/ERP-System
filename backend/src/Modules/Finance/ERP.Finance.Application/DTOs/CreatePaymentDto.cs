using System.ComponentModel.DataAnnotations;

namespace ERP.Finance.Application.DTOs;

public class CreatePaymentDto
{
    [Required]
    public Guid InvoiceId { get; set; }

    [Required]
    public Guid AccountId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal AmountPaid { get; set; }

    [Required]
    public DateTime PaymentDate { get; set; }

    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;

    [StringLength(100)]
    public string? ReferenceNumber { get; set; }
}
