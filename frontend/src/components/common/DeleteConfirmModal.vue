<template>
  <Teleport to="body">
    <div
      v-if="modelValue"
      class="fixed inset-0 z-60 flex items-center justify-center bg-black/40 backdrop-blur-sm px-4"
      @click.self="$emit('update:modelValue', false)"
    >
      <div
        class="relative w-full max-w-sm rounded-3xl bg-white p-6 shadow-2xl border border-slate-100 text-center animate-in fade-in zoom-in duration-200"
      >
        <div
          class="mx-auto mb-4 flex h-14 w-14 items-center justify-center rounded-full bg-rose-50"
        >
          <div
            class="flex h-10 w-10 items-center justify-center rounded-full bg-rose-100 text-rose-600"
          >
            <span class="text-xl">🗑️</span>
          </div>
        </div>

        <h2 class="mb-2 text-xl font-bold text-slate-900">{{ title }}</h2>
        <p class="mb-6 text-sm text-slate-500 leading-relaxed">
          {{ description }} <br v-if="itemName" />
          <span v-if="itemName" class="font-bold text-slate-900">"{{ itemName }}"</span>
        </p>

        <div class="flex gap-3">
          <button
            @click="$emit('confirm')"
            :disabled="loading"
            class="flex-1 rounded-2xl bg-rose-600 py-3 text-sm font-semibold text-white transition hover:bg-rose-700 active:scale-95 disabled:opacity-50 disabled:pointer-events-none"
          >
            {{ loading ? 'กำลังลบ...' : confirmText }}
          </button>
          <button
            @click="$emit('update:modelValue', false)"
            :disabled="loading"
            class="flex-1 rounded-2xl border border-slate-200 py-3 text-sm font-semibold text-slate-600 transition hover:bg-slate-50 active:scale-95"
          >
            {{ cancelText }}
          </button>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
// ปรับ Props ให้เป็นแบบ Generic เพื่อรองรับทุกหน้า
interface Props {
  modelValue: boolean
  title?: string
  description?: string
  itemName?: string | null
  confirmText?: string
  cancelText?: string
  loading?: boolean
}

withDefaults(defineProps<Props>(), {
  title: 'ยืนยันการลบ',
  description: 'คุณแน่ใจหรือไม่ที่จะลบรายการนี้? การกระทำนี้ไม่สามารถย้อนคืนได้',
  itemName: '',
  confirmText: 'ลบข้อมูล',
  cancelText: 'ยกเลิก',
  loading: false,
})

defineEmits(['update:modelValue', 'confirm'])
</script>
