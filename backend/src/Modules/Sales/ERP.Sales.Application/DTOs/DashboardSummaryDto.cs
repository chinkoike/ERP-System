namespace ERP.Sales.Application.DTOs;

public class DashboardSummaryDto
{
    // Sales Metrics
    public decimal TotalSales { get; set; }
    public int PendingOrdersCount { get; set; }
    public IEnumerable<OrderSummaryDto> RecentOrders { get; set; } = [];

    // Inventory Metrics
    public int LowStockProductsCount { get; set; }

    // Customer Metrics
    public int TotalCustomers { get; set; }
}