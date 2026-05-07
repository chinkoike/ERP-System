<template>
  <div class="min-h-screen bg-slate-50 font-sans">
    <main class="px-8 py-8 max-w-7xl mx-auto">
      <!-- Heading -->
      <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between mb-6">
        <div>
          <h1 class="text-3xl font-semibold tracking-tight text-slate-900">Sales</h1>
          <p class="text-sm text-slate-500 mt-2">จัดการ Order และลูกค้า</p>
        </div>
        <button
          v-if="authStore.isAdmin || authStore.isManager"
          @click="openOrderModal()"
          class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
        >
          <span class="text-lg leading-none">+</span> สร้าง Order
        </button>
        <button
          v-if="authStore.isAdmin || authStore.isManager"
          @click="openCustomerModal()"
          class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
        >
          <span class="text-lg leading-none">+</span> เพิ่มลูกค้า
        </button>
      </div>

      <!-- Tabs -->
      <div class="inline-flex items-center rounded-2xl border border-slate-200 bg-white p-1 mb-6">
        <button
          v-for="tab in tabs"
          :key="tab.key"
          @click="activeTab = tab.key"
          :class="
            activeTab === tab.key
              ? 'rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white'
              : 'rounded-2xl px-4 py-2 text-sm font-medium text-slate-500 hover:text-slate-900'
          "
          class="transition"
        >
          {{ tab.label }}
        </button>
      </div>
      <!-- Loading -->
      <TableSkeleton v-if="isInitialLoading" :rows="6" />

      <template v-else>
        <!-- Orders Tab -->
        <div v-if="activeTab === 'orders'">
          <OrderTable
            :orders="store.orders"
            :total-items="store.totalItems"
            :current-page="store.currentPage"
            :total-pages="store.totalPages"
            :is-initial-loading="isInitialLoading"
            :is-searching="isSearching"
            v-model:searchOrder="searchOrder"
            v-model:filterStatus="filterStatus"
            @statusChange="handleStatusChange"
            @cancel="confirmCancel"
            @pageChange="loadOrders"
          />
        </div>

        <!-- Customers Tab -->
        <div v-if="activeTab === 'customers'">
          <CustomerList
            :customers="store.customers"
            :total-items="store.customerTotal"
            :current-page="store.customerPages"
            :total-pages="store.customerPages"
            :is-searching="isSearching"
            :is-initial-loading="isInitialLoading"
            v-model:searchCustomer="searchCustomer"
            @edit="openCustomerModal"
            @delete="confirmDeleteCustomer"
            @pageChange="loadCustomers"
          />
        </div>
      </template>
    </main>

    <!-- Order Modal -->
    <OrderModal
      :show="showOrderModal"
      :customers="store.customers"
      :products="products"
      :loading="modalLoading"
      :error="modalError"
      @close="showOrderModal = false"
      @submit="submitOrder"
    />

    <!-- Customer Modal -->
    <CustomerModal
      :show="showCustomerModal"
      :editing-customer="editingCustomer"
      :loading="modalLoading"
      :error="modalError"
      @close="showCustomerModal = false"
      @submit="submitCustomer"
    />

    <!-- Confirm Modal -->
    <ConfirmModal
      :show="showConfirmModal"
      :title="confirmTitle"
      :message="confirmMessage"
      :confirm-label="confirmBtn"
      :loading="modalLoading"
      @close="showConfirmModal = false"
      @confirm="confirmAction"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useSalesStore } from '@/stores/salesStore'
import { useInventoryStore } from '@/stores/inventoryStore'
import { useAuthStore } from '@/stores/authStore'
import type {
  Customer,
  OrderSummary,
  CreateOrderPayload,
  CreateCustomerPayload,
  UpdateCustomerPayload,
} from '@/types/sales'

import OrderTable from '@/components/sales/OrderTable.vue'
import CustomerList from '@/components/sales/CustomerList.vue'
import OrderModal from '@/components/sales/OrderModal.vue'
import CustomerModal from '@/components/sales/CustomerModal.vue'
import ConfirmModal from '@/components/common/ConfirmModal.vue'

const store = useSalesStore()
const inventoryStore = useInventoryStore()
const authStore = useAuthStore()

const activeTab = ref<'orders' | 'customers'>('orders')
const tabs = [
  { key: 'orders', label: 'Orders' },
  { key: 'customers', label: 'ลูกค้า' },
] as const

const isInitialLoading = ref(true)
const isSearching = ref(false)
const products = computed(() => inventoryStore.products)

const statusMap: Record<string, number> = {
  Pending: 0,
  Confirmed: 1,
  Processing: 2,
  Shipped: 3,
  Delivered: 4,
  Cancelled: 5,
}

// ─── Orders ───────────────────────────────────────────────────
const searchOrder = ref('')
const filterStatus = ref('')

async function loadOrders(page = 1) {
  if (page === 1 && store.orders.length === 0) {
    isInitialLoading.value = true
  } else {
    isSearching.value = true
  }
  try {
    await store.fetchOrders({
      searchTerm: searchOrder.value.trim() || undefined,
      status: filterStatus.value || undefined,
      pageNumber: page,
    })
  } finally {
    isInitialLoading.value = false
    isSearching.value = false
  }
}

watch([searchOrder, filterStatus], () => loadOrders(1))

async function handleStatusChange(orderId: string, statusStr: string) {
  if (!statusStr) return
  const status = statusMap[statusStr]
  if (status === undefined) return
  try {
    await store.updateOrderStatus(orderId, status)
  } catch {
    /* silent */
  }
}

// ─── Customers ────────────────────────────────────────────────
const searchCustomer = ref('')

async function loadCustomers(page = 1) {
  if (page === 1 && store.customers.length === 0) {
    isInitialLoading.value = true
  } else {
    isSearching.value = true
  }
  try {
    await store.fetchCustomers({
      searchTerm: searchCustomer.value.trim() || undefined,
      pageNumber: page,
      pageSize: 100,
    })
  } finally {
    isInitialLoading.value = false
    isSearching.value = false
  }
}

watch([searchCustomer], () => loadCustomers(1))

// ─── Shared modal state ────────────────────────────────────────
const modalLoading = ref(false)
const modalError = ref('')

// ─── Order Modal ───────────────────────────────────────────────
const showOrderModal = ref(false)

function openOrderModal() {
  modalError.value = ''
  showOrderModal.value = true
}

async function submitOrder(payload: CreateOrderPayload) {
  if (!payload.customerId || payload.items.length === 0 || !payload.shippingAddress) {
    modalError.value = 'กรุณาเลือกลูกค้า ที่อยู่จัดส่ง และเพิ่มสินค้าอย่างน้อย 1 รายการ'
    return
  }
  if (payload.items.some((i) => !i.productId || i.quantity < 1)) {
    modalError.value = 'กรุณาเลือกสินค้าและจำนวนให้ครบทุกรายการ'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    await store.createOrder(payload)
    showOrderModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'สร้าง order ไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// ─── Customer Modal ────────────────────────────────────────────
const showCustomerModal = ref(false)
const editingCustomer = ref<Customer | null>(null)

function openCustomerModal(c?: Customer) {
  editingCustomer.value = c ?? null
  modalError.value = ''
  showCustomerModal.value = true
}

async function submitCustomer(payload: CreateCustomerPayload | UpdateCustomerPayload) {
  if (
    !('firstName' in payload && payload.firstName?.trim()) ||
    !('email' in payload && payload.email?.trim())
  ) {
    modalError.value = 'กรุณากรอกชื่อและ email'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    if (editingCustomer.value) {
      await store.updateCustomer(editingCustomer.value.id, payload as UpdateCustomerPayload)
    } else {
      await store.createCustomer(payload as CreateCustomerPayload)
    }
    showCustomerModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// ─── Confirm Modal ─────────────────────────────────────────────
const showConfirmModal = ref(false)
const confirmTitle = ref('')
const confirmMessage = ref('')
const confirmBtn = ref('')
const confirmAction = ref<() => Promise<void>>(async () => {})

function confirmCancel(o: OrderSummary) {
  confirmTitle.value = 'ยืนยันการยกเลิก Order'
  confirmMessage.value = `ต้องการยกเลิก ${o.orderNumber} ใช่หรือไม่? stock จะถูกคืนอัตโนมัติ`
  confirmBtn.value = 'ยกเลิก Order'
  confirmAction.value = async () => {
    modalLoading.value = true
    try {
      await store.cancelOrder(o.orderId)
      showConfirmModal.value = false
    } finally {
      modalLoading.value = false
    }
  }
  showConfirmModal.value = true
}

function confirmDeleteCustomer(c: Customer) {
  confirmTitle.value = 'ยืนยันการลบลูกค้า'
  confirmMessage.value = `ต้องการลบ ${c.fullName} ใช่หรือไม่?`
  confirmBtn.value = 'ลบ'
  confirmAction.value = async () => {
    modalLoading.value = true
    try {
      await store.deleteCustomer(c.id)
      showConfirmModal.value = false
    } finally {
      modalLoading.value = false
    }
  }
  showConfirmModal.value = true
}

onMounted(async () => {
  await Promise.all([loadOrders(), loadCustomers(), inventoryStore.fetchProducts()])
})
</script>
