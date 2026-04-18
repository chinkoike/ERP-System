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
          :value="searchInvoice"
          @input="$emit('update:searchInvoice', ($event.target as HTMLInputElement).value)"
          type="text"
          placeholder="ค้นหาเลข Invoice..."
          class="w-full rounded-2xl border border-slate-200 bg-white py-2 pl-10 pr-4 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
        />
      </div>
      <select
        :value="filterAccount"
        @change="$emit('update:filterAccount', ($event.target as HTMLSelectElement).value)"
        class="rounded-2xl border border-slate-200 bg-white py-2 px-4 text-sm text-slate-700 outline-none cursor-pointer focus:ring-2 focus:ring-slate-300"
      >
        <option value="">ทุกบัญชี</option>
        <option v-for="acc in accounts" :key="acc.id" :value="acc.id">
          {{ acc.accountName }}
        </option>
      </select>
      <select
        :value="filterStatus"
        @change="$emit('update:filterStatus', ($event.target as HTMLSelectElement).value)"
        class="rounded-2xl border border-slate-200 bg-white py-2 px-4 text-sm text-slate-700 outline-none cursor-pointer focus:ring-2 focus:ring-slate-300"
      >
        <option value="">ทุกสถานะ</option>
        <option v-for="s in invoiceStatuses" :key="s.value" :value="s.value">
          {{ s.label }}
        </option>
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
              เลข Invoice
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              คำอธิบาย
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              วันที่ออก
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              ครบกำหนด
            </th>
            <th
              class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              ยอดรวม
            </th>
            <th
              class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              ยอดค้างชำระ
            </th>
            <th
              class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              สถานะ
            </th>
            <th class="px-6 py-3"></th>
          </tr>
        </thead>
        <tbody :class="{ 'opacity-50 pointer-events-none': isSearching }">
          <tr v-if="invoices.length === 0">
            <td colspan="8" class="px-6 py-20 text-center text-sm text-slate-400">ไม่พบ Invoice</td>
          </tr>
          <tr
            v-for="(inv, i) in invoices"
            :key="inv.id"
            :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
          >
            <td class="px-6 py-4 font-mono text-sm font-semibold text-slate-900">
              {{ inv.invoiceNumber }}
            </td>
            <td class="px-6 py-4 max-w-45 truncate text-sm text-slate-600">
              {{ inv.description ?? '—' }}
            </td>
            <td class="px-6 py-4 text-sm text-slate-500">{{ formatDate(inv.invoiceDate) }}</td>
            <td
              class="px-6 py-4 text-sm"
              :class="isOverdue(inv) ? 'font-semibold text-rose-600' : 'text-slate-500'"
            >
              {{ formatDate(inv.dueDate) }}
            </td>
            <td class="px-6 py-4 text-right text-sm font-semibold text-slate-900">
              {{ formatCurrency(inv.totalAmount) }}
            </td>
            <td class="px-6 py-4 text-right text-sm font-semibold text-slate-900">
              {{ formatCurrency(inv.amountDue) }}
            </td>
            <td class="px-6 py-4 text-center">
              <span :class="invoiceStatusClass(inv.status)">{{
                invoiceStatusLabel(inv.status)
              }}</span>
            </td>
            <td class="px-6 py-4">
              <div class="flex items-center gap-2 justify-end">
                <button
                  v-if="inv.status !== 'Paid' && inv.status !== 'Cancelled'"
                  @click="$emit('pay', inv)"
                  class="rounded-2xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                >
                  ชำระเงิน
                </button>
                <button
                  v-if="inv.status !== 'Paid' && inv.status !== 'Cancelled'"
                  @click="$emit('edit', inv)"
                  class="rounded-2xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                >
                  แก้ไข
                </button>
                <button
                  @click="$emit('updateStatus', inv)"
                  class="rounded-2xl border border-emerald-200 bg-emerald-50 px-3 py-1.5 text-xs font-medium text-emerald-700 transition hover:bg-emerald-100"
                >
                  อัปเดตสถานะ
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Pagination -->
    <div class="flex items-center justify-between bg-white px-6 py-4 text-sm text-slate-500">
      <div>แสดง {{ invoices.length }} จาก {{ totalItems }} รายการ</div>
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
import type { Invoice, Account } from '@/types/finance'

defineProps<{
  invoices: Invoice[]
  isSearching: boolean
  accounts: Account[]
  totalItems: number
  currentPage: number
  totalPages: number
  searchInvoice: string
  filterAccount: string
  filterStatus: string
}>()

defineEmits<{
  (e: 'pay', inv: Invoice): void
  (e: 'edit', inv: Invoice): void
  (e: 'updateStatus', inv: Invoice): void
  (e: 'pageChange', page: number): void
  (e: 'update:searchInvoice', val: string): void
  (e: 'update:filterAccount', val: string): void
  (e: 'update:filterStatus', val: string): void
}>()

const invoiceStatuses = [
  { value: 'Draft', label: 'ร่าง' },
  { value: 'Issued', label: 'ออกแล้ว' },
  { value: 'Paid', label: 'ชำระแล้ว' },
  { value: 'Overdue', label: 'เกินกำหนด' },
  { value: 'Cancelled', label: 'ยกเลิก' },
]

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
function isOverdue(inv: Invoice) {
  return inv.status !== 'Paid' && inv.status !== 'Cancelled' && new Date(inv.dueDate) < new Date()
}
function invoiceStatusLabel(s: string) {
  return invoiceStatuses.find((x) => x.value === s)?.label ?? s
}
function invoiceStatusClass(s: string) {
  const map: Record<string, string> = {
    Draft: 'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-500',
    Issued: 'inline-flex rounded-full bg-blue-50 px-2 py-0.5 text-xs font-semibold text-blue-600',
    Paid: 'inline-flex rounded-full bg-emerald-50 px-2 py-0.5 text-xs font-semibold text-emerald-600',
    Overdue: 'inline-flex rounded-full bg-rose-50 px-2 py-0.5 text-xs font-semibold text-rose-600',
    Cancelled:
      'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-400',
  }
  return (
    map[s] ??
    'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-500'
  )
}
</script>
