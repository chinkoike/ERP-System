using ERP.Sales.Application.DTOs;
using ERP.Identity.Application.DTOs;
using ERP.Sales.Domain;
using ERP.Shared;
namespace ERP.Sales.Application.Services.Interfaces;

public interface ISalesService
{
    // === Customer Management ===
    Task<PagedResult<CustomerDto>> SearchCustomersAsync(CustomerFilterDto filter, CancellationToken ct = default);

    Task<CustomerDto?> GetCustomerByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CancellationToken ct = default);
    Task<Guid> CreateCustomerAsync(CreateCustomerDto dto, CancellationToken ct = default);
    Task UpdateCustomerAsync(Guid id, UpdateCustomerDto dto, CancellationToken ct = default);
    Task<bool> DeleteCustomerAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    // === Order Core Logic (หัวใจของระบบ) ===

    Task CreateOrderWithUserAsync(
         RegisterRequest registerRequest,
         string productSku,
         int quantity,
         CancellationToken cancellationToken = default);
    Task<Guid> PlaceOrderAsync(CreateOrderDto dto, CancellationToken ct = default);

    // การเปลี่ยนสถานะบิล (เช่น จาก Pending -> Paid -> Shipped)
    Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, CancellationToken ct = default);

    Task<bool> CancelOrderAsync(Guid orderId, CancellationToken ct = default);

    // ตรวจสอบสต็อกก่อนสร้าง Order (เรียกใช้ข้ามไปที่ Inventory Module)
    Task<bool> ValidateStockAvailabilityAsync(CreateOrderDto dto, CancellationToken ct = default);

    // === Order Queries ===
    Task<OrderSummaryDto?> GetOrderSummaryAsync(Guid orderId, CancellationToken ct = default);
    Task<IEnumerable<OrderSummaryDto>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken ct = default);
    Task<IEnumerable<OrderSummaryDto>> GetRecentOrdersAsync(int count, CancellationToken ct = default);
    Task<PagedResult<OrderSummaryDto>> SearchOrdersAsync(OrderFilterDto filter, CancellationToken ct = default);

    // === Dashboard & Reports ===
    Task<decimal> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken ct = default);
    Task<int> GetPendingOrdersCountAsync(CancellationToken ct = default);
}