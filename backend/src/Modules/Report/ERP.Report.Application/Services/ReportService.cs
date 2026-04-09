using System.Globalization;
using System.Linq;
using ERP.Inventory.Application.Repositories;
using ERP.Finance.Application.Repositories;
using ERP.Report.Application.DTOs;
using ERP.Report.Application.Services.Interfaces;
using ERP.Sales.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Report.Application.Services;

public class ReportService : IReportService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IAccountRepository _accountRepository;

    public ReportService(
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IInvoiceRepository invoiceRepository,
        IPaymentRepository paymentRepository,
        IAccountRepository accountRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
        _accountRepository = accountRepository;
    }

    public async Task<SalesReportDto> GetSalesReportAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var start = startDate.HasValue ? DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc) : (DateTime?)null;
        var end = endDate.HasValue ? DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc) : (DateTime?)null;
        var ordersQuery = _orderRepository.Query().Where(o => o.OrderDate >= start && o.OrderDate <= end);

        var totalSales = await ordersQuery.SumAsync(o => o.TotalAmount, cancellationToken);
        var totalOrders = await ordersQuery.CountAsync(cancellationToken);

        // 1. ดึงข้อมูลที่ Group แล้วออกมาเป็นแค่ตัวเลขก่อน (SQL แปลส่วนนี้ได้)
        var groupedData = await ordersQuery
            .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Total = g.Sum(o => o.TotalAmount),
                Count = g.Count()
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync(cancellationToken);

        // 2. มาสร้าง DateTime และจัด Format ใน Memory (Client-side)
        var monthlySalesDetails = groupedData.Select(x => new
        {
            Period = new DateTime(x.Year, x.Month, 1),
            x.Total,
            x.Count
        }).ToList();

        return new SalesReportDto
        {
            TotalSales = totalSales,
            TotalOrders = totalOrders,
            MonthlySales = monthlySalesDetails.Select(x => new ReportGraphDataPointDto
            {
                Label = x.Period.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                Value = x.Total
            }),
            MonthlyOrderCount = monthlySalesDetails.Select(x => new ReportGraphDataPointDto
            {
                Label = x.Period.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                Value = x.Count
            })
        };
    }

    public async Task<IEnumerable<InventoryStatusDto>> GetInventoryStatusAsync(CancellationToken cancellationToken = default)
    {
        // 1. ดึงข้อมูลดิบจาก DB ออกมาก่อน (Select เฉพาะที่จำเป็น)
        var productsData = await _productRepository.Query()
            .Include(p => p.Category)
            .Select(p => new
            {
                p.Id,
                p.SKU,
                p.Name,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                p.CurrentStock
            })
            .ToListAsync(cancellationToken);

        // 2. มาคำนวณ Logic C# ต่อข้างนอก (ใน Memory)
        return productsData.Select(p =>
        {
            var reorderLevel = Math.Max(10, (int)Math.Ceiling(p.CurrentStock * 0.15m));
            return new InventoryStatusDto
            {
                ProductId = p.Id,
                SKU = p.SKU,
                ProductName = p.Name,
                CategoryName = p.CategoryName,
                CurrentStock = p.CurrentStock,
                ReorderLevel = reorderLevel,
                StockStatus = p.CurrentStock <= reorderLevel ? "Low" : "Healthy"
            };
        })
        .OrderBy(p => p.StockStatus)
        .ThenByDescending(p => p.CurrentStock);
    }

    public async Task<FinancialSummaryDto> GetFinancialSummaryAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var start = startDate.HasValue
            ? DateTime.SpecifyKind(startDate.Value.Date, DateTimeKind.Utc)
            : DateTime.UtcNow.AddMonths(-6).Date;
        var end = endDate.HasValue
            ? DateTime.SpecifyKind(endDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc)
            : DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);

        var invoicesQuery = _invoiceRepository.Query().Where(i => i.InvoiceDate >= start && i.InvoiceDate <= end);
        var paymentsQuery = _paymentRepository.Query().Where(p => p.PaymentDate >= start && p.PaymentDate <= end);

        // แก้ไข: ใช้ TotalAmount แทน AmountDue สำหรับยอดรวมใบแจ้งหนี้ (ตามที่คุยกันเรื่องความถูกต้องของตัวเลข)
        var totalInvoiced = await invoicesQuery.SumAsync(i => i.TotalAmount, cancellationToken);
        var totalPaid = await paymentsQuery.SumAsync(p => p.AmountPaid, cancellationToken);

        // ยอดค้างรับ (ต้องใช้ AmountDue ถูกต้องแล้ว)
        var totalReceivable = await _invoiceRepository.Query()
            .Where(i => i.Status != ERP.Finance.Domain.Entities.InvoiceStatus.Paid)
            .SumAsync(i => i.AmountDue, cancellationToken);

        // 1. ดึงข้อมูลรายได้รายเดือน (ดึงแค่ Year, Month, Sum ออกมา)
        var monthlyRevenueData = await paymentsQuery
            .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Value = g.Sum(p => p.AmountPaid) })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync(cancellationToken);

        // 2. ดึงข้อมูลยอดเรียกเก็บรายเดือน
        var monthlyInvoiceData = await invoicesQuery
            .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Value = g.Sum(i => i.TotalAmount) })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync(cancellationToken);

        var topAccounts = await _accountRepository.Query()
            .OrderByDescending(a => a.Balance)
            .Take(5)
            .Select(a => new AccountBalanceDto
            {
                AccountId = a.Id,
                AccountName = a.AccountName,
                Balance = a.Balance
            })
            .ToListAsync(cancellationToken);

        return new FinancialSummaryDto
        {
            TotalInvoiced = totalInvoiced,
            TotalPaid = totalPaid,
            TotalReceivable = totalReceivable,
            // มาปั้น Label วันที่ในส่วนนี้ (In-memory)
            MonthlyRevenue = monthlyRevenueData.Select(x => new ReportGraphDataPointDto
            {
                Label = new DateTime(x.Year, x.Month, 1).ToString("MMM yyyy", CultureInfo.InvariantCulture),
                Value = x.Value
            }),
            MonthlyInvoiceAmount = monthlyInvoiceData.Select(x => new ReportGraphDataPointDto
            {
                Label = new DateTime(x.Year, x.Month, 1).ToString("MMM yyyy", CultureInfo.InvariantCulture),
                Value = x.Value
            }),
            TopAccounts = topAccounts
        };
    }
}
