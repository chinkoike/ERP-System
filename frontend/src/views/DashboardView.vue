<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Top bar -->
    <header class="bg-white border-b border-gray-100 px-8 py-4 flex items-center justify-between">
      <div class="flex items-center gap-3">
        <div class="w-8 h-8 bg-gray-900 rounded-lg flex items-center justify-center">
          <svg class="w-4 h-4 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10"
            />
          </svg>
        </div>
        <span class="font-semibold text-gray-900 tracking-tight">ERP System</span>
      </div>

      <div class="flex items-center gap-4">
        <span class="text-sm text-gray-400">{{ today }}</span>
        <div class="flex items-center gap-2">
          <div class="w-7 h-7 rounded-full bg-gray-200 flex items-center justify-center">
            <span class="text-xs font-medium text-gray-600">{{ userInitial }}</span>
          </div>
          <span class="text-sm text-gray-700">{{ authStore.user?.username }}</span>
        </div>
        <button
          @click="handleLogout"
          class="text-sm text-gray-400 hover:text-gray-600 transition-colors"
        >
          Sign out
        </button>
      </div>
    </header>

    <main class="px-8 py-8 max-w-7xl mx-auto">
      <!-- Page title -->
      <div class="mb-8">
        <h1 class="text-2xl font-semibold text-gray-900 tracking-tight">Dashboard</h1>
        <p class="text-sm text-gray-400 mt-1">ภาพรวมของระบบ ERP ประจำเดือนนี้</p>
      </div>

      <!-- Loading -->
      <div v-if="loading" class="flex items-center justify-center py-24">
        <svg class="animate-spin w-6 h-6 text-gray-300" fill="none" viewBox="0 0 24 24">
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

      <!-- Error -->
      <div
        v-else-if="error"
        class="bg-red-50 border border-red-100 text-red-600 text-sm rounded-xl px-5 py-4"
      >
        {{ error }}
      </div>

      <template v-else-if="summary">
        <!-- Stat Cards -->
        <div class="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
          <div
            v-for="card in statCards"
            :key="card.label"
            class="bg-white rounded-2xl border border-gray-100 p-5 shadow-sm"
          >
            <div class="flex items-start justify-between mb-3">
              <span class="text-xs font-medium text-gray-400 uppercase tracking-wider">{{
                card.label
              }}</span>
              <div :class="['w-8 h-8 rounded-xl flex items-center justify-center', card.iconBg]">
                <component :is="card.icon" class="w-4 h-4" :class="card.iconColor" />
              </div>
            </div>
            <div class="text-2xl font-semibold text-gray-900 tracking-tight">{{ card.value }}</div>
            <div v-if="card.sub" class="text-xs text-gray-400 mt-1">{{ card.sub }}</div>
          </div>
        </div>

        <!-- Recent Orders -->
        <div class="bg-white rounded-2xl border border-gray-100 shadow-sm overflow-hidden">
          <div class="px-6 py-5 border-b border-gray-50 flex items-center justify-between">
            <h2 class="text-sm font-semibold text-gray-900">Recent Orders</h2>
            <router-link
              to="/orders"
              class="text-xs text-gray-400 hover:text-gray-600 transition-colors"
            >
              View all →
            </router-link>
          </div>

          <!-- Empty -->
          <div
            v-if="summary.recentOrders.length === 0"
            class="py-12 text-center text-sm text-gray-300"
          >
            No recent orders
          </div>

          <!-- Table -->
          <div v-else class="overflow-x-auto">
            <table class="w-full">
              <thead>
                <tr class="border-b border-gray-50">
                  <th class="text-left text-xs font-medium text-gray-400 px-6 py-3">Order</th>
                  <th class="text-left text-xs font-medium text-gray-400 px-6 py-3">Customer</th>
                  <th class="text-left text-xs font-medium text-gray-400 px-6 py-3">Date</th>
                  <th class="text-left text-xs font-medium text-gray-400 px-6 py-3">Items</th>
                  <th class="text-right text-xs font-medium text-gray-400 px-6 py-3">Amount</th>
                  <th class="text-left text-xs font-medium text-gray-400 px-6 py-3">Status</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-gray-50">
                <tr
                  v-for="order in summary.recentOrders"
                  :key="order.orderId"
                  class="hover:bg-gray-50/50 transition-colors"
                >
                  <td class="px-6 py-4 text-sm font-medium text-gray-900">
                    {{ order.orderNumber }}
                  </td>
                  <td class="px-6 py-4 text-sm text-gray-600">{{ order.customerName }}</td>
                  <td class="px-6 py-4 text-sm text-gray-400">{{ formatDate(order.orderDate) }}</td>
                  <td class="px-6 py-4 text-sm text-gray-400">{{ order.itemCount }} items</td>
                  <td class="px-6 py-4 text-sm font-medium text-gray-900 text-right">
                    {{ formatCurrency(order.totalAmount) }}
                  </td>
                  <td class="px-6 py-4">
                    <span
                      :class="[
                        'inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium',
                        statusClass(order.status),
                      ]"
                    >
                      {{ order.status }}
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
import { ref, computed, onMounted, h } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import { dashboardService } from '@/services/dashboardService'
import type { DashboardSummary } from '@/types/dashboard'

const router = useRouter()
const authStore = useAuthStore()

const summary = ref<DashboardSummary | null>(null)
const loading = ref(true)
const error = ref('')

const today = computed(() =>
  new Date().toLocaleDateString('th-TH', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  }),
)

const userInitial = computed(() => authStore.user?.username?.charAt(0).toUpperCase() ?? 'U')

// --- SVG Icon components (inline เพื่อไม่ต้องลง icon library) ---
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
      iconBg: summary.value.pendingOrdersCount > 0 ? 'bg-amber-50' : 'bg-gray-50',
      iconColor: summary.value.pendingOrdersCount > 0 ? 'text-amber-500' : 'text-gray-400',
    },
    {
      label: 'สินค้าใกล้หมด',
      value: summary.value.lowStockProductsCount.toLocaleString(),
      sub: 'ต่ำกว่า 10 ชิ้น',
      icon: IconBox,
      iconBg: summary.value.lowStockProductsCount > 0 ? 'bg-red-50' : 'bg-gray-50',
      iconColor: summary.value.lowStockProductsCount > 0 ? 'text-red-500' : 'text-gray-400',
    },
    {
      label: 'ลูกค้าทั้งหมด',
      value: summary.value.totalCustomers.toLocaleString(),
      sub: undefined,
      icon: IconUsers,
      iconBg: 'bg-green-50',
      iconColor: 'text-green-500',
    },
  ]
})

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('th-TH', {
    style: 'currency',
    currency: 'THB',
    maximumFractionDigits: 0,
  }).format(value)
}

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('th-TH', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  })
}

function statusClass(status: string): string {
  const map: Record<string, string> = {
    Pending: 'bg-amber-50 text-amber-600',
    Confirmed: 'bg-blue-50 text-blue-600',
    Shipped: 'bg-purple-50 text-purple-600',
    Delivered: 'bg-green-50 text-green-600',
    Cancelled: 'bg-red-50 text-red-500',
  }
  return map[status] ?? 'bg-gray-100 text-gray-500'
}

async function handleLogout() {
  await authStore.logout()
  router.push({ name: 'login' })
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
