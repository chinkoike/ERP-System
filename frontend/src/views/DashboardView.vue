<template>
  <div class="min-h-screen bg-slate-50">
    <!-- Top bar -->

    <main class="px-8 py-8 max-w-7xl mx-auto">
      <!-- Page title -->
      <div class="mb-8">
        <h1 class="text-3xl font-semibold tracking-tight text-slate-900">Dashboard</h1>
        <p class="mt-2 text-sm text-slate-500">ภาพรวมของระบบ ERP ประจำเดือนนี้</p>
      </div>

      <!-- Loading -->
      <div v-if="loading" class="animate-pulse">
        <div class="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
          <div
            v-for="i in 4"
            :key="i"
            class="rounded-2xl border border-slate-100 bg-white p-5 shadow-sm"
          >
            <div class="flex items-start justify-between mb-3">
              <div class="h-3 w-20 bg-slate-100 rounded"></div>
              <div class="h-8 w-8 bg-slate-50 rounded-xl"></div>
            </div>
            <div class="h-8 w-28 bg-slate-100 rounded mb-2"></div>
            <div class="h-3 w-24 bg-slate-50 rounded"></div>
          </div>
        </div>

        <div class="rounded-2xl border border-slate-100 bg-white shadow-sm overflow-hidden">
          <div class="px-6 py-4 border-b border-slate-50">
            <div class="h-4 w-32 bg-slate-100 rounded"></div>
          </div>
          <div class="p-6 space-y-4">
            <div
              v-for="i in 5"
              :key="i"
              class="flex items-center justify-between py-2 border-b border-slate-50 last:border-0"
            >
              <div class="flex gap-4">
                <div class="h-4 w-16 bg-slate-100 rounded"></div>
                <div class="h-4 w-32 bg-slate-50 rounded"></div>
              </div>
              <div class="flex gap-8">
                <div class="h-4 w-20 bg-slate-50 rounded hidden md:block"></div>
                <div class="h-4 w-12 bg-slate-100 rounded"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <!-- Error -->
      <div
        v-else-if="error"
        class="rounded-2xl bg-rose-50 border border-rose-100 px-5 py-4 text-sm text-rose-700"
      >
        {{ error }}
      </div>

      <template v-else-if="summary">
        <!-- Stat Cards -->
        <div class="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
          <div
            v-for="card in statCards"
            :key="card.label"
            class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"
          >
            <div class="flex items-start justify-between mb-3">
              <span class="text-[11px] font-semibold uppercase tracking-widest text-slate-400">{{
                card.label
              }}</span>
              <div :class="['flex h-8 w-8 items-center justify-center rounded-xl', card.iconBg]">
                <component :is="card.icon" class="h-4 w-4" :class="card.iconColor" />
              </div>
            </div>
            <div class="text-2xl font-semibold tracking-tight text-slate-900">{{ card.value }}</div>
            <div v-if="card.sub" class="mt-1 text-xs text-slate-400">{{ card.sub }}</div>
          </div>
        </div>

        <!-- Recent Orders -->
        <div class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
          <div class="flex items-center justify-between border-b border-slate-100 px-6 py-4">
            <h2 class="text-sm font-semibold text-slate-900">Recent Orders</h2>
            <router-link to="/sales" class="text-xs text-slate-400 hover:text-slate-600 transition">
              View all →
            </router-link>
          </div>

          <!-- Empty -->
          <div
            v-if="summary.recentOrders.length === 0"
            class="py-16 text-center text-sm text-slate-400"
          >
            No recent orders
          </div>

          <!-- Table -->
          <div v-else class="overflow-x-auto">
            <table class="min-w-full border-collapse">
              <thead>
                <tr class="border-b border-slate-100">
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    Order
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    Customer
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    Date
                  </th>
                  <th
                    class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    Items
                  </th>
                  <th
                    class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    Amount
                  </th>
                  <th
                    class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    Status
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="(order, i) in summary.recentOrders"
                  :key="order.orderId"
                  :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
                >
                  <td class="px-6 py-4 font-mono text-sm font-semibold text-slate-900">
                    {{ order.orderNumber }}
                  </td>
                  <td class="px-6 py-4 text-sm text-slate-600">{{ order.customerName }}</td>
                  <td class="px-6 py-4 text-sm text-slate-500">
                    {{ formatDate(order.orderDate) }}
                  </td>
                  <td class="px-6 py-4 text-center text-sm text-slate-600">
                    {{ order.itemCount }}
                  </td>
                  <td class="px-6 py-4 text-right text-sm font-semibold text-slate-900">
                    {{ formatCurrency(order.totalAmount) }}
                  </td>
                  <td class="px-6 py-4 text-center">
                    <span :class="statusClass(order.status)">{{ order.status }}</span>
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
import { ref, computed, onMounted, h } from 'vue'

import { dashboardService } from '@/services/dashboardService'
import type { DashboardSummary } from '@/types/dashboard'

const summary = ref<DashboardSummary | null>(null)
const loading = ref(true)
const error = ref('')

const IconMoney = {
  render: () =>
    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
      h('path', {
        'stroke-linecap': 'round',
        'stroke-linejoin': 'round',
        'stroke-width': '2',
        d: 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
      }),
    ]),
}
const IconOrder = {
  render: () =>
    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
      h('path', {
        'stroke-linecap': 'round',
        'stroke-linejoin': 'round',
        'stroke-width': '2',
        d: 'M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2',
      }),
    ]),
}
const IconBox = {
  render: () =>
    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
      h('path', {
        'stroke-linecap': 'round',
        'stroke-linejoin': 'round',
        'stroke-width': '2',
        d: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10',
      }),
    ]),
}
const IconUsers = {
  render: () =>
    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor' }, [
      h('path', {
        'stroke-linecap': 'round',
        'stroke-linejoin': 'round',
        'stroke-width': '2',
        d: 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z',
      }),
    ]),
}

const statCards = computed(() => {
  if (!summary.value) return []
  return [
    {
      label: 'ยอดขายเดือนนี้',
      value: formatCurrency(summary.value.totalSales),
      sub: 'รวมทุก order ที่สำเร็จ',
      icon: IconMoney,
      iconBg: 'bg-blue-50',
      iconColor: 'text-blue-500',
    },
    {
      label: 'Order รอดำเนินการ',
      value: summary.value.pendingOrdersCount.toLocaleString(),
      sub: 'รอการยืนยัน',
      icon: IconOrder,
      iconBg: summary.value.pendingOrdersCount > 0 ? 'bg-amber-50' : 'bg-slate-50',
      iconColor: summary.value.pendingOrdersCount > 0 ? 'text-amber-500' : 'text-slate-400',
    },
    {
      label: 'สินค้าใกล้หมด',
      value: summary.value.lowStockProductsCount.toLocaleString(),
      sub: 'ต่ำกว่า 10 ชิ้น',
      icon: IconBox,
      iconBg: summary.value.lowStockProductsCount > 0 ? 'bg-rose-50' : 'bg-slate-50',
      iconColor: summary.value.lowStockProductsCount > 0 ? 'text-rose-500' : 'text-slate-400',
    },
    {
      label: 'ลูกค้าทั้งหมด',
      value: summary.value.totalCustomers.toLocaleString(),
      sub: undefined,
      icon: IconUsers,
      iconBg: 'bg-emerald-50',
      iconColor: 'text-emerald-500',
    },
  ]
})

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
function statusClass(status: string) {
  const map: Record<string, string> = {
    Pending:
      'inline-flex rounded-full bg-amber-50 px-2 py-0.5 text-xs font-semibold text-amber-600',
    Confirmed:
      'inline-flex rounded-full bg-blue-50 px-2 py-0.5 text-xs font-semibold text-blue-600',
    Shipped:
      'inline-flex rounded-full bg-purple-50 px-2 py-0.5 text-xs font-semibold text-purple-600',
    Delivered:
      'inline-flex rounded-full bg-emerald-50 px-2 py-0.5 text-xs font-semibold text-emerald-600',
    Cancelled:
      'inline-flex rounded-full bg-rose-50 px-2 py-0.5 text-xs font-semibold text-rose-500',
  }
  return (
    map[status] ??
    'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-500'
  )
}

onMounted(async () => {
  try {
    summary.value = await dashboardService.getSummary()
  } catch (e: unknown) {
    error.value = e instanceof Error ? e.message : 'ไม่สามารถโหลดข้อมูลได้'
  } finally {
    loading.value = false
  }
})
</script>
