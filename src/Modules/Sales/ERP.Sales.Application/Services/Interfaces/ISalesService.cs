using ERP.Sales.Application.DTOs;
using ERP.Sales.Domain;
namespace ERP.Sales.Application.Services.Interfaces;

public interface ISalesService
{
    // === Customer Management ===
    Task<CustomerDto?> GetCustomerByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CancellationToken ct = default);
    Task<Guid> CreateCustomerAsync(CreateCustomerDto dto, CancellationToken ct = default);
    Task UpdateCustomerAsync(Guid id, UpdateCustomerDto dto, CancellationToken ct = default);
    Task<bool> DeleteCustomerAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);

    // === Order Core Logic (หัวใจของระบบ) ===

    // ใช้ CreateOrderDto เพื่อรับข้อมูลจากหน้าบ้าน และคืนค่าเป็น Guid ของบิลที่สร้างสำเร็จ
    Task<Guid> PlaceOrderAsync(CreateOrderDto dto, CancellationToken ct = default);

    // การเปลี่ยนสถานะบิล (เช่น จาก Pending -> Paid -> Shipped)
    Task<bool> UpdateOrderStatusAsync(Guid orderId, string newStatus, CancellationToken ct = default);

    // ตรวจสอบสต็อกก่อนสร้าง Order (เรียกใช้ข้ามไปที่ Inventory Module)
    Task<bool> ValidateStockAvailabilityAsync(CreateOrderDto dto, CancellationToken ct = default);

    // === Order Queries ===
    Task<OrderSummaryDto?> GetOrderSummaryAsync(Guid orderId, CancellationToken ct = default);
    Task<IEnumerable<OrderSummaryDto>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken ct = default);
    Task<IEnumerable<OrderSummaryDto>> GetRecentOrdersAsync(int count, CancellationToken ct = default);

    // === Dashboard & Reports ===
    Task<decimal> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken ct = default);
    Task<int> GetPendingOrdersCountAsync(CancellationToken ct = default);
}