<template>
  <TransitionRoot appear :show="modelValue" as="template">
    <Dialog as="div" class="relative z-50" @close="$emit('update:modelValue', false)">
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
              class="w-full max-w-md transform overflow-hidden rounded-3xl bg-white p-6 text-left align-middle shadow-2xl transition-all"
            >
              <div class="flex items-center justify-between mb-6">
                <DialogTitle as="h3" class="text-lg font-bold text-slate-900">
                  {{ editingItem ? 'แก้ไขสินค้า' : 'เพิ่มสินค้าใหม่' }}
                </DialogTitle>
                <button
                  @click="$emit('update:modelValue', false)"
                  class="rounded-full p-1 text-slate-400 hover:bg-slate-100 hover:text-slate-600 transition-colors"
                >
                  <span class="text-2xl leading-none">&times;</span>
                </button>
              </div>

              <form @submit.prevent="submit" class="space-y-4">
                <div>
                  <label class="block text-xs font-semibold text-slate-500 mb-1.5 ml-1"
                    >ชื่อสินค้า</label
                  >
                  <input
                    v-model="form.name"
                    type="text"
                    placeholder="เช่น เสื้อยืด Oversize"
                    class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900"
                    required
                  />
                </div>

                <div class="grid grid-cols-2 gap-4">
                  <div>
                    <label class="block text-xs font-semibold text-slate-500 mb-1.5 ml-1"
                      >SKU</label
                    >
                    <input
                      v-model="form.sku"
                      placeholder="SKU-001"
                      :disabled="!!editingItem"
                      class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900 disabled:opacity-50 disabled:bg-slate-100"
                    />
                  </div>
                  <div>
                    <label class="block text-xs font-semibold text-slate-500 mb-1.5 ml-1"
                      >ราคา (฿)</label
                    >
                    <input
                      v-model.number="form.basePrice"
                      type="number"
                      step="0.01"
                      placeholder="0.00"
                      class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900"
                      required
                    />
                  </div>
                </div>

                <div v-if="!editingItem">
                  <label class="block text-xs font-semibold text-slate-500 mb-1.5 ml-1"
                    >สต็อกเริ่มต้น</label
                  >
                  <input
                    v-model.number="form.initialStock"
                    type="number"
                    placeholder="0"
                    class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900"
                  />
                </div>

                <div>
                  <label class="block text-xs font-semibold text-slate-500 mb-1.5 ml-1"
                    >หมวดหมู่</label
                  >
                  <div class="relative">
                    <select
                      v-model="form.categoryId"
                      class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900 appearance-none cursor-pointer"
                      required
                    >
                      <option value="">-- เลือกหมวดหมู่ --</option>
                      <option v-for="c in store.categories" :key="c.id" :value="c.id">
                        {{ c.name }}
                      </option>
                    </select>
                    <div
                      class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-4 text-slate-400"
                    >
                      <svg class="h-4 w-4 fill-current" viewBox="0 0 20 20">
                        <path
                          d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z"
                        />
                      </svg>
                    </div>
                  </div>
                </div>

                <div>
                  <label class="block text-xs font-semibold text-slate-500 mb-1.5 ml-1"
                    >คำอธิบาย</label
                  >
                  <textarea
                    v-model="form.description"
                    placeholder="รายละเอียดสินค้า..."
                    rows="2"
                    class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900 resize-none"
                  ></textarea>
                </div>

                <div
                  v-if="error"
                  class="text-xs text-rose-600 bg-rose-50 p-3 rounded-2xl border border-rose-100"
                >
                  {{ error }}
                </div>

                <div class="flex gap-3 pt-4">
                  <button
                    type="button"
                    @click="$emit('update:modelValue', false)"
                    class="flex-1 rounded-2xl border border-slate-200 py-3 text-sm font-bold text-slate-600 hover:bg-slate-50 transition-all"
                  >
                    ยกเลิก
                  </button>
                  <button
                    type="submit"
                    :disabled="loading"
                    class="flex-1 rounded-2xl bg-slate-900 py-3 text-sm font-bold text-white shadow-lg shadow-slate-200 transition-all hover:bg-slate-800 active:scale-[0.98] disabled:opacity-50"
                  >
                    {{ loading ? 'กำลังบันทึก...' : 'บันทึกข้อมูล' }}
                  </button>
                </div>
              </form>
            </DialogPanel>
          </TransitionChild>
        </div>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script setup lang="ts">
import { reactive, ref, watch } from 'vue'
import { useInventoryStore } from '@/stores/inventoryStore'
import { Dialog, DialogPanel, DialogTitle, TransitionRoot, TransitionChild } from '@headlessui/vue'
import type { Product } from '@/types/inventory'

const props = defineProps<{
  modelValue: boolean
  editingItem: Product | null
}>()

const emit = defineEmits(['update:modelValue', 'success'])
const store = useInventoryStore()

const loading = ref(false)
const error = ref('')

const form = reactive({
  name: '',
  sku: '',
  basePrice: 0,
  initialStock: 0,
  categoryId: '',
  description: '',
})

watch(
  () => props.modelValue,
  (isOpen) => {
    if (isOpen) {
      if (props.editingItem) {
        Object.assign(form, {
          name: props.editingItem.name,
          sku: props.editingItem.sku,
          basePrice: props.editingItem.basePrice || 0,
          categoryId: props.editingItem.categoryId || '',
          description: props.editingItem.description || '',
          initialStock: 0, // ไม่ต้องใช้ตอนแก้
        })
      } else {
        Object.assign(form, {
          name: '',
          sku: '',
          basePrice: 0,
          initialStock: 0,
          categoryId: '',
          description: '',
        })
      }
      error.value = ''
    }
  },
)

async function submit() {
  loading.value = true
  error.value = ''
  try {
    if (props.editingItem) {
      // สำหรับการ Update: มักไม่ส่ง SKU และ InitialStock คืนไป
      await store.updateProduct(props.editingItem.id, {
        name: form.name,
        BasePrice: form.basePrice,
        categoryId: form.categoryId,
        description: form.description,
      })
    } else {
      await store.createProduct({ ...form })
    }
    emit('success')
    emit('update:modelValue', false)
  } catch (e: unknown) {
    error.value = e instanceof Error ? e.message : 'เกิดข้อผิดพลาดในการบันทึกข้อมูล'
  } finally {
    loading.value = false
  }
}
</script>
