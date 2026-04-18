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
                <DialogTitle as="h3" class="text-lg font-semibold leading-6 text-slate-900">
                  {{ editingItem ? 'แก้ไขหมวดหมู่' : 'เพิ่มหมวดหมู่ใหม่' }}
                </DialogTitle>
                <button
                  type="button"
                  @click="$emit('update:modelValue', false)"
                  class="rounded-full p-1 text-slate-400 hover:bg-slate-100 hover:text-slate-600 transition-colors"
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-6 w-6"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M6 18L18 6M6 6l12 12"
                    />
                  </svg>
                </button>
              </div>

              <form @submit.prevent="submit" class="space-y-5">
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-1.5"
                    >ชื่อหมวดหมู่</label
                  >
                  <input
                    v-model="form.name"
                    type="text"
                    placeholder="เช่น อุปกรณ์ไอที, เครื่องใช้ไฟฟ้า"
                    class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none transition-focus focus:border-slate-900 focus:ring-1 focus:ring-slate-900"
                    required
                  />
                </div>

                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-1.5"
                    >รายละเอียด (ไม่บังคับ)</label
                  >
                  <textarea
                    v-model="form.description"
                    rows="3"
                    placeholder="ระบุรายละเอียดเพิ่มเติม..."
                    class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm outline-none transition-focus focus:border-slate-900 focus:ring-1 focus:ring-slate-900 resize-none"
                  ></textarea>
                </div>

                <div
                  v-if="error"
                  class="text-xs text-rose-600 bg-rose-50 p-3 rounded-xl border border-rose-100"
                >
                  {{ error }}
                </div>

                <div class="flex gap-3 pt-2">
                  <button
                    type="button"
                    @click="$emit('update:modelValue', false)"
                    class="flex-1 rounded-2xl border border-slate-200 py-3 text-sm font-bold text-slate-600 hover:bg-slate-50 transition-colors"
                  >
                    ยกเลิก
                  </button>
                  <button
                    type="submit"
                    :disabled="loading"
                    class="flex-1 rounded-2xl bg-slate-900 py-3 text-sm font-bold text-white shadow-lg shadow-slate-200 transition-all hover:bg-slate-800 active:scale-[0.98] disabled:opacity-50"
                  >
                    <span v-if="loading" class="flex items-center justify-center gap-2">
                      <svg
                        class="animate-spin h-4 w-4 text-white"
                        xmlns="http://www.w3.org/2000/svg"
                        fill="none"
                        viewBox="0 0 24 24"
                      >
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
                      กำลังบันทึก...
                    </span>
                    <span v-else>บันทึกข้อมูล</span>
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
