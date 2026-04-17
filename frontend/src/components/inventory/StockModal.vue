<template>
  <Teleport to="body">
    <div v-if="modelValue" class="modal-overlay" @click.self="$emit('update:modelValue', false)">
      <div class="modal-content max-w-sm">
        <h2 class="mb-2 text-xl font-semibold text-slate-900">ปรับ Stock</h2>
        <p class="mb-4 text-sm text-slate-500">
          {{ target?.name }} (ปัจจุบัน: {{ target?.currentStock }})
        </p>
        <div class="space-y-3">
          <input
            v-model.number="form.quantityChange"
            type="number"
            placeholder="ปรับ (+ หรือ -)"
            class="input-field"
          />
          <textarea
            v-model="form.note"
            placeholder="หมายเหตุ"
            rows="2"
            class="input-field"
          ></textarea>
        </div>
        <p v-if="error" class="mt-2 text-xs text-rose-600">{{ error }}</p>
        <div class="mt-4 flex gap-2">
          <button @click="submit" :disabled="loading" class="btn-save">บันทึก</button>
          <button @click="$emit('update:modelValue', false)" class="btn-cancel">ยกเลิก</button>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useInventoryStore } from '@/stores/inventoryStore'

const props = defineProps(['modelValue', 'target'])
const emit = defineEmits(['update:modelValue', 'success'])
const store = useInventoryStore()

const loading = ref(false)
const error = ref('')
const form = reactive({ quantityChange: 0, note: '' })

async function submit() {
  if (props.target.currentStock + form.quantityChange < 0) {
    error.value = 'Stock ไม่สามารถติดลบได้'
    return
  }
  loading.value = true
  try {
    await store.updateStock(props.target.id, {
      productId: props.target.id,
      quantityChange: form.quantityChange,
      note: form.note,
    })
    emit('success')
    emit('update:modelValue', false)
  } catch (e: unknown) {
    error.value = (e as Error).message
  } finally {
    loading.value = false
  }
}
</script>
