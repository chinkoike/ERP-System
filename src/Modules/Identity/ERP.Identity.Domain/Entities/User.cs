using ERP.Shared;

namespace ERP.Identity.Domain;

public class User : BaseEntity
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = [];
}