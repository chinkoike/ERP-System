<template>
  <div>
    <!-- Filter row -->
    <div class="flex items-center gap-3 mb-4">
      <div class="relative flex-1 max-w-md">
        <svg
          class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400"
          fill="none" viewBox="0 0 24 24" stroke="currentColor"
        >
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0" />
        </svg>
        <input
          :value="searchOrder"
          @input="$emit('update:searchOrder', ($event.target as HTMLInputElement).value)"
          type="text"
          placeholder="ค้นหา order, ลูกค้า..."
          class="w-full rounded-2xl border border-slate-200 bg-white py-2 pl-10 pr-4 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
        />
      </div>
      <select
        :value="filterStatus"
        @change="$emit('update:filterStatus', ($event.target as HTMLSelectElement).value)"
        class="rounded-2xl border border-slate-200 bg-white py-2 px-4 text-sm text-slate-700 outline-none cursor-pointer focus:ring-2 focus:ring-slate-300"
      >
        <option value="">ทุกสถานะ</option>
        <option v-for="s in orderStatuses" :key="s.value" :value="s.value">{{ s.label }}</option>
      </select>
    </div>

    <!-- Table -->
    <div class="overflow-x-auto rounded-2xl border border-slate-200 bg-white">
      <table class="min-w-full border-collapse">
        <thead>
          <tr class="border-b border-slate-100">
            <th class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">Order</th>
            <th class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">ลูกค้า</th>
            <th class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">วันที่</th>
            <th class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">รายการ</th>
            <th class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">ยอดรวม</th>
            <th class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">สถานะ</th>
            <th class="px-6 py-3"></th>
          </tr>
        </thead>
        <tbody>
          <!-- Skeleton -->
          <template v-if="isInitialLoading">
            <tr v-for="n in 5" :key="`order-skeleton-${n}`" class="border-b border-slate-100">
              <td class="px-6 py-4"><div class="h-4 bg-slate-200 rounded-md w-32 animate-pulse"></div></td>
              <td class="px-6 py-4"><div class="h-3 bg-slate-100 rounded-md w-40 animate-pulse"></div></td>
              <td class="px-6 py-4"><div class="h-3 bg-slate-200 rounded-md w-24 animate-pulse"></div></td>
              <td class="px-6 py-4"><div class="h-3 bg-slate-200 rounded-md w-8 mx-auto animate-pulse"></div></td>
              <td class="px-6 py-4"><div class="h-3 bg-slate-200 rounded-md w-20 ml-auto animate-pulse"></div></td>
              <td class="px-6 py-4"><div class="h-6 bg-slate-200 rounded-full w-20 mx-auto animate-pulse"></div></td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-2 justify-end">
                  <div class="h-8 bg-slate-200 rounded-2xl w-28 animate-pulse"></div>
                  <div class="h-8 bg-slate-200 rounded-2xl w-14 animate-pulse"></div>
                </div>
              </td>
            </tr>
          </template>

          <template v-else>
            <tr v-if="orders.length === 0">
              <td colspan="7" class="px-6 py-20 text-center text-sm text-slate-400">ไม่พบ order</td>
            </tr>
            <tr
              v-for="(o, i) in orders"
              :key="o.orderId"
              :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100 transition-opacity duration-200', { 'opacity-50': isSearching }]"
            >
              <td class="px-6 py-4 text-sm font-mono font-semibold text-slate-900">{{ o.orderNumber }}</td>
              <td class="px-6 py-4 text-sm text-slate-600">{{ o.customerName || '—' }}</td>
              <td class="px-6 py-4 text-sm text-slate-500">{{ formatDate(o.orderDate) }}</td>
              <td class="px-6 py-4 text-center text-sm text-slate-600">{{ o.itemCount }}</td>
              <td class="px-6 py-4 text-right text-sm font-semibold text-slate-900">{{ formatCurrency(o.totalAmount) }}</td>
              <td class="px-6 py-4 text-center">
                <span :class="statusBadgeClass(o.status)">{{ statusLabel(o.status) }}</span>
              </td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-2 justify-end">
                  <select
                    v-if="o.status !== 'Cancelled' && o.status !== 'Delivered'"
                    @change="$emit('statusChange', o.orderId, ($event.target as HTMLSelectElement).value)"
                    class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-xs font-medium text-slate-600 outline-none transition hover:border-slate-300"
                  >
                    <option value="">เปลี่ยนสถานะ</option>
                    <option v-for="s in nextStatuses(o.status)" :key="s.value" :value="s.value">{{ s.label }}</option>
                  </select>
                  <button
                    v-if="o.status === 'Pending'"
                    @click="$emit('cancel', o)"
                    class="rounded-2xl border border-rose-200 bg-white px-3 py-2 text-xs font-medium text-rose-600 transition hover:bg-rose-50"
                  >
                    ยกเลิก
                  </button>
                </div>
              </td>
            </tr>
          </template>
        </tbody>
      </table>
    </div>

    <!-- Pagination -->
    <div class="flex items-center justify-between bg-white px-6 py-4 text-sm text-slate-500">
      <div>แสดง {{ orders.length }} จาก {{ totalItems }} รายการ</div>
      <div class="flex items-center gap-2">
        <button
          @click="$emit('pageChange', currentPage - 1)"
          :disabled="currentPage <= 1 || isSearching"
          class="rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-600 disabled:cursor-not-allowed disabled:opacity-50"
        >
          ก่อนหน้า
        </button>
        <span>หน้า {{ currentPage }} / {{ totalPages }}</span>
        <button
          @click="$emit('pageChange', currentPage + 1)"
          :disabled="currentPage >= totalPages || isSearching"
          class="rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-600 disabled:cursor-not-allowed disabled:opacity-50"
        >
          ถัดไป
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { OrderSummary, OrderStatus } from '@/types/sales'

defineProps<{
  orders: OrderSummary[]
  totalItems: number
  currentPage: number
  totalPages: number
  searchOrder: string
  filterStatus: string
  isInitialLoading: boolean
  isSearching: boolean
}>()

defineEmits<{
  (e: 'update:searchOrder', val: string): void
  (e: 'update:filterStatus', val: string): void
  (e: 'statusChange', orderId: string, status: string): void
  (e: 'cancel', order: OrderSummary): void
  (e: 'pageChange', page: number): void
}>()

const orderStatuses = [
  { value: 'Pending', label: 'รอยืนยัน' },
  { value: 'Confirmed', label: 'ยืนยันแล้ว' },
  { value: 'Processing', label: 'กำลังเตรียม' },
  { value: 'Shipped', label: 'ส่งแล้ว' },
  { value: 'Delivered', label: 'ถึงมือลูกค้า' },
  { value: 'Cancelled', label: 'ยกเลิก' },
]

function nextStatuses(current: OrderStatus) {
  const flow: Record<string, string[]> = {
    Pending: ['Confirmed', 'Processing'],
    Confirmed: ['Processing'],
    Processing: ['Shipped'],
    Shipped: ['Delivered'],
  }
  return (flow[current] ?? []).map((v) => orderStatuses.find((s) => s.value === v)!)
}

function statusLabel(s: string) {
  return orderStatuses.find((x) => x.value === s)?.label ?? s
}

function statusBadgeClass(s: string) {
  const map: Record<string, string> = {
    Pending: 'inline-flex items-center rounded-full bg-amber-50 px-2.5 py-1 text-[11px] font-semibold text-amber-700',
    Confirmed: 'inline-flex items-center rounded-full bg-sky-100 px-2.5 py-1 text-[11px] font-semibold text-sky-700',
    Processing: 'inline-flex items-center rounded-full bg-violet-100 px-2.5 py-1 text-[11px] font-semibold text-violet-700',
    Shipped: 'inline-flex items-center rounded-full bg-cyan-100 px-2.5 py-1 text-[11px] font-semibold text-cyan-700',
    Delivered: 'inline-flex items-center rounded-full bg-emerald-100 px-2.5 py-1 text-[11px] font-semibold text-emerald-700',
    Cancelled: 'inline-flex items-center rounded-full bg-rose-100 px-2.5 py-1 text-[11px] font-semibold text-rose-700',
  }
  return map[s] ?? 'inline-flex items-center rounded-full bg-slate-100 px-2.5 py-1 text-[11px] font-semibold text-slate-600'
}

function formatCurrency(v: number) {
  return new Intl.NumberFormat('th-TH', { style: 'currency', currency: 'THB', maximumFractionDigits: 0 }).format(v)
}
function formatDate(d: string) {
  return new Date(d).toLocaleDateString('th-TH', { day: 'numeric', month: 'short', year: 'numeric' })
}
</script>
