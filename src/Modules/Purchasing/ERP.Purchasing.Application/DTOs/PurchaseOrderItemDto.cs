using System.ComponentModel.DataAnnotations;

namespace ERP.Purchasing.Application.DTOs;

public class PurchaseOrderItemDto
{
    [Required]
    public Guid ProductId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Quantity ordered must be at least 1")]
    public int QuantityOrdered { get; set; }
    public int QuantityReceived { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
