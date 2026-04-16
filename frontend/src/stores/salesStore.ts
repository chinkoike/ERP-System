import { defineStore } from 'pinia'
import { ref } from 'vue'
import { salesService } from '@/services/salesService'
import type {
  OrderSummary,
  Customer,
  CreateOrderPayload,
  CreateCustomerPayload,
  UpdateCustomerPayload,
} from '@/types/sales'

export const useSalesStore = defineStore('sales', () => {
  const orders = ref<OrderSummary[]>([])
  const currentPage = ref(1)
  const pageSize = ref(10)
  const totalItems = ref(0)
  const totalPages = ref(1)

  const customers = ref<Customer[]>([])
  const customerTotal = ref(0)
  const customerPages = ref(1)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchOrders(
    filter: {
      searchTerm?: string
      status?: string
      pageNumber?: number
      pageSize?: number
    } = {},
  ) {
    loading.value = true
    error.value = null
    try {
      const result = await salesService.searchOrders({
        searchTerm: filter.searchTerm,
        status: filter.status,
        pageNumber: filter.pageNumber ?? currentPage.value,
        pageSize: filter.pageSize ?? pageSize.value,
      })
      orders.value = result.items
      currentPage.value = result.pageNumber
      pageSize.value = result.pageSize
      totalItems.value = result.totalCount
      totalPages.value = Math.max(1, result.totalPages)
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'โหลด order ไม่สำเร็จ'
    } finally {
      loading.value = false
    }
  }

  async function fetchCustomers(
    filter: { searchTerm?: string; pageNumber?: number; pageSize?: number } = {},
  ) {
    loading.value = true
    try {
      const result = await salesService.searchCustomers({
        searchTerm: filter.searchTerm,
        pageNumber: filter.pageNumber ?? 1,
        pageSize: filter.pageSize ?? 10,
      })
      customers.value = result.items
      customerTotal.value = result.totalCount // เก็บแยกกัน
      customerPages.value = Math.max(1, result.totalPages)
    } catch (e: unknown) {
      error.value = 'โหลดลูกค้าไม่สำเร็จ'
    } finally {
      loading.value = false
    }
  }

  async function createOrder(payload: CreateOrderPayload) {
    await salesService.createOrder(payload)
    await fetchOrders()
  }

  async function updateOrderStatus(id: string, status: number) {
    await salesService.updateOrderStatus(id, status)
    await fetchOrders()
  }

  async function cancelOrder(id: string) {
    await salesService.cancelOrder(id)
    await fetchOrders()
  }

  async function createCustomer(payload: CreateCustomerPayload) {
    await salesService.createCustomer(payload)
    await fetchCustomers()
  }

  async function updateCustomer(id: string, payload: UpdateCustomerPayload) {
    await salesService.updateCustomer(id, payload)
    await fetchCustomers()
  }

  async function deleteCustomer(id: string) {
    await salesService.deleteCustomer(id)
    customers.value = customers.value.filter((c) => c.id !== id)
  }

  return {
    orders,
    currentPage,
    pageSize,
    totalItems,
    totalPages,
    customers,
    customerTotal,
    customerPages,
    loading,
    error,
    fetchOrders,
    fetchCustomers,
    createOrder,
    updateOrderStatus,
    cancelOrder,
    createCustomer,
    updateCustomer,
    deleteCustomer,
  }
})
