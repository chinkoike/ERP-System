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
          <DialogPanel class="w-full max-w-2xl max-h-[90vh] overflow-y-auto rounded-2xl bg-white shadow-2xl">
            <!-- Header (sticky) -->
            <div class="sticky top-0 z-10 flex items-center justify-between border-b border-slate-200 bg-white px-6 py-5">
              <DialogTitle class="text-base font-semibold text-slate-900">สร้าง Order ใหม่</DialogTitle>
              <button @click="$emit('close')" class="text-slate-500 hover:text-slate-700 text-2xl leading-none">×</button>
            </div>

            <!-- Body -->
            <div class="space-y-4 p-6">
              <!-- Customer -->
              <div>
                <label class="mb-2 block text-sm font-medium text-slate-600">ลูกค้า *</label>
                <select
                  v-model="local.customerId"
                  class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                >
                  <option value="">เลือกลูกค้า</option>
                  <option v-for="c in customers" :key="c.id" :value="c.id">{{ c.fullName }}</option>
                </select>
              </div>

              <!-- Shipping address -->
              <div>
                <label class="mb-2 block text-sm font-medium text-slate-600">ที่อยู่จัดส่ง *</label>
                <input
                  v-model="local.shippingAddress"
                  type="text"
                  class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>

              <!-- Items -->
              <div>
                <div class="flex items-center justify-between mb-2">
                  <label class="text-sm font-medium text-slate-600">รายการสินค้า *</label>
                  <button
                    @click="addItem"
                    class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-xs font-medium text-slate-900 transition hover:bg-slate-50"
                  >
                    + เพิ่ม
                  </button>
                </div>

                <div
                  v-for="(item, idx) in local.items"
                  :key="idx"
                  class="grid grid-cols-[2fr_1fr_1fr_auto] gap-2 mb-2 items-center"
                >
                  <select
                    v-model="item.productId"
                    @change="onProductSelect(idx)"
                    class="rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  >
                    <option value="">เลือกสินค้า</option>
                    <option v-for="p in products" :key="p.id" :value="p.id">{{ p.name }}</option>
                  </select>
                  <input
                    v-model.number="item.quantity"
                    @input="calcItemTotal(idx)"
                    type="number"
                    min="1"
                    placeholder="จำนวน"
                    class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 text-center outline-none focus:ring-2 focus:ring-slate-300"
                  />
                  <div class="rounded-2xl border border-slate-200 bg-slate-50 px-3 py-2 text-right text-sm text-slate-500">
                    {{ formatCurrency(item.totalPrice) }}
                  </div>
                  <button @click="removeItem(idx)" class="text-slate-400 hover:text-slate-600 text-lg px-1">×</button>
                </div>

                <!-- Total -->
                <div v-if="local.items.length > 0" class="mt-2 border-t border-slate-200 pt-2 text-right">
                  <span class="text-sm text-slate-500">ยอดรวม: </span>
                  <span class="text-base font-semibold text-slate-900">{{ formatCurrency(orderTotal) }}</span>
                </div>
              </div>

              <!-- Remarks -->
              <div>
                <label class="mb-2 block text-sm font-medium text-slate-600">หมายเหตุ</label>
                <input
                  v-model="local.remarks"
                  type="text"
                  class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>

              <div v-if="error" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{{ error }}</div>
            </div>

            <!-- Footer (sticky) -->
            <div class="sticky bottom-0 z-10 flex justify-end gap-2 border-t border-slate-200 bg-white p-4">
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
                {{ loading ? 'กำลังสร้าง...' : 'สร้าง Order' }}
              </button>
            </div>
          </DialogPanel>
        </TransitionChild>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script setup lang="ts">
import { reactive, computed, watch } from 'vue'
import { Dialog, DialogPanel, DialogTitle, TransitionRoot, TransitionChild } from '@headlessui/vue'
import type { Customer, CreateOrderPayload, OrderItem } from '@/types/sales'

// Product shape from inventoryStore (ใช้แค่ id, name, basePrice)
interface ProductOption {
  id: string
  name: string
  basePrice: number
}

const props = defineProps<{
  show: boolean
  customers: Customer[]
  products: ProductOption[]
  loading: boolean
  error: string
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit', payload: CreateOrderPayload): void
}>()

const local = reactive<{
  customerId: string
  shippingAddress: string
  remarks: string
  items: OrderItem[]
}>({
  customerId: '',
  shippingAddress: '',
  remarks: '',
  items: [],
})

const orderTotal = computed(() => local.items.reduce((s, i) => s + i.totalPrice, 0))

// Reset form every time modal opens
watch(
  () => props.show,
  (opened) => {
    if (!opened) return
    Object.assign(local, { customerId: '', shippingAddress: '', remarks: '', items: [] })
  }
)

function addItem() {
  local.items.push({ productId: '', quantity: 1, unitPrice: 0, totalPrice: 0 })
}

function removeItem(idx: number) {
  local.items.splice(idx, 1)
}

function onProductSelect(idx: number) {
  const item = local.items[idx]
  if (!item) return
  const product = props.products.find((p) => p.id === item.productId)
  if (product) {
    item.unitPrice = product.basePrice
    item.totalPrice = product.basePrice * item.quantity
  }
}

function calcItemTotal(idx: number) {
  const item = local.items[idx]
  if (!item) return
  item.totalPrice = item.unitPrice * item.quantity
}

function handleSubmit() {
  emit('submit', {
    customerId: local.customerId,
    shippingAddress: local.shippingAddress,
    remarks: local.remarks || undefined,
    items: local.items,
  } satisfies CreateOrderPayload)
}

function formatCurrency(v: number) {
  return new Intl.NumberFormat('th-TH', { style: 'currency', currency: 'THB', maximumFractionDigits: 0 }).format(v)
}
</script>
