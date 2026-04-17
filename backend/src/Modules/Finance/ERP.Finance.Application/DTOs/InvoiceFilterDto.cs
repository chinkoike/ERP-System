using ERP.Finance.Domain.Entities;
using ERP.Shared;

namespace ERP.Finance.Application.DTOs;

public class InvoiceFilterDto : BaseFilterDto
{
    public Guid? CustomerId { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid? AccountId { get; set; }
    public InvoiceStatus? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
