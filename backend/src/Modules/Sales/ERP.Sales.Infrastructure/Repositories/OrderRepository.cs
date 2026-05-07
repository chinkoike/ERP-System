using System.Linq;
using ERP.Sales.Application.DTOs;
using ERP.Sales.Application.Repositories;
using ERP.Sales.Domain;
using ERP.Shared;
using ERP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Sales.Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException("Order number cannot be null or empty", nameof(orderNumber));

        var orders = await FindAsync(o => o.OrderNumber == orderNumber, cancellationToken);
        return orders.FirstOrDefault();
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(o => o.CustomerId == customerId, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await FindAsync(o => o.OrderDate >= startDate && o.OrderDate <= endDate, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken = default)
    {
        // Note: This assumes there's a status field, but the Order entity doesn't have one yet
        // For now, return all orders - in a real implementation you'd filter by status
        return await GetAllAsync(cancellationToken);
    }

    public async Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException("Order number cannot be null or empty", nameof(orderNumber));

        return await ExistsAsync(o => o.OrderNumber == orderNumber, cancellationToken);
    }

    public async Task<PagedResult<Order>> SearchOrdersAsync(OrderFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = Query().Include(o => o.Customer).Include(o => o.Items).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim().ToLower();

            query = query.Where(o =>
                o.OrderNumber.ToLower().Contains(term) ||
                (o.Customer != null && (
                    o.Customer.FirstName.ToLower().Contains(term) ||
                    o.Customer.LastName.ToLower().Contains(term)
                ))
            );
        }

        if (filter.CustomerId.HasValue)
        {
            query = query.Where(o => o.CustomerId == filter.CustomerId.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(o => o.Status == filter.Status.Value);
        }

        if (filter.StartDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            query = query.Where(o => o.OrderDate <= filter.EndDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(filter.PageNumber, 1);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var items = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Order>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<decimal> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var query = Query();

        if (startDate.HasValue)
        {
            // แปลงเป็น UTC ก่อนส่งเข้า Query
            var startUtc = DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
            query = query.Where(o => o.OrderDate >= startUtc);
        }

        if (endDate.HasValue)
        {
            // แปลงเป็น UTC ก่อนส่งเข้า Query
            var endUtc = DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
            query = query.Where(o => o.OrderDate <= endUtc);
        }

        return await query.SumAsync(o => o.TotalAmount, cancellationToken);
    }
}