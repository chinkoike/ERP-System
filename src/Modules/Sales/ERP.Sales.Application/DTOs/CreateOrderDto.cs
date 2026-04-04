using System.ComponentModel.DataAnnotations;
namespace ERP.Sales.Application.DTOs;


public class CreateOrderDto
{
    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Order must have at least one item")]
    public List<OrderItemDto> Items { get; set; } = new();

    [StringLength(500)]
    public string? Remarks { get; set; }

    [Required]
    [StringLength(300)]
    public string ShippingAddress { get; set; } = string.Empty;
}