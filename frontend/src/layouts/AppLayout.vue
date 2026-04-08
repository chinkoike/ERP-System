<template>
  <div class="flex h-screen overflow-hidden bg-slate-50">

    <!-- Sidebar -->
    <AppSidebar v-model:collapsed="collapsed"/>

    <!-- Main content area -->
    <div class="flex flex-1 flex-col overflow-hidden">

      <!-- Top bar -->
      <header class="flex h-16 flex-shrink-0 items-center justify-between border-b border-slate-100 bg-white px-6">
        <!-- Breadcrumb -->
        <div class="flex items-center gap-2">
          <span class="text-sm text-slate-400">ERP System</span>
          <span class="text-sm text-slate-300">/</span>
          <span class="text-sm font-medium text-slate-900">{{ currentPageLabel }}</span>
        </div>

        <!-- Right side -->
        <div class="flex items-center gap-4">
          <span class="text-sm text-slate-400">{{ today }}</span>

          <!-- Notification placeholder -->
          <button class="relative rounded-xl p-1.5 text-slate-400 hover:bg-slate-50 hover:text-slate-600 transition">
            <svg class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"/>
            </svg>
          </button>

          <!-- User avatar -->
          <div class="flex items-center gap-2">
            <div class="flex h-7 w-7 items-center justify-center rounded-full bg-slate-200 text-[11px] font-semibold text-slate-600">
              {{ userInitial }}
            </div>
            <span class="text-sm font-medium text-slate-700">{{ authStore.user?.username }}</span>
          </div>
        </div>
      </header>

      <!-- Page content (scrollable) -->
      <main class="flex-1 overflow-y-auto">
        <router-view/>
      </main>

    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import AppSidebar from '@/components/AppSidebar.vue'

const authStore = useAuthStore()
const route = useRoute()
const collapsed = ref(false)

const userInitial = computed(() => authStore.user?.username?.charAt(0).toUpperCase() ?? 'U')

const today = computed(() =>
  new Date().toLocaleDateString('th-TH', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })
)

const pageLabels: Record<string, string> = {
  dashboard:  'Dashboard',
  inventory:  'Inventory',
  sales:      'Sales',
  purchasing: 'Purchasing',
  finance:    'Finance',
  report:     'Report',
  users:      'Users & Roles',
}

const currentPageLabel = computed(() => pageLabels[route.name as string] ?? '')
</script>
