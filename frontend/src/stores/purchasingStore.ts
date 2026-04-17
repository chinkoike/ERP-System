import { defineStore } from 'pinia'
import { ref } from 'vue'
import { purchasingService } from '@/services/purchasingService'
import type {
  PurchaseOrder,
  Supplier,
  CreatePurchaseOrderPayload,
  PurchaseOrderItem,
  CreateSupplierPayload,
  UpdateSupplierPayload,
} from '@/types/purchasing'

export const usePurchasingStore = defineStore('purchasing', () => {
  const purchaseOrders = ref<PurchaseOrder[]>([])
  const currentPage = ref(1)
  const pageSize = ref(10)
  const totalItems = ref(0)
  const totalPages = ref(1)
  const suppliers = ref<Supplier[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchPurchaseOrders(
    filter: {
      searchTerm?: string
      supplierId?: string
      status?: string
      pageNumber?: number
      pageSize?: number
    } = {},
  ) {
    loading.value = true
    error.value = null
    try {
      const result = await purchasingService.searchPurchaseOrders({
        searchTerm: filter.searchTerm,
        supplierId: filter.supplierId,
        status: filter.status,
        pageNumber: filter.pageNumber ?? currentPage.value,
        pageSize: filter.pageSize ?? pageSize.value,
      })
      purchaseOrders.value = result.items
      currentPage.value = result.pageNumber
      pageSize.value = result.pageSize
      totalItems.value = result.totalCount
      totalPages.value = Math.max(1, result.totalPages)
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
    suppliers.value = suppliers.value.filter((s) => s.id !== id)
  }

  return {
    purchaseOrders,
    currentPage,
    pageSize,
    totalItems,
    totalPages,
    suppliers,
    loading,
    error,
    fetchPurchaseOrders,
    fetchSuppliers,
    createPurchaseOrder,
    receivePurchaseOrder,
    cancelPurchaseOrder,
    createSupplier,
    updateSupplier,
    deleteSupplier,
  }
})
