<template>
  <div
    :class="[
      'animate-pulse bg-slate-200/40', // ปรับค่าความบางที่นี่ (/40 คือ 40%)
      roundedClass,
      customClass,
    ]"
    :style="{
      width: width,
      height: height,
      minHeight: height,
      animationDuration: '2s', // ปรับให้กระพริบช้าลงดูหรูขึ้น
    }"
  ></div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = withDefaults(
  defineProps<{
    width?: string
    height?: string
    circle?: boolean
    rounded?: 'sm' | 'md' | 'lg' | 'xl' | '2xl' | '3xl' | 'full'
    customClass?: string
  }>(),
  {
    width: '100%',
    height: '20px',
    rounded: 'md',
  },
)

const roundedClass = computed(() => {
  if (props.circle) return 'rounded-full'
  return `rounded-${props.rounded}`
})
</script>
