<template>
  <div>
    <!-- Search -->
    <div class="relative max-w-md mb-4">
      <svg
        class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400"
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
        :value="searchCustomer"
        @input="$emit('update:searchCustomer', ($event.target as HTMLInputElement).value)"
        type="text"
        placeholder="ค้นหาลูกค้า..."
        class="w-full rounded-2xl border border-slate-200 bg-white py-2 pl-10 pr-4 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
      />
    </div>

    <!-- Table -->
    <div class="overflow-hidden rounded-2xl border border-slate-200 bg-white">
      <table class="min-w-full border-collapse">
        <thead>
          <tr class="border-b border-slate-100">
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              ชื่อ
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              Email
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              เบอร์โทร
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              ที่อยู่
            </th>
            <th class="px-6 py-3"></th>
          </tr>
        </thead>
        <tbody>
          <!-- Skeleton -->
          <template v-if="isInitialLoading">
            <tr v-for="n in 5" :key="`cust-skeleton-${n}`" class="border-b border-slate-100">
              <td class="px-6 py-4">
                <div class="space-y-2">
                  <div class="h-4 bg-slate-200 rounded-md w-32 animate-pulse"></div>
                  <div class="h-3 bg-slate-100 rounded-md w-48 animate-pulse"></div>
                </div>
              </td>
              <td class="px-6 py-4">
                <div class="h-3 bg-slate-200 rounded-md w-20 animate-pulse"></div>
              </td>
              <td class="px-6 py-4">
                <div class="h-3 bg-slate-200 rounded-md w-24 animate-pulse"></div>
              </td>
              <td class="px-6 py-4">
                <div class="h-3 bg-slate-200 rounded-md w-32 animate-pulse"></div>
              </td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-2 justify-end">
                  <div class="h-8 bg-slate-200 rounded-2xl w-16 animate-pulse"></div>
                  <div class="h-8 bg-slate-200 rounded-2xl w-12 animate-pulse"></div>
                </div>
              </td>
            </tr>
          </template>

          <tr v-if="!isInitialLoading && customers.length === 0">
            <td colspan="5" class="px-6 py-12 text-center text-sm text-slate-400">ไม่พบลูกค้า</td>
          </tr>
          <tr
            v-for="(c, i) in customers"
            :key="c.id"
            :class="[
              { 'bg-slate-50': i % 2 === 1 },
              'border-b border-slate-100 transition-opacity duration-200',
              { 'opacity-50': isSearching },
            ]"
          >
            <td class="px-6 py-4 text-sm font-semibold text-slate-900">{{ c.fullName }}</td>
            <td class="px-6 py-4 text-sm text-slate-600">{{ c.email ?? '—' }}</td>
            <td class="px-6 py-4 text-sm text-slate-600">{{ c.phone ?? '—' }}</td>
            <td class="px-6 py-4 max-w-50 truncate text-sm text-slate-500">
              {{ c.address ?? '—' }}
            </td>
            <td class="px-6 py-4">
              <div class="flex items-center gap-2 justify-end">
                <button
                  @click="$emit('edit', c)"
                  class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                >
                  แก้ไข
                </button>
                <button
                  @click="$emit('delete', c)"
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

    <!-- Pagination -->
    <div class="flex items-center justify-between bg-white px-6 py-4 text-sm text-slate-500">
      <div>แสดง {{ customers.length }} จาก {{ totalItems }} รายการ</div>
      <div class="flex items-center gap-2">
        <button
          @click="$emit('pageChange', currentPage - 1)"
          :disabled="currentPage <= 1"
          class="rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-600 disabled:cursor-not-allowed disabled:opacity-50"
        >
          ก่อนหน้า
        </button>
        <span>หน้า {{ currentPage }} / {{ totalPages }}</span>
        <button
          @click="$emit('pageChange', currentPage + 1)"
          :disabled="currentPage >= totalPages"
          class="rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-600 disabled:cursor-not-allowed disabled:opacity-50"
        >
          ถัดไป
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Customer } from '@/types/sales'

defineProps<{
  customers: Customer[]
  totalItems: number
  currentPage: number
  totalPages: number
  searchCustomer: string
  isInitialLoading: boolean
  isSearching: boolean
}>()

defineEmits<{
  (e: 'update:searchCustomer', val: string): void
  (e: 'edit', customer: Customer): void
  (e: 'delete', customer: Customer): void
  (e: 'pageChange', page: number): void
}>()
</script>
