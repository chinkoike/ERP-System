export interface ReportDataPoint {
  label: string
  value: number
}

export interface SalesReport {
  totalSales: number
  totalOrders: number
  monthlySales: ReportDataPoint[]
  monthlyOrderCount: ReportDataPoint[]
}

export interface InventoryStatus {
  productId: string
  sku: string
  productName: string
  categoryName: string
  currentStock: number
  reorderLevel: number
  stockStatus: string
}

export interface AccountBalance {
  accountId: string
  accountName: string
  balance: number
}

export interface FinancialSummary {
  totalInvoiced: number
  totalPaid: number
  totalReceivable: number
  monthlyRevenue: ReportDataPoint[]
  monthlyInvoiceAmount: ReportDataPoint[]
  topAccounts: AccountBalance[]
}
