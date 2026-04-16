using ERP.Shared;

namespace ERP.Sales.Application.DTOs;

public class CustomerFilterDto : BaseFilterDto
{
    public bool? IsActive { get; set; }
}