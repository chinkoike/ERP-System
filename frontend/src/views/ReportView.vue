<template>
  <div class="min-h-screen bg-slate-50">
    <!-- Top bar -->

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
            class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
          >
            โหลดข้อมูล
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

      <!-- Loading -->
      <div v-if="loading" class="flex items-center justify-center py-24">
        <svg class="animate-spin h-5 w-5 text-slate-300" fill="none" viewBox="0 0 24 24">
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
      </div>

      <template v-else>
        <!-- ===== SALES REPORT ===== -->
        <div v-if="activeTab === 'sales' && salesReport">
          <!-- Summary cards -->
          <div class="grid grid-cols-2 gap-4 mb-6">
            <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
              <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">
                ยอดขายรวม
              </p>
              <p class="text-3xl font-semibold tracking-tight text-slate-900">
                {{ formatCurrency(salesReport.totalSales) }}
              </p>
            </div>
            <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
              <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">
                จำนวน Order
              </p>
              <p class="text-3xl font-semibold tracking-tight text-slate-900">
                {{ salesReport.totalOrders.toLocaleString() }}
              </p>
            </div>
          </div>

          <!-- Monthly Sales Bar Chart -->
          <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm mb-4">
            <h2 class="text-sm font-semibold text-slate-900 mb-6">ยอดขายรายเดือน</h2>
            <div
              v-if="salesReport.monthlySales.length === 0"
              class="py-12 text-center text-sm text-slate-400"
            >
              ไม่มีข้อมูล
            </div>
            <div v-else class="flex items-end gap-2 h-48">
              <div
                v-for="point in salesReport.monthlySales"
                :key="point.label"
                class="flex flex-1 flex-col justify-end items-center gap-1.5 group h-full"
              >
                <span
                  class="text-[10px] text-slate-400 group-hover:text-slate-900 transition whitespace-nowrap"
                >
                  {{ formatCurrency(point.value) }}
                </span>
                <div
                  class="w-full rounded-t-lg bg-slate-900 transition-all duration-300 hover:bg-slate-700 min-h-1"
                  :style="{ height: barHeight(point.value, salesReport.monthlySales) }"
                ></div>
                <span class="text-[10px] text-slate-400 rotate-0">{{ point.label }}</span>
              </div>
            </div>
          </div>

          <!-- Monthly Order Count Bar Chart -->
          <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
            <h2 class="text-sm font-semibold text-slate-900 mb-6">จำนวน Order รายเดือน</h2>
            <div
              v-if="salesReport.monthlyOrderCount.length === 0"
              class="py-12 text-center text-sm text-slate-400"
            >
              ไม่มีข้อมูล
            </div>
            <div v-else class="flex items-end gap-2 h-36">
              <div
                v-for="point in salesReport.monthlyOrderCount"
                :key="point.label"
                class="flex flex-1 flex-col justify-end items-center gap-1.5 group h-full"
              >
                <span class="text-[10px] text-slate-400 group-hover:text-slate-900 transition">
                  {{ point.value }}
                </span>
                <div
                  class="w-full rounded-t-lg bg-blue-400 transition-all hover:bg-blue-500 min-h-1"
                  :style="{ height: barHeight(point.value, salesReport.monthlyOrderCount) }"
                ></div>
                <span class="text-[10px] text-slate-400">{{ point.label }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- ===== FINANCIAL REPORT ===== -->
        <div v-if="activeTab === 'financial' && financialReport">
          <!-- Summary cards -->
          <div class="grid grid-cols-3 gap-4 mb-6">
            <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
              <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">
                ยอด Invoice ทั้งหมด
              </p>
              <p class="text-2xl font-semibold tracking-tight text-slate-900">
                {{ formatCurrency(financialReport.totalInvoiced) }}
              </p>
            </div>
            <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
              <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">
                ชำระแล้ว
              </p>
              <p class="text-2xl font-semibold tracking-tight text-emerald-600">
                {{ formatCurrency(financialReport.totalPaid) }}
              </p>
            </div>
            <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
              <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">
                ค้างชำระ
              </p>
              <p class="text-2xl font-semibold tracking-tight text-rose-600">
                {{ formatCurrency(financialReport.totalReceivable) }}
              </p>
            </div>
          </div>

          <div class="grid grid-cols-2 gap-4 mb-4">
            <!-- Monthly Revenue -->
            <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
              <h2 class="text-sm font-semibold text-slate-900 mb-6">รายรับรายเดือน</h2>
              <div
                v-if="financialReport.monthlyRevenue.length === 0"
                class="py-8 text-center text-sm text-slate-400"
              >
                ไม่มีข้อมูล
              </div>
              <div v-else class="flex items-end gap-2 h-36">
                <div
                  v-for="point in financialReport.monthlyRevenue"
                  :key="point.label"
                  class="flex flex-1 flex-col justify-end items-center gap-1.5 group h-full"
                >
                  <span
                    class="text-[10px] text-slate-400 group-hover:text-slate-900 transition whitespace-nowrap"
                    >{{ formatCurrency(point.value) }}</span
                  >
                  <div
                    class="w-full rounded-t-lg bg-emerald-400 hover:bg-emerald-500 transition-all min-h-1"
                    :style="{ height: barHeight(point.value, financialReport.monthlyRevenue) }"
                  ></div>
                  <span class="text-[10px] text-slate-400">{{ point.label }}</span>
                </div>
              </div>
            </div>

            <!-- Monthly Invoice Amount -->
            <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
              <h2 class="text-sm font-semibold text-slate-900 mb-6">ยอด Invoice รายเดือน</h2>
              <div
                v-if="financialReport.monthlyInvoiceAmount.length === 0"
                class="py-8 text-center text-sm text-slate-400"
              >
                ไม่มีข้อมูล
              </div>
              <div v-else class="flex items-end gap-2 h-36">
                <div
                  v-for="point in financialReport.monthlyInvoiceAmount"
                  :key="point.label"
                  class="flex flex-1 flex-col justify-end items-center gap-1.5 group h-full"
                >
                  <span
                    class="text-[10px] text-slate-400 group-hover:text-slate-900 transition whitespace-nowrap"
                    >{{ formatCurrency(point.value) }}</span
                  >
                  <div
                    class="w-full rounded-t-lg bg-blue-400 hover:bg-blue-500 transition-all min-h-1"
                    :style="{
                      height: barHeight(point.value, financialReport.monthlyInvoiceAmount),
                    }"
                  ></div>
                  <span class="text-[10px] text-slate-400">{{ point.label }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Top Accounts -->
          <div class="rounded-2xl border border-slate-200 bg-white shadow-sm overflow-hidden">
            <div class="border-b border-slate-100 px-6 py-4">
              <h2 class="text-sm font-semibold text-slate-900">Top บัญชี</h2>
            </div>
            <table class="min-w-full border-collapse">
              <thead>
                <tr class="border-b border-slate-100">
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    ชื่อบัญชี
                  </th>
                  <th
                    class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    ยอดคงเหลือ
                  </th>
                  <th
                    class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    สัดส่วน
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr v-if="financialReport.topAccounts.length === 0">
                  <td colspan="3" class="px-6 py-12 text-center text-sm text-slate-400">
                    ไม่มีข้อมูล
                  </td>
                </tr>
                <tr
                  v-for="(acc, i) in financialReport.topAccounts"
                  :key="acc.accountId"
                  :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
                >
                  <td class="px-6 py-4 text-sm font-semibold text-slate-900">
                    {{ acc.accountName }}
                  </td>
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
                          class="h-full rounded-full bg-emerald-400"
                          :style="{
                            width: accountBarWidth(acc.balance, financialReport.topAccounts),
                          }"
                        ></div>
                      </div>
                      <span class="text-xs text-slate-400 w-10 text-right">
                        {{ accountPercent(acc.balance, financialReport.topAccounts) }}%
                      </span>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- ===== INVENTORY STATUS ===== -->
        <div v-if="activeTab === 'inventory' && inventoryStatus">
          <!-- Summary badges -->
          <div class="grid grid-cols-3 gap-4 mb-6">
            <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
              <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">
                สินค้าทั้งหมด
              </p>
              <p class="text-3xl font-semibold tracking-tight text-slate-900">
                {{ inventoryStatus.length }}
              </p>
            </div>
            <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
              <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">
                ใกล้หมด / หมด
              </p>
              <p class="text-3xl font-semibold tracking-tight text-rose-600">
                {{ inventoryStatus.filter((i) => i.stockStatus === 'Low').length }}
              </p>
            </div>
            <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
              <p class="text-[11px] font-semibold uppercase tracking-widest text-slate-400 mb-2">
                สต็อกปกติ
              </p>
              <p class="text-3xl font-semibold tracking-tight text-emerald-600">
                {{ inventoryStatus.filter((i) => i.stockStatus === 'Healthy').length }}
              </p>
            </div>
          </div>

          <!-- Search -->
          <div class="relative max-w-md mb-4">
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
              v-model="searchInventory"
              type="text"
              placeholder="ค้นหาสินค้า..."
              class="w-full rounded-2xl border border-slate-200 bg-white py-2 pl-10 pr-4 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
            />
          </div>

          <div class="overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-sm">
            <table class="min-w-full border-collapse">
              <thead>
                <tr class="border-b border-slate-100">
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    สินค้า
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    SKU
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    หมวดหมู่
                  </th>
                  <th
                    class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    Stock
                  </th>
                  <th
                    class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    Reorder Level
                  </th>
                  <th
                    class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    สถานะ
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr v-if="filteredInventory.length === 0">
                  <td colspan="6" class="px-6 py-20 text-center text-sm text-slate-400">
                    ไม่พบสินค้า
                  </td>
                </tr>
                <tr
                  v-for="(item, i) in filteredInventory"
                  :key="item.productId"
                  :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
                >
                  <td class="px-6 py-4 text-sm font-semibold text-slate-900">
                    {{ item.productName }}
                  </td>
                  <td class="px-6 py-4 font-mono text-sm text-slate-500">{{ item.sku }}</td>
                  <td class="px-6 py-4 text-sm text-slate-600">{{ item.categoryName }}</td>
                  <td
                    class="px-6 py-4 text-center text-sm font-semibold"
                    :class="item.stockStatus === 'Low' ? 'text-rose-600' : 'text-slate-900'"
                  >
                    {{ item.currentStock }}
                  </td>
                  <td class="px-6 py-4 text-center text-sm text-slate-500">
                    {{ item.reorderLevel }}
                  </td>
                  <td class="px-6 py-4 text-center">
                    <span
                      :class="
                        item.stockStatus === 'Low'
                          ? 'inline-flex rounded-full bg-rose-50 px-2 py-0.5 text-xs font-semibold text-rose-600'
                          : 'inline-flex rounded-full bg-emerald-50 px-2 py-0.5 text-xs font-semibold text-emerald-600'
                      "
                    >
                      {{ item.stockStatus === 'Low' ? 'ใกล้หมด' : 'ปกติ' }}
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </template>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { reportService } from '@/services/reportService'
import type {
  SalesReport,
  FinancialSummary,
  InventoryStatus,
  ReportDataPoint,
  AccountBalance,
} from '@/types/report'

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
const filteredInventory = computed(() =>
  (inventoryStatus.value ?? []).filter(
    (i) =>
      i.productName.toLowerCase().includes(searchInventory.value.toLowerCase()) ||
      i.sku.toLowerCase().includes(searchInventory.value.toLowerCase()) ||
      i.categoryName.toLowerCase().includes(searchInventory.value.toLowerCase()),
  ),
)

async function loadAll() {
  loading.value = true
  try {
    const [sales, financial, inventory] = await Promise.all([
      reportService.getSalesReport(startDate.value, endDate.value),
      reportService.getFinancialSummary(startDate.value, endDate.value),
      reportService.getInventoryStatus(),
    ])

    console.log('Sales Data Points:', sales.monthlySales)
    salesReport.value = sales
    financialReport.value = financial
    inventoryStatus.value = inventory
  } catch (e: unknown) {
    console.error(e)
  } finally {
    loading.value = false
  }
}

// Bar chart helpers
function barHeight(value: number, points: ReportDataPoint[]): string {
  const numericValues = points.map((p) => Number(p.value) || 0)
  const max = Math.max(...numericValues, 1)
  const pct = ((Number(value) || 0) / max) * 100
  return `${Math.max(pct, 2)}%`
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

function formatCurrency(v: number) {
  return new Intl.NumberFormat('th-TH', {
    style: 'currency',
    currency: 'THB',
    maximumFractionDigits: 0,
  }).format(v)
}

onMounted(loadAll)
</script>
