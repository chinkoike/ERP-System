import http from './http'
import type { PagedResult } from '@/types/pagination'
import type {
  PurchaseOrder,
  Supplier,
  CreatePurchaseOrderPayload,
  PurchaseOrderItem,
  CreateSupplierPayload,
  UpdateSupplierPayload,
} from '@/types/purchasing'

export const purchasingService = {
  // --- Purchase Orders ---
  async getPurchaseOrders(): Promise<PurchaseOrder[]> {
    const res = await http.get<PurchaseOrder[]>('/api/purchasing/purchase-orders')
    return res.data
  },
  async searchPurchaseOrders(filter: {
    searchTerm?: string
    supplierId?: string
    status?: string
    pageNumber?: number
    pageSize?: number
  }): Promise<PagedResult<PurchaseOrder>> {
    const res = await http.get<PagedResult<PurchaseOrder>>(
      '/api/purchasing/purchase-orders/search',
      { params: filter },
    )
    return res.data
  },
  async getPurchaseOrderById(id: string): Promise<PurchaseOrder> {
    const res = await http.get<PurchaseOrder>(`/api/purchasing/purchase-orders/${id}`)
    return res.data
  },
  async createPurchaseOrder(payload: CreatePurchaseOrderPayload): Promise<{ id: string }> {
    const res = await http.post<{ id: string }>('/api/purchasing/purchase-orders', payload)
    return res.data
  },
  async receivePurchaseOrder(id: string, items: PurchaseOrderItem[]): Promise<void> {
    await http.post(`/api/purchasing/purchase-orders/${id}/receive`, items)
  },
  async cancelPurchaseOrder(id: string): Promise<void> {
    await http.patch(`/api/purchasing/purchase-orders/${id}/cancel`, {})
  },

  // --- Suppliers ---
  async getSuppliers(): Promise<Supplier[]> {
    const res = await http.get<Supplier[]>('/api/purchasing/suppliers')
    return res.data
  },
  async getSupplierById(id: string): Promise<Supplier> {
    const res = await http.get<Supplier>(`/api/purchasing/suppliers/${id}`)
    return res.data
  },
  async createSupplier(payload: CreateSupplierPayload): Promise<{ id: string }> {
    const res = await http.post<{ id: string }>('/api/purchasing/suppliers', payload)
    return res.data
  },
  async updateSupplier(id: string, payload: UpdateSupplierPayload): Promise<void> {
    await http.put(`/api/purchasing/suppliers/${id}`, payload)
  },
  async deleteSupplier(id: string): Promise<void> {
    await http.delete(`/api/purchasing/suppliers/${id}`)
  },
}
