import http from './http'
import type {
  Invoice, Payment, Account,
  CreateInvoicePayload, UpdateInvoicePayload,
  CreatePaymentPayload, CreateAccountPayload,
} from '@/types/finance'

export const financeService = {
  // --- Invoices ---
  async getInvoices(): Promise<Invoice[]> {
    const res = await http.get<Invoice[]>('/api/finance/invoices')
    return res.data
  },
  async getInvoiceById(id: string): Promise<Invoice> {
    const res = await http.get<Invoice>(`/api/finance/invoices/${id}`)
    return res.data
  },
  async createInvoice(payload: CreateInvoicePayload): Promise<{ id: string }> {
    const res = await http.post<{ id: string }>('/api/finance/invoices', payload)
    return res.data
  },
  async updateInvoice(id: string, payload: UpdateInvoicePayload): Promise<void> {
    await http.put(`/api/finance/invoices/${id}`, payload)
  },
  async setInvoicePaid(id: string): Promise<void> {
    await http.patch(`/api/finance/invoices/${id}/pay`, {})
  },

  // --- Payments ---
  async getPaymentsByInvoice(invoiceId: string): Promise<Payment[]> {
    const res = await http.get<Payment[]>(`/api/finance/payments/invoice/${invoiceId}`)
    return res.data
  },
  async createPayment(payload: CreatePaymentPayload): Promise<{ id: string }> {
    const res = await http.post<{ id: string }>('/api/finance/payments', payload)
    return res.data
  },

  // --- Accounts ---
  async getAccounts(): Promise<Account[]> {
    const res = await http.get<Account[]>('/api/finance/accounts')
    return res.data
  },
  async createAccount(payload: CreateAccountPayload): Promise<{ id: string }> {
    const res = await http.post<{ id: string }>('/api/finance/accounts', payload)
    return res.data
  },
}
