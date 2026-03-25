using ERP.Shared;
namespace ERP.Inventory.Domain;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<Product> Products { get; set; } = [];
}