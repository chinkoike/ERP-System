<template>
  <div class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
    <table class="min-w-full border-collapse">
      <thead>
        <tr class="border-b border-slate-100">
          <th
            class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
          >
            ชื่อบัญชี
          </th>
          <th
            class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
          >
            รหัสบัญชี
          </th>
          <th
            class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
          >
            ยอดคงเหลือ
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="accounts.length === 0">
          <td colspan="3" class="px-6 py-20 text-center text-sm text-slate-400">ไม่พบบัญชี</td>
        </tr>
        <tr
          v-for="(acc, i) in accounts"
          :key="acc.id"
          :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
        >
          <td class="px-6 py-4 text-sm font-semibold text-slate-900">{{ acc.accountName }}</td>
          <td class="px-6 py-4 font-mono text-sm text-slate-500">{{ acc.accountCode }}</td>
          <td
            class="px-6 py-4 text-right text-sm font-semibold"
            :class="acc.balance >= 0 ? 'text-emerald-600' : 'text-rose-600'"
          >
            {{ formatCurrency(acc.balance) }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import type { Account } from '@/types/finance'

defineProps<{
  accounts: Account[]
}>()

function formatCurrency(v: number) {
  return new Intl.NumberFormat('th-TH', {
    style: 'currency',
    currency: 'THB',
    maximumFractionDigits: 0,
  }).format(v)
}
</script>
