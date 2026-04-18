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
          <DialogPanel class="w-full max-w-xl max-h-[90vh] overflow-y-auto rounded-2xl bg-white shadow-2xl">
            <!-- Header (sticky) -->
            <div class="sticky top-0 z-10 flex items-center justify-between border-b border-slate-200 bg-white px-6 py-5">
              <DialogTitle class="text-base font-semibold text-slate-900">สร้างใบสั่งซื้อ</DialogTitle>
              <button @click="$emit('close')" class="text-slate-500 hover:text-slate-700 text-2xl leading-none">×</button>
            </div>

            <!-- Body -->
            <div class="space-y-4 p-6">
              <!-- Supplier -->
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">Supplier *</label>
                <select
                  v-model="local.supplierId"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                >
                  <option value="">เลือก Supplier</option>
                  <option v-for="s in suppliers" :key="s.id" :value="s.id">{{ s.name }}</option>
                </select>
              </div>

              <!-- Items -->
              <div>
                <div class="flex items-center justify-between mb-2">
                  <label class="text-sm font-medium text-slate-600">รายการสินค้า *</label>
                  <button
                    @click="addItem"
                    class="rounded-2xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-medium text-slate-900 transition hover:bg-slate-50"
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
                    class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  >
                    <option value="">เลือกสินค้า</option>
                    <option v-for="p in products" :key="p.id" :value="p.id">{{ p.name }}</option>
                  </select>
                  <input
                    v-model.number="item.quantityOrdered"
                    type="number" min="1" placeholder="จำนวน"
                    class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-center text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                  <input
                    v-model.number="item.unitPrice"
                    @input="calcItemTotal(idx)"
                    type="number" min="0" placeholder="ราคา/หน่วย"
                    class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-right text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                  <button @click="removeItem(idx)" class="text-slate-400 hover:text-slate-600 text-lg px-1">×</button>
                </div>

                <!-- Total -->
                <div v-if="local.items.length > 0" class="mt-2 border-t border-slate-200 pt-2 text-right">
                  <span class="text-sm text-slate-500">ยอดรวม: </span>
                  <span class="text-base font-semibold text-slate-900">{{ formatCurrency(poTotal) }}</span>
                </div>
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
                class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {{ loading ? 'กำลังสร้าง...' : 'สร้าง PO' }}
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
import type { Supplier, CreatePurchaseOrderPayload } from '@/types/purchasing'

interface ProductOption {
  id: string
  name: string
}

interface PoItem {
  productId: string
  quantityOrdered: number
  unitPrice: number
  totalPrice: number
}

const props = defineProps<{
  show: boolean
  suppliers: Supplier[]
  products: ProductOption[]
  loading: boolean
  error: string
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit', payload: CreatePurchaseOrderPayload): void
}>()

const local = reactive<{ supplierId: string; items: PoItem[] }>({
  supplierId: '',
  items: [],
})

const poTotal = computed(() => local.items.reduce((s, i) => s + i.totalPrice, 0))

watch(
  () => props.show,
  (opened) => {
    if (!opened) return
    local.supplierId = ''
    local.items = []
  }
)

function addItem() {
  local.items.push({ productId: '', quantityOrdered: 1, unitPrice: 0, totalPrice: 0 })
}
function removeItem(idx: number) {
  local.items.splice(idx, 1)
}
function calcItemTotal(idx: number) {
  const item = local.items[idx]
  if (item) item.totalPrice = item.unitPrice * item.quantityOrdered
}

function handleSubmit() {
  emit('submit', {
    supplierId: local.supplierId,
    items: local.items.map((i) => ({ ...i, quantityReceived: 0 })),
  } satisfies CreatePurchaseOrderPayload)
}

function formatCurrency(v: number) {
  return new Intl.NumberFormat('th-TH', { style: 'currency', currency: 'THB', maximumFractionDigits: 0 }).format(v)
}
</script>
