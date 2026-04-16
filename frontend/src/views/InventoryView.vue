<template>
  <div class="min-h-screen bg-slate-50 font-sans">
    <!-- Top bar -->

    <main class="px-8 py-8 max-w-7xl mx-auto">
      <!-- Page heading -->
      <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between mb-6">
        <div>
          <h1 class="text-3xl font-semibold text-slate-900">Inventory</h1>
          <p class="text-sm text-slate-500 mt-2">จัดการสินค้าและหมวดหมู่</p>
        </div>
        <button
          @click="openProductModal()"
          class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
        >
          <span class="text-lg leading-none">+</span> เพิ่มสินค้า
        </button>
      </div>

      <!-- Tabs -->
      <div
        class="inline-flex items-center rounded-2xl border border-slate-200 bg-white p-1 mb-6 shadow-sm"
      >
        <button
          v-for="tab in tabs"
          :key="tab.key"
          @click="activeTab = tab.key"
          :class="
            activeTab === tab.key
              ? 'rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white'
              : 'rounded-2xl px-4 py-2 text-sm font-medium text-slate-500 hover:text-slate-900'
          "
        >
          {{ tab.label }}
        </button>
      </div>

      <!-- PRODUCTS TAB -->
      <div v-if="activeTab === 'products'">
        <!-- Filter row -->
        <div class="flex items-center gap-3 mb-4">
          <div class="relative flex-1 max-w-xs">
            <svg
              class="absolute left-3 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-slate-400"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0"
              />
            </svg>
            <input
              v-model="searchProduct"
              type="text"
              placeholder="ค้นหาสินค้า..."
              class="w-full rounded-2xl border border-slate-200 bg-white py-2.5 pl-9 pr-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
            />
          </div>
          <select
            v-model="filterCategory"
            class="rounded-2xl border border-slate-200 bg-white py-2.5 px-3 text-sm text-slate-600 outline-none focus:ring-2 focus:ring-slate-300"
          >
            <option value="">ทุกหมวดหมู่</option>
            <option v-for="c in store.categories" :key="c.id" :value="c.id">{{ c.name }}</option>
          </select>
        </div>

        <!-- Products table -->
        <div class="overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-sm">
          <table class="min-w-full border-collapse">
            <thead>
              <tr class="border-b border-slate-100 bg-slate-50">
                <th
                  class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  สินค้า
                </th>
                <th
                  class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  SKU
                </th>
                <th
                  class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  หมวดหมู่
                </th>
                <th
                  class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  ราคา
                </th>
                <th
                  class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  Stock
                </th>
                <th
                  class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  สถานะ
                </th>
                <th class="px-6 py-3"></th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="filteredProducts.length === 0">
                <td colspan="7" class="px-6 py-12 text-center text-sm text-slate-400">
                  ไม่พบสินค้า
                </td>
              </tr>
              <tr
                v-for="(p, i) in filteredProducts"
                :key="p.id"
                :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
              >
                <td class="px-6 py-4">
                  <div class="text-sm font-semibold text-slate-900">{{ p.name }}</div>
                  <div v-if="p.description" class="max-w-50 truncate text-xs text-slate-500">
                    {{ p.description }}
                  </div>
                </td>
                <td class="px-6 py-4 text-xs font-medium text-slate-500 font-mono">
                  {{ p.sku }}
                </td>
                <td class="px-6 py-4 text-sm text-slate-500">{{ p.categoryName }}</td>
                <td class="px-6 py-4 text-right text-sm font-semibold text-slate-900">
                  {{ formatCurrency(p.basePrice) }}
                </td>
                <td class="px-6 py-4 text-center text-sm">
                  <span
                    :class="
                      p.currentStock <= 10
                        ? 'font-semibold text-rose-600'
                        : 'font-semibold text-slate-900'
                    "
                    >{{ p.currentStock }}</span
                  >
                </td>
                <td class="px-6 py-4 text-center">
                  <span :class="stockStatusClass(p.currentStock)">{{
                    stockStatusLabel(p.currentStock)
                  }}</span>
                </td>
                <td class="px-6 py-4">
                  <div class="flex items-center gap-2 justify-end">
                    <button
                      @click="openStockModal(p)"
                      title="ปรับ Stock"
                      class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                    >
                      ±Stock
                    </button>
                    <button
                      @click="openProductModal(p)"
                      class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                    >
                      แก้ไข
                    </button>
                    <button
                      @click="confirmDelete('product', p.id, p.name)"
                      class="rounded-2xl border border-rose-200 bg-white px-3 py-2 text-xs font-medium text-rose-600 transition hover:bg-rose-50"
                    >
                      ลบ
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="flex items-center justify-between bg-white px-6 py-4 text-sm text-slate-500">
          <div>แสดง {{ store.products.length }} จาก {{ store.totalItems }} รายการ</div>
          <div class="flex items-center gap-2">
            <button
              @click="loadProducts(store.currentPage - 1)"
              :disabled="store.currentPage <= 1"
              class="rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-600 disabled:cursor-not-allowed disabled:opacity-50"
            >
              ก่อนหน้า
            </button>
            <span>หน้า {{ store.currentPage }} / {{ store.totalPages }}</span>
            <button
              @click="loadProducts(store.currentPage + 1)"
              :disabled="store.currentPage >= store.totalPages"
              class="rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-600 disabled:cursor-not-allowed disabled:opacity-50"
            >
              ถัดไป
            </button>
          </div>
        </div>
      </div>

      <!-- CATEGORIES TAB -->
      <div v-if="activeTab === 'categories'">
        <div class="flex justify-between mb-4">
          <div class="relative flex-1 max-w-xs">
            <svg
              class="absolute left-3 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-slate-400"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0"
              />
            </svg>
            <input
              ref="searchInputRef"
              v-model="searchCategoryText"
              type="text"
              placeholder="ค้นหาหมวดหมู่..."
              class="w-full rounded-2xl border border-slate-200 bg-white py-2.5 pl-9 pr-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
            />
          </div>
          <button
            @click="openCategoryModal()"
            class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
          >
            <span class="text-lg leading-none">+</span> เพิ่มหมวดหมู่
          </button>
        </div>
        <div class="overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-sm">
          <table class="min-w-full border-collapse">
            <thead>
              <tr class="border-b border-slate-100 bg-slate-50">
                <th
                  class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  ชื่อหมวดหมู่
                </th>
                <th
                  class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  คำอธิบาย
                </th>
                <th
                  class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  สร้างเมื่อ
                </th>
                <th class="px-6 py-3"></th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="store.categories.length === 0">
                <td colspan="4" class="px-6 py-12 text-center text-sm text-slate-400">
                  ไม่พบหมวดหมู่
                </td>
              </tr>
              <tr
                v-for="(c, i) in store.categories"
                :key="c.id"
                :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
              >
                <td class="px-6 py-4 text-sm font-semibold text-slate-900">{{ c.name }}</td>
                <td class="px-6 py-4 text-sm text-slate-500">{{ c.description ?? '—' }}</td>
                <td class="px-6 py-4 text-center text-sm text-slate-500">
                  {{ formatDate(c.createdAt) }}
                </td>
                <td class="px-6 py-4">
                  <div class="flex items-center gap-2 justify-end">
                    <button
                      @click="openCategoryModal(c)"
                      class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                    >
                      แก้ไข
                    </button>
                    <button
                      @click="confirmDelete('category', c.id, c.name)"
                      class="rounded-2xl border border-rose-200 bg-white px-3 py-2 text-xs font-medium text-rose-600 transition hover:bg-rose-50"
                    >
                      ลบ
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="flex items-center justify-between bg-white px-6 py-4 text-sm text-slate-500">
          <div>แสดง {{ store.categories.length }} จาก {{ store.categoryTotalItems }} รายการ</div>
          <div class="flex items-center gap-2">
            <button
              @click="loadCategories(store.categoryPage - 1)"
              :disabled="store.categoryPage <= 1"
              class="rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-600 disabled:cursor-not-allowed disabled:opacity-50"
            >
              ก่อนหน้า
            </button>
            <span>หน้า {{ store.categoryPage }} / {{ store.categoryTotalPages }}</span>
            <button
              @click="loadCategories(store.categoryPage + 1)"
              :disabled="store.categoryPage >= store.categoryTotalPages"
              class="rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-600 disabled:cursor-not-allowed disabled:opacity-50"
            >
              ถัดไป
            </button>
          </div>
        </div>
      </div>
    </main>

    <!-- ===== PRODUCT MODAL ===== -->
    <Teleport to="body">
      <div
        v-if="showProductModal"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-xl overflow-hidden rounded-2xl bg-white shadow-2xl">
          <div class="flex items-center justify-between border-b border-slate-200 px-6 py-5">
            <span class="text-base font-semibold text-slate-900">{{
              editingProduct ? 'แก้ไขสินค้า' : 'เพิ่มสินค้า'
            }}</span>
            <button
              @click="showProductModal = false"
              class="text-slate-500 hover:text-slate-700 text-2xl"
            >
              ×
            </button>
          </div>
          <div class="space-y-4 p-6">
            <div>
              <label class="mb-2 block text-sm font-medium text-slate-600">ชื่อสินค้า *</label>
              <input
                v-model="productForm.name"
                type="text"
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>
            <div v-if="!editingProduct">
              <label class="mb-2 block text-sm font-medium text-slate-600">SKU *</label>
              <input
                v-model="productForm.sku"
                type="text"
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm font-mono text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>
            <div class="grid grid-cols-1 gap-3 md:grid-cols-2">
              <div>
                <label class="mb-2 block text-sm font-medium text-slate-600">ราคา (บาท) *</label>
                <input
                  v-model.number="productForm.basePrice"
                  type="number"
                  min="0"
                  class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
              <div v-if="!editingProduct">
                <label class="mb-2 block text-sm font-medium text-slate-600">Stock เริ่มต้น</label>
                <input
                  v-model.number="productForm.initialStock"
                  type="number"
                  min="0"
                  class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
            </div>
            <div>
              <label class="mb-2 block text-sm font-medium text-slate-600">หมวดหมู่ *</label>
              <select
                v-model="productForm.categoryId"
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              >
                <option value="">เลือกหมวดหมู่</option>
                <option v-for="c in store.categories" :key="c.id" :value="c.id">
                  {{ c.name }}
                </option>
              </select>
            </div>
            <div>
              <label class="mb-2 block text-sm font-medium text-slate-600">คำอธิบาย</label>
              <textarea
                v-model="productForm.description"
                rows="2"
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none resize-vertical focus:ring-2 focus:ring-slate-300"
              ></textarea>
            </div>
            <div v-if="modalError" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">
              {{ modalError }}
            </div>
          </div>
          <div class="flex justify-end gap-2 border-t border-slate-200 bg-slate-50 px-4 py-4">
            <button
              @click="showProductModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="submitProduct"
              :disabled="modalLoading"
              class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-50"
            >
              {{ modalLoading ? 'กำลังบันทึก...' : 'บันทึก' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- ===== STOCK MODAL ===== -->
    <Teleport to="body">
      <div
        v-if="showStockModal"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-md overflow-hidden rounded-2xl bg-white shadow-2xl">
          <div class="flex items-center justify-between border-b border-slate-200 px-6 py-5">
            <span class="text-base font-semibold text-slate-900">ปรับ Stock</span>
            <button
              @click="showStockModal = false"
              class="text-slate-500 hover:text-slate-700 text-2xl"
            >
              ×
            </button>
          </div>
          <div class="space-y-4 p-6">
            <div class="rounded-2xl bg-slate-50 px-4 py-3">
              <div class="text-sm font-semibold text-slate-900">{{ stockTarget?.name }}</div>
              <div class="mt-1 text-sm text-slate-500">
                Stock ปัจจุบัน:
                <strong class="text-slate-900">{{ stockTarget?.currentStock }}</strong>
              </div>
            </div>
            <div>
              <label class="mb-2 block text-sm font-medium text-slate-600"
                >จำนวนที่เปลี่ยน (+ เพิ่ม / - ลด)</label
              >
              <input
                v-model.number="stockForm.quantityChange"
                type="number"
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
              <div v-if="stockTarget" class="mt-2 text-sm text-slate-500">
                Stock ใหม่:
                <strong
                  :class="
                    stockTarget.currentStock + stockForm.quantityChange < 0
                      ? 'text-rose-600'
                      : 'text-slate-900'
                  "
                  >{{ stockTarget.currentStock + stockForm.quantityChange }}</strong
                >
              </div>
            </div>
            <div>
              <label class="mb-2 block text-sm font-medium text-slate-600">หมายเหตุ</label>
              <input
                v-model="stockForm.note"
                type="text"
                placeholder="เช่น รับสินค้าใหม่, ของเสียหาย"
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>
            <div v-if="modalError" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">
              {{ modalError }}
            </div>
          </div>
          <div class="flex justify-end gap-2 border-t border-slate-200 bg-slate-50 px-4 py-4">
            <button
              @click="showStockModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="submitStock"
              :disabled="modalLoading"
              class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-50"
            >
              {{ modalLoading ? 'กำลังบันทึก...' : 'บันทึก' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- ===== CATEGORY MODAL ===== -->
    <Teleport to="body">
      <div
        v-if="showCategoryModal"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-md overflow-hidden rounded-2xl bg-white shadow-2xl">
          <div class="flex items-center justify-between border-b border-slate-200 px-6 py-5">
            <span class="text-base font-semibold text-slate-900">{{
              editingCategory ? 'แก้ไขหมวดหมู่' : 'เพิ่มหมวดหมู่'
            }}</span>
            <button
              @click="showCategoryModal = false"
              class="text-slate-500 hover:text-slate-700 text-2xl"
            >
              ×
            </button>
          </div>
          <div class="space-y-4 p-6">
            <div>
              <label class="mb-2 block text-sm font-medium text-slate-600">ชื่อหมวดหมู่ *</label>
              <input
                v-model="categoryForm.name"
                type="text"
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>
            <div>
              <label class="mb-2 block text-sm font-medium text-slate-600">คำอธิบาย</label>
              <textarea
                v-model="categoryForm.description"
                rows="2"
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none resize-vertical focus:ring-2 focus:ring-slate-300"
              ></textarea>
            </div>
            <div v-if="modalError" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">
              {{ modalError }}
            </div>
          </div>
          <div class="flex justify-end gap-2 border-t border-slate-200 bg-slate-50 px-4 py-4">
            <button
              @click="showCategoryModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="submitCategory"
              :disabled="modalLoading"
              class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-50"
            >
              {{ modalLoading ? 'กำลังบันทึก...' : 'บันทึก' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- ===== CONFIRM DELETE MODAL ===== -->
    <Teleport to="body">
      <div
        v-if="showDeleteModal"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-md overflow-hidden rounded-2xl bg-white p-6 shadow-2xl">
          <div class="mb-2 text-base font-semibold text-slate-900">ยืนยันการลบ</div>
          <div class="mb-6 text-sm text-slate-500">
            คุณต้องการลบ <strong class="text-slate-900">{{ deleteTarget.name }}</strong> ใช่หรือไม่?
            ไม่สามารถยกเลิกได้
          </div>
          <div class="flex justify-end gap-2">
            <button
              @click="showDeleteModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="submitDelete"
              :disabled="modalLoading"
              class="rounded-2xl bg-rose-600 px-5 py-2 text-sm font-medium text-white transition hover:bg-rose-700 disabled:cursor-not-allowed disabled:opacity-50"
            >
              {{ modalLoading ? 'กำลังลบ...' : 'ลบ' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted, watch } from 'vue'
import { useInventoryStore } from '@/stores/inventoryStore'
import type { Product, Category } from '@/types/inventory'
const store = useInventoryStore()

const activeTab = ref<'products' | 'categories'>('products')
const tabs = [
  { key: 'products', label: 'สินค้า' },
  { key: 'categories', label: 'หมวดหมู่' },
] as const

// --- Filter ---
const searchInputRef = ref<HTMLInputElement | null>(null)

//product filter states
const searchProduct = ref('')
const filterCategory = ref('')
const filteredProducts = computed(() => store.products)

async function loadProducts(page = 1) {
  await store.fetchProducts({
    searchTerm: searchProduct.value.trim() || undefined,
    categoryId: filterCategory.value || undefined,
    pageNumber: page,
  })
}

watch([searchProduct, filterCategory], () => loadProducts(1))
//categories are loaded in inventoryStore on app start, so no need to fetch here
const searchCategoryText = ref('')
const categoryPage = ref(1)

// --- ฟังก์ชันโหลดข้อมูล Category ---
async function loadCategories(page = 1) {
  categoryPage.value = page

  await store.fetchCategories({
    searchTerm: searchCategoryText.value.trim() || undefined,
    pageNumber: page,
    pageSize: 10,
  })
}
watch(searchCategoryText, () => {
  if (activeTab.value === 'categories') {
    loadCategories(1)
  }
})

// เมื่อสลับ Tab มาที่ Categories ให้โหลดข้อมูล
watch(activeTab, (newTab) => {
  if (newTab === 'categories') {
    loadCategories(1)
  }
})
// --- Modal state ---
const modalLoading = ref(false)
const modalError = ref('')

// Product modal
const showProductModal = ref(false)
const editingProduct = ref<Product | null>(null)
const productForm = reactive({
  name: '',
  sku: '',
  basePrice: 0,
  initialStock: 0,
  categoryId: '',
  description: '',
})

function openProductModal(p?: Product) {
  editingProduct.value = p ?? null
  modalError.value = ''
  if (p) {
    productForm.name = p.name
    productForm.sku = p.sku
    productForm.basePrice = p.basePrice
    productForm.initialStock = 0
    productForm.categoryId = p.categoryId
    productForm.description = p.description ?? ''
  } else {
    Object.assign(productForm, {
      name: '',
      sku: '',
      basePrice: 0,
      initialStock: 0,
      categoryId: '',
      description: '',
    })
  }
  showProductModal.value = true
}

async function submitProduct() {
  if (!productForm.name.trim() || !productForm.categoryId) {
    modalError.value = 'กรุณากรอกชื่อสินค้าและเลือกหมวดหมู่'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    if (editingProduct.value) {
      await store.updateProduct(editingProduct.value.id, {
        name: productForm.name,
        description: productForm.description,
        BasePrice: productForm.basePrice,
        categoryId: productForm.categoryId,
      })
    } else {
      if (!productForm.sku.trim()) {
        modalError.value = 'กรุณากรอก SKU'
        return
      }
      await store.createProduct({ ...productForm })
    }
    showProductModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// Stock modal
const showStockModal = ref(false)
const stockTarget = ref<Product | null>(null)
const stockForm = reactive({ quantityChange: 0, note: '' })

function openStockModal(p: Product) {
  stockTarget.value = p
  stockForm.quantityChange = 0
  stockForm.note = ''
  modalError.value = ''
  showStockModal.value = true
}

async function submitStock() {
  if (!stockTarget.value) return
  if (stockTarget.value.currentStock + stockForm.quantityChange < 0) {
    modalError.value = 'Stock ไม่สามารถติดลบได้'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    await store.updateStock(stockTarget.value.id, {
      productId: stockTarget.value.id,
      quantityChange: stockForm.quantityChange,
      note: stockForm.note,
    })
    showStockModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// Category modal
const showCategoryModal = ref(false)
const editingCategory = ref<Category | null>(null)
const categoryForm = reactive({ name: '', description: '' })

function openCategoryModal(c?: Category) {
  editingCategory.value = c ?? null
  modalError.value = ''
  categoryForm.name = c?.name ?? ''
  categoryForm.description = c?.description ?? ''
  showCategoryModal.value = true
}

async function submitCategory() {
  if (!categoryForm.name.trim()) {
    modalError.value = 'กรุณากรอกชื่อหมวดหมู่'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    if (editingCategory.value) {
      await store.updateCategory(editingCategory.value.id, {
        name: categoryForm.name,
        description: categoryForm.description,
      })
    } else {
      await store.createCategory({ name: categoryForm.name, description: categoryForm.description })
    }
    showCategoryModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// Delete modal
const showDeleteModal = ref(false)
const deleteTarget = reactive({ type: '' as 'product' | 'category', id: '', name: '' })

function confirmDelete(type: 'product' | 'category', id: string, name: string) {
  deleteTarget.type = type
  deleteTarget.id = id
  deleteTarget.name = name
  showDeleteModal.value = true
}

async function submitDelete() {
  modalLoading.value = true
  try {
    if (deleteTarget.type === 'product') await store.deleteProduct(deleteTarget.id)
    else await store.deleteCategory(deleteTarget.id)
    showDeleteModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'ลบไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// --- Helpers ---
function formatCurrency(v: number) {
  return new Intl.NumberFormat('th-TH', {
    style: 'currency',
    currency: 'THB',
    maximumFractionDigits: 0,
  }).format(v)
}
function formatDate(d: string) {
  return new Date(d).toLocaleDateString('th-TH', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  })
}
function stockStatusLabel(stock: number) {
  if (stock === 0) return 'หมด'
  if (stock <= 10) return 'ใกล้หมด'
  return 'ปกติ'
}
function stockStatusClass(stock: number) {
  if (stock === 0)
    return 'inline-flex rounded-full bg-rose-100 px-2.5 py-1 text-[11px] font-semibold text-rose-700'
  if (stock <= 10)
    return 'inline-flex rounded-full bg-amber-100 px-2.5 py-1 text-[11px] font-semibold text-amber-700'
  return 'inline-flex rounded-full bg-emerald-100 px-2.5 py-1 text-[11px] font-semibold text-emerald-700'
}

onMounted(async () => {
  await Promise.all([loadProducts(1), loadCategories(1)])
})
</script>
