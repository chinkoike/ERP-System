<template>
  <Teleport to="body">
    <div
      v-if="modelValue"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 backdrop-blur-sm px-4"
      @click.self="$emit('update:modelValue', false)"
    >
      <div class="relative w-full max-w-md rounded-3xl bg-white p-6 shadow-2xl">
        <div class="flex items-center justify-between mb-6">
          <h2 class="text-xl font-bold text-slate-900">
            {{ editingItem ? 'แก้ไขหมวดหมู่' : 'เพิ่มหมวดหมู่ใหม่' }}
          </h2>
          <button
            @click="$emit('update:modelValue', false)"
            class="text-slate-400 hover:text-slate-600"
          >
            <span class="text-2xl">&times;</span>
          </button>
        </div>

        <form @submit.prevent="submit" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1">ชื่อหมวดหมู่</label>
            <input
              v-model="form.name"
              type="text"
              placeholder="เช่น อุปกรณ์ไอที, เครื่องใช้ไฟฟ้า"
              class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-2.5 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900"
              required
            />
          </div>

          <div>
            <label class="block text-sm font-medium text-slate-700 mb-1"
              >รายละเอียด (ไม่บังคับ)</label
            >
            <textarea
              v-model="form.description"
              rows="3"
              placeholder="ระบุรายละเอียดเพิ่มเติม..."
              class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-2.5 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900"
            ></textarea>
          </div>

          <div v-if="error" class="text-xs text-rose-600 bg-rose-50 p-3 rounded-xl">
            {{ error }}
          </div>

          <div class="flex gap-3 pt-2">
            <button
              type="submit"
              :disabled="loading"
              class="flex-1 rounded-2xl bg-slate-900 py-3 text-sm font-semibold text-white transition hover:bg-slate-800 disabled:opacity-50"
            >
              {{ loading ? 'กำลังบันทึก...' : 'บันทึกข้อมูล' }}
            </button>
            <button
              type="button"
              @click="$emit('update:modelValue', false)"
              class="flex-1 rounded-2xl border border-slate-200 py-3 text-sm font-semibold text-slate-600 hover:bg-slate-50"
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
  editingItem: { id: string; name: string; description: string } | null
}>()

const emit = defineEmits(['update:modelValue', 'success'])
const store = useInventoryStore()

const loading = ref(false)
const error = ref('')

const form = reactive({
  name: '',
  description: '',
})

watch(
  () => props.modelValue,
  (isOpen) => {
    if (isOpen) {
      if (props.editingItem) {
        form.name = props.editingItem.name
        form.description = props.editingItem.description || ''
      } else {
        form.name = ''
        form.description = ''
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
      await store.updateCategory(props.editingItem.id, { ...form })
    } else {
      await store.createCategory({ ...form })
    }
    emit('success')
    emit('update:modelValue', false)
  } catch (e: unknown) {
    error.value =
      (e as { response?: { data?: { message?: string } } }).response?.data?.message ||
      'เกิดข้อผิดพลาดในการบันทึกข้อมูล'
  } finally {
    loading.value = false
  }
}
</script>
