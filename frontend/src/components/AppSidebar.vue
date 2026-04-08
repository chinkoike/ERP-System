<template>
  <aside
    :class="collapsed ? 'w-16' : 'w-56'"
    class="flex h-screen flex-col border-r border-slate-100 bg-white transition-all duration-200 flex-shrink-0"
  >
    <!-- Logo -->
    <div
      class="flex h-16 items-center border-b border-slate-100 px-4"
      :class="collapsed ? 'justify-center' : 'gap-3'"
    >
      <div class="flex h-7 w-7 flex-shrink-0 items-center justify-center rounded-lg bg-slate-900">
        <svg width="12" height="12" fill="none" viewBox="0 0 24 24" stroke="white" stroke-width="2">
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10"
          />
        </svg>
      </div>
      <span v-if="!collapsed" class="text-sm font-semibold text-slate-900 tracking-tight"
        >ERP System</span
      >
    </div>

    <!-- Nav -->
    <nav class="flex-1 overflow-y-auto py-4 px-2 space-y-0.5">
      <router-link
        v-for="item in navItems"
        :key="item.name"
        :to="item.to"
        custom
        v-slot="{ isActive, navigate }"
      >
        <button
          @click="navigate"
          :title="collapsed ? item.label : ''"
          :class="[
            isActive
              ? 'bg-slate-900 text-white'
              : 'text-slate-500 hover:bg-slate-50 hover:text-slate-900',
            collapsed ? 'justify-center px-0' : 'px-3',
          ]"
          class="flex w-full items-center gap-3 rounded-xl py-2.5 text-sm font-medium transition-all duration-150"
        >
          <component :is="item.icon" class="h-4 w-4 flex-shrink-0" />
          <span v-if="!collapsed" class="truncate">{{ item.label }}</span>
        </button>
      </router-link>
    </nav>

    <!-- Bottom: collapse toggle + user -->
    <div class="border-t border-slate-100 p-2 space-y-0.5">
      <!-- User -->
      <div
        :class="collapsed ? 'justify-center px-0' : 'px-3'"
        class="flex items-center gap-3 rounded-xl py-2.5"
      >
        <div
          class="flex h-7 w-7 flex-shrink-0 items-center justify-center rounded-full bg-slate-200 text-[11px] font-semibold text-slate-600"
        >
          {{ userInitial }}
        </div>
        <div v-if="!collapsed" class="min-w-0 flex-1">
          <div class="truncate text-sm font-medium text-slate-900">
            {{ authStore.user?.username }}
          </div>
          <div class="truncate text-xs text-slate-400">{{ authStore.user?.email }}</div>
        </div>
      </div>

      <!-- Logout -->
      <button
        @click="handleLogout"
        :title="collapsed ? 'Sign out' : ''"
        :class="collapsed ? 'justify-center px-0' : 'px-3'"
        class="flex w-full items-center gap-3 rounded-xl py-2.5 text-sm font-medium text-slate-400 transition hover:bg-rose-50 hover:text-rose-600"
      >
        <LogoutIcon class="h-4 w-4 flex-shrink-0" />
        <span v-if="!collapsed">Sign out</span>
      </button>

      <!-- Collapse toggle -->
      <button
        @click="$emit('update:collapsed', !collapsed)"
        :title="collapsed ? 'Expand' : 'Collapse'"
        :class="collapsed ? 'justify-center px-0' : 'px-3'"
        class="flex w-full items-center gap-3 rounded-xl py-2.5 text-sm font-medium text-slate-400 transition hover:bg-slate-50 hover:text-slate-600"
      >
        <ChevronIcon
          :class="collapsed ? 'rotate-180' : ''"
          class="h-4 w-4 flex-shrink-0 transition-transform duration-200"
        />
        <span v-if="!collapsed">ย่อเมนู</span>
      </button>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { computed, h } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

defineProps<{ collapsed: boolean }>()
defineEmits<{ (e: 'update:collapsed', v: boolean): void }>()

const router = useRouter()
const authStore = useAuthStore()
const userInitial = computed(() => authStore.user?.username?.charAt(0).toUpperCase() ?? 'U')

async function handleLogout() {
  await authStore.logout()
  router.push({ name: 'login' })
}

// --- Inline SVG icon components ---
const DashboardIcon = {
  render: () =>
    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor', 'stroke-width': '2' }, [
      h('path', {
        'stroke-linecap': 'round',
        'stroke-linejoin': 'round',
        d: 'M4 5a1 1 0 011-1h4a1 1 0 011 1v5a1 1 0 01-1 1H5a1 1 0 01-1-1V5zm10 0a1 1 0 011-1h4a1 1 0 011 1v2a1 1 0 01-1 1h-4a1 1 0 01-1-1V5zM4 15a1 1 0 011-1h4a1 1 0 011 1v4a1 1 0 01-1 1H5a1 1 0 01-1-1v-4zm10-3a1 1 0 011-1h4a1 1 0 011 1v7a1 1 0 01-1 1h-4a1 1 0 01-1-1v-7z',
      }),
    ]),
}
const InventoryIcon = {
  render: () =>
    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor', 'stroke-width': '2' }, [
      h('path', {
        'stroke-linecap': 'round',
        'stroke-linejoin': 'round',
        d: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10',
      }),
    ]),
}
const SalesIcon = {
  render: () =>
    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor', 'stroke-width': '2' }, [
      h('path', {
        'stroke-linecap': 'round',
        'stroke-linejoin': 'round',
        d: 'M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z',
      }),
    ]),
}
const PurchasingIcon = {
  render: () =>
    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor', 'stroke-width': '2' }, [
      h('path', {
        'stroke-linecap': 'round',
        'stroke-linejoin': 'round',
        d: 'M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01',
      }),
    ]),
}
const FinanceIcon = {
  render: () =>
    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor', 'stroke-width': '2' }, [
      h('path', {
        'stroke-linecap': 'round',
        'stroke-linejoin': 'round',
        d: 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
      }),
    ]),
}
const ReportIcon = {
  render: () =>
    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor', 'stroke-width': '2' }, [
      h('path', {
        'stroke-linecap': 'round',
        'stroke-linejoin': 'round',
        d: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z',
      }),
    ]),
}
const UsersIcon = {
  render: () =>
    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor', 'stroke-width': '2' }, [
      h('path', {
        'stroke-linecap': 'round',
        'stroke-linejoin': 'round',
        d: 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z',
      }),
    ]),
}
const LogoutIcon = {
  render: () =>
    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor', 'stroke-width': '2' }, [
      h('path', {
        'stroke-linecap': 'round',
        'stroke-linejoin': 'round',
        d: 'M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1',
      }),
    ]),
}
const ChevronIcon = {
  render: () =>
    h('svg', { fill: 'none', viewBox: '0 0 24 24', stroke: 'currentColor', 'stroke-width': '2' }, [
      h('path', {
        'stroke-linecap': 'round',
        'stroke-linejoin': 'round',
        d: 'M11 19l-7-7 7-7m8 14l-7-7 7-7',
      }),
    ]),
}

const rawNavItems = [
  {
    name: 'dashboard',
    label: 'Dashboard',
    to: '/dashboard',
    icon: DashboardIcon,
    roles: ['Admin', 'Manager'],
  },
  {
    name: 'inventory',
    label: 'Inventory',
    to: '/inventory',
    icon: InventoryIcon,
    roles: ['Admin', 'Manager', 'User'],
  },
  {
    name: 'sales',
    label: 'Sales',
    to: '/sales',
    icon: SalesIcon,
    roles: ['Admin', 'Manager', 'User'],
  },
  {
    name: 'purchasing',
    label: 'Purchasing',
    to: '/purchasing',
    icon: PurchasingIcon,
    roles: ['Admin', 'Manager', 'User'],
  },
  {
    name: 'finance',
    label: 'Finance',
    to: '/finance',
    icon: FinanceIcon,
    roles: ['Admin', 'Manager'],
  },
  { name: 'report', label: 'Report', to: '/report', icon: ReportIcon, roles: ['Admin', 'Manager'] },
  { name: 'users', label: 'Users', to: '/users', icon: UsersIcon, roles: ['Admin', 'Manager'] },
]

const navItems = computed(() =>
  rawNavItems.filter((item) => item.roles.some((role) => authStore.hasRole(role))),
)
</script>
