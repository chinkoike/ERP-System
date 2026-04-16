using ERP.Purchasing.Domain.Entities;
using ERP.Shared;

namespace ERP.Purchasing.Application.DTOs;

public class PurchaseOrderFilterDto : BaseFilterDto
{
    public Guid? SupplierId { get; set; }
    public PurchaseOrderStatus? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
