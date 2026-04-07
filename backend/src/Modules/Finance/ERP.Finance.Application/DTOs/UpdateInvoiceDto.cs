namespace ERP.Finance.Application.DTOs;

public class UpdateInvoiceDto
{
    public decimal? TotalAmount { get; set; }
    public decimal? AmountDue { get; set; }
    public DateTime DueDate { get; set; }
    public string? Status { get; set; }
    public string? Description { get; set; }
}