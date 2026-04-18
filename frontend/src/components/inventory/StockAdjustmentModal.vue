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
              class="w-full max-w-sm transform overflow-hidden rounded-3xl bg-white p-6 text-left align-middle shadow-2xl transition-all"
            >
              <div class="mb-5 text-center">
                <div
                  class="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-slate-100 text-xl"
                >
                  📦
                </div>
                <DialogTitle as="h3" class="text-xl font-bold text-slate-900">
                  ปรับสต็อกสินค้า
                </DialogTitle>
                <p class="text-sm text-slate-500 mt-1">{{ target?.name }}</p>

                <div
                  class="mt-3 inline-flex items-center gap-2 rounded-xl bg-slate-50 px-4 py-1.5 border border-slate-100"
                >
                  <span class="text-xs font-medium text-slate-500">คงเหลือปัจจุบัน:</span>
                  <span class="text-sm font-bold text-slate-900">{{ target?.currentStock }}</span>
                </div>
              </div>

              <form @submit.prevent="submit" class="space-y-5">
                <div>
                  <label class="block text-sm font-semibold text-slate-700 mb-2 text-center">
                    จำนวนที่ต้องการปรับ
                  </label>
                  <div class="flex items-center gap-3">
                    <button
                      type="button"
                      @click="form.quantityChange--"
                      class="flex h-12 w-12 items-center justify-center rounded-2xl border border-slate-200 bg-white text-xl font-bold hover:bg-slate-50 active:bg-slate-100 transition-colors"
                    >
                      −
                    </button>
                    <input
                      v-model.number="form.quantityChange"
                      type="number"
                      class="block flex-1 rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 text-center text-xl font-bold outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900"
                    />
                    <button
                      type="button"
                      @click="form.quantityChange++"
                      class="flex h-12 w-12 items-center justify-center rounded-2xl border border-slate-200 bg-white text-xl font-bold hover:bg-slate-50 active:bg-slate-100 transition-colors"
                    >
                      +
                    </button>
                  </div>
                  <p class="mt-2 text-center text-[11px] text-slate-400">
                    ใส่ค่าลบเพื่อลดสต็อก เช่น <span class="text-rose-500 font-medium">-5</span>
                  </p>
                </div>

                <div>
                  <label class="block text-xs font-semibold text-slate-500 mb-1.5 ml-1"
                    >หมายเหตุ / เหตุผล</label
                  >
                  <textarea
                    v-model="form.note"
                    rows="2"
                    placeholder="เช่น นำเข้าสินค้าใหม่, สินค้าชำรุด..."
                    class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900 resize-none"
                  ></textarea>
                </div>

                <div
                  v-if="error"
                  class="text-xs text-rose-600 bg-rose-50 p-3 rounded-xl border border-rose-100 text-center"
                >
                  {{ error }}
                </div>

                <div class="flex flex-col gap-2 pt-2">
                  <button
                    type="submit"
                    :disabled="loading || form.quantityChange === 0"
                    class="w-full rounded-2xl bg-slate-900 py-3.5 text-sm font-bold text-white shadow-lg shadow-slate-200 transition-all hover:bg-slate-800 active:scale-[0.98] disabled:opacity-50"
                  >
                    {{ loading ? 'กำลังอัปเดต...' : 'ยืนยันการปรับสต็อก' }}
                  </button>
                  <button
                    type="button"
                    @click="$emit('update:modelValue', false)"
                    class="w-full rounded-2xl border border-slate-200 py-3.5 text-sm font-bold text-slate-600 hover:bg-slate-50 transition-colors"
                  >
                    ยกเลิก
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

const props = defineProps<{
  modelValue: boolean
  target: { id: string; name: string; currentStock: number } | null
}>()

const emit = defineEmits(['update:modelValue', 'success'])
const store = useInventoryStore()

const loading = ref(false)
const error = ref('')

const form = reactive({
  quantityChange: 0,
  note: '',
})

watch(
  () => props.modelValue,
  (isOpen) => {
    if (isOpen) {
      form.quantityChange = 0
      form.note = ''
      error.value = ''
    }
  },
)

async function submit() {
  if (!props.target) return

  // Check valid result (cannot be negative)
  if (props.target.currentStock + form.quantityChange < 0) {
    error.value = 'จำนวนสินค้าคงเหลือไม่สามารถติดลบได้'
    return
  }

  loading.value = true
  error.value = ''
  try {
    await store.updateStock(props.target.id, {
      productId: props.target.id,
      quantityChange: form.quantityChange,
      note: form.note,
    })
    emit('success')
    emit('update:modelValue', false)
  } catch (e: unknown) {
    error.value = e instanceof Error ? e.message : 'เกิดข้อผิดพลาดในการบันทึกข้อมูล'
  } finally {
    loading.value = false
  }
}
</script>
