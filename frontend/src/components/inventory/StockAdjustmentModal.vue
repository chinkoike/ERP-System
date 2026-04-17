<template>
  <Teleport to="body">
    <div
      v-if="modelValue"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 backdrop-blur-sm px-4"
      @click.self="$emit('update:modelValue', false)"
    >
      <div
        class="relative w-full max-w-sm rounded-3xl bg-white p-6 shadow-2xl border border-slate-100"
      >
        <div class="mb-5 text-center">
          <div
            class="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-slate-100"
          >
            <span class="text-xl">📦</span>
          </div>
          <h2 class="text-xl font-bold text-slate-900">ปรับสต็อกสินค้า</h2>
          <p class="text-sm text-slate-500 mt-1">{{ target?.name }}</p>
          <div class="mt-2 inline-flex items-center gap-2 rounded-lg bg-slate-50 px-3 py-1">
            <span class="text-xs text-slate-500">คงเหลือปัจจุบัน:</span>
            <span class="text-sm font-bold text-slate-900">{{ target?.currentStock }}</span>
          </div>
        </div>

        <form @submit.prevent="submit" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1 text-center"
              >จำนวนที่ต้องการปรับ</label
            >
            <div class="flex items-center gap-4">
              <button
                type="button"
                @click="form.quantityChange--"
                class="flex h-10 w-10 items-center justify-center rounded-xl border border-slate-200 hover:bg-slate-50"
              >
                -
              </button>
              <input
                v-model.number="form.quantityChange"
                type="number"
                class="block w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 text-center text-lg font-bold outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900"
              />
              <button
                type="button"
                @click="form.quantityChange++"
                class="flex h-10 w-10 items-center justify-center rounded-xl border border-slate-200 hover:bg-slate-50"
              >
                +
              </button>
            </div>
            <p class="mt-2 text-center text-[11px] text-slate-400">ใส่ค่าลบเพื่อลดสต็อก เช่น -5</p>
          </div>

          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">หมายเหตุ / เหตุผล</label>
            <textarea
              v-model="form.note"
              rows="2"
              placeholder="เช่น นำเข้าสินค้าใหม่, สินค้าชำรุด..."
              class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-2.5 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900"
            ></textarea>
          </div>

          <div v-if="error" class="text-xs text-rose-600 bg-rose-50 p-3 rounded-xl text-center">
            {{ error }}
          </div>

          <div class="flex flex-col gap-2 pt-2">
            <button
              type="submit"
              :disabled="loading || form.quantityChange === 0"
              class="w-full rounded-2xl bg-slate-900 py-3 text-sm font-semibold text-white transition hover:bg-slate-800 disabled:opacity-50"
            >
              {{ loading ? 'กำลังอัปเดต...' : 'ยืนยันการปรับสต็อก' }}
            </button>
            <button
              type="button"
              @click="$emit('update:modelValue', false)"
              class="w-full rounded-2xl border border-slate-200 py-3 text-sm font-semibold text-slate-600 hover:bg-slate-50"
            >
              ยกเลิก
            </button>
          </div>
        </form>
      </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { reactive, ref, watch } from 'vue'
import { useInventoryStore } from '@/stores/inventoryStore'

const props = defineProps<{
  modelValue: boolean
  target: { id: string; name: string; currentStock: number }
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
  if (props.target && props.target.currentStock + form.quantityChange < 0) {
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
    error.value =
      (e as { response?: { data?: { message?: string } } }).response?.data?.message ||
      'ไม่สามารถปรับสต็อกได้'
  } finally {
    loading.value = false
  }
}
</script>
