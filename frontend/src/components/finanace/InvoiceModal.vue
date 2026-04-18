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
          <DialogPanel class="w-full max-w-lg overflow-hidden rounded-2xl bg-white shadow-2xl">
            <div class="flex items-center justify-between border-b border-slate-200 px-6 py-5">
              <DialogTitle class="text-base font-semibold text-slate-900">
                {{ editingInvoice ? 'แก้ไข Invoice' : 'สร้าง Invoice' }}
              </DialogTitle>
              <button
                @click="$emit('close')"
                class="text-slate-500 hover:text-slate-700 text-2xl leading-none"
              >
                ×
              </button>
            </div>

            <div class="space-y-4 p-6">
              <div v-if="!editingInvoice">
                <label class="mb-1.5 block text-sm font-medium text-slate-600">เลข Invoice *</label>
                <input
                  v-model="local.invoiceNumber"
                  type="text"
                  placeholder="INV-2026-001"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 font-mono text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>

              <div v-if="!editingInvoice" class="grid grid-cols-2 gap-3">
                <div>
                  <label class="mb-1.5 block text-sm font-medium text-slate-600">Customer ID</label>
                  <input
                    v-model="local.customerId"
                    type="text"
                    placeholder="UUID (ถ้ามี)"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                </div>
                <div>
                  <label class="mb-1.5 block text-sm font-medium text-slate-600">Supplier ID</label>
                  <input
                    v-model="local.supplierId"
                    type="text"
                    placeholder="UUID (ถ้ามี)"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                </div>
              </div>
              <p v-if="!editingInvoice" class="text-xs text-slate-500">
                เลือกกรอก Customer ID หรือ Supplier ID อย่างใดอย่างหนึ่งก็พอ
              </p>

              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="mb-1.5 block text-sm font-medium text-slate-600"
                    >ยอดรวม (บาท) *</label
                  >
                  <input
                    v-model.number="local.totalAmount"
                    :disabled="!!editingInvoice"
                    type="number"
                    min="0.01"
                    step="0.01"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300 disabled:bg-slate-50 disabled:text-slate-400"
                  />
                </div>
                <div>
                  <label class="mb-1.5 block text-sm font-medium text-slate-600"
                    >ยอดค้างชำระ (บาท) *</label
                  >
                  <input
                    v-model.number="local.amountDue"
                    type="number"
                    min="0.01"
                    step="0.01"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                </div>
              </div>

              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="mb-1.5 block text-sm font-medium text-slate-600">วันที่ออก *</label>
                  <input
                    v-model="local.invoiceDate"
                    :disabled="!!editingInvoice"
                    type="date"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300 disabled:bg-slate-50 disabled:text-slate-400"
                  />
                </div>
                <div>
                  <label class="mb-1.5 block text-sm font-medium text-slate-600">ครบกำหนด *</label>
                  <input
                    v-model="local.dueDate"
                    type="date"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                </div>
              </div>

              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">คำอธิบาย</label>
                <textarea
                  v-model="local.description"
                  rows="2"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none resize-y focus:ring-2 focus:ring-slate-300"
                ></textarea>
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
                :disabled="loading"
                class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {{ loading ? 'กำลังบันทึก...' : 'บันทึก' }}
              </button>
            </div>
          </DialogPanel>
        </TransitionChild>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script setup lang="ts">
import { reactive, watch } from 'vue'
import { Dialog, DialogPanel, DialogTitle, TransitionRoot, TransitionChild } from '@headlessui/vue'
import type { Invoice, CreateInvoicePayload, UpdateInvoicePayload } from '@/types/finance'

const props = defineProps<{
  show: boolean
  editingInvoice: Invoice | null
  loading: boolean
  error: string
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit', payload: CreateInvoicePayload | UpdateInvoicePayload): void
}>()

const local = reactive({
  invoiceNumber: '',
  customerId: '',
  supplierId: '',
  description: '',
  totalAmount: 0,
  amountDue: 0,
  invoiceDate: '',
  dueDate: '',
})

watch(
  () => props.show,
  (opened) => {
    if (!opened) return
    if (props.editingInvoice) {
      const inv = props.editingInvoice
      Object.assign(local, {
        invoiceNumber: inv.invoiceNumber,
        customerId: inv.customerId ?? '',
        supplierId: inv.supplierId ?? '',
        description: inv.description ?? '',
        totalAmount: inv.totalAmount,
        amountDue: inv.amountDue,
        invoiceDate: inv.invoiceDate.split('T')[0],
        dueDate: inv.dueDate.split('T')[0],
      })
    } else {
      const today = new Date().toISOString().split('T')[0]
      const due = new Date(Date.now() + 30 * 86400000).toISOString().split('T')[0]
      Object.assign(local, {
        invoiceNumber: '',
        customerId: '',
        supplierId: '',
        description: '',
        totalAmount: 0,
        amountDue: 0,
        invoiceDate: today,
        dueDate: due,
      })
    }
  },
)

function handleSubmit() {
  if (props.editingInvoice) {
    emit('submit', {
      amountDue: local.amountDue,
      dueDate: local.dueDate,
      description: local.description,
    } satisfies UpdateInvoicePayload)
  } else {
    emit('submit', {
      invoiceNumber: local.invoiceNumber,
      customerId: local.customerId || undefined,
      supplierId: local.supplierId || undefined,
      description: local.description,
      totalAmount: local.totalAmount,
      amountDue: local.amountDue,
      invoiceDate: local.invoiceDate,
      dueDate: local.dueDate,
    } satisfies CreateInvoicePayload)
  }
}
</script>
