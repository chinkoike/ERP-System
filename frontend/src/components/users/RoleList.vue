<template>
  <div class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
    <table class="min-w-full border-collapse">
      <thead>
        <tr class="border-b border-slate-100">
          <th class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">ชื่อ Role</th>
          <th class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">คำอธิบาย</th>
          <th class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400">สร้างเมื่อ</th>
          <th class="px-6 py-3"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="roles.length === 0">
          <td colspan="4" class="px-6 py-20 text-center text-sm text-slate-400">ไม่พบ Role</td>
        </tr>
        <tr
          v-for="(r, i) in roles"
          :key="r.id"
          :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
        >
          <td class="px-6 py-4">
            <span :class="roleBadgeClass(r.name)">{{ r.name }}</span>
          </td>
          <td class="px-6 py-4 text-sm text-slate-500">{{ r.description ?? '—' }}</td>
          <td class="px-6 py-4 text-center text-sm text-slate-500">{{ formatDate(r.createdAt) }}</td>
          <td class="px-6 py-4">
            <div class="flex items-center gap-2 justify-end">
              <button
                @click="$emit('edit', r)"
                class="rounded-2xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
              >
                แก้ไข
              </button>
              <button
                @click="$emit('delete', r)"
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
</template>

<script setup lang="ts">
import type { Role } from '@/types/identity'

defineProps<{
  roles: Role[]
}>()

defineEmits<{
  (e: 'edit', role: Role): void
  (e: 'delete', role: Role): void
}>()

function roleBadgeClass(name: string) {
  if (name === 'Admin') return 'inline-flex rounded-full bg-violet-50 px-2.5 py-1 text-sm font-semibold text-violet-600'
  if (name === 'Manager') return 'inline-flex rounded-full bg-blue-50 px-2.5 py-1 text-sm font-semibold text-blue-600'
  return 'inline-flex rounded-full bg-slate-100 px-2.5 py-1 text-sm font-semibold text-slate-600'
}

function formatDate(d: string) {
  return new Date(d).toLocaleDateString('th-TH', { day: 'numeric', month: 'short', year: 'numeric' })
}
</script>
