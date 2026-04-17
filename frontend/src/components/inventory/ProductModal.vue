<template>
  <Teleport to="body">
    <div
      v-if="modelValue"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 backdrop-blur-sm px-4"
      @click.self="$emit('update:modelValue', false)"
    >
      <div
        class="relative w-full max-w-md rounded-3xl bg-white p-6 shadow-2xl border border-slate-100"
      >
        <h2 class="mb-5 text-xl font-bold text-slate-900">
          {{ editingItem ? 'แก้ไขสินค้า' : 'เพิ่มสินค้าใหม่' }}
        </h2>

        <div class="space-y-4">
          <div>
            <label class="block text-xs font-medium text-slate-500 mb-1 ml-1">ชื่อสินค้า</label>
            <input
              v-model="form.name"
              placeholder="เช่น เสื้อยืด Oversize"
              class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-2.5 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900"
            />
          </div>

          <div class="grid grid-cols-2 gap-3">
            <div>
              <label class="block text-xs font-medium text-slate-500 mb-1 ml-1">SKU</label>
              <input
                v-model="form.sku"
                placeholder="SKU-001"
                :disabled="!!editingItem"
                class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-2.5 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900 disabled:opacity-50 disabled:bg-slate-100"
              />
            </div>
            <div>
              <label class="block text-xs font-medium text-slate-500 mb-1 ml-1">ราคา (฿)</label>
              <input
                v-model.number="form.basePrice"
                type="number"
                placeholder="0.00"
                class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-2.5 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900"
              />
            </div>
          </div>

          <div v-if="!editingItem">
            <label class="block text-xs font-medium text-slate-500 mb-1 ml-1">สต็อกเริ่มต้น</label>
            <input
              v-model.number="form.initialStock"
              type="number"
              placeholder="0"
              class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-2.5 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900"
            />
          </div>

          <div>
            <label class="block text-xs font-medium text-slate-500 mb-1 ml-1">หมวดหมู่</label>
            <select
              v-model="form.categoryId"
              class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-2.5 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900 appearance-none"
            >
              <option value="">-- เลือกหมวดหมู่ --</option>
              <option v-for="c in store.categories" :key="c.id" :value="c.id">{{ c.name }}</option>
            </select>
          </div>

          <div>
            <label class="block text-xs font-medium text-slate-500 mb-1 ml-1">คำอธิบาย</label>
            <textarea
              v-model="form.description"
              placeholder="รายละเอียดสินค้า..."
              rows="2"
              class="w-full rounded-2xl border border-slate-200 bg-slate-50 px-4 py-2.5 text-sm outline-none focus:border-slate-900 focus:ring-1 focus:ring-slate-900"
            ></textarea>
          </div>
        </div>

        <p v-if="error" class="mt-4 text-xs text-rose-600 bg-rose-50 p-2.5 rounded-xl text-center">
          {{ error }}
        </p>

        <div class="mt-6 flex gap-2">
          <button
            @click="submit"
            :disabled="loading"
            class="flex-1 rounded-2xl bg-slate-900 py-3 text-sm font-semibold text-white transition hover:bg-slate-800 disabled:opacity-50"
          >
            {{ loading ? 'กำลังบันทึก...' : 'บันทึกข้อมูล' }}
          </button>
          <button
            @click="$emit('update:modelValue', false)"
            class="flex-1 rounded-2xl border border-slate-200 py-3 text-sm font-semibold text-slate-600 transition hover:bg-slate-50"
          >
            ยกเลิก
          </button>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { reactive, ref, watch } from 'vue'
import { useInventoryStore } from '@/stores/inventoryStore'
import type { Product } from '@/types/inventory'

// เพิ่มการระบุ Type ให้ชัดเจน
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

// คอยดูว่า Modal เปิดหรือปิด เพื่อล้างข้อมูลหรือใส่ข้อมูลแก้ไข
watch(
  () => props.modelValue,
  (isOpen) => {
    if (isOpen) {
      if (props.editingItem) {
        // ก๊อปปี้ค่าจาก editingItem มาใส่ฟอร์ม
        Object.assign(form, {
          ...props.editingItem,
          basePrice: props.editingItem.basePrice || 0,
        })
      } else {
        // Reset ฟอร์มเป็นค่าว่าง
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
      // สร้าง Object ใหม่เพื่อส่งไป โดยเปลี่ยน b เป็น B ให้ตรงกับ Type
      await store.updateProduct(props.editingItem.id, {
        name: form.name,
        BasePrice: form.basePrice, // แก้จุดนี้ให้เป็นตัวใหญ่
        categoryId: form.categoryId,
        description: form.description,
        // ปกติ Update มักไม่ส่ง initialStock ไปด้วย
      })
    } else {
      await store.createProduct({
        ...form,
        basePrice: form.basePrice, // เพิ่ม/ทับ ค่าเดิมด้วยชื่อที่ถูกต้อง
      })
    }
    emit('success')
    emit('update:modelValue', false)
  } catch (e: unknown) {
    error.value =
      (e as { response?: { data?: { message?: string } }; message?: string }).response?.data
        ?.message ||
      (e as { message?: string }).message ||
      'เกิดข้อผิดพลาดในการบันทึก'
  } finally {
    loading.value = false
  }
}
</script>
