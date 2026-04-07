import { defineStore } from 'pinia'
import { ref } from 'vue'
import { financeService } from '@/services/financeService'
import type {
  Invoice,
  Payment,
  Account,
  CreateInvoicePayload,
  UpdateInvoicePayload,
  CreatePaymentPayload,
  CreateAccountPayload,
} from '@/types/finance'

export const useFinanceStore = defineStore('finance', () => {
  const invoices = ref<Invoice[]>([])
  const payments = ref<Payment[]>([])
  const accounts = ref<Account[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchInvoices() {
    loading.value = true
    error.value = null
    try {
      invoices.value = await financeService.getInvoices()
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'โหลด invoice ไม่สำเร็จ'
    } finally {
      loading.value = false
    }
  }

  async function fetchAccounts() {
    try {
      accounts.value = await financeService.getAccounts()
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'โหลด account ไม่สำเร็จ'
    }
  }

  async function fetchPaymentsByInvoice(invoiceId: string) {
    payments.value = await financeService.getPaymentsByInvoice(invoiceId)
  }

  async function createInvoice(payload: CreateInvoicePayload) {
    await financeService.createInvoice(payload)
    await fetchInvoices()
  }

  async function updateInvoice(id: string, payload: UpdateInvoicePayload) {
    await financeService.updateInvoice(id, payload)
    await fetchInvoices()
  }

  async function setInvoicePaid(id: string) {
    await financeService.setInvoicePaid(id)
    await fetchInvoices()
  }

  async function createPayment(payload: CreatePaymentPayload) {
    await financeService.createPayment(payload)
    await Promise.all([fetchInvoices(), fetchAccounts()])
  }

  async function createAccount(payload: CreateAccountPayload) {
    await financeService.createAccount(payload)
    await fetchAccounts()
  }

  return {
    invoices,
    payments,
    accounts,
    loading,
    error,
    fetchInvoices,
    fetchAccounts,
    fetchPaymentsByInvoice,
    createInvoice,
    updateInvoice,
    setInvoicePaid,
    createPayment,
    createAccount,
  }
})
