using ERP.Sales.Domain;
using ERP.Sales.Application.DTOs;

namespace ERP.Sales.Application.Services.Interfaces;

public interface ISalesService
{
    Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Customer>> GetCustomersWithOrdersAsync(CancellationToken cancellationToken = default);
    Task CreateCustomerAsync(Customer customer, CancellationToken cancellationToken = default);
    Task UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken = default);
    Task DeleteCustomerAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Order?> GetOrderByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken = default);
    Task CreateOrderAsync(Order order, CancellationToken cancellationToken = default);
    Task UpdateOrderAsync(Order order, CancellationToken cancellationToken = default);
    Task DeleteOrderAsync(Order order, CancellationToken cancellationToken = default);
    Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);

    Task<OrderItem?> GetOrderItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync(CancellationToken cancellationToken = default);
    Task CreateOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
    Task UpdateOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
    Task DeleteOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
    Task AddOrderItemsRangeAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<OrderSummaryDto> GetOrderSummaryAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task CreateOrderWithUserAsync(string username, string email, string productSku, int quantity, CancellationToken cancellationToken = default);
}