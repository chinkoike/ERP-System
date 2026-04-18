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
          <DialogPanel class="w-full max-w-md overflow-hidden rounded-2xl bg-white shadow-2xl">
            <div class="flex items-center justify-between border-b border-slate-200 px-6 py-5">
              <div>
                <DialogTitle class="text-base font-semibold text-slate-900"
                  >บันทึกการชำระเงิน</DialogTitle
                >
                <span v-if="invoice" class="font-mono text-sm text-slate-400">{{
                  invoice.invoiceNumber
                }}</span>
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
                class="rounded-2xl border border-slate-100 bg-slate-50 p-4 space-y-2"
              >
                <div class="flex justify-between text-sm">
                  <span class="text-slate-500">ยอดรวม</span>
                  <span class="font-semibold text-slate-900">{{
                    formatCurrency(invoice.totalAmount)
                  }}</span>
                </div>
                <div class="flex justify-between text-sm">
                  <span class="text-slate-500">ยอดค้างชำระ</span>
                  <span class="font-semibold text-slate-900">{{
                    formatCurrency(invoice.amountDue)
                  }}</span>
                </div>
              </div>

              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600"
                  >บัญชีที่รับเงิน *</label
                >
                <select
                  v-model="local.accountId"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                >
                  <option value="">เลือกบัญชี</option>
                  <option v-for="acc in accounts" :key="acc.id" :value="acc.id">
                    {{ acc.accountName }} ({{ formatCurrency(acc.balance) }})
                  </option>
                </select>
              </div>

              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600"
                  >จำนวนเงิน (บาท) *</label
                >
                <input
                  v-model.number="local.amountPaid"
                  type="number"
                  min="0.01"
                  step="0.01"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>

              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="mb-1.5 block text-sm font-medium text-slate-600"
                    >วันที่ชำระ *</label
                  >
                  <input
                    v-model="local.paymentDate"
                    type="date"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                </div>
                <div>
                  <label class="mb-1.5 block text-sm font-medium text-slate-600">วิธีชำระ *</label>
                  <select
                    v-model="local.paymentMethod"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  >
                    <option value="">เลือกวิธี</option>
                    <option v-for="m in paymentMethods" :key="m.value" :value="m.value">
                      {{ m.label }}
                    </option>
                  </select>
                </div>
              </div>

              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">เลขอ้างอิง</label>
                <input
                  v-model="local.referenceNumber"
                  type="text"
                  placeholder="เลขโอน, เลขเช็ค ฯลฯ"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
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
                {{ loading ? 'กำลังบันทึก...' : 'บันทึกการชำระ' }}
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
import type { Invoice, Account, CreatePaymentPayload, PaymentMethod } from '@/types/finance'

const props = defineProps<{
  show: boolean
  invoice: Invoice | null
  accounts: Account[]
  loading: boolean
  error: string
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit', payload: CreatePaymentPayload): void
}>()

const paymentMethods: { value: PaymentMethod; label: string }[] = [
  { value: 'Cash', label: 'เงินสด' },
  { value: 'CreditCard', label: 'บัตรเครดิต' },
  { value: 'BankTransfer', label: 'โอนธนาคาร' },
  { value: 'Cheque', label: 'เช็ค' },
  { value: 'Others', label: 'อื่นๆ' },
]

const local = reactive({
  accountId: '',
  amountPaid: 0,
  paymentDate: '',
  paymentMethod: '' as PaymentMethod | '',
  referenceNumber: '',
})

watch(
  () => props.show,
  (opened) => {
    if (!opened || !props.invoice) return
    Object.assign(local, {
      accountId: '',
      amountPaid: props.invoice.amountDue,
      paymentDate: new Date().toISOString().split('T')[0],
      paymentMethod: '',
      referenceNumber: '',
    })
  },
)

function handleSubmit() {
  if (!props.invoice) return
  emit('submit', {
    invoiceId: props.invoice.id,
    accountId: local.accountId,
    amountPaid: local.amountPaid,
    paymentDate: local.paymentDate,
    paymentMethod: local.paymentMethod as PaymentMethod,
    referenceNumber: local.referenceNumber || undefined,
  } satisfies CreatePaymentPayload)
}

function formatCurrency(v: number) {
  return new Intl.NumberFormat('th-TH', {
    style: 'currency',
    currency: 'THB',
    maximumFractionDigits: 0,
  }).format(v)
}
</script>
