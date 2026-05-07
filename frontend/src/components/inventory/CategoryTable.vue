<template>
  <div>
    <div class="flex justify-between items-center mb-4 gap-4">
      <div class="relative flex-1 max-w-xs">
        <input
          v-model="search"
          type="text"
          placeholder="ค้นหาหมวดหมู่..."
          class="w-full rounded-2xl border border-slate-200 bg-white py-2.5 px-4 text-sm outline-none focus:ring-2 focus:ring-slate-300 transition-all"
        />
      </div>
    </div>

    <TableSkeleton v-if="loading" :rows="5" />

    <div v-else class="overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-sm">
      <table class="min-w-full">
        <thead>
          <tr class="border-b border-slate-100 bg-slate-50">
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-wider text-slate-400"
            >
              ชื่อหมวดหมู่
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-wider text-slate-400"
            >
              คำอธิบาย
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-wider text-slate-400"
            >
              วันที่สร้าง
            </th>
            <th class="px-6 py-3"></th>
          </tr>
        </thead>
        <tbody :class="{ 'opacity-50 pointer-events-none': isSearching }">
          <template v-if="store.categories.length > 0">
            <tr
              v-for="cat in store.categories"
              :key="cat.id"
              class="border-b border-slate-100 hover:bg-slate-50/50 transition-colors"
            >
              <td class="px-6 py-4 text-sm font-semibold text-slate-900">{{ cat.name }}</td>
              <td class="px-6 py-4 text-sm text-slate-500 max-w-xs truncate">
                {{ cat.description || '-' }}
              </td>
              <td class="px-6 py-4 text-sm text-slate-500">{{ formatDate(cat.createdAt) }}</td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-2 justify-end">
                  <button
                    v-if="authStore.isAdmin || authStore.isManager"
                    @click="$emit('edit', cat)"
                    class="rounded-2xl border border-slate-200 px-3 py-2 text-xs font-medium text-slate-600 hover:bg-slate-50 transition-colors"
                  >
                    แก้ไข
                  </button>
                  <button
                    v-if="authStore.isAdmin || authStore.isManager"
                    @click="$emit('delete', 'category', cat.id, cat.name)"
                    class="rounded-2xl border border-rose-200 px-3 py-2 text-xs font-medium text-rose-600 hover:bg-rose-50 transition-colors"
                  >
                    ลบ
                  </button>
                </div>
              </td>
            </tr>
          </template>

          <tr v-else>
            <td colspan="4" class="py-16 text-center">
              <div class="flex flex-col items-center justify-center gap-2 text-slate-400">
                <p class="text-sm font-medium">ไม่พบข้อมูลหมวดหมู่</p>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import { useInventoryStore } from '@/stores/inventoryStore'
import TableSkeleton from '@/components/common/TableSkeleton.vue' // เพิ่มการ Import

const authStore = useAuthStore()
const store = useInventoryStore()
const loading = ref(true)
const isSearching = ref(false)
const search = ref('')

const loadData = async (page = 1) => {
  isSearching.value = true
  try {
    await store.fetchCategories({ searchTerm: search.value, pageNumber: page })
  } finally {
    loading.value = false
    isSearching.value = false
  }
}

const formatDate = (d: string) => {
  if (!d) return '-'
  return new Date(d).toLocaleDateString('th-TH', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })
}

watch(search, () => loadData(1))
onMounted(() => loadData(1))
</script>
