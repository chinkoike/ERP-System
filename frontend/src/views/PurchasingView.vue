<template>
  <div class="min-h-screen bg-slate-50">
    <main class="px-8 py-8 max-w-7xl mx-auto">
      <!-- Heading -->
      <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between mb-6">
        <div>
          <h1 class="text-3xl font-semibold tracking-tight text-slate-900">Purchasing</h1>
          <p class="mt-2 text-sm text-slate-500">จัดการใบสั่งซื้อและผู้ขาย</p>
        </div>
        <div class="flex flex-wrap items-center gap-3">
          <button
            v-if="
              activeTab === 'orders' &&
              (authStore.isAdmin || authStore.isManager || authStore.isUser)
            "
            @click="openPoModal()"
            class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
          >
            <span class="text-lg leading-none">+</span> สร้าง PO
          </button>
          <button
            v-if="activeTab === 'suppliers' && (authStore.isAdmin || authStore.isManager)"
            @click="openSupplierModal()"
            class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
          >
            <span class="text-lg leading-none">+</span> เพิ่ม Supplier
          </button>
        </div>
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
      <TableSkeleton v-if="loading" :rows="6" />

      <template v-else>
        <!-- Purchase Orders Tab -->
        <div v-if="activeTab === 'orders'">
          <PurchaseOrderTable
            :purchase-orders="store.purchaseOrders"
            :is-searching="isSearching"
            :suppliers="store.suppliers"
            :total-items="store.totalItems"
            :current-page="store.currentPage"
            :total-pages="store.totalPages"
            v-model:searchPo="searchPo"
            v-model:filterSupplier="filterPoSupplier"
            v-model:filterStatus="filterPoStatus"
            @receive="openReceiveModal"
            @cancel="confirmCancelPo"
            @pageChange="loadPurchaseOrders"
          />
        </div>

        <!-- Suppliers Tab -->
        <div v-if="activeTab === 'suppliers'">
          <SupplierList
            :suppliers="filteredSuppliers"
            :is-searching="isSearching"
            v-model:searchSupplier="searchSupplier"
            @edit="openSupplierModal"
            @delete="confirmDeleteSupplier"
          />
        </div>
      </template>
    </main>

    <!-- PO Modal -->
    <PurchaseOrderModal
      :show="showPoModal"
      :suppliers="store.suppliers"
      :products="products"
      :loading="modalLoading"
      :error="modalError"
      @close="showPoModal = false"
      @submit="submitPo"
    />

    <!-- Receive Modal -->
    <ReceiveModal
      :show="showReceiveModal"
      :purchase-order="receivingPo"
      :products="products"
      :loading="modalLoading"
      :error="modalError"
      @close="showReceiveModal = false"
      @submit="submitReceive"
    />

    <!-- Supplier Modal -->
    <SupplierModal
      :show="showSupplierModal"
      :editing-supplier="editingSupplier"
      :loading="modalLoading"
      :error="modalError"
      @close="showSupplierModal = false"
      @submit="submitSupplier"
    />

    <!-- Confirm Modal -->
    <ConfirmModal
      :show="showConfirmModal"
      :title="confirmTitle"
      :message="confirmMessage"
      :confirm-label="confirmBtn"
      :loading="modalLoading"
      @close="showConfirmModal = false"
      @confirm="runConfirmAction"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { usePurchasingStore } from '@/stores/purchasingStore'
import { useInventoryStore } from '@/stores/inventoryStore'
import { useAuthStore } from '@/stores/authStore'
import type {
  PurchaseOrder,
  PurchaseOrderItem,
  Supplier,
  CreatePurchaseOrderPayload,
  CreateSupplierPayload,
  UpdateSupplierPayload,
} from '@/types/purchasing'

import PurchaseOrderTable from '@/components/purchasing/PurchaseOrderTable.vue'
import SupplierList from '@/components/purchasing/SupplierList.vue'
import PurchaseOrderModal from '@/components/purchasing/PurchaseOrderModal.vue'
import ReceiveModal from '@/components/purchasing/ReceiveModal.vue'
import type { ReceivePayload } from '@/components/purchasing/ReceiveModal.vue'
import SupplierModal from '@/components/purchasing/SupplierModal.vue'
import ConfirmModal from '@/components/common/ConfirmModal.vue'

const store = usePurchasingStore()
const inventoryStore = useInventoryStore()
const authStore = useAuthStore()

const products = computed(() => inventoryStore.products)

const activeTab = ref<'orders' | 'suppliers'>('orders')
const tabs = [
  { key: 'orders', label: 'Purchase Orders' },
  { key: 'suppliers', label: 'Suppliers' },
] as const

// ─── Purchase Orders ───────────────────────────────────────────
const searchPo = ref('')
const filterPoSupplier = ref('')
const filterPoStatus = ref('')
const loading = ref(true)
const isSearching = ref(false)
async function loadPurchaseOrders(page = 1) {
  try {
    isSearching.value = true

    await store.fetchPurchaseOrders({
      searchTerm: searchPo.value.trim() || undefined,
      supplierId: filterPoSupplier.value || undefined,
      status: filterPoStatus.value || undefined,
      pageNumber: page,
    })
  } finally {
    loading.value = false
    isSearching.value = false
  }
}

watch([searchPo, filterPoSupplier, filterPoStatus], () => loadPurchaseOrders(1))

// ─── Suppliers ─────────────────────────────────────────────────
const searchSupplier = ref('')
let debounceTimer = null as ReturnType<typeof setTimeout> | null
watch(searchSupplier, (newValue) => {
  const val = newValue.trim()

  if (val.length > 0) {
    isSearching.value = true

    if (debounceTimer) clearTimeout(debounceTimer)

    debounceTimer = setTimeout(() => {
      isSearching.value = false
    }, 100)
  } else {
    isSearching.value = false
    if (debounceTimer) clearTimeout(debounceTimer)
  }
})
const filteredSuppliers = computed(() =>
  store.suppliers.filter(
    (s) =>
      s.name.toLowerCase().includes(searchSupplier.value.toLowerCase()) ||
      (s.email ?? '').toLowerCase().includes(searchSupplier.value.toLowerCase()),
  ),
)

// ─── Shared modal state ────────────────────────────────────────
const modalLoading = ref(false)
const modalError = ref('')

// ─── PO Modal ──────────────────────────────────────────────────
const showPoModal = ref(false)

function openPoModal() {
  modalError.value = ''
  showPoModal.value = true
}

async function submitPo(payload: CreatePurchaseOrderPayload) {
  if (!payload.supplierId || payload.items.length === 0) {
    modalError.value = 'กรุณาเลือก Supplier และเพิ่มสินค้าอย่างน้อย 1 รายการ'
    return
  }
  if (payload.items.some((i) => !i.productId || i.quantityOrdered < 1)) {
    modalError.value = 'กรุณาเลือกสินค้าและจำนวนให้ครบ'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    await store.createPurchaseOrder(payload)
    showPoModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'สร้าง PO ไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// ─── Receive Modal ─────────────────────────────────────────────
const showReceiveModal = ref(false)
const receivingPo = ref<PurchaseOrder | null>(null)

function openReceiveModal(po: PurchaseOrder) {
  receivingPo.value = po
  modalError.value = ''
  showReceiveModal.value = true
}

async function submitReceive(payload: ReceivePayload) {
  if (!receivingPo.value) return
  if (payload.items.length === 0) {
    modalError.value = 'กรุณาระบุจำนวนที่รับ'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    await store.receivePurchaseOrder(receivingPo.value.id, payload.items as PurchaseOrderItem[])
    showReceiveModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// ─── Supplier Modal ────────────────────────────────────────────
const showSupplierModal = ref(false)
const editingSupplier = ref<Supplier | null>(null)

function openSupplierModal(s?: Supplier) {
  editingSupplier.value = s ?? null
  modalError.value = ''
  showSupplierModal.value = true
}

async function submitSupplier(payload: CreateSupplierPayload | UpdateSupplierPayload) {
  if (!('name' in payload && payload.name?.trim())) {
    modalError.value = 'กรุณากรอกชื่อบริษัท'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    if (editingSupplier.value) {
      await store.updateSupplier(editingSupplier.value.id, payload as UpdateSupplierPayload)
    } else {
      await store.createSupplier(payload as CreateSupplierPayload)
    }
    showSupplierModal.value = false
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

function confirmCancelPo(po: PurchaseOrder) {
  confirmTitle.value = 'ยืนยันการยกเลิก PO'
  confirmMessage.value = `ต้องการยกเลิก ${po.purchaseOrderNumber} ใช่หรือไม่?`
  confirmBtn.value = 'ยกเลิก PO'
  confirmAction.value = async () => {
    await store.cancelPurchaseOrder(po.id)
    showConfirmModal.value = false
  }
  showConfirmModal.value = true
}

function confirmDeleteSupplier(s: Supplier) {
  confirmTitle.value = 'ยืนยันการลบ Supplier'
  confirmMessage.value = `ต้องการลบ ${s.name} ใช่หรือไม่?`
  confirmBtn.value = 'ลบ'
  confirmAction.value = async () => {
    await store.deleteSupplier(s.id)
    showConfirmModal.value = false
  }
  showConfirmModal.value = true
}

async function runConfirmAction() {
  modalLoading.value = true
  try {
    await confirmAction.value()
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'เกิดข้อผิดพลาด'
  } finally {
    modalLoading.value = false
  }
}

onMounted(async () => {
  await Promise.all([loadPurchaseOrders(), store.fetchSuppliers(), inventoryStore.fetchProducts()])
})
</script>
