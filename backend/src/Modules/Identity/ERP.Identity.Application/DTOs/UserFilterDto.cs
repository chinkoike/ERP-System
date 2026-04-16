using ERP.Shared;

namespace ERP.Identity.Application.DTOs;

public class UserFilterDto : BaseFilterDto
{
    public Guid? RoleId { get; set; }
    public bool? IsActive { get; set; }
    public string? Department { get; set; }
}
