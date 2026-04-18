<template>
  <div class="space-y-4">
    <!-- Summary cards -->
    <div class="grid grid-cols-3 gap-4">
      <!-- Skeleton -->
      <template v-if="skeleton">
        <div v-for="n in 3" :key="n" class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <div class="h-3 w-20 rounded bg-slate-100 animate-pulse mb-3"></div>
          <div class="h-8 w-16 rounded bg-slate-200 animate-pulse"></div>
        </div>
      </template>
      <template v-else-if="items">
        <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">สินค้าทั้งหมด</p>
          <p class="text-3xl font-semibold tracking-tight text-slate-900">{{ items.length }}</p>
        </div>
        <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">ใกล้หมด / หมด</p>
          <p class="text-3xl font-semibold tracking-tight text-rose-600">
            {{ items.filter((i) => i.stockStatus === 'Low').length }}
          </p>
        </div>
        <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">สต็อกปกติ</p>
          <p class="text-3xl font-semibold tracking-tight text-emerald-600">
            {{ items.filter((i) => i.stockStatus === 'Healthy').length }}
          </p>
        </div>
      </template>
    </div>

    <!-- Search -->
    <div class="relative max-w-md">
      <svg
        class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400"
        fill="none" viewBox="0 0 24 24" stroke="currentColor"
      >
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0" />
      </svg>
      <input
        :value="searchInventory"
        @input="$emit('update:searchInventory', ($event.target as HTMLInputElement).value)"
        type="text"
        placeholder="ค้นหาสินค้า..."
        class="w-full rounded-2xl border border-slate-200 bg-white py-2 pl-10 pr-4 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
      />
    </div>

    <!-- Table -->
    <div class="overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-sm">
      <table class="min-w-full border-collapse">
        <thead>
          <tr class="border-b border-slate-100">
            <th class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">สินค้า</th>
            <th class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">SKU</th>
            <th class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">หมวดหมู่</th>
            <th class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">Stock</th>
            <th class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">Reorder Level</th>
            <th class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">สถานะ</th>
          </tr>
        </thead>
        <tbody>
          <!-- Skeleton rows -->
          <template v-if="skeleton">
            <tr v-for="n in 6" :key="n" class="border-b border-slate-100">
              <td class="px-6 py-4"><div class="h-3.5 w-36 rounded bg-slate-200 animate-pulse"></div></td>
              <td class="px-6 py-4"><div class="h-3 w-20 rounded bg-slate-100 animate-pulse"></div></td>
              <td class="px-6 py-4"><div class="h-3 w-24 rounded bg-slate-100 animate-pulse"></div></td>
              <td class="px-6 py-4"><div class="h-3 w-8 rounded bg-slate-200 animate-pulse mx-auto"></div></td>
              <td class="px-6 py-4"><div class="h-3 w-8 rounded bg-slate-100 animate-pulse mx-auto"></div></td>
              <td class="px-6 py-4"><div class="h-5 w-16 rounded-full bg-slate-100 animate-pulse mx-auto"></div></td>
            </tr>
          </template>
          <template v-else>
            <tr v-if="filteredItems.length === 0">
              <td colspan="6" class="px-6 py-20 text-center text-sm text-slate-400">ไม่พบสินค้า</td>
            </tr>
            <tr
              v-for="(item, i) in filteredItems"
              :key="item.productId"
              :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
            >
              <td class="px-6 py-4 text-sm font-semibold text-slate-900">{{ item.productName }}</td>
              <td class="px-6 py-4 font-mono text-sm text-slate-500">{{ item.sku }}</td>
              <td class="px-6 py-4 text-sm text-slate-600">{{ item.categoryName }}</td>
              <td
                class="px-6 py-4 text-center text-sm font-semibold tabular-nums"
                :class="item.stockStatus === 'Low' ? 'text-rose-600' : 'text-slate-900'"
              >
                {{ item.currentStock }}
              </td>
              <td class="px-6 py-4 text-center text-sm text-slate-500 tabular-nums">{{ item.reorderLevel }}</td>
              <td class="px-6 py-4 text-center">
                <span
                  :class="item.stockStatus === 'Low'
                    ? 'inline-flex rounded-full bg-rose-50 px-2 py-0.5 text-xs font-semibold text-rose-600'
                    : 'inline-flex rounded-full bg-emerald-50 px-2 py-0.5 text-xs font-semibold text-emerald-600'"
                >
                  {{ item.stockStatus === 'Low' ? 'ใกล้หมด' : 'ปกติ' }}
                </span>
              </td>
            </tr>
          </template>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { InventoryStatus } from '@/types/report'

const props = defineProps<{
  items: InventoryStatus[] | null
  searchInventory: string
  skeleton: boolean
}>()

defineEmits<{
  (e: 'update:searchInventory', val: string): void
}>()

const filteredItems = computed(() =>
  (props.items ?? []).filter(
    (i) =>
      i.productName.toLowerCase().includes(props.searchInventory.toLowerCase()) ||
      i.sku.toLowerCase().includes(props.searchInventory.toLowerCase()) ||
      i.categoryName.toLowerCase().includes(props.searchInventory.toLowerCase()),
  )
)
</script>
