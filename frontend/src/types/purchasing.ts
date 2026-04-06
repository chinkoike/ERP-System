export type PurchaseOrderStatus = 'Ordered' | 'Receiving' | 'Completed' | 'Cancelled' | 'Draft'

export interface PurchaseOrderItem {
  productId: string
  quantityOrdered: number
  quantityReceived: number
  unitPrice: number
  totalPrice: number
}

export interface PurchaseOrder {
  id: string
  purchaseOrderNumber: string
  supplierId: string
  supplierName: string
  orderDate: string
  status: PurchaseOrderStatus
  totalAmount: number
  items: PurchaseOrderItem[]
}

export interface CreatePurchaseOrderPayload {
  supplierId: string
  items: {
    productId: string
    quantityOrdered: number
    quantityReceived: number
    unitPrice: number
    totalPrice: number
  }[]
}

export interface Supplier {
  id: string
  name: string
  contactName?: string
  email?: string
  phone?: string
  address?: string
  createdAt: string
  createdBy?: string
  updatedAt?: string
  updatedBy?: string
}

export interface CreateSupplierPayload {
  name: string
  contactName?: string
  email?: string
  phone?: string
  address?: string
}

export interface UpdateSupplierPayload {
  name?: string
  contactName?: string
  email?: string
  phone?: string
  address?: string
}
