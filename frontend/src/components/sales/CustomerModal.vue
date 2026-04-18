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
          <DialogPanel class="w-full max-w-xl overflow-hidden rounded-2xl bg-white shadow-2xl">
            <div class="flex items-center justify-between border-b border-slate-200 px-6 py-5">
              <DialogTitle class="text-base font-semibold text-slate-900">
                {{ editingCustomer ? 'แก้ไขลูกค้า' : 'เพิ่มลูกค้า' }}
              </DialogTitle>
              <button @click="$emit('close')" class="text-slate-500 hover:text-slate-700 text-2xl leading-none">×</button>
            </div>

            <div class="space-y-4 p-6">
              <div class="grid grid-cols-1 gap-3 md:grid-cols-2">
                <div>
                  <label class="mb-2 block text-sm font-medium text-slate-600">ชื่อ *</label>
                  <input
                    v-model="local.firstName"
                    type="text"
                    class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                </div>
                <div>
                  <label class="mb-2 block text-sm font-medium text-slate-600">นามสกุล</label>
                  <input
                    v-model="local.lastName"
                    type="text"
                    class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                </div>
              </div>
              <div>
                <label class="mb-2 block text-sm font-medium text-slate-600">Email *</label>
                <input
                  v-model="local.email"
                  type="email"
                  class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
              <div>
                <label class="mb-2 block text-sm font-medium text-slate-600">เบอร์โทร</label>
                <input
                  v-model="local.phone"
                  type="text"
                  class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
              <div>
                <label class="mb-2 block text-sm font-medium text-slate-600">ที่อยู่</label>
                <textarea
                  v-model="local.address"
                  rows="2"
                  class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none resize-vertical focus:ring-2 focus:ring-slate-300"
                ></textarea>
              </div>

              <div v-if="error" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{{ error }}</div>
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
                class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-50"
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
import type { Customer, CreateCustomerPayload, UpdateCustomerPayload } from '@/types/sales'

const props = defineProps<{
  show: boolean
  editingCustomer: Customer | null
  loading: boolean
  error: string
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit', payload: CreateCustomerPayload | UpdateCustomerPayload): void
}>()

const local = reactive({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  address: '',
})

watch(
  () => props.show,
  (opened) => {
    if (!opened) return
    const c = props.editingCustomer
    Object.assign(local, {
      firstName: c?.firstName ?? '',
      lastName: c?.lastName ?? '',
      email: c?.email ?? '',
      phone: c?.phone ?? '',
      address: c?.address ?? '',
    })
  }
)

function handleSubmit() {
  if (props.editingCustomer) {
    emit('submit', { ...local } satisfies UpdateCustomerPayload)
  } else {
    emit('submit', { ...local } satisfies CreateCustomerPayload)
  }
}
</script>
