using System.ComponentModel.DataAnnotations;
namespace ERP.Purchasing.Application.DTOs;

public class CreatePurchaseOrderDto
{
    [Required]
    public Guid SupplierId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Purchase order must have at least one item")]
    public List<PurchaseOrderItemDto> Items { get; set; } = new List<PurchaseOrderItemDto>();
}
