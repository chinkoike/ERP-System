<template>
  <div>
    <!-- Filters -->
    <div class="flex flex-col gap-3 mb-4 lg:flex-row lg:items-center">
      <div class="relative flex-1 max-w-md">
        <svg
          class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400"
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
          :value="searchPo"
          @input="$emit('update:searchPo', ($event.target as HTMLInputElement).value)"
          type="text"
          placeholder="ค้นหา PO, Supplier..."
          class="w-full rounded-2xl border border-slate-200 bg-white py-2 pl-10 pr-4 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
        />
      </div>
      <select
        :value="filterSupplier"
        @change="$emit('update:filterSupplier', ($event.target as HTMLSelectElement).value)"
        class="rounded-2xl border border-slate-200 bg-white py-2 px-4 text-sm text-slate-700 outline-none cursor-pointer focus:ring-2 focus:ring-slate-300"
      >
        <option value="">ทุก Supplier</option>
        <option v-for="s in suppliers" :key="s.id" :value="s.id">{{ s.name }}</option>
      </select>
      <select
        :value="filterStatus"
        @change="$emit('update:filterStatus', ($event.target as HTMLSelectElement).value)"
        class="rounded-2xl border border-slate-200 bg-white py-2 px-4 text-sm text-slate-700 outline-none cursor-pointer focus:ring-2 focus:ring-slate-300"
      >
        <option value="">ทุกสถานะ</option>
        <option v-for="s in poStatuses" :key="s.value" :value="s.value">{{ s.label }}</option>
      </select>
    </div>

    <!-- Table -->
    <div class="overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-sm">
      <table class="min-w-full border-collapse">
        <thead>
          <tr class="border-b border-slate-100">
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              เลขที่ PO
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              Supplier
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              วันที่สั่ง
            </th>
            <th
              class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              รายการ
            </th>
            <th
              class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              ยอดรวม
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
          <tr v-if="purchaseOrders.length === 0">
            <td colspan="7" class="px-6 py-20 text-center text-sm text-slate-400">
              ไม่พบใบสั่งซื้อ
            </td>
          </tr>
          <tr
            v-for="(po, i) in purchaseOrders"
            :key="po.id"
            :class="[
              { 'bg-slate-50': i % 2 === 1 },
              'border-b border-slate-100 transition-opacity duration-200',
              { 'opacity-50': isSearching },
            ]"
          >
            <td class="px-6 py-4 font-mono text-sm font-semibold text-slate-900">
              {{ po.purchaseOrderNumber }}
            </td>
            <td class="px-6 py-4 text-sm text-slate-600">{{ po.supplierName }}</td>
            <td class="px-6 py-4 text-sm text-slate-500">{{ formatDate(po.orderDate) }}</td>
            <td class="px-6 py-4 text-center text-sm text-slate-600">{{ po.items.length }}</td>
            <td class="px-6 py-4 text-right text-sm font-semibold text-slate-900">
              {{ formatCurrency(po.totalAmount) }}
            </td>
            <td class="px-6 py-4 text-center">
              <span :class="poStatusClass(po.status)">{{ poStatusLabel(po.status) }}</span>
            </td>
            <td class="px-6 py-4">
              <div class="flex items-center gap-2 justify-end">
                <button
                  v-if="
                    (po.status === 'Ordered' || po.status === 'Receiving') &&
                    (authStore.isAdmin || authStore.isManager)
                  "
                  @click="$emit('receive', po)"
                  class="rounded-2xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                >
                  รับสินค้า
                </button>
                <button
                  v-if="po.status === 'Ordered' && (authStore.isAdmin || authStore.isManager)"
                  @click="$emit('cancel', po)"
                  class="rounded-2xl border border-rose-200 bg-white px-3 py-1.5 text-xs font-medium text-rose-600 transition hover:bg-rose-50"
                >
                  ยกเลิก
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Pagination -->
    <div class="flex items-center justify-between bg-white px-6 py-4 text-sm text-slate-500">
      <div>แสดง {{ purchaseOrders.length }} จาก {{ totalItems }} รายการ</div>
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
import { useAuthStore } from '@/stores/authStore'
import type { PurchaseOrder, PurchaseOrderStatus, Supplier } from '@/types/purchasing'

const authStore = useAuthStore()

defineProps<{
  purchaseOrders: PurchaseOrder[]
  suppliers: Supplier[]
  totalItems: number
  currentPage: number
  totalPages: number
  searchPo: string
  filterSupplier: string
  filterStatus: string
  isSearching: boolean
}>()

defineEmits<{
  (e: 'update:searchPo', val: string): void
  (e: 'update:filterSupplier', val: string): void
  (e: 'update:filterStatus', val: string): void
  (e: 'receive', po: PurchaseOrder): void
  (e: 'cancel', po: PurchaseOrder): void
  (e: 'pageChange', page: number): void
}>()

const poStatuses = [
  { value: 'Ordered', label: 'รอรับสินค้า' },
  { value: 'Receiving', label: 'รับบางส่วน' },
  { value: 'Completed', label: 'รับครบแล้ว' },
  { value: 'Cancelled', label: 'ยกเลิก' },
]

function poStatusLabel(s: string) {
  return poStatuses.find((x) => x.value === s)?.label ?? s
}
function poStatusClass(s: PurchaseOrderStatus) {
  const map: Record<PurchaseOrderStatus, string> = {
    Draft: 'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-500',
    Ordered:
      'inline-flex rounded-full bg-amber-50 px-2 py-0.5 text-xs font-semibold text-amber-600',
    Receiving:
      'inline-flex rounded-full bg-blue-50 px-2 py-0.5 text-xs font-semibold text-blue-600',
    Completed:
      'inline-flex rounded-full bg-emerald-50 px-2 py-0.5 text-xs font-semibold text-emerald-600',
    Cancelled:
      'inline-flex rounded-full bg-rose-50 px-2 py-0.5 text-xs font-semibold text-rose-500',
  }
  return (
    map[s] ??
    'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-500'
  )
}
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
</script>
