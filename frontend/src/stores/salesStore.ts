import { defineStore } from 'pinia'
import { ref } from 'vue'
import { salesService } from '@/services/salesService'
import type { OrderSummary, Customer, CreateOrderPayload, CreateCustomerPayload, UpdateCustomerPayload } from '@/types/sales'

export const useSalesStore = defineStore('sales', () => {
  const orders = ref<OrderSummary[]>([])
  const customers = ref<Customer[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchOrders() {
    loading.value = true
    error.value = null
    try {
      orders.value = await salesService.getOrders()
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'โหลด order ไม่สำเร็จ'
    } finally {
      loading.value = false
    }
  }

  async function fetchCustomers() {
    try {
      customers.value = await salesService.getCustomers()
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'โหลดลูกค้าไม่สำเร็จ'
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
    customers.value = customers.value.filter(c => c.id !== id)
  }

  return {
    orders, customers, loading, error,
    fetchOrders, fetchCustomers,
    createOrder, updateOrderStatus, cancelOrder,
    createCustomer, updateCustomer, deleteCustomer,
  }
})
