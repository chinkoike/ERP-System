using System.Linq;
using ERP.Shared;
using ERP.Sales.Application.Services.Interfaces;
using ERP.Sales.Application.Repositories;
using ERP.Sales.Domain;
using ERP.Sales.Application.DTOs;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Identity.Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ERP.Inventory.Application.DTOs;
using ERP.Identity.Application.DTOs;
using ERP.Shared.Exceptions;
using ERP.Inventory.Domain;
namespace ERP.Sales.Application.Services;

public class SalesService : ISalesService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly IInventoryService _inventoryService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;

    public SalesService(
        IUnitOfWork unitOfWork,
        IIdentityService identityService,
        IInventoryService inventoryService,
        ICustomerRepository customerRepository,
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _inventoryService = inventoryService;
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
    }

    // Customer operations
    public async Task<CustomerDto?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

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
        var customer = await _customerRepository.GetByEmailAsync(email, cancellationToken);

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
        var customers = await _customerRepository.GetAllAsync(ct);
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
        var customers = await _customerRepository.GetCustomersWithOrdersAsync(cancellationToken);

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
        // เช็ค Email ซ้ำที่นี่! (Business Logic)
        var isExist = await _customerRepository.AnyAsync(c => c.Email != null && c.Email.ToLower() == dto.Email.ToLower(), cancellationToken);
        if (isExist)
        {
            throw new Exception("Email นี้ถูกใช้งานไปแล้ว");
        }
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

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
    public async Task UpdateCustomerAsync(Guid id, UpdateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

        if (customer == null) throw new Exception("Customer not found");

        customer.FirstName = dto.FirstName ?? customer.FirstName;
        customer.LastName = dto.LastName ?? customer.LastName;
        customer.Email = dto.Email ?? customer.Email;
        customer.Phone = dto.Phone ?? customer.Phone;
        customer.Address = dto.Address ?? customer.Address;

        _customerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteCustomerAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer == null) return false;

        _customerRepository.Remove(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true; // ลบสำเร็จ
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _customerRepository.ExistsAsync(c => c.Email == email, cancellationToken);
    }

    // Order operations
    public async Task<Guid> PlaceOrderAsync(CreateOrderDto dto, CancellationToken ct = default)
    {
        // 1. เตรียมข้อมูลพื้นฐานของ Order
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            CustomerId = dto.CustomerId,
            OrderDate = DateTime.UtcNow,
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{orderId.ToString()[..4].ToUpper()}",
            Status = OrderStatus.Pending,
            ShippingAddress = dto.ShippingAddress,
            Items = new List<OrderItem>(),
            TotalAmount = 0 // จะคำนวณใน Loop ด้านล่าง
        };

        decimal totalOrderAmount = 0;

        // 2. วนลูปเพื่อดึงราคาและเตรียมข้อมูล Item
        foreach (var itemDto in dto.Items)
        {
            // ดึงข้อมูลสินค้าจาก Inventory (เพื่อเอา Price ล่าสุดมาใช้)
            var productDto = await _inventoryService.GetProductByIdAsync(itemDto.ProductId, ct);

            if (productDto == null)
                throw new Exception($"ไม่พบสินค้าไอดี: {itemDto.ProductId}");

            // หากใน inventory ให้ราคาหลัก (Price) เป็น 0 ก็ใช้ BasePrice เป็น fallback
            var unitPrice = productDto.Price > 0 ? productDto.Price : productDto.BasePrice;

            if (unitPrice <= 0)
                throw new Exception($"ราคาสินค้าไม่ถูกต้อง (ProductId={itemDto.ProductId}, Price={productDto.Price}, BasePrice={productDto.BasePrice})");

            // คำนวณราคา ณ วันที่ขาย (Snapshot Price)
            var itemTotal = unitPrice * itemDto.Quantity;

            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = unitPrice, // ใช้ราคาจาก Database เท่านั้น
                TotalPrice = itemTotal
            });

            totalOrderAmount += itemTotal;

            // 3. ตัดสต็อกผ่าน Inventory Service
            var updateStockDto = new UpdateProductStockDto
            {
                QuantityChange = -itemDto.Quantity
            };
            await _inventoryService.UpdateProductStockAsync(itemDto.ProductId, updateStockDto, ct);
        }

        // 4. บันทึกราคาสุทธิที่คำนวณได้
        order.TotalAmount = totalOrderAmount;

        if (totalOrderAmount <= 0)
        {
            throw new Exception("คำนวณ TotalAmount แล้วได้ 0 หรือค่าติดลบ (ตรวจสอบราคาสินค้าใน Inventory)");
        }

        // 5. บันทึก Order (OrderItems อยู่ใน relation แล้ว)
        await _orderRepository.AddAsync(order, ct);

        // 6. Commit Transaction ทั้งหมด
        await _unitOfWork.SaveChangesAsync(ct);

        return order.Id;
    }
    public async Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _orderRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Order?> GetOrderByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.FindAsync(o => o.OrderNumber == orderNumber, cancellationToken);
        return orders.FirstOrDefault();
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await _orderRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<OrderSummaryDto>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken ct = default)
    {
        var orders = await _orderRepository.FindAsync(o => o.CustomerId == customerId, ct);
        var customer = await _customerRepository.GetByIdAsync(customerId, ct);
        var customerName = customer != null
            ? $"{customer.FirstName} {customer.LastName}"
            : "Unknown";

        var orderSummaries = new List<OrderSummaryDto>();
        foreach (var order in orders)
        {
            var items = await _orderItemRepository.GetByOrderIdAsync(order.Id, ct);
            orderSummaries.Add(new OrderSummaryDto
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                CustomerName = customerName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                ItemCount = items.Count(),
                Status = order.Status.ToString()
            });
        }

        return orderSummaries;
    }

    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _orderRepository.GetOrdersByDateRangeAsync(startDate, endDate, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await _orderRepository.GetPendingOrdersAsync(cancellationToken);
    }

    public async Task CreateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    public async Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, ct);
        if (order == null) return false;

        order.Status = newStatus;
        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> CancelOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, ct);
        if (order == null)
            return false;

        if (order.Status == OrderStatus.Cancelled)
            return true;

        var orderItems = await _orderItemRepository.GetByOrderIdAsync(orderId, ct);

        // คืน stock ทุก item ไปยัง inventory
        foreach (var item in orderItems)
        {
            var inventoryProduct = await _inventoryService.GetProductByIdAsync(item.ProductId, ct);
            if (inventoryProduct == null)
            {
                throw new Exception($"ไม่พบสินค้าใน inventory (ProductId = {item.ProductId}) เพื่อคืนสต็อก");
            }

            var restockDto = new UpdateProductStockDto
            {
                QuantityChange = item.Quantity
            };
            var updated = await _inventoryService.UpdateProductStockAsync(item.ProductId, restockDto, ct);
            if (!updated)
            {
                throw new Exception($"คืนสต็อกสินค้าไม่สำเร็จ (ProductId = {item.ProductId})");
            }
        }

        order.Status = OrderStatus.Cancelled;
        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> ValidateStockAvailabilityAsync(CreateOrderDto dto, CancellationToken ct = default)
    {
        foreach (var item in dto.Items)
        {
            var product = await _inventoryService.GetProductByIdAsync(item.ProductId, ct);
            if (product == null || product.CurrentStock < item.Quantity)
                return false;
        }
        return true;
    }

    public async Task<IEnumerable<OrderSummaryDto>> GetRecentOrdersAsync(int count, CancellationToken ct = default)
    {
        var orders = await _orderRepository.GetAllAsync(ct);
        var recentOrders = orders.OrderByDescending(o => o.OrderDate).Take(count).ToList();
        var orderSummaries = new List<OrderSummaryDto>();

        foreach (var order in recentOrders)
        {
            var customer = await _customerRepository.GetByIdAsync(order.CustomerId, ct);
            var items = await _orderItemRepository.GetByOrderIdAsync(order.Id, ct);

            orderSummaries.Add(new OrderSummaryDto
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                CustomerName = customer != null
                    ? $"{customer.FirstName} {customer.LastName}"
                    : "Unknown",
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                ItemCount = items.Count(),
                Status = order.Status.ToString()
            });
        }

        return orderSummaries;
    }

    public async Task<int> GetPendingOrdersCountAsync(CancellationToken ct = default)
    {
        var orders = await _orderRepository.GetPendingOrdersAsync(ct);
        return orders.Count();
    }
    public async Task DeleteOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        _orderRepository.Remove(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await _orderRepository.ExistsByOrderNumberAsync(orderNumber, cancellationToken);
    }

    public async Task<decimal> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        return await _orderRepository.GetTotalSalesAsync(startDate, endDate, cancellationToken);
    }

    // OrderItem operations
    public async Task<OrderItem?> GetOrderItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _orderItemRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _orderItemRepository.GetByOrderIdAsync(orderId, cancellationToken);
    }

    public async Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _orderItemRepository.GetAllAsync(cancellationToken);
    }

    public async Task CreateOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        await _orderItemRepository.AddAsync(orderItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        _orderItemRepository.Update(orderItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        _orderItemRepository.Remove(orderItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AddOrderItemsRangeAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default)
    {
        await _orderItemRepository.AddRangeAsync(orderItems, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var orderItems = await _orderItemRepository.GetByOrderIdAsync(orderId, cancellationToken);
        return orderItems.Sum(oi => oi.UnitPrice * oi.Quantity);
    }

    public async Task<OrderSummaryDto?> GetOrderSummaryAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null) return null;

        var customer = await _customerRepository.GetByIdAsync(order.CustomerId, cancellationToken);
        var items = await _orderItemRepository.GetByOrderIdAsync(orderId, cancellationToken);

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
            Status = order.Status.ToString() // เพิ่ม Status เข้าไปด้วย
        };
    }
    public async Task CreateOrderWithUserAsync(RegisterRequest registerRequest, string productSku, int quantity, CancellationToken cancellationToken = default)
    {
        // 1. ดึงข้อมูลสินค้ามาเช็คก่อนเริ่ม Transaction (ประหยัด Resource)
        var product = await _inventoryService.GetProductBySkuAsync(productSku, cancellationToken);
        if (product == null)
            throw new NotFoundException($"ไม่พบสินค้า SKU: {productSku} ในระบบ");

        // 2. เริ่ม Transaction (ควรคลุมทั้ง Register และ Create Order)
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 3. สมัครสมาชิก (ส่งก้อน Request เข้าไปเลย)
            // Note: RegisterAsync ภายในควรใช้ _unitOfWork เดียวกันเพื่อให้ Rollback ได้ถ้า Order พัง
            var userDto = await _identityService.RegisterAsync(registerRequest, cancellationToken);

            // 4. สร้างข้อมูลลูกค้า (Customer) ใน Module Sales
            var customerRepository = _customerRepository;
            var newCustomer = new Customer
            {
                FirstName = registerRequest.FirstName, // ใช้จาก DTO
                LastName = registerRequest.LastName,   // ใช้จาก DTO
                Email = registerRequest.Email,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };
            await customerRepository.AddAsync(newCustomer, cancellationToken);

            // 5. สร้างใบสั่งซื้อ (Order)
            var newOrder = new Order
            {
                OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8]}",
                CustomerId = newCustomer.Id,
                OrderDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userDto.Username // ใช้ Username ที่เพิ่งสมัครสำเร็จ
            };
            await _orderRepository.AddAsync(newOrder, cancellationToken);

            // 6. เพิ่มรายการสินค้า (OrderItem)
            var unitPrice = product.Price > 0 ? product.Price : product.BasePrice;

            var newOrderItem = new OrderItem
            {
                OrderId = newOrder.Id,
                ProductId = product.Id,
                Quantity = quantity,
                UnitPrice = unitPrice,
                TotalPrice = unitPrice * quantity,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userDto.Username
            };
            await _orderItemRepository.AddAsync(newOrderItem, cancellationToken);

            // 7. ปรับ total order ให้ตรงกับ order item (กรณีมีหลายรายการในอนาคต)
            newOrder.TotalAmount = newOrderItem.TotalPrice;
            _orderRepository.Update(newOrder);

            // 7. บันทึกและ Commit ทีเดียว
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            // ถ้าขั้นไหนพัง (เช่น Register ผ่านแต่สร้าง Order ไม่ได้) มันจะ Rollback ทั้งหมด!
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}