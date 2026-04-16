using ERP.Sales.Domain;
using ERP.Shared;

namespace ERP.Sales.Application.DTOs;

public class OrderFilterDto : BaseFilterDto
{
    public Guid? CustomerId { get; set; }
    public OrderStatus? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
