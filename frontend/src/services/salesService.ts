import http from './http'
import type { PagedResult } from '@/types/pagination'
import type {
  OrderSummary,
  Customer,
  CreateOrderPayload,
  CreateCustomerPayload,
  UpdateCustomerPayload,
} from '@/types/sales'

export const salesService = {
  // --- Orders ---
  async getOrders(): Promise<OrderSummary[]> {
    const res = await http.get<OrderSummary[]>('/api/orders')
    return res.data
  },
  async searchOrders(filter: {
    searchTerm?: string
    status?: string
    pageNumber?: number
    pageSize?: number
  }): Promise<PagedResult<OrderSummary>> {
    const res = await http.get<PagedResult<OrderSummary>>('/api/orders/search', { params: filter })
    return res.data
  },
  async getOrderById(id: string): Promise<OrderSummary> {
    const res = await http.get<OrderSummary>(`/api/orders/${id}`)
    return res.data
  },
  async getOrdersByCustomer(customerId: string): Promise<OrderSummary[]> {
    const res = await http.get<OrderSummary[]>(`/api/orders/by-customer/${customerId}`)
    return res.data
  },
  async createOrder(payload: CreateOrderPayload): Promise<{ id: string }> {
    const res = await http.post<{ id: string }>('/api/orders', payload)
    return res.data
  },
  async updateOrderStatus(id: string, status: number): Promise<void> {
    await http.patch(`/api/orders/${id}/status`, status)
  },
  async cancelOrder(id: string): Promise<void> {
    await http.post(`/api/orders/${id}/cancel`, {})
  },

  // --- Customers ---
  async getCustomers(): Promise<Customer[]> {
    const res = await http.get<Customer[]>('/api/customers')
    return res.data
  },
  async searchCustomers(filter: {
    searchTerm?: string
    pageNumber?: number
    pageSize?: number
  }): Promise<PagedResult<Customer>> {
    const res = await http.get<PagedResult<Customer>>('/api/customers/search', { params: filter })
    return res.data
  },
  async getCustomerById(id: string): Promise<Customer> {
    const res = await http.get<Customer>(`/api/customers/${id}`)
    return res.data
  },
  async createCustomer(payload: CreateCustomerPayload): Promise<{ id: string }> {
    const res = await http.post<{ id: string }>('/api/customers', payload)
    return res.data
  },
  async updateCustomer(id: string, payload: UpdateCustomerPayload): Promise<void> {
    await http.put(`/api/customers/${id}`, payload)
  },
  async deleteCustomer(id: string): Promise<void> {
    await http.delete(`/api/customers/${id}`)
  },
}
