<template>
  <div class="min-h-screen bg-slate-50 font-sans">
    <!-- Top bar -->

    <main class="px-8 py-8 max-w-7xl mx-auto">
      <!-- Heading -->
      <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between mb-6">
        <div>
          <h1 class="text-3xl font-semibold tracking-tight text-slate-900">Sales</h1>
          <p class="text-sm text-slate-500 mt-2">จัดการ Order และลูกค้า</p>
        </div>
        <button
          v-if="activeTab === 'orders'"
          @click="openOrderModal()"
          class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
        >
          <span class="text-lg leading-none">+</span> สร้าง Order
        </button>
        <button
          v-if="activeTab === 'customers'"
          @click="openCustomerModal()"
          class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
        >
          <span class="text-lg leading-none">+</span> เพิ่มลูกค้า
        </button>
      </div>

      <!-- Tabs -->
      <div class="inline-flex items-center rounded-2xl border border-slate-200 bg-white p-1 mb-6">
        <button
          v-for="tab in tabs"
          :key="tab.key"
          @click="activeTab = tab.key"
          :class="
            activeTab === tab.key
              ? 'rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white'
              : 'rounded-2xl px-4 py-2 text-sm font-medium text-slate-500 hover:text-slate-900'
          "
          class="transition"
        >
          {{ tab.label }}
        </button>
      </div>

      <!-- ORDERS TAB -->
      <div v-if="activeTab === 'orders'">
        <!-- Filter row -->
        <div class="flex items-center gap-3 mb-4">
          <div class="relative flex-1 max-w-md">
            <svg
              class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0"
              />
            </svg>
            <input
              v-model="searchOrder"
              type="text"
              placeholder="ค้นหา order, ลูกค้า..."
              class="w-full rounded-2xl border border-slate-200 bg-white py-2 pl-10 pr-4 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
            />
          </div>
          <select
            v-model="filterStatus"
            class="rounded-2xl border border-slate-200 bg-white py-2 px-4 text-sm text-slate-700 outline-none cursor-pointer focus:ring-2 focus:ring-slate-300"
          >
            <option value="">ทุกสถานะ</option>
            <option v-for="s in orderStatuses" :key="s.value" :value="s.value">
              {{ s.label }}
            </option>
          </select>
        </div>

        <div class="overflow-x-auto rounded-2xl border border-slate-200 bg-white">
          <table class="min-w-full border-collapse">
            <thead>
              <tr class="border-b border-slate-100">
                <th
                  class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  Order
                </th>
                <th
                  class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  ลูกค้า
                </th>
                <th
                  class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  วันที่
                </th>
                <th
                  class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  รายการ
                </th>
                <th
                  class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  ยอดรวม
                </th>
                <th
                  class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  สถานะ
                </th>
                <th class="px-6 py-3"></th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="filteredOrders.length === 0">
                <td colspan="7" class="px-6 py-20 text-center text-sm text-slate-400">
                  ไม่พบ order
                </td>
              </tr>
              <tr
                v-for="(o, i) in filteredOrders"
                :key="o.orderId"
                :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
              >
                <td class="px-6 py-4 text-sm font-mono font-semibold text-slate-900">
                  {{ o.orderNumber }}
                </td>
                <td class="px-6 py-4 text-sm text-slate-600">
                  {{ o.customerName || '—' }}
                </td>
                <td class="px-6 py-4 text-sm text-slate-500">
                  {{ formatDate(o.orderDate) }}
                </td>
                <td class="px-6 py-4 text-center text-sm text-slate-600">
                  {{ o.itemCount }}
                </td>
                <td class="px-6 py-4 text-right text-sm font-semibold text-slate-900">
                  {{ formatCurrency(o.totalAmount) }}
                </td>
                <td class="px-6 py-4 text-center">
                  <span :class="statusBadgeClass(o.status)">{{ statusLabel(o.status) }}</span>
                </td>
                <td class="px-6 py-4">
                  <div class="flex items-center gap-2 justify-end">
                    <select
                      v-if="o.status !== 'Cancelled' && o.status !== 'Delivered'"
                      @change="
                        handleStatusChange(o.orderId, ($event.target as HTMLSelectElement).value)
                      "
                      class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-xs font-medium text-slate-600 outline-none transition hover:border-slate-300"
                    >
                      <option value="">เปลี่ยนสถานะ</option>
                      <option v-for="s in nextStatuses(o.status)" :key="s.value" :value="s.value">
                        {{ s.label }}
                      </option>
                    </select>
                    <button
                      v-if="o.status === 'Pending'"
                      @click="confirmCancel(o)"
                      class="rounded-2xl border border-rose-200 bg-white px-3 py-2 text-xs font-medium text-rose-600 transition hover:bg-rose-50"
                    >
                      ยกเลิก
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="flex items-center justify-between bg-white px-6 py-4 text-sm text-slate-500">
          <div>แสดง {{ store.orders.length }} จาก {{ store.totalItems }} รายการ</div>
          <div class="flex items-center gap-2">
            <button
              @click="loadOrders(store.currentPage - 1)"
              :disabled="store.currentPage <= 1"
              class="rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-600 disabled:cursor-not-allowed disabled:opacity-50"
            >
              ก่อนหน้า
            </button>
            <span>หน้า {{ store.currentPage }} / {{ store.totalPages }}</span>
            <button
              @click="loadOrders(store.currentPage + 1)"
              :disabled="store.currentPage >= store.totalPages"
              class="rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-600 disabled:cursor-not-allowed disabled:opacity-50"
            >
              ถัดไป
            </button>
          </div>
        </div>
      </div>

      <!-- CUSTOMERS TAB -->
      <div v-if="activeTab === 'customers'">
        <div class="relative max-w-md mb-4">
          <svg
            class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0"
            />
          </svg>
          <input
            v-model="searchCustomer"
            type="text"
            placeholder="ค้นหาลูกค้า..."
            class="w-full rounded-2xl border border-slate-200 bg-white py-2 pl-10 pr-4 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
          />
        </div>

        <div class="overflow-hidden rounded-2xl border border-slate-200 bg-white">
          <table class="min-w-full border-collapse">
            <thead>
              <tr class="border-b border-slate-100">
                <th
                  class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  ชื่อ
                </th>
                <th
                  class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  Email
                </th>
                <th
                  class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  เบอร์โทร
                </th>
                <th
                  class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                >
                  ที่อยู่
                </th>
                <th class="px-6 py-3"></th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="filteredCustomers.length === 0">
                <td colspan="5" class="px-6 py-12 text-center text-sm text-slate-400">
                  ไม่พบลูกค้า
                </td>
              </tr>
              <tr
                v-for="(c, i) in filteredCustomers"
                :key="c.id"
                :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
              >
                <td class="px-6 py-4">
                  <div class="text-sm font-semibold text-slate-900">
                    {{ c.fullName }}
                  </div>
                </td>
                <td class="px-6 py-4 text-sm text-slate-600">
                  {{ c.email ?? '—' }}
                </td>
                <td class="px-6 py-4 text-sm text-slate-600">
                  {{ c.phone ?? '—' }}
                </td>
                <td class="px-6 py-4 max-w-50 truncate text-sm text-slate-500">
                  {{ c.address ?? '—' }}
                </td>
                <td class="px-6 py-4">
                  <div class="flex items-center gap-2 justify-end">
                    <button
                      @click="openCustomerModal(c)"
                      class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                    >
                      แก้ไข
                    </button>
                    <button
                      @click="confirmDeleteCustomer(c)"
                      class="rounded-2xl border border-rose-200 bg-white px-3 py-2 text-xs font-medium text-rose-600 transition hover:bg-rose-50"
                    >
                      ลบ
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="flex items-center justify-between bg-white px-6 py-4 text-sm text-slate-500">
          <div>แสดง {{ store.customers.length }} จาก {{ store.customerTotal }} รายการ</div>
          <div class="flex items-center gap-2">
            <button
              @click="loadCustomers(store.currentPage - 1)"
              :disabled="store.currentPage <= 1"
              class="rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-600 disabled:cursor-not-allowed disabled:opacity-50"
            >
              ก่อนหน้า
            </button>
            <span>หน้า {{ store.currentPage }} / {{ store.customerPages }}</span>
            <button
              @click="loadOrders(store.currentPage + 1)"
              :disabled="store.currentPage >= store.totalPages"
              class="rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-600 disabled:cursor-not-allowed disabled:opacity-50"
            >
              ถัดไป
            </button>
          </div>
        </div>
      </div>
    </main>

    <!-- ===== CREATE ORDER MODAL ===== -->
    <Teleport to="body">
      <div
        v-if="showOrderModal"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-2xl max-h-[90vh] overflow-y-auto rounded-2xl bg-white shadow-2xl">
          <div
            class="sticky top-0 z-10 flex items-center justify-between border-b border-slate-200 bg-white px-6 py-5"
          >
            <span class="text-base font-semibold text-slate-900">สร้าง Order ใหม่</span>
            <button
              @click="showOrderModal = false"
              class="text-slate-500 hover:text-slate-700 text-2xl"
            >
              ×
            </button>
          </div>
          <div class="space-y-4 p-6">
            <!-- Customer -->
            <div>
              <label class="mb-2 block text-sm font-medium text-slate-600">ลูกค้า *</label>
              <select
                v-model="orderForm.customerId"
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              >
                <option value="">เลือกลูกค้า</option>
                <option v-for="c in store.customers" :key="c.id" :value="c.id">
                  {{ c.fullName }}
                </option>
              </select>
            </div>

            <!-- Shipping address -->
            <div>
              <label class="mb-2 block text-sm font-medium text-slate-600">ที่อยู่จัดส่ง *</label>
              <input
                v-model="orderForm.shippingAddress"
                type="text"
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>

            <!-- Items -->
            <div>
              <div class="flex items-center justify-between mb-2">
                <label class="text-sm font-medium text-slate-600">รายการสินค้า *</label>
                <button
                  @click="addOrderItem"
                  class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-xs font-medium text-slate-900 transition hover:bg-slate-50"
                >
                  + เพิ่ม
                </button>
              </div>
              <div
                v-for="(item, idx) in orderForm.items"
                :key="idx"
                class="grid grid-cols-[2fr_1fr_1fr_auto] gap-2 mb-2 items-center"
              >
                <select
                  v-model="item.productId"
                  @change="onProductSelect(idx)"
                  class="rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                >
                  <option value="">เลือกสินค้า</option>
                  <option v-for="p in products" :key="p.id" :value="p.id">{{ p.name }}</option>
                </select>
                <input
                  v-model.number="item.quantity"
                  @input="calcItemTotal(idx)"
                  type="number"
                  min="1"
                  placeholder="จำนวน"
                  class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 text-center outline-none focus:ring-2 focus:ring-slate-300"
                />
                <div
                  class="rounded-2xl border border-slate-200 bg-slate-50 px-3 py-2 text-right text-sm text-slate-500"
                >
                  {{ formatCurrency(item.totalPrice) }}
                </div>
                <button
                  @click="removeOrderItem(idx)"
                  class="text-slate-400 hover:text-slate-600 text-lg px-1"
                >
                  ×
                </button>
              </div>
              <!-- Total -->
              <div
                v-if="orderForm.items.length > 0"
                class="mt-2 border-t border-slate-200 pt-2 text-right"
              >
                <span class="text-sm text-slate-500">ยอดรวม: </span>
                <span class="text-base font-semibold text-slate-900">{{
                  formatCurrency(orderTotal)
                }}</span>
              </div>
            </div>

            <!-- Remarks -->
            <div>
              <label class="mb-2 block text-sm font-medium text-slate-600">หมายเหตุ</label>
              <input
                v-model="orderForm.remarks"
                type="text"
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>

            <div v-if="modalError" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">
              {{ modalError }}
            </div>
          </div>
          <div
            class="sticky bottom-0 z-10 flex justify-end gap-2 border-t border-slate-200 bg-white p-4"
          >
            <button
              @click="showOrderModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="submitOrder"
              :disabled="modalLoading"
              class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-50"
            >
              {{ modalLoading ? 'กำลังสร้าง...' : 'สร้าง Order' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- ===== CUSTOMER MODAL ===== -->
    <Teleport to="body">
      <div
        v-if="showCustomerModal"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-xl overflow-hidden rounded-2xl bg-white shadow-2xl">
          <div class="flex items-center justify-between border-b border-slate-200 px-6 py-5">
            <span class="text-base font-semibold text-slate-900">{{
              editingCustomer ? 'แก้ไขลูกค้า' : 'เพิ่มลูกค้า'
            }}</span>
            <button
              @click="showCustomerModal = false"
              class="text-slate-500 hover:text-slate-700 text-2xl"
            >
              ×
            </button>
          </div>
          <div class="space-y-4 p-6">
            <div class="grid grid-cols-1 gap-3 md:grid-cols-2">
              <div>
                <label class="mb-2 block text-sm font-medium text-slate-600">ชื่อ *</label>
                <input
                  v-model="customerForm.firstName"
                  type="text"
                  class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
              <div>
                <label class="mb-2 block text-sm font-medium text-slate-600">นามสกุล</label>
                <input
                  v-model="customerForm.lastName"
                  type="text"
                  class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
            </div>
            <div>
              <label class="mb-2 block text-sm font-medium text-slate-600">Email *</label>
              <input
                v-model="customerForm.email"
                type="email"
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>
            <div>
              <label class="mb-2 block text-sm font-medium text-slate-600">เบอร์โทร</label>
              <input
                v-model="customerForm.phone"
                type="text"
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>
            <div>
              <label class="mb-2 block text-sm font-medium text-slate-600">ที่อยู่</label>
              <textarea
                v-model="customerForm.address"
                rows="2"
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 px-3 text-sm text-slate-900 outline-none resize-vertical focus:ring-2 focus:ring-slate-300"
              ></textarea>
            </div>
            <div v-if="modalError" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">
              {{ modalError }}
            </div>
          </div>
          <div class="flex justify-end gap-2 border-t border-slate-200 bg-slate-50 px-4 py-4">
            <button
              @click="showCustomerModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="submitCustomer"
              :disabled="modalLoading"
              class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-50"
            >
              {{ modalLoading ? 'กำลังบันทึก...' : 'บันทึก' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- ===== CONFIRM MODAL ===== -->
    <Teleport to="body">
      <div
        v-if="showConfirmModal"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-sm rounded-2xl bg-white p-6 shadow-2xl">
          <div class="mb-2 text-base font-semibold text-slate-900">
            {{ confirmTitle }}
          </div>
          <div class="mb-6 text-sm text-slate-500">
            {{ confirmMessage }}
          </div>
          <div class="flex justify-end gap-2">
            <button
              @click="showConfirmModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="confirmAction"
              :disabled="modalLoading"
              class="rounded-2xl bg-rose-600 px-5 py-2 text-sm font-medium text-white transition hover:bg-rose-700 disabled:cursor-not-allowed disabled:opacity-50"
            >
              {{ modalLoading ? 'กำลังดำเนินการ...' : confirmBtn }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted, watch } from 'vue'
import { useSalesStore } from '@/stores/salesStore'
import { useInventoryStore } from '@/stores/inventoryStore'
import type { Customer, OrderStatus } from '@/types/sales'

const store = useSalesStore()
const inventoryStore = useInventoryStore()

const activeTab = ref<'orders' | 'customers'>('orders')
const tabs = [
  { key: 'orders', label: 'Orders' },
  { key: 'customers', label: 'ลูกค้า' },
] as const

// products สำหรับ dropdown ใน order form
const products = computed(() => inventoryStore.products)

const orderStatuses = [
  { value: 'Pending', label: 'รอยืนยัน' },
  { value: 'Confirmed', label: 'ยืนยันแล้ว' },
  { value: 'Processing', label: 'กำลังเตรียม' },
  { value: 'Shipped', label: 'ส่งแล้ว' },
  { value: 'Delivered', label: 'ถึงมือลูกค้า' },
  { value: 'Cancelled', label: 'ยกเลิก' },
]

const statusMap: Record<string, number> = {
  Pending: 0,
  Confirmed: 1,
  Processing: 2,
  Shipped: 3,
  Delivered: 4,
  Cancelled: 5,
}

function nextStatuses(current: OrderStatus) {
  const flow: Record<string, string[]> = {
    Pending: ['Confirmed', 'Processing'],
    Confirmed: ['Processing'],
    Processing: ['Shipped'],
    Shipped: ['Delivered'],
  }
  return (flow[current] ?? []).map((v) => orderStatuses.find((s) => s.value === v)!)
}

// --- Filter ---
const searchOrder = ref('')
const filterStatus = ref('')
const filteredOrders = computed(() => store.orders)
//load orders on mount and when filters change
async function loadOrders(page = 1) {
  await store.fetchOrders({
    searchTerm: searchOrder.value.trim() || undefined,
    status: filterStatus.value || undefined,
    pageNumber: page,
  })
}

watch([searchOrder, filterStatus], () => loadOrders(1))

//load customers on mount
const searchCustomer = ref('')
const filteredCustomers = computed(() => store.customers)
async function loadCustomers(page = 1) {
  await store.fetchCustomers({
    searchTerm: searchCustomer.value.trim() || undefined,
    pageNumber: page,
    pageSize: 100, // load all customers for dropdown
  })
}
watch(searchCustomer, () => loadCustomers(1))

// --- Modal state ---
const modalLoading = ref(false)
const modalError = ref('')

// Order modal
const showOrderModal = ref(false)
const orderForm = reactive({
  customerId: '',
  shippingAddress: '',
  remarks: '',
  items: [] as { productId: string; quantity: number; unitPrice: number; totalPrice: number }[],
})
const orderTotal = computed(() => orderForm.items.reduce((s, i) => s + i.totalPrice, 0))

function openOrderModal() {
  Object.assign(orderForm, { customerId: '', shippingAddress: '', remarks: '', items: [] })
  modalError.value = ''
  showOrderModal.value = true
}

function addOrderItem() {
  orderForm.items.push({ productId: '', quantity: 1, unitPrice: 0, totalPrice: 0 })
}

function removeOrderItem(idx: number) {
  orderForm.items.splice(idx, 1)
}

function onProductSelect(idx: number) {
  const item = orderForm.items[idx]
  if (!item) return

  const product = products.value.find((p) => p.id === item.productId)
  if (product) {
    item.unitPrice = product.basePrice
    item.totalPrice = product.basePrice * item.quantity
  }
}

function calcItemTotal(idx: number) {
  const item = orderForm.items[idx]
  if (!item) return
  item.totalPrice = item.unitPrice * item.quantity
}

async function submitOrder() {
  if (!orderForm.customerId || orderForm.items.length === 0 || !orderForm.shippingAddress) {
    modalError.value = 'กรุณาเลือกลูกค้า ที่อยู่จัดส่ง และเพิ่มสินค้าอย่างน้อย 1 รายการ'
    return
  }
  if (orderForm.items.some((i) => !i.productId || i.quantity < 1)) {
    modalError.value = 'กรุณาเลือกสินค้าและจำนวนให้ครบทุกรายการ'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    await store.createOrder({
      customerId: orderForm.customerId,
      shippingAddress: orderForm.shippingAddress,
      remarks: orderForm.remarks,
      items: orderForm.items,
    })
    showOrderModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'สร้าง order ไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

async function handleStatusChange(orderId: string, statusStr: string) {
  if (!statusStr) return

  const status = statusMap[statusStr]
  if (status === undefined) return

  try {
    await store.updateOrderStatus(orderId, status)
  } catch {
    /* silent */
  }
}

// Customer modal
const showCustomerModal = ref(false)
const editingCustomer = ref<Customer | null>(null)
const customerForm = reactive({ firstName: '', lastName: '', email: '', phone: '', address: '' })

function openCustomerModal(c?: Customer) {
  editingCustomer.value = c ?? null
  modalError.value = ''
  customerForm.firstName = c?.firstName ?? ''
  customerForm.lastName = c?.lastName ?? ''
  customerForm.email = c?.email ?? ''
  customerForm.phone = c?.phone ?? ''
  customerForm.address = c?.address ?? ''
  showCustomerModal.value = true
}

async function submitCustomer() {
  if (!customerForm.firstName.trim() || !customerForm.email.trim()) {
    modalError.value = 'กรุณากรอกชื่อและ email'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    if (editingCustomer.value) {
      await store.updateCustomer(editingCustomer.value.id, { ...customerForm })
    } else {
      await store.createCustomer({ ...customerForm })
    }
    showCustomerModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// Confirm modal
const showConfirmModal = ref(false)
const confirmTitle = ref('')
const confirmMessage = ref('')
const confirmBtn = ref('')
const confirmAction = ref<() => Promise<void>>(async () => {})

function confirmCancel(o: { orderId: string; orderNumber: string }) {
  confirmTitle.value = 'ยืนยันการยกเลิก Order'
  confirmMessage.value = `ต้องการยกเลิก ${o.orderNumber} ใช่หรือไม่? stock จะถูกคืนอัตโนมัติ`
  confirmBtn.value = 'ยกเลิก Order'
  confirmAction.value = async () => {
    modalLoading.value = true
    try {
      await store.cancelOrder(o.orderId)
      showConfirmModal.value = false
    } finally {
      modalLoading.value = false
    }
  }
  showConfirmModal.value = true
}

function confirmDeleteCustomer(c: Customer) {
  confirmTitle.value = 'ยืนยันการลบลูกค้า'
  confirmMessage.value = `ต้องการลบ ${c.fullName} ใช่หรือไม่?`
  confirmBtn.value = 'ลบ'
  confirmAction.value = async () => {
    modalLoading.value = true
    try {
      await store.deleteCustomer(c.id)
      showConfirmModal.value = false
    } finally {
      modalLoading.value = false
    }
  }
  showConfirmModal.value = true
}

// --- Helpers ---
function formatCurrency(v: number) {
  return new Intl.NumberFormat('th-TH', {
    style: 'currency',
    currency: 'THB',
    maximumFractionDigits: 0,
  }).format(v)
}
function formatDate(d: string) {
  return new Date(d).toLocaleDateString('th-TH', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  })
}
function statusLabel(s: string) {
  return orderStatuses.find((x) => x.value === s)?.label ?? s
}
function statusBadgeClass(s: string) {
  const map: Record<string, string> = {
    Pending:
      'inline-flex items-center rounded-full bg-amber-50 px-2.5 py-1 text-[11px] font-semibold text-amber-700',
    Confirmed:
      'inline-flex items-center rounded-full bg-sky-100 px-2.5 py-1 text-[11px] font-semibold text-sky-700',
    Processing:
      'inline-flex items-center rounded-full bg-violet-100 px-2.5 py-1 text-[11px] font-semibold text-violet-700',
    Shipped:
      'inline-flex items-center rounded-full bg-cyan-100 px-2.5 py-1 text-[11px] font-semibold text-cyan-700',
    Delivered:
      'inline-flex items-center rounded-full bg-emerald-100 px-2.5 py-1 text-[11px] font-semibold text-emerald-700',
    Cancelled:
      'inline-flex items-center rounded-full bg-rose-100 px-2.5 py-1 text-[11px] font-semibold text-rose-700',
  }
  return (
    map[s] ??
    'inline-flex items-center rounded-full bg-slate-100 px-2.5 py-1 text-[11px] font-semibold text-slate-600'
  )
}

onMounted(async () => {
  await Promise.all([loadOrders(), loadCustomers(), inventoryStore.fetchProducts()])
})
</script>
