<template>
  <div class="space-y-4">
    <!-- Summary cards -->
    <div class="grid grid-cols-2 gap-4">
      <!-- Skeleton -->
      <template v-if="skeleton">
        <div v-for="n in 2" :key="n" class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <div class="h-3 w-20 rounded bg-slate-100 animate-pulse mb-3"></div>
          <div class="h-8 w-36 rounded bg-slate-200 animate-pulse"></div>
        </div>
      </template>
      <template v-else-if="report">
        <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">ยอดขายรวม</p>
          <p class="text-3xl font-semibold tracking-tight text-slate-900">{{ formatCurrency(report.totalSales) }}</p>
        </div>
        <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">จำนวน Order</p>
          <p class="text-3xl font-semibold tracking-tight text-slate-900">{{ report.totalOrders.toLocaleString() }}</p>
        </div>
      </template>
    </div>

    <!-- Monthly Sales Bar Chart -->
    <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
      <BarChart
        title="ยอดขายรายเดือน"
        :data="report?.monthlySales ?? []"
        bar-color="bg-slate-900 hover:bg-slate-700"
        :chart-height="220"
        :skeleton="skeleton"
        :currency="true"
      />
    </div>

    <!-- Monthly Order Count Bar Chart -->
    <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
      <BarChart
        title="จำนวน Order รายเดือน"
        :data="report?.monthlyOrderCount ?? []"
        bar-color="bg-sky-500 hover:bg-sky-400"
        :chart-height="180"
        :skeleton="skeleton"
        :currency="false"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import type { SalesReport } from '@/types/report'
import BarChart from '@/components/report/BarChart.vue'

defineProps<{
  report: SalesReport | null
  skeleton: boolean
}>()

function formatCurrency(v: number) {
  return new Intl.NumberFormat('th-TH', { style: 'currency', currency: 'THB', maximumFractionDigits: 0 }).format(v)
}
</script>
