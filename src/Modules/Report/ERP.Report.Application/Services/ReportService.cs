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
        var start = startDate?.Date ?? DateTime.UtcNow.AddMonths(-6).Date;
        var end = endDate?.Date.AddDays(1).AddTicks(-1) ?? DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);

        var ordersQuery = _orderRepository.Query().Where(o => o.OrderDate >= start && o.OrderDate <= end);

        var totalSales = await ordersQuery.SumAsync(o => o.TotalAmount, cancellationToken);
        var totalOrders = await ordersQuery.CountAsync(cancellationToken);

        var monthlySales = await ordersQuery
            .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
            .Select(g => new
            {
                Period = new DateTime(g.Key.Year, g.Key.Month, 1),
                Total = g.Sum(o => o.TotalAmount),
                Count = g.Count()
            })
            .OrderBy(x => x.Period)
            .ToListAsync(cancellationToken);

        return new SalesReportDto
        {
            TotalSales = totalSales,
            TotalOrders = totalOrders,
            MonthlySales = monthlySales.Select(x => new ReportGraphDataPointDto
            {
                Label = x.Period.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                Value = x.Total
            }),
            MonthlyOrderCount = monthlySales.Select(x => new ReportGraphDataPointDto
            {
                Label = x.Period.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                Value = x.Count
            })
        };
    }

    public async Task<IEnumerable<InventoryStatusDto>> GetInventoryStatusAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.Query()
            .Include(p => p.Category)
            .Select(p => new InventoryStatusDto
            {
                ProductId = p.Id,
                SKU = p.SKU,
                ProductName = p.Name,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                CurrentStock = p.CurrentStock,
                ReorderLevel = Math.Max(10, (int)Math.Ceiling(p.CurrentStock * 0.15m)),
                StockStatus = p.CurrentStock <= Math.Max(10, (int)Math.Ceiling(p.CurrentStock * 0.15m)) ? "Low" : "Healthy"
            })
            .OrderBy(p => p.StockStatus)
            .ThenByDescending(p => p.CurrentStock)
            .ToListAsync(cancellationToken);

        return products;
    }

    public async Task<FinancialSummaryDto> GetFinancialSummaryAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var start = startDate?.Date ?? DateTime.UtcNow.AddMonths(-6).Date;
        var end = endDate?.Date.AddDays(1).AddTicks(-1) ?? DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);

        var invoicesQuery = _invoiceRepository.Query().Where(i => i.InvoiceDate >= start && i.InvoiceDate <= end);
        var paymentsQuery = _paymentRepository.Query().Where(p => p.PaymentDate >= start && p.PaymentDate <= end);

        var totalInvoiced = await invoicesQuery.SumAsync(i => i.AmountDue, cancellationToken);
        var totalPaid = await paymentsQuery.SumAsync(p => p.AmountPaid, cancellationToken);
        var totalReceivable = await _invoiceRepository.Query().Where(i => i.Status != ERP.Finance.Domain.Entities.InvoiceStatus.Paid).SumAsync(i => i.AmountDue, cancellationToken);

        var monthlyRevenue = await paymentsQuery
            .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
            .Select(g => new { Period = new DateTime(g.Key.Year, g.Key.Month, 1), Value = g.Sum(p => p.AmountPaid) })
            .OrderBy(x => x.Period)
            .ToListAsync(cancellationToken);

        var monthlyInvoiceAmount = await invoicesQuery
            .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
            .Select(g => new { Period = new DateTime(g.Key.Year, g.Key.Month, 1), Value = g.Sum(i => i.AmountDue) })
            .OrderBy(x => x.Period)
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
            MonthlyRevenue = monthlyRevenue.Select(x => new ReportGraphDataPointDto
            {
                Label = x.Period.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                Value = x.Value
            }),
            MonthlyInvoiceAmount = monthlyInvoiceAmount.Select(x => new ReportGraphDataPointDto
            {
                Label = x.Period.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                Value = x.Value
            }),
            TopAccounts = topAccounts
        };
    }
}
