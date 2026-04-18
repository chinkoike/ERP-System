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
          <DialogPanel class="w-full max-w-lg max-h-[90vh] overflow-y-auto rounded-2xl bg-white shadow-2xl">
            <!-- Header (sticky) -->
            <div class="sticky top-0 z-10 flex items-center justify-between border-b border-slate-200 bg-white px-6 py-5">
              <div>
                <DialogTitle class="text-base font-semibold text-slate-900">รับสินค้า</DialogTitle>
                <span v-if="purchaseOrder" class="font-mono text-sm text-slate-400">{{ purchaseOrder.purchaseOrderNumber }}</span>
              </div>
              <button @click="$emit('close')" class="text-slate-500 hover:text-slate-700 text-2xl leading-none">×</button>
            </div>

            <!-- Body -->
            <div class="space-y-3 p-6">
              <div
                v-for="item in localItems"
                :key="item.productId"
                class="rounded-2xl border border-slate-100 bg-slate-50 p-4"
              >
                <div class="flex items-center justify-between mb-3">
                  <span class="text-sm font-semibold text-slate-900">{{ productName(item.productId) }}</span>
                  <span class="text-xs text-slate-400">สั่ง {{ item.quantityOrdered }} / รับแล้ว {{ item.quantityReceived }}</span>
                </div>
                <div class="flex items-center gap-3">
                  <label class="w-20 shrink-0 text-sm text-slate-600">รับเพิ่ม</label>
                  <input
                    v-model.number="item.toReceive"
                    type="number"
                    min="0"
                    :max="item.quantityOrdered - item.quantityReceived"
                    class="flex-1 rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-center text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                  <span class="text-xs text-slate-400">/ {{ item.quantityOrdered - item.quantityReceived }} คงเหลือ</span>
                </div>
              </div>

              <div v-if="error" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{{ error }}</div>
            </div>

            <!-- Footer (sticky) -->
            <div class="sticky bottom-0 flex justify-end gap-2 border-t border-slate-200 bg-white p-4">
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
                {{ loading ? 'กำลังบันทึก...' : 'ยืนยันรับสินค้า' }}
              </button>
            </div>
          </DialogPanel>
        </TransitionChild>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { Dialog, DialogPanel, DialogTitle, TransitionRoot, TransitionChild } from '@headlessui/vue'
import type { PurchaseOrder, PurchaseOrderItem } from '@/types/purchasing'

interface ProductOption {
  id: string
  name: string
}

// ReceiveItem extends PurchaseOrderItem with toReceive field
interface ReceiveItem extends PurchaseOrderItem {
  toReceive: number
}

export interface ReceivePayload {
  items: PurchaseOrderItem[]
}

const props = defineProps<{
  show: boolean
  purchaseOrder: PurchaseOrder | null
  products: ProductOption[]
  loading: boolean
  error: string
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit', payload: ReceivePayload): void
}>()

const localItems = ref<ReceiveItem[]>([])

watch(
  () => props.show,
  (opened) => {
    if (!opened || !props.purchaseOrder) return
    localItems.value = props.purchaseOrder.items.map((i) => ({
      ...i,
      toReceive: i.quantityOrdered - i.quantityReceived,
    }))
  }
)

function productName(id: string) {
  return props.products.find((p) => p.id === id)?.name ?? id
}

function handleSubmit() {
  const itemsToSend = localItems.value.filter((i) => i.toReceive > 0)
  emit('submit', {
    items: itemsToSend.map((i) => ({
      productId: i.productId,
      quantityOrdered: i.quantityOrdered,
      quantityReceived: i.toReceive,
      unitPrice: i.unitPrice,
      totalPrice: i.totalPrice,
    })),
  })
}
</script>
