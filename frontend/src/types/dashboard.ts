export interface OrderSummary {
  orderId: string
  orderNumber: string
  customerName: string
  orderDate: string
  totalAmount: number
  itemCount: number
  customerId: string
  status: string
}

export interface DashboardSummary {
  totalSales: number
  pendingOrdersCount: number
  recentOrders: OrderSummary[]
  lowStockProductsCount: number
  totalCustomers: number
}
