<template>
  <div>
    <h2 class="text-sm font-semibold text-slate-900 mb-5">{{ title }}</h2>

    <!-- Skeleton -->
    <template v-if="skeleton">
      <div :style="{ height: chartHeight + 'px' }" class="flex items-end gap-2">
        <div
          v-for="n in 6"
          :key="n"
          class="flex-1 rounded-t-lg bg-slate-100 animate-pulse"
          :style="{ height: `${30 + (n * 11) % 70}%` }"
        ></div>
      </div>
      <div class="flex gap-2 mt-2">
        <div v-for="n in 6" :key="n" class="flex-1 h-3 rounded bg-slate-100 animate-pulse"></div>
      </div>
    </template>

    <!-- Empty -->
    <div
      v-else-if="!data || data.length === 0"
      :style="{ height: chartHeight + 'px' }"
      class="flex items-center justify-center text-sm text-slate-400"
    >
      ไม่มีข้อมูล
    </div>

    <!-- Chart -->
    <template v-else>
      <div class="relative" :style="{ height: chartHeight + 'px' }">
        <!-- Y-axis gridlines -->
        <div class="absolute inset-0 flex flex-col justify-between pointer-events-none">
          <div
            v-for="(tick, idx) in yTicks"
            :key="idx"
            class="flex items-center gap-2 w-full"
          >
            <span class="text-[10px] text-slate-300 w-12 text-right shrink-0 tabular-nums">
              {{ formatTick(tick) }}
            </span>
            <div class="flex-1 border-t border-dashed border-slate-100"></div>
          </div>
        </div>

        <!-- Bars -->
        <div class="absolute inset-0 flex items-end gap-1.5 pl-14 pb-0">
          <div
            v-for="point in data"
            :key="point.label"
            class="relative flex flex-1 flex-col justify-end items-center h-full group"
          >
            <!-- Tooltip -->
            <div class="absolute bottom-full mb-1.5 left-1/2 -translate-x-1/2 z-10
              hidden group-hover:flex flex-col items-center pointer-events-none">
              <div class="rounded-xl bg-slate-900 px-2.5 py-1.5 text-center shadow-lg">
                <p class="text-[11px] font-semibold text-white whitespace-nowrap">
                  {{ formatValue(point.value) }}
                </p>
                <p class="text-[10px] text-slate-400">{{ point.label }}</p>
              </div>
              <div class="w-0 h-0 border-l-4 border-r-4 border-t-4
                border-l-transparent border-r-transparent border-t-slate-900"></div>
            </div>

            <!-- Bar -->
            <div
              class="w-full rounded-t-md transition-all duration-500 ease-out cursor-default"
              :class="barColor"
              :style="{ height: computedHeight(point.value) }"
            ></div>
          </div>
        </div>
      </div>

      <!-- X-axis labels -->
      <div class="flex gap-1.5 mt-2 pl-14">
        <div
          v-for="point in data"
          :key="point.label"
          class="flex-1 text-center text-[10px] text-slate-400 truncate"
        >
          {{ point.label }}
        </div>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { ReportDataPoint } from '@/types/report'

const props = withDefaults(defineProps<{
  title: string
  data: ReportDataPoint[]
  barColor?: string
  chartHeight?: number
  skeleton?: boolean
  currency?: boolean
}>(), {
  barColor: 'bg-slate-900 hover:bg-slate-700',
  chartHeight: 200,
  skeleton: false,
  currency: false,
})

const maxValue = computed(() => Math.max(...(props.data ?? []).map((p) => p.value), 1))

// 4 nice round ticks from max → 0
const yTicks = computed(() => {
  const step = maxValue.value / 4
  return [maxValue.value, step * 3, step * 2, step].map((v) => Math.round(v))
})

function computedHeight(value: number): string {
  // leave ~32px room at top for gridline labels (approx 16% of 200px)
  const usable = props.chartHeight - 16
  const pct = (value / maxValue.value) * usable
  return `${Math.max(pct, 3)}px`
}

function formatTick(v: number): string {
  if (!props.currency) return v >= 1000 ? `${(v / 1000).toFixed(0)}k` : String(v)
  if (v >= 1_000_000) return `${(v / 1_000_000).toFixed(1)}M`
  if (v >= 1_000) return `${(v / 1_000).toFixed(0)}k`
  return String(v)
}

function formatValue(v: number): string {
  if (!props.currency) return v.toLocaleString('th-TH')
  return new Intl.NumberFormat('th-TH', {
    style: 'currency',
    currency: 'THB',
    maximumFractionDigits: 0,
  }).format(v)
}
</script>
