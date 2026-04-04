using ERP.Sales.Domain;
using ERP.Shared;

namespace ERP.Sales.Application.Repositories;

public interface IOrderItemRepository : IGenericRepository<OrderItem>
{
    Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}