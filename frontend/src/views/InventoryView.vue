<template>
  <div class="min-h-screen bg-slate-50 font-sans">
    <main class="px-8 py-8 max-w-7xl mx-auto">
      <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between mb-6">
        <div>
          <h1 class="text-3xl font-semibold text-slate-900">Inventory</h1>
          <p class="text-sm text-slate-500 mt-2">จัดการสินค้าและหมวดหมู่ในระบบ</p>
        </div>
        <button
          v-if="activeTab === 'products'"
          @click="openProductModal()"
          class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2.5 text-sm font-medium text-white transition hover:bg-slate-800 shadow-sm"
        >
          <span class="text-lg leading-none">+</span> เพิ่มสินค้า
        </button>
        <button
          v-if="activeTab === 'categories'"
          @click="openCategoryModal()"
          class="rounded-2xl bg-slate-900 px-4 py-2.5 text-sm font-semibold text-white hover:bg-slate-800 transition-colors shadow-sm"
        >
          + เพิ่มหมวดหมู่
        </button>
      </div>

      <div
        class="inline-flex items-center rounded-2xl border border-slate-200 bg-white p-1 mb-6 shadow-sm"
      >
        <button
          v-for="tab in tabs"
          :key="tab.key"
          @click="activeTab = tab.key"
          :class="[
            'rounded-2xl px-5 py-2.5 text-sm transition-all duration-200',
            activeTab === tab.key
              ? 'bg-slate-900 font-semibold text-white shadow-md'
              : 'font-medium text-slate-500 hover:text-slate-900 hover:bg-slate-50',
          ]"
        >
          {{ tab.label }}
        </button>
      </div>

      <div class="transition-all duration-300">
        <ProductTable
          v-if="activeTab === 'products'"
          @edit="openProductModal"
          @adjust-stock="openStockAdjustment"
          @delete="openDeleteConfirm"
        />

        <CategoryTable
          v-if="activeTab === 'categories'"
          @edit="openCategoryModal"
          @delete="openDeleteConfirm"
          @add-click="openCategoryModal()"
        />
      </div>
    </main>

    <ProductModal
      v-model="showProductModal"
      :editing-item="editingProduct"
      @success="refreshProducts"
    />

    <CategoryModal
      v-model="showCategoryModal"
      :editing-item="editingCategory"
      @success="refreshCategories"
    />

    <StockAdjustmentModal
      v-model="showStockModal"
      :target="stockTarget || { id: '', name: '', currentStock: 0 }"
      @success="refreshProducts"
    />

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
import { ref } from 'vue'
import { useInventoryStore } from '@/stores/inventoryStore'

// Components
import ProductTable from '@/components/inventory/ProductTable.vue'
import CategoryTable from '@/components/inventory/CategoryTable.vue'
import ProductModal from '@/components/inventory/ProductModal.vue'
import CategoryModal from '@/components/inventory/CategoryModal.vue'
import StockAdjustmentModal from '@/components/inventory/StockAdjustmentModal.vue'
import ConfirmModal from '@/components/common/ConfirmModal.vue'
const store = useInventoryStore()
const activeTab = ref<'products' | 'categories'>('products')
const tabs = [
  { key: 'products', label: 'สินค้า' },
  { key: 'categories', label: 'หมวดหมู่' },
] as const

// --- Modal States ---
const showProductModal = ref(false)
const editingProduct = ref(null)

const showCategoryModal = ref(false)
const editingCategory = ref(null)

const showStockModal = ref(false)
const stockTarget = ref(null)

// --- Modal Logic ---
const openProductModal = (product = null) => {
  editingProduct.value = product
  showProductModal.value = true
}

const openCategoryModal = (category = null) => {
  editingCategory.value = category
  showCategoryModal.value = true
}

const openStockAdjustment = (product = null) => {
  stockTarget.value = product
  showStockModal.value = true
}

// --- Actions ---
const refreshProducts = () => store.fetchProducts({ pageNumber: 1 })
const refreshCategories = () => store.fetchCategories({ pageNumber: 1 })

// --- Confirm Modal States ---
const showConfirmModal = ref(false)
const confirmTitle = ref('')
const confirmMessage = ref('')
const confirmBtn = ref('')
const modalLoading = ref(false)
const confirmAction = ref<() => Promise<void>>(async () => {})

// --- ฟังก์ชันกลางสำหรับยืนยันการลบ ---
function openDeleteConfirm(type: 'product' | 'category', id: string, name: string) {
  confirmTitle.value = type === 'product' ? 'ยืนยันการลบสินค้า' : 'ยืนยันการลบหมวดหมู่'
  confirmMessage.value = `คุณแน่ใจหรือไม่ที่จะลบ "${name}"? การกระทำนี้ไม่สามารถย้อนคืนได้`
  confirmBtn.value = 'ลบข้อมูล'

  confirmAction.value = async () => {
    modalLoading.value = true
    try {
      if (type === 'product') {
        await store.deleteProduct(id)
        refreshProducts()
      } else {
        await store.deleteCategory(id)
        refreshCategories()
      }
      showConfirmModal.value = false
    } catch (error: unknown) {
      alert(
        (error as { response?: { data?: { message?: string } } }).response?.data?.message ||
          'เกิดข้อผิดพลาดในการลบ',
      )
    } finally {
      modalLoading.value = false
    }
  }
  showConfirmModal.value = true
}
</script>
