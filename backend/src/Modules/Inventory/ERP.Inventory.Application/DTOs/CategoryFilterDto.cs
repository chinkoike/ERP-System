using ERP.Shared;

namespace ERP.Inventory.Application.DTOs;

public class CategoryFilterDto : BaseFilterDto
{
    public DateTime? CreatedAfter { get; set; }
}