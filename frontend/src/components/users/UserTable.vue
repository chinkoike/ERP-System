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
          :value="searchUser"
          @input="$emit('update:searchUser', ($event.target as HTMLInputElement).value)"
          type="text"
          placeholder="ค้นหา username, email..."
          class="w-full rounded-2xl border border-slate-200 bg-white py-2 pl-10 pr-4 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
        />
      </div>
      <select
        :value="filterRole"
        @change="$emit('update:filterRole', ($event.target as HTMLSelectElement).value)"
        class="rounded-2xl border border-slate-200 bg-white py-2 px-4 text-sm text-slate-700 outline-none cursor-pointer focus:ring-2 focus:ring-slate-300"
      >
        <option value="">ทุก Role</option>
        <option v-for="role in roles" :key="role.id" :value="role.id">{{ role.name }}</option>
      </select>
      <select
        :value="filterActive"
        @change="$emit('update:filterActive', ($event.target as HTMLSelectElement).value)"
        class="rounded-2xl border border-slate-200 bg-white py-2 px-4 text-sm text-slate-700 outline-none cursor-pointer focus:ring-2 focus:ring-slate-300"
      >
        <option value="">ทุกสถานะ</option>
        <option value="true">Active</option>
        <option value="false">Inactive</option>
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
              ผู้ใช้
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              Email
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              ตำแหน่ง / แผนก
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              Role
            </th>
            <th
              class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              สถานะ
            </th>
            <th
              class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
            >
              Login ล่าสุด
            </th>
            <th class="px-6 py-3"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="users.length === 0">
            <td colspan="7" class="px-6 py-20 text-center text-sm text-slate-400">ไม่พบผู้ใช้</td>
          </tr>
          <tr
            v-for="(u, i) in users"
            :key="u.id"
            :class="[
              { 'bg-slate-50': i % 2 === 1 },
              'border-b border-slate-100 transition-opacity duration-200',
              { 'opacity-50': isSearching },
            ]"
          >
            <!-- Avatar + name -->
            <td class="px-6 py-4">
              <div class="flex items-center gap-3">
                <div
                  class="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-slate-200"
                >
                  <span class="text-xs font-semibold text-slate-600">
                    {{ u.fullName?.charAt(0) ?? u.username.charAt(0).toUpperCase() }}
                  </span>
                </div>
                <div>
                  <div class="text-sm font-semibold text-slate-900">
                    {{ u.fullName || u.username }}
                  </div>
                  <div class="text-xs text-slate-400">@{{ u.username }}</div>
                </div>
              </div>
            </td>
            <td class="px-6 py-4 text-sm text-slate-600">{{ u.email }}</td>
            <td class="px-6 py-4">
              <div class="text-sm text-slate-600">{{ u.jobTitle ?? '—' }}</div>
              <div class="text-xs text-slate-400">{{ u.department ?? '' }}</div>
            </td>
            <td class="px-6 py-4">
              <div class="flex flex-wrap gap-1">
                <span v-for="role in u.roles" :key="role" :class="roleBadgeClass(role)">
                  {{ role }}
                </span>
                <span v-if="u.roles.length === 0" class="text-xs text-slate-400">—</span>
              </div>
            </td>
            <td class="px-6 py-4 text-center">
              <span
                :class="
                  u.isActive
                    ? 'inline-flex rounded-full bg-emerald-50 px-2 py-0.5 text-xs font-semibold text-emerald-600'
                    : 'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-400'
                "
              >
                {{ u.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
            <td class="px-6 py-4 text-sm text-slate-500">
              {{ u.lastLoginAt ? formatDate(u.lastLoginAt) : '—' }}
            </td>
            <td class="px-6 py-4">
              <div class="flex items-center gap-2 justify-end">
                <button
                  @click="$emit('edit', u)"
                  class="rounded-2xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                >
                  แก้ไข
                </button>
                <button
                  @click="$emit('delete', u)"
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

    <!-- Pagination -->
    <div class="flex items-center justify-between bg-white px-6 py-4 text-sm text-slate-500">
      <div>แสดง {{ users.length }} จาก {{ totalItems }} รายการ</div>
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
import type { User, Role } from '@/types/identity'

defineProps<{
  users: User[]
  roles: Role[]
  totalItems: number
  currentPage: number
  totalPages: number
  searchUser: string
  filterRole: string
  filterActive: string
  isSearching: boolean
}>()

defineEmits<{
  (e: 'update:searchUser', val: string): void
  (e: 'update:filterRole', val: string): void
  (e: 'update:filterActive', val: string): void
  (e: 'edit', user: User): void
  (e: 'delete', user: User): void
  (e: 'pageChange', page: number): void
}>()

function roleBadgeClass(role: string) {
  if (role === 'Admin')
    return 'inline-flex rounded-full bg-violet-50 px-2 py-0.5 text-xs font-semibold text-violet-600'
  if (role === 'Manager')
    return 'inline-flex rounded-full bg-blue-50 px-2 py-0.5 text-xs font-semibold text-blue-600'
  return 'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-600'
}

function formatDate(d: string) {
  return new Date(d).toLocaleDateString('th-TH', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  })
}
</script>
