<template>
  <div>
    <div class="flex items-center gap-3 mb-4">
      <div class="relative flex-1 max-w-xs">
        <input
          v-model="search"
          type="text"
          placeholder="ค้นหาสินค้า..."
          class="w-full rounded-2xl border border-slate-200 bg-white py-2.5 pl-4 pr-3 text-sm outline-none focus:ring-2 focus:ring-slate-300 transition-all"
        />
      </div>
      <select
        v-model="category"
        class="rounded-2xl border border-slate-200 bg-white py-2.5 px-3 text-sm outline-none focus:ring-2 focus:ring-slate-300"
      >
        <option value="">ทุกหมวดหมู่</option>
        <option v-for="c in store.categories" :key="c.id" :value="c.id">{{ c.name }}</option>
      </select>
    </div>

    <TableSkeleton v-if="loading" :rows="8" />

    <div v-else class="overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-sm">
      <table class="min-w-full">
        <thead>
          <tr class="border-b border-slate-100 bg-slate-50">
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-wider text-slate-400"
            >
              สินค้า
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-wider text-slate-400"
            >
              SKU
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-wider text-slate-400"
            >
              หมวดหมู่
            </th>
            <th
              class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-wider text-slate-400"
            >
              ราคา
            </th>
            <th
              class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-wider text-slate-400"
            >
              Stock
            </th>
            <th
              class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-wider text-slate-400"
            >
              สถานะ
            </th>
            <th class="px-6 py-3"></th>
          </tr>
        </thead>

        <tbody :class="{ 'opacity-50 pointer-events-none': isSearching }">
          <template v-if="store.products.length > 0">
            <tr
              v-for="p in store.products"
              :key="p.id"
              class="border-b border-slate-100 hover:bg-slate-50/50 transition-colors"
            >
              <td class="px-6 py-4">
                <div class="text-sm font-semibold text-slate-900">{{ p.name }}</div>
                <div class="text-xs text-slate-500 truncate max-w-50">{{ p.description }}</div>
              </td>
              <td class="px-6 py-4 text-xs font-mono text-slate-600">{{ p.sku }}</td>
              <td class="px-6 py-4 text-sm text-slate-600">{{ p.categoryName }}</td>
              <td class="px-6 py-4 text-right text-sm font-semibold text-slate-900">
                {{ formatCurrency(p.basePrice) }}
              </td>
              <td
                class="px-6 py-4 text-center text-sm"
                :class="p.currentStock <= 10 ? 'text-rose-600 font-bold' : 'text-slate-600'"
              >
                {{ p.currentStock }}
              </td>
              <td class="px-6 py-4 text-center">
                <span :class="stockStatusClass(p.currentStock)">
                  {{ stockStatusLabel(p.currentStock) }}
                </span>
              </td>
              <td class="px-6 py-4">
                <div class="flex gap-2 justify-end">
                  <button
                    @click="emit('adjust-stock', p)"
                    class="rounded-2xl border border-slate-200 px-3 py-2 text-xs font-medium text-slate-600 hover:bg-slate-50 transition-colors"
                  >
                    ±Stock
                  </button>
                  <button
                    @click="emit('edit', p)"
                    class="rounded-2xl border border-slate-200 px-3 py-2 text-xs font-medium text-slate-600 hover:bg-slate-50 transition-colors"
                  >
                    แก้ไข
                  </button>
                  <button
                    @click="emit('delete', 'product', p.id, p.name)"
                    class="rounded-2xl border border-rose-200 px-3 py-2 text-xs font-medium text-rose-600 hover:bg-rose-50 transition-colors"
                  >
                    ลบ
                  </button>
                </div>
              </td>
            </tr>
          </template>

          <tr v-else>
            <td colspan="7" class="py-20 text-center">
              <div class="flex flex-col items-center justify-center gap-3">
                <p class="text-slate-500 font-medium">ไม่พบข้อมูลสินค้าที่ค้นหา</p>
                <button
                  v-if="search"
                  @click="search = ''"
                  class="text-sm text-slate-900 font-semibold hover:underline"
                >
                  ล้างการค้นหา
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div
      v-if="store.products.length > 0"
      class="flex items-center justify-between px-6 py-4 text-sm text-slate-500"
    >
      <div>แสดง {{ store.products.length }} รายการ</div>
      <div class="flex items-center gap-2">
        <button
          @click="loadPage(store.currentPage - 1)"
          :disabled="store.currentPage <= 1"
          class="rounded-full border border-slate-200 px-3 py-1 text-xs hover:bg-slate-50 disabled:opacity-50 transition-colors"
        >
          ก่อนหน้า
        </button>
        <span class="font-medium text-slate-700"
          >หน้า {{ store.currentPage }} / {{ store.totalPages }}</span
        >
        <button
          @click="loadPage(store.currentPage + 1)"
          :disabled="store.currentPage >= store.totalPages"
          class="rounded-full border border-slate-200 px-3 py-1 text-xs hover:bg-slate-50 disabled:opacity-50 transition-colors"
        >
          ถัดไป
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { useInventoryStore } from '@/stores/inventoryStore'
import { formatCurrency, stockStatusClass, stockStatusLabel } from '@/utils/formatters'
import TableSkeleton from '@/components/common/TableSkeleton.vue' // อย่าลืม import มาด้วยนะครับ

const store = useInventoryStore()
const loading = ref(true)
const isSearching = ref(false)
const search = ref('')
const category = ref('')
const emit = defineEmits(['edit', 'adjust-stock', 'delete'])

const loadPage = async (page = 1) => {
  isSearching.value = true
  try {
    await store.fetchProducts({
      searchTerm: search.value,
      categoryId: category.value,
      pageNumber: page,
    })
  } finally {
    loading.value = false
    isSearching.value = false
  }
}

watch([search, category], () => loadPage(1))
onMounted(() => loadPage(1))
</script>
