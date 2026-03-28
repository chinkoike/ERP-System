using ERP.Shared;
namespace ERP.Inventory.Domain;

public class Product : BaseEntity
{
    public required string SKU { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal BasePrice { get; set; }

    public int CurrentStock { get; set; }
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
}