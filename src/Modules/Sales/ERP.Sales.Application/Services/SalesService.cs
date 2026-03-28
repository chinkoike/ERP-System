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
using ERP.Inventory.Application.DTOs;
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
    public async Task<CustomerDto?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(id, cancellationToken);

        if (customer == null) return null;

        return new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email ?? string.Empty,
            Phone = customer.Phone,
            Address = customer.Address,
            CreatedAt = customer.CreatedAt,
            CreatedBy = customer.CreatedBy
        };
    }

    public async Task<CustomerDto?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var customers = await _unitOfWork.Repository<Customer>().FindAsync(c => c.Email == email, cancellationToken);
        var customer = customers.FirstOrDefault();

        if (customer == null) return null;

        return new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email ?? string.Empty,
            Phone = customer.Phone,
            Address = customer.Address
        };
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CancellationToken ct = default)
    {
        var customers = await _unitOfWork.Repository<Customer>().GetAllAsync(ct);
        return customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            Phone = c.Phone,
            Address = c.Address
        });
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersWithOrdersAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _unitOfWork.Repository<Customer>().GetAllAsync(cancellationToken);

        return customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email ?? string.Empty,
            Phone = c.Phone,
            Address = c.Address,

        });
    }
    public async Task<Guid> CreateCustomerAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var customer = new Customer
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        var customerRepository = _unitOfWork.Repository<Customer>();
        await customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
    public async Task UpdateCustomerAsync(Guid id, UpdateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var customerRepository = _unitOfWork.Repository<Customer>();
        var customer = await customerRepository.GetByIdAsync(id, cancellationToken);

        if (customer == null) throw new Exception("Customer not found");

        customer.FirstName = dto.FirstName ?? customer.FirstName;
        customer.LastName = dto.LastName ?? customer.LastName;
        customer.Email = dto.Email ?? customer.Email;
        customer.Phone = dto.Phone ?? customer.Phone;
        customer.Address = dto.Address ?? customer.Address;

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
    public async Task<Guid> PlaceOrderAsync(CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. สร้างหัวบิล (Order Header)
            var order = new Order
            {
                OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8]}",
                CustomerId = dto.CustomerId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            // 2. วนลูปจัดการรายการสินค้า
            foreach (var itemDto in dto.Items)
            {
                // ดึงข้อมูลสินค้าเพื่อเช็คสต็อกและราคาล่าสุด
                var product = await _inventoryService.GetProductByIdAsync(itemDto.ProductId, cancellationToken);
                if (product == null) throw new Exception($"Product {itemDto.ProductId} not found");

                // ตรวจสอบสต็อก (Business Logic)
                if (product.StockQuantity < itemDto.Quantity)
                    throw new Exception($"สินค้า {product.Name} มีสต็อกไม่พอ");

                // สร้าง OrderItem
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.BasePrice, // ใช้ราคาจริงจาก Inventory
                    CreatedAt = DateTime.UtcNow
                };

                orderItems.Add(orderItem);
                totalAmount += orderItem.UnitPrice * orderItem.Quantity;

                // 3. สั่งตัดสต็อกในโมดูล Inventory (Cross-module call)
                // สมมติว่า InventoryService มีเมธอดอัปเดตสต็อก
                await _inventoryService.UpdateProductStockAsync(new UpdateProductStockDto
                {
                    ProductId = itemDto.ProductId,
                    QuantityChange = -itemDto.Quantity
                }, cancellationToken);
            }

            order.TotalAmount = totalAmount;
            order.Items = orderItems;

            await _unitOfWork.Repository<Order>().AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return order.Id;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
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

    public async Task<IEnumerable<OrderSummaryDto>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken ct = default)
    {
        var orders = await _unitOfWork.Repository<Order>().FindAsync(o => o.CustomerId == customerId, ct);
        return orders.Select(o => new OrderSummaryDto
        {
            OrderId = o.Id,
            OrderNumber = o.OrderNumber,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            Status = o.Status
        });
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
    public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string newStatus, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Order>();
        var order = await repo.GetByIdAsync(orderId, ct);
        if (order == null) return false;

        order.Status = newStatus;
        repo.Update(order);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> ValidateStockAvailabilityAsync(CreateOrderDto dto, CancellationToken ct = default)
    {
        foreach (var item in dto.Items)
        {
            var product = await _inventoryService.GetProductByIdAsync(item.ProductId, ct);
            if (product == null || product.StockQuantity < item.Quantity)
                return false;
        }
        return true;
    }

    public async Task<IEnumerable<OrderSummaryDto>> GetRecentOrdersAsync(int count, CancellationToken ct = default)
    {
        var orders = await _unitOfWork.Repository<Order>().GetAllAsync(ct);
        return orders.OrderByDescending(o => o.OrderDate)
                     .Take(count)
                     .Select(o => new OrderSummaryDto
                     {
                         OrderId = o.Id,
                         OrderNumber = o.OrderNumber,
                         TotalAmount = o.TotalAmount,
                         Status = o.Status
                     });
    }

    public async Task<int> GetPendingOrdersCountAsync(CancellationToken ct = default)
    {
        var orders = await _unitOfWork.Repository<Order>().FindAsync(o => o.Status == "Pending", ct);
        return orders.Count();
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

    public async Task<OrderSummaryDto?> GetOrderSummaryAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId, cancellationToken);
        if (order == null) return null;

        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(order.CustomerId, cancellationToken);
        var items = await _unitOfWork.Repository<OrderItem>().FindAsync(oi => oi.OrderId == orderId, cancellationToken);

        return new OrderSummaryDto
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerName = customer != null
            ? $"{customer.FirstName} {customer.LastName}"
            : "Unknown",
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            ItemCount = items.Count(),
            Status = order.Status // เพิ่ม Status เข้าไปด้วย
        };
    }
    public async Task CreateOrderWithUserAsync(string username, string email, string password, string productSku, int quantity, CancellationToken cancellationToken = default)
    {

        await _identityService.RegisterAsync(username, email, password, cancellationToken);
        var product = await _inventoryService.GetProductBySkuAsync(productSku, cancellationToken);
        if (product == null) throw new Exception("ไม่พบสินค้าในระบบ");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var customerRepository = _unitOfWork.Repository<Customer>();
            var newCustomer = new Customer
            {
                FirstName = username,
                LastName = "-",
                Email = email,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };
            await customerRepository.AddAsync(newCustomer, cancellationToken);

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