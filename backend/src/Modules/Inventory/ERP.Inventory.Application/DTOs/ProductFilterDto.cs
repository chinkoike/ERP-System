using ERP.Shared;

namespace ERP.Inventory.Application.DTOs;

public class ProductFilterDto : BaseFilterDto
{
    public Guid? CategoryId { get; set; }
    public int? MinStock { get; set; }
    public int? MaxStock { get; set; }
}
