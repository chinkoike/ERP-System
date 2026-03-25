using ERP.Shared;

namespace ERP.Identity.Domain;

public class Role : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = [];
}