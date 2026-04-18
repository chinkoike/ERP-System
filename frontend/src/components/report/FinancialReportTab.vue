<template>
  <div class="space-y-4">
    <!-- Summary cards -->
    <div class="grid grid-cols-3 gap-4">
      <!-- Skeleton -->
      <template v-if="skeleton">
        <div v-for="n in 3" :key="n" class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <div class="h-3 w-24 rounded bg-slate-100 animate-pulse mb-3"></div>
          <div class="h-7 w-32 rounded bg-slate-200 animate-pulse"></div>
        </div>
      </template>
      <template v-else-if="report">
        <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">ยอด Invoice ทั้งหมด</p>
          <p class="text-2xl font-semibold tracking-tight text-slate-900">{{ formatCurrency(report.totalInvoiced) }}</p>
        </div>
        <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">ชำระแล้ว</p>
          <p class="text-2xl font-semibold tracking-tight text-emerald-600">{{ formatCurrency(report.totalPaid) }}</p>
        </div>
        <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">ค้างชำระ</p>
          <p class="text-2xl font-semibold tracking-tight text-rose-600">{{ formatCurrency(report.totalReceivable) }}</p>
        </div>
      </template>
    </div>

    <!-- Charts row -->
    <div class="grid grid-cols-2 gap-4">
      <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
        <BarChart
          title="รายรับรายเดือน"
          :data="report?.monthlyRevenue ?? []"
          bar-color="bg-emerald-500 hover:bg-emerald-400"
          :chart-height="200"
          :skeleton="skeleton"
          :currency="true"
        />
      </div>
      <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
        <BarChart
          title="ยอด Invoice รายเดือน"
          :data="report?.monthlyInvoiceAmount ?? []"
          bar-color="bg-sky-500 hover:bg-sky-400"
          :chart-height="200"
          :skeleton="skeleton"
          :currency="true"
        />
      </div>
    </div>

    <!-- Top Accounts -->
    <div class="rounded-2xl border border-slate-200 bg-white shadow-sm overflow-hidden">
      <div class="border-b border-slate-100 px-6 py-4">
        <h2 class="text-sm font-semibold text-slate-900">Top บัญชี</h2>
      </div>

      <!-- Skeleton -->
      <template v-if="skeleton">
        <div class="divide-y divide-slate-100">
          <div v-for="n in 4" :key="n" class="flex items-center gap-4 px-6 py-4">
            <div class="flex-1 h-3.5 rounded bg-slate-200 animate-pulse"></div>
            <div class="w-24 h-3.5 rounded bg-slate-100 animate-pulse"></div>
            <div class="w-28 h-2 rounded-full bg-slate-100 animate-pulse"></div>
          </div>
        </div>
      </template>

      <table v-else-if="report" class="min-w-full border-collapse">
        <thead>
          <tr class="border-b border-slate-100">
            <th class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">ชื่อบัญชี</th>
            <th class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">ยอดคงเหลือ</th>
            <th class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">สัดส่วน</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="report.topAccounts.length === 0">
            <td colspan="3" class="px-6 py-12 text-center text-sm text-slate-400">ไม่มีข้อมูล</td>
          </tr>
          <tr
            v-for="(acc, i) in report.topAccounts"
            :key="acc.accountId"
            :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
          >
            <td class="px-6 py-4 text-sm font-semibold text-slate-900">{{ acc.accountName }}</td>
            <td
              class="px-6 py-4 text-right text-sm font-semibold"
              :class="acc.balance >= 0 ? 'text-emerald-600' : 'text-rose-600'"
            >
              {{ formatCurrency(acc.balance) }}
            </td>
            <td class="px-6 py-4">
              <div class="flex items-center justify-end gap-2">
                <div class="h-1.5 w-24 overflow-hidden rounded-full bg-slate-100">
                  <div
                    class="h-full rounded-full bg-emerald-400 transition-all duration-500"
                    :style="{ width: accountBarWidth(acc.balance, report.topAccounts) }"
                  ></div>
                </div>
                <span class="text-xs text-slate-400 w-10 text-right tabular-nums">
                  {{ accountPercent(acc.balance, report.topAccounts) }}%
                </span>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { FinancialSummary, AccountBalance } from '@/types/report'
import BarChart from '@/components/report/BarChart.vue'

defineProps<{
  report: FinancialSummary | null
  skeleton: boolean
}>()

function formatCurrency(v: number) {
  return new Intl.NumberFormat('th-TH', { style: 'currency', currency: 'THB', maximumFractionDigits: 0 }).format(v)
}
function accountBarWidth(balance: number, accounts: AccountBalance[]): string {
  const max = Math.max(...accounts.map((a) => Math.abs(a.balance)), 1)
  return `${(Math.abs(balance) / max) * 100}%`
}
function accountPercent(balance: number, accounts: AccountBalance[]): string {
  const total = accounts.reduce((s, a) => s + Math.abs(a.balance), 0)
  if (total === 0) return '0'
  return ((Math.abs(balance) / total) * 100).toFixed(1)
}
</script>
