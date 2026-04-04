using ERP.Shared;

namespace ERP.Sales.Domain;

public class Customer : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }

    public ICollection<Order> Orders { get; set; } = [];
}
