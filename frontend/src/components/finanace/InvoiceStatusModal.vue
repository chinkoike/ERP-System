<template>
  <TransitionRoot appear :show="show" as="template">
    <Dialog as="div" class="relative z-50" @close="$emit('close')">
      <div class="fixed inset-0 bg-black/30" aria-hidden="true" />

      <div class="fixed inset-0 flex items-center justify-center p-4">
        <TransitionChild
          as="template"
          enter="ease-out duration-200"
          enter-from="opacity-0 scale-95"
          enter-to="opacity-100 scale-100"
          leave="ease-in duration-150"
          leave-from="opacity-100 scale-100"
          leave-to="opacity-0 scale-95"
        >
          <DialogPanel class="w-full max-w-sm overflow-hidden rounded-2xl bg-white shadow-2xl">
            <div class="flex items-center justify-between border-b border-slate-200 px-6 py-5">
              <div>
                <DialogTitle class="text-base font-semibold text-slate-900"
                  >อัปเดตสถานะ</DialogTitle
                >
                <p v-if="invoice" class="font-mono text-xs text-slate-400 mt-0.5">
                  {{ invoice.invoiceNumber }}
                </p>
              </div>
              <button
                @click="$emit('close')"
                class="text-slate-500 hover:text-slate-700 text-2xl leading-none"
              >
                ×
              </button>
            </div>

            <div class="space-y-4 p-6">
              <div
                v-if="invoice"
                class="flex items-center gap-3 rounded-2xl border border-slate-100 bg-slate-50 px-4 py-3"
              >
                <span class="text-sm text-slate-500">สถานะปัจจุบัน:</span>
                <span :class="invoiceStatusClass(invoice.status)">{{
                  invoiceStatusLabel(invoice.status)
                }}</span>
              </div>

              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600"
                  >เปลี่ยนเป็นสถานะ *</label
                >
                <select
                  v-model="selectedStatus"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                >
                  <option value="">เลือกสถานะ</option>
                  <option v-for="s in availableStatuses" :key="s.value" :value="s.value">
                    {{ s.label }}
                  </option>
                </select>
              </div>

              <div v-if="error" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">
                {{ error }}
              </div>
            </div>

            <div class="flex justify-end gap-2 border-t border-slate-200 bg-slate-50 px-4 py-4">
              <button
                @click="$emit('close')"
                class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
              >
                ยกเลิก
              </button>
              <button
                @click="handleSubmit"
                :disabled="loading || !selectedStatus"
                class="rounded-2xl bg-emerald-600 px-5 py-2 text-sm font-medium text-white transition hover:bg-emerald-700 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {{ loading ? 'กำลังบันทึก...' : 'อัปเดตสถานะ' }}
              </button>
            </div>
          </DialogPanel>
        </TransitionChild>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Dialog, DialogPanel, DialogTitle, TransitionRoot, TransitionChild } from '@headlessui/vue'
import type { Invoice, InvoiceStatus } from '@/types/finance'

const props = defineProps<{
  show: boolean
  invoice: Invoice | null
  loading: boolean
  error: string
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit', status: InvoiceStatus): void
}>()

const invoiceStatuses: { value: InvoiceStatus; label: string }[] = [
  { value: 'Draft', label: 'ร่าง' },
  { value: 'Issued', label: 'ออกแล้ว' },
  { value: 'Paid', label: 'ชำระแล้ว' },
  { value: 'Overdue', label: 'เกินกำหนด' },
  { value: 'Cancelled', label: 'ยกเลิก' },
]

const availableStatuses = computed(() =>
  invoiceStatuses.filter((s) => s.value !== props.invoice?.status),
)

const selectedStatus = ref<InvoiceStatus | ''>('')

watch(
  () => props.show,
  (opened) => {
    if (opened) selectedStatus.value = ''
  },
)

function handleSubmit() {
  if (!selectedStatus.value) return
  emit('submit', selectedStatus.value as InvoiceStatus)
}

function invoiceStatusLabel(s: InvoiceStatus) {
  return invoiceStatuses.find((x) => x.value === s)?.label ?? s
}
function invoiceStatusClass(s: InvoiceStatus) {
  const map: Record<InvoiceStatus, string> = {
    Draft: 'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-500',
    Issued: 'inline-flex rounded-full bg-blue-50 px-2 py-0.5 text-xs font-semibold text-blue-600',
    Paid: 'inline-flex rounded-full bg-emerald-50 px-2 py-0.5 text-xs font-semibold text-emerald-600',
    Overdue: 'inline-flex rounded-full bg-rose-50 px-2 py-0.5 text-xs font-semibold text-rose-600',
    Cancelled:
      'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-400',
  }
  return map[s]
}
</script>
