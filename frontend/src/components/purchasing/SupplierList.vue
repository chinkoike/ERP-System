<template>
  <div>
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
        :value="searchSupplier"
        @input="$emit('update:searchSupplier', ($event.target as HTMLInputElement).value)"
        type="text"
        placeholder="ค้นหา Supplier..."
        class="w-full rounded-2xl border border-slate-200 bg-white py-2 pl-10 pr-4 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
      />
    </div>

    <!-- Table -->
    <div class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
      <table class="min-w-full border-collapse">
        <thead>
          <tr class="border-b border-slate-100">
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              ชื่อบริษัท
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              ผู้ติดต่อ
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              Email
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              เบอร์โทร
            </th>
            <th class="px-6 py-3"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="suppliers.length === 0">
            <td colspan="5" class="px-6 py-20 text-center text-sm text-slate-400">
              ไม่พบ Supplier
            </td>
          </tr>
          <tr
            v-for="(s, i) in suppliers"
            :key="s.id"
            :class="[
              { 'bg-slate-50': i % 2 === 1 },
              'border-b border-slate-100 transition-opacity duration-200',
              { 'opacity-50': isSearching },
            ]"
          >
            <td class="px-6 py-4 text-sm font-semibold text-slate-900">{{ s.name }}</td>
            <td class="px-6 py-4 text-sm text-slate-600">{{ s.contactName ?? '—' }}</td>
            <td class="px-6 py-4 text-sm text-slate-600">{{ s.email ?? '—' }}</td>
            <td class="px-6 py-4 text-sm text-slate-600">{{ s.phone ?? '—' }}</td>
            <td class="px-6 py-4">
              <div class="flex items-center gap-2 justify-end">
                <button
                  v-if="authStore.isAdmin || authStore.isManager"
                  @click="$emit('edit', s)"
                  class="rounded-2xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                >
                  แก้ไข
                </button>
                <button
                  v-if="authStore.isAdmin || authStore.isManager"
                  @click="$emit('delete', s)"
                  class="rounded-2xl border border-rose-200 bg-white px-3 py-1.5 text-xs font-medium text-rose-600 transition hover:bg-rose-50"
                >
                  ลบ
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore'
import type { Supplier } from '@/types/purchasing'

const authStore = useAuthStore()

defineProps<{
  suppliers: Supplier[]
  searchSupplier: string
  isSearching: boolean
}>()

defineEmits<{
  (e: 'update:searchSupplier', val: string): void
  (e: 'edit', supplier: Supplier): void
  (e: 'delete', supplier: Supplier): void
}>()
</script>
