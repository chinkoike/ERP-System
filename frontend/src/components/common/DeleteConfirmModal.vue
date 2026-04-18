<template>
  <TransitionRoot appear :show="modelValue" as="template">
    <Dialog as="div" class="relative z-100" @close="$emit('update:modelValue', false)">
      <TransitionChild
        as="template"
        enter="duration-300 ease-out"
        enter-from="opacity-0"
        enter-to="opacity-100"
        leave="duration-200 ease-in"
        leave-from="opacity-100"
        leave-to="opacity-0"
      >
        <div class="fixed inset-0 bg-slate-900/40 backdrop-blur-sm" aria-hidden="true" />
      </TransitionChild>

      <div class="fixed inset-0 overflow-y-auto">
        <div class="flex min-h-full items-center justify-center p-4 text-center">
          <TransitionChild
            as="template"
            enter="duration-300 ease-out"
            enter-from="opacity-0 scale-95"
            enter-to="opacity-100 scale-100"
            leave="duration-200 ease-in"
            leave-from="opacity-100 scale-100"
            leave-to="opacity-0 scale-95"
          >
            <DialogPanel
              class="w-full max-w-sm transform overflow-hidden rounded-[2.5rem] bg-white p-8 text-center align-middle shadow-2xl transition-all border border-slate-100"
            >
              <div
                class="mx-auto mb-5 flex h-16 w-16 items-center justify-center rounded-full bg-rose-50 border border-rose-100"
              >
                <div
                  class="flex h-12 w-12 items-center justify-center rounded-full bg-rose-100 text-rose-600"
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-7 w-7"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                    />
                  </svg>
                </div>
              </div>

              <DialogTitle as="h3" class="text-xl font-bold leading-6 text-slate-900">
                {{ title }}
              </DialogTitle>

              <div class="mt-3">
                <p class="text-sm text-slate-500 leading-relaxed">
                  {{ description }}
                </p>
                <p
                  v-if="itemName"
                  class="mt-2 text-sm font-bold text-slate-900 bg-slate-50 py-2 px-3 rounded-xl inline-block border border-slate-100"
                >
                  "{{ itemName }}"
                </p>
              </div>

              <div class="mt-8 flex flex-col gap-2">
                <button
                  type="button"
                  @click="$emit('confirm')"
                  :disabled="loading"
                  class="w-full rounded-2xl bg-rose-600 py-3.5 text-sm font-bold text-white shadow-lg shadow-rose-200 transition-all hover:bg-rose-700 active:scale-[0.98] disabled:opacity-50"
                >
                  <span v-if="loading" class="flex items-center justify-center gap-2">
                    <svg class="animate-spin h-4 w-4 text-white" viewBox="0 0 24 24">
                      <circle
                        class="opacity-25"
                        cx="12"
                        cy="12"
                        r="10"
                        stroke="currentColor"
                        stroke-width="4"
                      ></circle>
                      <path
                        class="opacity-75"
                        fill="currentColor"
                        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                      ></path>
                    </svg>
                    กำลังลบ...
                  </span>
                  <span v-else>{{ confirmText }}</span>
                </button>

                <button
                  type="button"
                  @click="$emit('update:modelValue', false)"
                  :disabled="loading"
                  class="w-full rounded-2xl border border-slate-200 py-3.5 text-sm font-bold text-slate-600 hover:bg-slate-50 transition-colors"
                >
                  {{ cancelText }}
                </button>
              </div>
            </DialogPanel>
          </TransitionChild>
        </div>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script setup lang="ts">
import { Dialog, DialogPanel, DialogTitle, TransitionRoot, TransitionChild } from '@headlessui/vue'

interface Props {
  modelValue: boolean
  title?: string
  description?: string
  itemName?: string | null
  confirmText?: string
  cancelText?: string
  loading?: boolean
}

withDefaults(defineProps<Props>(), {
  title: 'ยืนยันการลบ',
  description: 'คุณแน่ใจหรือไม่ที่จะลบรายการนี้? การกระทำนี้ไม่สามารถย้อนคืนได้',
  itemName: '',
  confirmText: 'ลบข้อมูล',
  cancelText: 'ยกเลิก',
  loading: false,
})

defineEmits(['update:modelValue', 'confirm'])
</script>
