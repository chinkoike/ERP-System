export type OrderStatus = 'Pending' | 'Confirmed' | 'Processing' | 'Shipped' | 'Delivered' | 'Cancelled'

export interface OrderItem {
  productId: string
  quantity: number
  unitPrice: number
  totalPrice: number
}

export interface OrderSummary {
  orderId: string
  orderNumber: string
  customerName: string
  customerId: string
  orderDate: string
  totalAmount: number
  itemCount: number
  status: OrderStatus
}

export interface CreateOrderPayload {
  customerId: string
  items: OrderItem[]
  shippingAddress: string
  remarks?: string
}

export interface Customer {
  id: string
  firstName: string
  lastName: string
  email?: string
  phone?: string
  address?: string
  createdAt: string
  createdBy?: string
  updatedAt?: string
  updatedBy?: string
  fullName: string
}

export interface CreateCustomerPayload {
  firstName: string
  lastName: string
  email: string
  phone?: string
  address?: string
}

export interface UpdateCustomerPayload {
  firstName?: string
  lastName?: string
  email?: string
  phone?: string
  address?: string
}
