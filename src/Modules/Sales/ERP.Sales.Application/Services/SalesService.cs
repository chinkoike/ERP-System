using System.Linq;
using ERP.Shared;
using ERP.Sales.Application.Services.Interfaces;
using ERP.Sales.Domain;
using ERP.Sales.Application.DTOs;
using ERP.Identity.Domain;
using ERP.Inventory.Domain;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Identity.Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ERP.Sales.Application.Services;

public class SalesService : ISalesService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly IInventoryService _inventoryService;

    public SalesService(
    IUnitOfWork unitOfWork,
    IIdentityService identityService,
    IInventoryService inventoryService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _inventoryService = inventoryService;
    }

    // Customer operations
    public async Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customerRepository = _unitOfWork.Repository<Customer>();
        return await customerRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var customerRepository = _unitOfWork.Repository<Customer>();
        var customers = await customerRepository.FindAsync(c => c.Email == email, cancellationToken);
        return customers.FirstOrDefault();
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync(CancellationToken cancellationToken = default)
    {
        var customerRepository = _unitOfWork.Repository<Customer>();
        return await customerRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetCustomersWithOrdersAsync(CancellationToken cancellationToken = default)
    {
        var customerRepository = _unitOfWork.Repository<Customer>();
        return await customerRepository.GetAllAsync(cancellationToken);
    }

    public async Task CreateCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        var customerRepository = _unitOfWork.Repository<Customer>();
        await customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        var customerRepository = _unitOfWork.Repository<Customer>();
        customerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        var customerRepository = _unitOfWork.Repository<Customer>();
        customerRepository.Remove(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var customerRepository = _unitOfWork.Repository<Customer>();
        return await customerRepository.ExistsAsync(c => c.Email == email, cancellationToken);
    }

    // Order operations
    public async Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var orderRepository = _unitOfWork.Repository<Order>();
        return await orderRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Order?> GetOrderByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        var orderRepository = _unitOfWork.Repository<Order>();
        var orders = await orderRepository.FindAsync(o => o.OrderNumber == orderNumber, cancellationToken);
        return orders.FirstOrDefault();
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orderRepository = _unitOfWork.Repository<Order>();
        return await orderRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var orderRepository = _unitOfWork.Repository<Order>();
        return await orderRepository.FindAsync(o => o.CustomerId == customerId, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var orderRepository = _unitOfWork.Repository<Order>();
        return await orderRepository.FindAsync(o => o.OrderDate >= startDate && o.OrderDate <= endDate, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orderRepository = _unitOfWork.Repository<Order>();
        return await orderRepository.GetAllAsync(cancellationToken);
    }

    public async Task CreateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        var orderRepository = _unitOfWork.Repository<Order>();
        await orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        var orderRepository = _unitOfWork.Repository<Order>();
        orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        var orderRepository = _unitOfWork.Repository<Order>();
        orderRepository.Remove(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        var orderRepository = _unitOfWork.Repository<Order>();
        return await orderRepository.ExistsAsync(o => o.OrderNumber == orderNumber, cancellationToken);
    }

    public async Task<decimal> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var orderRepository = _unitOfWork.Repository<Order>();
        var query = orderRepository.Query();

        if (startDate.HasValue)
            query = query.Where(o => o.OrderDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(o => o.OrderDate <= endDate.Value);

        return await query.SumAsync(o => o.TotalAmount, cancellationToken);
    }

    // OrderItem operations
    public async Task<OrderItem?> GetOrderItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var orderItemRepository = _unitOfWork.Repository<OrderItem>();
        return await orderItemRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var orderItemRepository = _unitOfWork.Repository<OrderItem>();
        return await orderItemRepository.FindAsync(oi => oi.OrderId == orderId, cancellationToken);
    }

    public async Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync(CancellationToken cancellationToken = default)
    {
        var orderItemRepository = _unitOfWork.Repository<OrderItem>();
        return await orderItemRepository.GetAllAsync(cancellationToken);
    }

    public async Task CreateOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        var orderItemRepository = _unitOfWork.Repository<OrderItem>();
        await orderItemRepository.AddAsync(orderItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        var orderItemRepository = _unitOfWork.Repository<OrderItem>();
        orderItemRepository.Update(orderItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        var orderItemRepository = _unitOfWork.Repository<OrderItem>();
        orderItemRepository.Remove(orderItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AddOrderItemsRangeAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default)
    {
        var orderItemRepository = _unitOfWork.Repository<OrderItem>();
        foreach (var item in orderItems)
        {
            await orderItemRepository.AddAsync(item, cancellationToken);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var orderItemRepository = _unitOfWork.Repository<OrderItem>();
        var orderItems = await orderItemRepository.FindAsync(oi => oi.OrderId == orderId, cancellationToken);
        return orderItems.Sum(oi => oi.UnitPrice * oi.Quantity);
    }

    public async Task<OrderSummaryDto> GetOrderSummaryAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        // Query ข้อมูล Order พร้อม Customer และ OrderItems
        var orderRepository = _unitOfWork.Repository<Order>();
        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);

        if (order == null)
            throw new Exception("Order not found");

        var customerRepository = _unitOfWork.Repository<Customer>();
        var customer = await customerRepository.GetByIdAsync(order.CustomerId, cancellationToken);

        var orderItemRepository = _unitOfWork.Repository<OrderItem>();
        var orderItems = await orderItemRepository.FindAsync(oi => oi.OrderId == orderId, cancellationToken);
        var itemsList = orderItems.ToList();

        return new OrderSummaryDto
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerName = customer?.Name ?? "Unknown",
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            ItemCount = itemsList.Count()
        };
    }

    public async Task CreateOrderWithUserAsync(string username, string email, string productSku, int quantity, CancellationToken cancellationToken = default)
    {
        // 1. จัดการข้อมูลนอกโมดูล Sales ก่อน (ห้ามใช้ UnitOfWork ของ Sales คุม)

        // สร้าง/ตรวจสอบ User ผ่าน Identity Service
        // สมมติว่า RegisterAsync จะคืนค่า User กลับมา หรือเช็คว่าถ้ามีแล้วก็ดึงมา
        await _identityService.RegisterAsync(username, email, cancellationToken);
        // ดึงข้อมูล Product ผ่าน Inventory Service
        var product = await _inventoryService.GetProductBySkuAsync(productSku, cancellationToken);
        if (product == null) throw new Exception("ไม่พบสินค้าในระบบ");

        // 2. เริ่มทำงานในส่วนของ Sales Module (เปิด Transaction เฉพาะของ Sales)
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 3. สร้าง Customer (คนละตารางกับ User ใน Identity นะ เป็นตารางใน Sales)
            var customerRepository = _unitOfWork.Repository<Customer>();
            var newCustomer = new Customer
            {
                Name = username,
                Email = email,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };
            await customerRepository.AddAsync(newCustomer, cancellationToken);

            // 4. สร้าง Order
            var orderRepository = _unitOfWork.Repository<Order>();
            var newOrder = new Order
            {
                OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8]}",
                CustomerId = newCustomer.Id,
                OrderDate = DateTime.UtcNow,
                TotalAmount = product.BasePrice * quantity, // ใช้ราคาที่ดึงมาจาก Inventory Service
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };
            await orderRepository.AddAsync(newOrder, cancellationToken);

            // 5. สร้าง OrderItem
            var orderItemRepository = _unitOfWork.Repository<OrderItem>();
            var newOrderItem = new OrderItem
            {
                OrderId = newOrder.Id,
                ProductId = product.Id, // ID จาก Inventory
                Quantity = quantity,
                UnitPrice = product.BasePrice,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };
            await orderItemRepository.AddAsync(newOrderItem, cancellationToken);

            // บันทึกเฉพาะข้อมูลใน SalesDbContext
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}