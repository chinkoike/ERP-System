<template>
  <div class="min-h-screen bg-slate-50">
    <main class="px-8 py-8 max-w-7xl mx-auto">
      <!-- Heading + date filter -->
      <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between mb-6">
        <div>
          <h1 class="text-3xl font-semibold tracking-tight text-slate-900">Report</h1>
          <p class="mt-2 text-sm text-slate-500">วิเคราะห์ข้อมูลและสรุปผล</p>
        </div>
        <div class="flex items-center gap-2">
          <input
            v-model="startDate"
            type="date"
            class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 outline-none focus:ring-2 focus:ring-slate-300"
          />
          <span class="text-sm text-slate-400">ถึง</span>
          <input
            v-model="endDate"
            type="date"
            class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 outline-none focus:ring-2 focus:ring-slate-300"
          />
          <button
            @click="loadAll"
            :disabled="loading"
            class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-60 disabled:cursor-not-allowed"
          >
            <svg v-if="loading" class="animate-spin h-3.5 w-3.5" fill="none" viewBox="0 0 24 24">
              <circle
                class="opacity-25"
                cx="12"
                cy="12"
                r="10"
                stroke="currentColor"
                stroke-width="4"
              />
              <path
                class="opacity-75"
                fill="currentColor"
                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"
              />
            </svg>
            {{ loading ? 'กำลังโหลด...' : 'โหลดข้อมูล' }}
          </button>
        </div>
      </div>

      <!-- Tabs -->
      <div class="inline-flex items-center rounded-2xl border border-slate-200 bg-white p-1 mb-6">
        <button
          v-for="tab in tabs"
          :key="tab.key"
          @click="activeTab = tab.key"
          :class="
            activeTab === tab.key
              ? 'rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white'
              : 'rounded-2xl px-4 py-2 text-sm font-medium text-slate-500 hover:text-slate-900'
          "
          class="transition"
        >
          {{ tab.label }}
        </button>
      </div>

      <!-- Content — tab components handle their own skeleton -->
      <SalesReportTab v-if="activeTab === 'sales'" :report="salesReport" :skeleton="loading" />

      <FinancialReportTab
        v-if="activeTab === 'financial'"
        :report="financialReport"
        :skeleton="loading"
      />

      <InventoryReportTab
        v-if="activeTab === 'inventory'"
        :items="inventoryStatus"
        :skeleton="loading"
        v-model:searchInventory="searchInventory"
      />
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { reportService } from '@/services/reportService'
import type { SalesReport, FinancialSummary, InventoryStatus } from '@/types/report'

import SalesReportTab from '@/components/report/SalesReportTab.vue'
import FinancialReportTab from '@/components/report/FinancialReportTab.vue'
import InventoryReportTab from '@/components/report/InventoryReportTab.vue'

const activeTab = ref<'sales' | 'financial' | 'inventory'>('sales')
const tabs = [
  { key: 'sales', label: 'Sales Report' },
  { key: 'financial', label: 'Financial' },
  { key: 'inventory', label: 'Inventory Status' },
] as const

// Date filter — default 6 months back
const now = new Date()
const sixMonthsAgo = new Date(now.getFullYear(), now.getMonth() - 5, 1)
const startDate = ref(sixMonthsAgo.toISOString().split('T')[0])
const endDate = ref(now.toISOString().split('T')[0])

const loading = ref(false)
const salesReport = ref<SalesReport | null>(null)
const financialReport = ref<FinancialSummary | null>(null)
const inventoryStatus = ref<InventoryStatus[] | null>(null)
const searchInventory = ref('')

async function loadAll() {
  loading.value = true
  try {
    const [sales, financial, inventory] = await Promise.all([
      reportService.getSalesReport(startDate.value, endDate.value),
      reportService.getFinancialSummary(startDate.value, endDate.value),
      reportService.getInventoryStatus(),
    ])
    salesReport.value = sales
    financialReport.value = financial
    inventoryStatus.value = inventory
  } catch (e: unknown) {
    console.error(e)
  } finally {
    loading.value = false
  }
}

onMounted(loadAll)
</script>
