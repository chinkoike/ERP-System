<template>
  <div class="min-h-screen bg-slate-50">
    <!-- Top bar -->

    <main class="px-8 py-8 max-w-7xl mx-auto">
      <!-- Heading -->
      <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between mb-6">
        <div>
          <h1 class="text-3xl font-semibold tracking-tight text-slate-900">Purchasing</h1>
          <p class="mt-2 text-sm text-slate-500">จัดการใบสั่งซื้อและผู้ขาย</p>
        </div>
        <button
          v-if="activeTab === 'orders'"
          @click="openPoModal()"
          class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
        >
          <span class="text-lg leading-none">+</span> สร้าง PO
        </button>
        <button
          v-if="activeTab === 'suppliers'"
          @click="openSupplierModal()"
          class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
        >
          <span class="text-lg leading-none">+</span> เพิ่ม Supplier
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

      <!-- Loading -->
      <div v-if="store.loading" class="flex items-center justify-center py-24">
        <svg class="animate-spin h-5 w-5 text-slate-300" fill="none" viewBox="0 0 24 24">
          <circle
            class="opacity-25"
            cx="12"
            cy="12"
            r="10"
            stroke="currentColor"
            stroke-width="4"
          />
          <path
            class="opacity-75"
            fill="currentColor"
            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"
          />
        </svg>
      </div>

      <template v-else>
        <!-- PURCHASE ORDERS TAB -->
        <div v-if="activeTab === 'orders'">
          <div class="flex items-center gap-3 mb-4">
            <div class="relative flex-1 max-w-md">
              <svg
                class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400"
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
                v-model="searchPo"
                type="text"
                placeholder="ค้นหา PO, Supplier..."
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 pl-10 pr-4 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>
            <select
              v-model="filterPoStatus"
              class="rounded-2xl border border-slate-200 bg-white py-2 px-4 text-sm text-slate-700 outline-none cursor-pointer focus:ring-2 focus:ring-slate-300"
            >
              <option value="">ทุกสถานะ</option>
              <option v-for="s in poStatuses" :key="s.value" :value="s.value">{{ s.label }}</option>
            </select>
          </div>

          <div class="overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-sm">
            <table class="min-w-full border-collapse">
              <thead>
                <tr class="border-b border-slate-100">
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    เลขที่ PO
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    Supplier
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    วันที่สั่ง
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
                <tr v-if="filteredPo.length === 0">
                  <td colspan="7" class="px-6 py-20 text-center text-sm text-slate-400">
                    ไม่พบใบสั่งซื้อ
                  </td>
                </tr>
                <tr
                  v-for="(po, i) in filteredPo"
                  :key="po.id"
                  :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
                >
                  <td class="px-6 py-4 font-mono text-sm font-semibold text-slate-900">
                    {{ po.purchaseOrderNumber }}
                  </td>
                  <td class="px-6 py-4 text-sm text-slate-600">{{ po.supplierName }}</td>
                  <td class="px-6 py-4 text-sm text-slate-500">{{ formatDate(po.orderDate) }}</td>
                  <td class="px-6 py-4 text-center text-sm text-slate-600">
                    {{ po.items.length }}
                  </td>
                  <td class="px-6 py-4 text-right text-sm font-semibold text-slate-900">
                    {{ formatCurrency(po.totalAmount) }}
                  </td>
                  <td class="px-6 py-4 text-center">
                    <span :class="poStatusClass(po.status)">{{ poStatusLabel(po.status) }}</span>
                  </td>
                  <td class="px-6 py-4">
                    <div class="flex items-center gap-2 justify-end">
                      <button
                        v-if="po.status === 'Ordered' || po.status === 'Receiving'"
                        @click="openReceiveModal(po)"
                        class="rounded-2xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                      >
                        รับสินค้า
                      </button>
                      <button
                        v-if="po.status === 'Ordered'"
                        @click="confirmCancelPo(po)"
                        class="rounded-2xl border border-rose-200 bg-white px-3 py-1.5 text-xs font-medium text-rose-600 transition hover:bg-rose-50"
                      >
                        ยกเลิก
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- SUPPLIERS TAB -->
        <div v-if="activeTab === 'suppliers'">
          <div class="relative max-w-md mb-4">
            <svg
              class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-slate-400"
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
              v-model="searchSupplier"
              type="text"
              placeholder="ค้นหา Supplier..."
              class="w-full rounded-2xl border border-slate-200 bg-white py-2 pl-10 pr-4 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
            />
          </div>

          <div class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
            <table class="min-w-full border-collapse">
              <thead>
                <tr class="border-b border-slate-100">
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    ชื่อบริษัท
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    ผู้ติดต่อ
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
                  <th class="px-6 py-3"></th>
                </tr>
              </thead>
              <tbody>
                <tr v-if="filteredSuppliers.length === 0">
                  <td colspan="5" class="px-6 py-20 text-center text-sm text-slate-400">
                    ไม่พบ Supplier
                  </td>
                </tr>
                <tr
                  v-for="(s, i) in filteredSuppliers"
                  :key="s.id"
                  :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
                >
                  <td class="px-6 py-4 text-sm font-semibold text-slate-900">{{ s.name }}</td>
                  <td class="px-6 py-4 text-sm text-slate-600">{{ s.contactName ?? '—' }}</td>
                  <td class="px-6 py-4 text-sm text-slate-600">{{ s.email ?? '—' }}</td>
                  <td class="px-6 py-4 text-sm text-slate-600">{{ s.phone ?? '—' }}</td>
                  <td class="px-6 py-4">
                    <div class="flex items-center gap-2 justify-end">
                      <button
                        @click="openSupplierModal(s)"
                        class="rounded-2xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                      >
                        แก้ไข
                      </button>
                      <button
                        @click="confirmDeleteSupplier(s)"
                        class="rounded-2xl border border-rose-200 bg-white px-3 py-1.5 text-xs font-medium text-rose-600 transition hover:bg-rose-50"
                      >
                        ลบ
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </template>
    </main>

    <!-- CREATE PO MODAL -->
    <Teleport to="body">
      <div
        v-if="showPoModal"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-xl max-h-[90vh] overflow-y-auto rounded-2xl bg-white shadow-2xl">
          <div
            class="sticky top-0 z-10 flex items-center justify-between border-b border-slate-200 bg-white px-6 py-5"
          >
            <span class="text-base font-semibold text-slate-900">สร้างใบสั่งซื้อ</span>
            <button
              @click="showPoModal = false"
              class="text-slate-500 hover:text-slate-700 text-2xl"
            >
              ×
            </button>
          </div>
          <div class="space-y-4 p-6">
            <div>
              <label class="mb-1.5 block text-sm font-medium text-slate-600">Supplier *</label>
              <select
                v-model="poForm.supplierId"
                class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              >
                <option value="">เลือก Supplier</option>
                <option v-for="s in store.suppliers" :key="s.id" :value="s.id">{{ s.name }}</option>
              </select>
            </div>
            <div>
              <div class="flex items-center justify-between mb-2">
                <label class="text-sm font-medium text-slate-600">รายการสินค้า *</label>
                <button
                  @click="addPoItem"
                  class="rounded-2xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-medium text-slate-900 transition hover:bg-slate-50"
                >
                  + เพิ่ม
                </button>
              </div>
              <div
                v-for="(item, idx) in poForm.items"
                :key="idx"
                class="grid grid-cols-[2fr_1fr_1fr_auto] gap-2 mb-2 items-center"
              >
                <select
                  v-model="item.productId"
                  class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                >
                  <option value="">เลือกสินค้า</option>
                  <option v-for="p in products" :key="p.id" :value="p.id">{{ p.name }}</option>
                </select>
                <input
                  v-model.number="item.quantityOrdered"
                  type="number"
                  min="1"
                  placeholder="จำนวน"
                  class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-center text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
                <input
                  v-model.number="item.unitPrice"
                  @input="calcPoItemTotal(idx)"
                  type="number"
                  min="0"
                  placeholder="ราคา/หน่วย"
                  class="rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-right text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
                <button
                  @click="removePoItem(idx)"
                  class="text-slate-400 hover:text-slate-600 text-lg px-1"
                >
                  ×
                </button>
              </div>
              <div
                v-if="poForm.items.length > 0"
                class="mt-2 border-t border-slate-200 pt-2 text-right"
              >
                <span class="text-sm text-slate-500">ยอดรวม: </span>
                <span class="text-base font-semibold text-slate-900">{{
                  formatCurrency(poTotal)
                }}</span>
              </div>
            </div>
            <div v-if="modalError" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">
              {{ modalError }}
            </div>
          </div>
          <div
            class="sticky bottom-0 z-10 flex justify-end gap-2 border-t border-slate-200 bg-white p-4"
          >
            <button
              @click="showPoModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="submitPo"
              :disabled="modalLoading"
              class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ modalLoading ? 'กำลังสร้าง...' : 'สร้าง PO' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- RECEIVE MODAL -->
    <Teleport to="body">
      <div
        v-if="showReceiveModal && receivingPo"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-lg max-h-[90vh] overflow-y-auto rounded-2xl bg-white shadow-2xl">
          <div
            class="sticky top-0 z-10 flex items-center justify-between border-b border-slate-200 bg-white px-6 py-5"
          >
            <div>
              <span class="text-base font-semibold text-slate-900">รับสินค้า</span>
              <span class="ml-2 font-mono text-sm text-slate-400">{{
                receivingPo.purchaseOrderNumber
              }}</span>
            </div>
            <button
              @click="showReceiveModal = false"
              class="text-slate-500 hover:text-slate-700 text-2xl"
            >
              ×
            </button>
          </div>
          <div class="space-y-3 p-6">
            <div
              v-for="item in receiveForm"
              :key="item.productId"
              class="rounded-2xl border border-slate-100 bg-slate-50 p-4"
            >
              <div class="flex items-center justify-between mb-3">
                <span class="text-sm font-semibold text-slate-900">{{
                  productName(item.productId)
                }}</span>
                <span class="text-xs text-slate-400"
                  >สั่ง {{ item.quantityOrdered }} / รับแล้ว {{ item.quantityReceived }}</span
                >
              </div>
              <div class="flex items-center gap-3">
                <label class="w-20 shrink-0 text-sm text-slate-600">รับเพิ่ม</label>
                <input
                  v-model.number="item.toReceive"
                  type="number"
                  min="0"
                  :max="item.quantityOrdered - item.quantityReceived"
                  class="flex-1 rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-center text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
                <span class="text-xs text-slate-400"
                  >/ {{ item.quantityOrdered - item.quantityReceived }} คงเหลือ</span
                >
              </div>
            </div>
            <div v-if="modalError" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">
              {{ modalError }}
            </div>
          </div>
          <div
            class="sticky bottom-0 flex justify-end gap-2 border-t border-slate-200 bg-white p-4"
          >
            <button
              @click="showReceiveModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="submitReceive"
              :disabled="modalLoading"
              class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ modalLoading ? 'กำลังบันทึก...' : 'ยืนยันรับสินค้า' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- SUPPLIER MODAL -->
    <Teleport to="body">
      <div
        v-if="showSupplierModal"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-md overflow-hidden rounded-2xl bg-white shadow-2xl">
          <div class="flex items-center justify-between border-b border-slate-200 px-6 py-5">
            <span class="text-base font-semibold text-slate-900">{{
              editingSupplier ? 'แก้ไข Supplier' : 'เพิ่ม Supplier'
            }}</span>
            <button
              @click="showSupplierModal = false"
              class="text-slate-500 hover:text-slate-700 text-2xl"
            >
              ×
            </button>
          </div>
          <div class="space-y-4 p-6">
            <div>
              <label class="mb-1.5 block text-sm font-medium text-slate-600">ชื่อบริษัท *</label>
              <input
                v-model="supplierForm.name"
                type="text"
                class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>
            <div>
              <label class="mb-1.5 block text-sm font-medium text-slate-600">ผู้ติดต่อ</label>
              <input
                v-model="supplierForm.contactName"
                type="text"
                class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>
            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">Email</label>
                <input
                  v-model="supplierForm.email"
                  type="email"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">เบอร์โทร</label>
                <input
                  v-model="supplierForm.phone"
                  type="text"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
            </div>
            <div>
              <label class="mb-1.5 block text-sm font-medium text-slate-600">ที่อยู่</label>
              <textarea
                v-model="supplierForm.address"
                rows="2"
                class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none resize-y focus:ring-2 focus:ring-slate-300"
              ></textarea>
            </div>
            <div v-if="modalError" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">
              {{ modalError }}
            </div>
          </div>
          <div class="flex justify-end gap-2 border-t border-slate-200 bg-slate-50 px-4 py-4">
            <button
              @click="showSupplierModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="submitSupplier"
              :disabled="modalLoading"
              class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ modalLoading ? 'กำลังบันทึก...' : 'บันทึก' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- CONFIRM MODAL -->
    <Teleport to="body">
      <div
        v-if="showConfirmModal"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-sm rounded-2xl bg-white p-6 shadow-2xl">
          <div class="mb-2 text-base font-semibold text-slate-900">{{ confirmTitle }}</div>
          <div class="mb-6 text-sm text-slate-500">{{ confirmMessage }}</div>
          <div class="flex justify-end gap-2">
            <button
              @click="showConfirmModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="runConfirmAction"
              :disabled="modalLoading"
              class="rounded-2xl bg-rose-600 px-5 py-2 text-sm font-medium text-white transition hover:bg-rose-700 disabled:opacity-50 disabled:cursor-not-allowed"
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
import { ref, computed, reactive, onMounted } from 'vue'
import { usePurchasingStore } from '@/stores/purchasingStore'
import { useInventoryStore } from '@/stores/inventoryStore'
import type { PurchaseOrder, Supplier } from '@/types/purchasing'

const store = usePurchasingStore()
const inventoryStore = useInventoryStore()

const products = computed(() => inventoryStore.products)

const activeTab = ref<'orders' | 'suppliers'>('orders')
const tabs = [
  { key: 'orders', label: 'Purchase Orders' },
  { key: 'suppliers', label: 'Suppliers' },
] as const

const poStatuses = [
  { value: 'Ordered', label: 'รอรับสินค้า' },
  { value: 'Receiving', label: 'รับบางส่วน' },
  { value: 'Completed', label: 'รับครบแล้ว' },
  { value: 'Cancelled', label: 'ยกเลิก' },
]

const searchPo = ref('')
const filterPoStatus = ref('')
const filteredPo = computed(() =>
  store.purchaseOrders.filter((po) => {
    const matchSearch =
      po.purchaseOrderNumber.toLowerCase().includes(searchPo.value.toLowerCase()) ||
      po.supplierName.toLowerCase().includes(searchPo.value.toLowerCase())
    return matchSearch && (!filterPoStatus.value || po.status === filterPoStatus.value)
  }),
)
const searchSupplier = ref('')
const filteredSuppliers = computed(() =>
  store.suppliers.filter(
    (s) =>
      s.name.toLowerCase().includes(searchSupplier.value.toLowerCase()) ||
      (s.email ?? '').toLowerCase().includes(searchSupplier.value.toLowerCase()),
  ),
)

const modalLoading = ref(false)
const modalError = ref('')

// PO Modal
const showPoModal = ref(false)
const poForm = reactive({
  supplierId: '',
  items: [] as {
    productId: string
    quantityOrdered: number
    unitPrice: number
    totalPrice: number
  }[],
})
const poTotal = computed(() => poForm.items.reduce((s, i) => s + i.totalPrice, 0))

function openPoModal() {
  poForm.supplierId = ''
  poForm.items = []
  modalError.value = ''
  showPoModal.value = true
}
function addPoItem() {
  poForm.items.push({ productId: '', quantityOrdered: 1, unitPrice: 0, totalPrice: 0 })
}
function removePoItem(idx: number) {
  poForm.items.splice(idx, 1)
}
function calcPoItemTotal(idx: number) {
  const i = poForm.items[idx]
  if (i) {
    i.totalPrice = i.unitPrice * i.quantityOrdered
  }
}

async function submitPo() {
  if (!poForm.supplierId || poForm.items.length === 0) {
    modalError.value = 'กรุณาเลือก Supplier และเพิ่มสินค้าอย่างน้อย 1 รายการ'
    return
  }
  if (poForm.items.some((i) => !i.productId || i.quantityOrdered < 1)) {
    modalError.value = 'กรุณาเลือกสินค้าและจำนวนให้ครบ'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    await store.createPurchaseOrder({
      supplierId: poForm.supplierId,
      items: poForm.items.map((i) => ({ ...i, quantityReceived: 0 })),
    })
    showPoModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'สร้าง PO ไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// Receive Modal
const showReceiveModal = ref(false)
const receivingPo = ref<PurchaseOrder | null>(null)
const receiveForm = ref<
  {
    productId: string
    quantityOrdered: number
    quantityReceived: number
    unitPrice: number
    totalPrice: number
    toReceive: number
  }[]
>([])

function openReceiveModal(po: PurchaseOrder) {
  receivingPo.value = po
  receiveForm.value = po.items.map((i) => ({
    ...i,
    toReceive: i.quantityOrdered - i.quantityReceived,
  }))
  modalError.value = ''
  showReceiveModal.value = true
}

async function submitReceive() {
  if (!receivingPo.value) return
  const itemsToSend = receiveForm.value.filter((i) => i.toReceive > 0)
  if (itemsToSend.length === 0) {
    modalError.value = 'กรุณาระบุจำนวนที่รับ'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    await store.receivePurchaseOrder(
      receivingPo.value.id,
      itemsToSend.map((i) => ({
        productId: i.productId,
        quantityOrdered: i.quantityOrdered,
        quantityReceived: i.toReceive,
        unitPrice: i.unitPrice,
        totalPrice: i.totalPrice,
      })),
    )
    showReceiveModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// Supplier Modal
const showSupplierModal = ref(false)
const editingSupplier = ref<Supplier | null>(null)
const supplierForm = reactive({ name: '', contactName: '', email: '', phone: '', address: '' })

function openSupplierModal(s?: Supplier) {
  editingSupplier.value = s ?? null
  modalError.value = ''
  supplierForm.name = s?.name ?? ''
  supplierForm.contactName = s?.contactName ?? ''
  supplierForm.email = s?.email ?? ''
  supplierForm.phone = s?.phone ?? ''
  supplierForm.address = s?.address ?? ''
  showSupplierModal.value = true
}

async function submitSupplier() {
  if (!supplierForm.name.trim()) {
    modalError.value = 'กรุณากรอกชื่อบริษัท'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    if (editingSupplier.value)
      await store.updateSupplier(editingSupplier.value.id, { ...supplierForm })
    else await store.createSupplier({ ...supplierForm })
    showSupplierModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// Confirm Modal
const showConfirmModal = ref(false)
const confirmTitle = ref('')
const confirmMessage = ref('')
const confirmBtn = ref('')
const confirmAction = ref<() => Promise<void>>(async () => {})

function confirmCancelPo(po: PurchaseOrder) {
  confirmTitle.value = 'ยืนยันการยกเลิก PO'
  confirmMessage.value = `ต้องการยกเลิก ${po.purchaseOrderNumber} ใช่หรือไม่?`
  confirmBtn.value = 'ยกเลิก PO'
  confirmAction.value = async () => {
    await store.cancelPurchaseOrder(po.id)
    showConfirmModal.value = false
  }
  showConfirmModal.value = true
}

function confirmDeleteSupplier(s: Supplier) {
  confirmTitle.value = 'ยืนยันการลบ Supplier'
  confirmMessage.value = `ต้องการลบ ${s.name} ใช่หรือไม่?`
  confirmBtn.value = 'ลบ'
  confirmAction.value = async () => {
    await store.deleteSupplier(s.id)
    showConfirmModal.value = false
  }
  showConfirmModal.value = true
}

async function runConfirmAction() {
  modalLoading.value = true
  try {
    await confirmAction.value()
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'เกิดข้อผิดพลาด'
  } finally {
    modalLoading.value = false
  }
}

function productName(id: string) {
  return products.value.find((p) => p.id === id)?.name ?? id
}
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
function poStatusLabel(s: string) {
  return poStatuses.find((x) => x.value === s)?.label ?? s
}
function poStatusClass(s: string) {
  const map: Record<string, string> = {
    Ordered:
      'inline-flex rounded-full bg-amber-50 px-2 py-0.5 text-xs font-semibold text-amber-600',
    Receiving:
      'inline-flex rounded-full bg-blue-50 px-2 py-0.5 text-xs font-semibold text-blue-600',
    Completed:
      'inline-flex rounded-full bg-emerald-50 px-2 py-0.5 text-xs font-semibold text-emerald-600',
    Cancelled:
      'inline-flex rounded-full bg-rose-50 px-2 py-0.5 text-xs font-semibold text-rose-500',
  }
  return (
    map[s] ??
    'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-500'
  )
}

onMounted(async () => {
  await Promise.all([
    store.fetchPurchaseOrders(),
    store.fetchSuppliers(),
    inventoryStore.fetchProducts(),
  ])
})
</script>
