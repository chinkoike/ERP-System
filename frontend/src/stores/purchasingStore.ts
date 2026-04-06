import { defineStore } from 'pinia'
import { ref } from 'vue'
import { purchasingService } from '@/services/purchasingService'
import type {
  PurchaseOrder, Supplier,
  CreatePurchaseOrderPayload, PurchaseOrderItem,
  CreateSupplierPayload, UpdateSupplierPayload,
} from '@/types/purchasing'

export const usePurchasingStore = defineStore('purchasing', () => {
  const purchaseOrders = ref<PurchaseOrder[]>([])
  const suppliers = ref<Supplier[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchPurchaseOrders() {
    loading.value = true
    error.value = null
    try {
      purchaseOrders.value = await purchasingService.getPurchaseOrders()
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'โหลด PO ไม่สำเร็จ'
    } finally {
      loading.value = false
    }
  }

  async function fetchSuppliers() {
    try {
      suppliers.value = await purchasingService.getSuppliers()
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'โหลด Supplier ไม่สำเร็จ'
    }
  }

  async function createPurchaseOrder(payload: CreatePurchaseOrderPayload) {
    await purchasingService.createPurchaseOrder(payload)
    await fetchPurchaseOrders()
  }

  async function receivePurchaseOrder(id: string, items: PurchaseOrderItem[]) {
    await purchasingService.receivePurchaseOrder(id, items)
    await fetchPurchaseOrders()
  }

  async function cancelPurchaseOrder(id: string) {
    await purchasingService.cancelPurchaseOrder(id)
    await fetchPurchaseOrders()
  }

  async function createSupplier(payload: CreateSupplierPayload) {
    await purchasingService.createSupplier(payload)
    await fetchSuppliers()
  }

  async function updateSupplier(id: string, payload: UpdateSupplierPayload) {
    await purchasingService.updateSupplier(id, payload)
    await fetchSuppliers()
  }

  async function deleteSupplier(id: string) {
    await purchasingService.deleteSupplier(id)
    suppliers.value = suppliers.value.filter(s => s.id !== id)
  }

  return {
    purchaseOrders, suppliers, loading, error,
    fetchPurchaseOrders, fetchSuppliers,
    createPurchaseOrder, receivePurchaseOrder, cancelPurchaseOrder,
    createSupplier, updateSupplier, deleteSupplier,
  }
})
