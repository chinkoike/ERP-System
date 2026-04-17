<template>
  <div class="w-full">
    <div class="overflow-hidden rounded-3xl border border-slate-200 bg-white shadow-sm">
      <div
        v-for="i in rows"
        :key="i"
        class="flex items-center justify-between px-6 py-6 border-b border-slate-50 w-full"
      >
        <div class="flex-1 pr-10">
          <BaseSkeleton width="80%" height="20px" class="mb-2" />
          <BaseSkeleton width="40%" height="14px" opacity-50 />
        </div>

        <div class="flex items-center gap-8">
          <BaseSkeleton width="80px" height="20px" />
          <BaseSkeleton width="100px" height="28px" rounded="full" />
          <div class="flex gap-2">
            <BaseSkeleton width="40px" height="40px" rounded="xl" />
            <BaseSkeleton width="40px" height="40px" rounded="xl" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import BaseSkeleton from '../BaseSkeleton.vue'
defineProps({ rows: { type: Number, default: 5 } })
</script>
