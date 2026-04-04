using System.Linq;
using ERP.Sales.Application.Repositories;
using ERP.Sales.Domain;
using ERP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Sales.Infrastructure.Repositories;

public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
{
    public OrderItemRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(oi => oi.OrderId == orderId, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default)
    {
        foreach (var item in orderItems)
        {
            await AddAsync(item, cancellationToken);
        }
    }

    public async Task<decimal> GetTotalByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var orderItems = await GetByOrderIdAsync(orderId, cancellationToken);
        decimal total = 0;
        foreach (var item in orderItems)
        {
            total += item.UnitPrice * item.Quantity;
        }
        return total;
    }
}