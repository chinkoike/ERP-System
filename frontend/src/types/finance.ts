export type InvoiceStatus = 'Draft' | 'Issued' | 'Paid' | 'Overdue' | 'Cancelled'
export type PaymentMethod = 'Cash' | 'CreditCard' | 'BankTransfer' | 'Cheque' | 'Others'

export interface Invoice {
  id: string
  invoiceNumber: string
  customerId: string
  supplierId: string
  accountId?: string
  purchaseOrderId?: string
  description?: string
  totalAmount: number
  amountDue: number
  invoiceDate: string
  dueDate: string
  status: InvoiceStatus
}

export interface CreateInvoicePayload {
  invoiceNumber: string
  customerId?: string
  supplierId?: string
  accountId?: string
  purchaseOrderId?: string
  description?: string
  totalAmount: number
  amountDue: number
  invoiceDate: string
  dueDate: string
}

export interface UpdateInvoicePayload {
  amountDue?: number
  dueDate?: string
  description?: string
  status?: string
}

export interface Payment {
  id: string
  invoiceId: string
  accountId: string
  amountPaid: number
  paymentDate: string
  paymentMethod: string
  referenceNumber?: string
}

export interface CreatePaymentPayload {
  invoiceId: string
  accountId: string
  amountPaid: number
  paymentDate: string
  paymentMethod: string
  referenceNumber?: string
}

export interface Account {
  id: string
  accountName: string
  accountCode: string
  balance: number
}

export interface CreateAccountPayload {
  accountName: string
  accountCode: string
  balance: number
}
