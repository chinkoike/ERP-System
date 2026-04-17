<template>
  <div class="min-h-screen bg-slate-50 font-sans">
    <main class="px-8 py-8 max-w-7xl mx-auto">
      <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between mb-6">
        <div>
          <h1 class="text-3xl font-semibold text-slate-900">Inventory</h1>
          <p class="text-sm text-slate-500 mt-2">จัดการสินค้าและหมวดหมู่ในระบบ</p>
        </div>
        <button
          @click="openProductModal()"
          class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2.5 text-sm font-medium text-white transition hover:bg-slate-800 shadow-sm"
        >
          <span class="text-lg leading-none">+</span> เพิ่มสินค้า
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
          @delete="(type, id, name) => confirmDelete(type, id, name)"
        />

        <CategoryTable
          v-if="activeTab === 'categories'"
          @edit="openCategoryModal"
          @delete="(type: 'category', id: string, name: string) => confirmDelete(type, id, name)"
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

    <DeleteConfirmModal
      v-model="showDeleteModal"
      :title="deleteTarget.type === 'product' ? 'ลบสินค้า' : 'ลบหมวดหมู่'"
      :item-name="deleteTarget.name"
      @confirm="handleDelete"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useInventoryStore } from '@/stores/inventoryStore'

// Components
import ProductTable from '@/components/inventory/ProductTable.vue'
import CategoryTable from '@/components/inventory/CategoryTable.vue'
import ProductModal from '@/components/inventory/ProductModal.vue'
import CategoryModal from '@/components/inventory/CategoryModal.vue'
import StockAdjustmentModal from '@/components/inventory/StockAdjustmentModal.vue'
import DeleteConfirmModal from '@/components/common/DeleteConfirmModal.vue'

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

const showDeleteModal = ref(false)
const deleteTarget = reactive({ type: '' as 'product' | 'category' | '', id: '', name: '' })

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

const confirmDelete = (type: 'product' | 'category', id: string, name: string) => {
  deleteTarget.type = type
  deleteTarget.id = id
  deleteTarget.name = name
  showDeleteModal.value = true
}

// --- Actions ---
const refreshProducts = () => store.fetchProducts({ pageNumber: 1 })
const refreshCategories = () => store.fetchCategories({ pageNumber: 1 })

const handleDelete = async () => {
  try {
    if (deleteTarget.type === 'product') {
      await store.deleteProduct(deleteTarget.id)
    } else if (deleteTarget.type === 'category') {
      await store.deleteCategory(deleteTarget.id)
    }

    showDeleteModal.value = false
    activeTab.value === 'products' ? refreshProducts() : refreshCategories()
  } catch (error: unknown) {
    const errorMsg =
      (error as { response?: { data?: { message?: string } } }).response?.data?.message ||
      'ไม่สามารถลบข้อมูลได้ กรุณาลองใหม่อีกครั้ง'
    alert(`ข้อผิดพลาด: ${errorMsg}`)
    console.error('Delete failed:', error)
  }
}
</script>
