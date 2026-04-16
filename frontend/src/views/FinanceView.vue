<template>
  <div class="min-h-screen bg-slate-50">
    <!-- Top bar -->

    <main class="px-8 py-8 max-w-7xl mx-auto">
      <!-- Heading -->
      <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between mb-6">
        <div>
          <h1 class="text-3xl font-semibold tracking-tight text-slate-900">Finance</h1>
          <p class="mt-2 text-sm text-slate-500">จัดการ Invoice, Payment และบัญชี</p>
        </div>
        <button
          v-if="activeTab === 'invoices'"
          @click="openInvoiceModal()"
          class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
        >
          <span class="text-lg leading-none">+</span> สร้าง Invoice
        </button>
        <button
          v-if="activeTab === 'accounts'"
          @click="openAccountModal()"
          class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
        >
          <span class="text-lg leading-none">+</span> เพิ่มบัญชี
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
        <!-- INVOICES TAB -->
        <div v-if="activeTab === 'invoices'">
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
                v-model="searchInvoice"
                type="text"
                placeholder="ค้นหาเลข Invoice..."
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 pl-10 pr-4 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>
            <select
              v-model="filterInvoiceStatus"
              class="rounded-2xl border border-slate-200 bg-white py-2 px-4 text-sm text-slate-700 outline-none cursor-pointer focus:ring-2 focus:ring-slate-300"
            >
              <option value="">ทุกสถานะ</option>
              <option v-for="s in invoiceStatuses" :key="s.value" :value="s.value">
                {{ s.label }}
              </option>
            </select>
          </div>

          <div class="overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-sm">
            <table class="min-w-full border-collapse">
              <thead>
                <tr class="border-b border-slate-100">
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    เลข Invoice
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    คำอธิบาย
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    วันที่ออก
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    ครบกำหนด
                  </th>
                  <th
                    class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    ยอดรวม
                  </th>
                  <th
                    class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    ยอดค้างชำระ
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
                <tr v-if="filteredInvoices.length === 0">
                  <td colspan="8" class="px-6 py-20 text-center text-sm text-slate-400">
                    ไม่พบ Invoice
                  </td>
                </tr>
                <tr
                  v-for="(inv, i) in filteredInvoices"
                  :key="inv.id"
                  :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
                >
                  <td class="px-6 py-4 font-mono text-sm font-semibold text-slate-900">
                    {{ inv.invoiceNumber }}
                  </td>
                  <td class="px-6 py-4 max-w-45 truncate text-sm text-slate-600">
                    {{ inv.description ?? '—' }}
                  </td>
                  <td class="px-6 py-4 text-sm text-slate-500">
                    {{ formatDate(inv.invoiceDate) }}
                  </td>
                  <td
                    class="px-6 py-4 text-sm"
                    :class="isOverdue(inv) ? 'font-semibold text-rose-600' : 'text-slate-500'"
                  >
                    {{ formatDate(inv.dueDate) }}
                  </td>
                  <td class="px-6 py-4 text-right text-sm font-semibold text-slate-900">
                    {{ formatCurrency(inv.totalAmount) }}
                  </td>
                  <td class="px-6 py-4 text-right text-sm font-semibold text-slate-900">
                    {{ formatCurrency(inv.amountDue) }}
                  </td>
                  <td class="px-6 py-4 text-center">
                    <span :class="invoiceStatusClass(inv.status)">{{
                      invoiceStatusLabel(inv.status)
                    }}</span>
                  </td>
                  <td class="px-6 py-4">
                    <div class="flex items-center gap-2 justify-end">
                      <button
                        @click="openPaymentModal(inv)"
                        v-if="inv.status !== 'Paid' && inv.status !== 'Cancelled'"
                        class="rounded-2xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                      >
                        ชำระเงิน
                      </button>

                      <button
                        @click="openEditInvoiceModal(inv)"
                        v-if="inv.status !== 'Paid' && inv.status !== 'Cancelled'"
                        class="rounded-2xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                      >
                        แก้ไข
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="flex items-center justify-between bg-white px-6 py-4 text-sm text-slate-500">
            <div>แสดง {{ store.invoices.length }} จาก {{ store.totalItems }} รายการ</div>
            <div class="flex items-center gap-2">
              <button
                @click="loadInvoices(store.currentPage - 1)"
                :disabled="store.currentPage <= 1"
                class="rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-600 disabled:cursor-not-allowed disabled:opacity-50"
              >
                ก่อนหน้า
              </button>
              <span>หน้า {{ store.currentPage }} / {{ store.totalPages }}</span>
              <button
                @click="loadInvoices(store.currentPage + 1)"
                :disabled="store.currentPage >= store.totalPages"
                class="rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-600 disabled:cursor-not-allowed disabled:opacity-50"
              >
                ถัดไป
              </button>
            </div>
          </div>
        </div>

        <!-- ACCOUNTS TAB -->
        <div v-if="activeTab === 'accounts'">
          <div class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
            <table class="min-w-full border-collapse">
              <thead>
                <tr class="border-b border-slate-100">
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    ชื่อบัญชี
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    รหัสบัญชี
                  </th>
                  <th
                    class="px-6 py-3 text-right text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    ยอดคงเหลือ
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr v-if="store.accounts.length === 0">
                  <td colspan="3" class="px-6 py-20 text-center text-sm text-slate-400">
                    ไม่พบบัญชี
                  </td>
                </tr>
                <tr
                  v-for="(acc, i) in store.accounts"
                  :key="acc.id"
                  :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
                >
                  <td class="px-6 py-4 text-sm font-semibold text-slate-900">
                    {{ acc.accountName }}
                  </td>
                  <td class="px-6 py-4 font-mono text-sm text-slate-500">{{ acc.accountCode }}</td>
                  <td
                    class="px-6 py-4 text-right text-sm font-semibold"
                    :class="acc.balance >= 0 ? 'text-emerald-600' : 'text-rose-600'"
                  >
                    {{ formatCurrency(acc.balance) }}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </template>
    </main>

    <!-- CREATE / EDIT INVOICE MODAL -->
    <Teleport to="body">
      <div
        v-if="showInvoiceModal"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-lg overflow-hidden rounded-2xl bg-white shadow-2xl">
          <div class="flex items-center justify-between border-b border-slate-200 px-6 py-5">
            <span class="text-base font-semibold text-slate-900">{{
              editingInvoice ? 'แก้ไข Invoice' : 'สร้าง Invoice'
            }}</span>
            <button
              @click="showInvoiceModal = false"
              class="text-slate-500 hover:text-slate-700 text-2xl"
            >
              ×
            </button>
          </div>
          <div class="space-y-4 p-6">
            <div v-if="!editingInvoice">
              <label class="mb-1.5 block text-sm font-medium text-slate-600">เลข Invoice *</label>
              <input
                v-model="invoiceForm.invoiceNumber"
                type="text"
                placeholder="INV-2026-001"
                class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 font-mono text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>

            <!-- Customer or Supplier -->
            <div v-if="!editingInvoice" class="grid grid-cols-2 gap-3">
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">Customer ID</label>
                <input
                  v-model="invoiceForm.customerId"
                  type="text"
                  placeholder="UUID (ถ้ามี)"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">Supplier ID</label>
                <input
                  v-model="invoiceForm.supplierId"
                  type="text"
                  placeholder="UUID (ถ้ามี)"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
            </div>
            <p class="text-xs text-slate-500">
              เลือกกรอก Customer ID หรือ Supplier ID อย่างใดอย่างหนึ่งก็พอ
              ไม่จำเป็นต้องกรอกทั้งสองช่อง
            </p>

            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600"
                  >ยอดรวม (บาท) *</label
                >
                <input
                  v-model.number="invoiceForm.totalAmount"
                  type="number"
                  min="0.01"
                  step="0.01"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600"
                  >ยอดค้างชำระ (บาท) *</label
                >
                <input
                  v-model.number="invoiceForm.amountDue"
                  type="number"
                  min="0.01"
                  step="0.01"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
            </div>

            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">วันที่ออก *</label>
                <input
                  v-model="invoiceForm.invoiceDate"
                  type="date"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">ครบกำหนด *</label>
                <input
                  v-model="invoiceForm.dueDate"
                  type="date"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
            </div>

            <div>
              <label class="mb-1.5 block text-sm font-medium text-slate-600">คำอธิบาย</label>
              <textarea
                v-model="invoiceForm.description"
                rows="2"
                class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none resize-y focus:ring-2 focus:ring-slate-300"
              ></textarea>
            </div>

            <!-- Status (edit only) -->
            <div v-if="editingInvoice">
              <label class="mb-1.5 block text-sm font-medium text-slate-600">สถานะ</label>
              <select
                v-model="invoiceForm.status"
                class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              >
                <option v-for="s in invoiceStatuses" :key="s.value" :value="s.value">
                  {{ s.label }}
                </option>
              </select>
            </div>

            <div v-if="modalError" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">
              {{ modalError }}
            </div>
          </div>
          <div class="flex justify-end gap-2 border-t border-slate-200 bg-slate-50 px-4 py-4">
            <button
              @click="showInvoiceModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="submitInvoice"
              :disabled="modalLoading"
              class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ modalLoading ? 'กำลังบันทึก...' : 'บันทึก' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- PAYMENT MODAL -->
    <Teleport to="body">
      <div
        v-if="showPaymentModal && payingInvoice"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-md overflow-hidden rounded-2xl bg-white shadow-2xl">
          <div class="flex items-center justify-between border-b border-slate-200 px-6 py-5">
            <div>
              <span class="text-base font-semibold text-slate-900">บันทึกการชำระเงิน</span>
              <span class="ml-2 font-mono text-sm text-slate-400">{{
                payingInvoice.invoiceNumber
              }}</span>
            </div>
            <button
              @click="showPaymentModal = false"
              class="text-slate-500 hover:text-slate-700 text-2xl"
            >
              ×
            </button>
          </div>
          <div class="space-y-4 p-6">
            <!-- Invoice summary -->
            <div class="rounded-2xl border border-slate-100 bg-slate-50 p-4 space-y-2">
              <div class="flex justify-between text-sm">
                <span class="text-slate-500">ยอดรวม</span>
                <span class="font-semibold text-slate-900">{{
                  formatCurrency(payingInvoice.totalAmount)
                }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-slate-500">ยอดค้างชำระ</span>
                <span class="font-semibold text-slate-900">{{
                  formatCurrency(payingInvoice.amountDue)
                }}</span>
              </div>
            </div>

            <div>
              <label class="mb-1.5 block text-sm font-medium text-slate-600"
                >บัญชีที่รับเงิน *</label
              >
              <select
                v-model="paymentForm.accountId"
                class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              >
                <option value="">เลือกบัญชี</option>
                <option v-for="acc in store.accounts" :key="acc.id" :value="acc.id">
                  {{ acc.accountName }} ({{ formatCurrency(acc.balance) }})
                </option>
              </select>
            </div>

            <div>
              <label class="mb-1.5 block text-sm font-medium text-slate-600"
                >จำนวนเงิน (บาท) *</label
              >
              <input
                v-model.number="paymentForm.amountPaid"
                type="number"
                min="0.01"
                step="0.01"
                class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>

            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">วันที่ชำระ *</label>
                <input
                  v-model="paymentForm.paymentDate"
                  type="date"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">วิธีชำระ *</label>
                <select
                  v-model="paymentForm.paymentMethod"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                >
                  <option value="">เลือกวิธี</option>
                  <option v-for="m in paymentMethods" :key="m.value" :value="m.value">
                    {{ m.label }}
                  </option>
                </select>
              </div>
            </div>

            <div>
              <label class="mb-1.5 block text-sm font-medium text-slate-600">เลขอ้างอิง</label>
              <input
                v-model="paymentForm.referenceNumber"
                type="text"
                placeholder="เลขโอน, เลขเช็ค ฯลฯ"
                class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>

            <div v-if="modalError" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">
              {{ modalError }}
            </div>
          </div>
          <div class="flex justify-end gap-2 border-t border-slate-200 bg-slate-50 px-4 py-4">
            <button
              @click="showPaymentModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="submitPayment"
              :disabled="modalLoading"
              class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ modalLoading ? 'กำลังบันทึก...' : 'บันทึกการชำระ' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- ACCOUNT MODAL -->
    <Teleport to="body">
      <div
        v-if="showAccountModal"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-sm overflow-hidden rounded-2xl bg-white shadow-2xl">
          <div class="flex items-center justify-between border-b border-slate-200 px-6 py-5">
            <span class="text-base font-semibold text-slate-900">เพิ่มบัญชี</span>
            <button
              @click="showAccountModal = false"
              class="text-slate-500 hover:text-slate-700 text-2xl"
            >
              ×
            </button>
          </div>
          <div class="space-y-4 p-6">
            <div>
              <label class="mb-1.5 block text-sm font-medium text-slate-600">ชื่อบัญชี *</label>
              <input
                v-model="accountForm.accountName"
                type="text"
                class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>
            <div>
              <label class="mb-1.5 block text-sm font-medium text-slate-600">รหัสบัญชี *</label>
              <input
                v-model="accountForm.accountCode"
                type="text"
                placeholder="ACC-001"
                class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 font-mono text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>
            <div>
              <label class="mb-1.5 block text-sm font-medium text-slate-600"
                >ยอดเริ่มต้น (บาท)</label
              >
              <input
                v-model.number="accountForm.balance"
                type="number"
                step="0.01"
                class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>
            <div v-if="modalError" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">
              {{ modalError }}
            </div>
          </div>
          <div class="flex justify-end gap-2 border-t border-slate-200 bg-slate-50 px-4 py-4">
            <button
              @click="showAccountModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="submitAccount"
              :disabled="modalLoading"
              class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ modalLoading ? 'กำลังบันทึก...' : 'บันทึก' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted, watch } from 'vue'
import { useFinanceStore } from '@/stores/financeStore'
import type { Invoice } from '@/types/finance'

const store = useFinanceStore()

const activeTab = ref<'invoices' | 'accounts'>('invoices')
const tabs = [
  { key: 'invoices', label: 'Invoices' },
  { key: 'accounts', label: 'บัญชี' },
] as const

const invoiceStatuses = [
  { value: 'Draft', label: 'ร่าง' },
  { value: 'Issued', label: 'ออกแล้ว' },
  { value: 'Paid', label: 'ชำระแล้ว' },
  { value: 'Overdue', label: 'เกินกำหนด' },
  { value: 'Cancelled', label: 'ยกเลิก' },
]

const paymentMethods = [
  { value: 'Cash', label: 'เงินสด' },
  { value: 'CreditCard', label: 'บัตรเครดิต' },
  { value: 'BankTransfer', label: 'โอนธนาคาร' },
  { value: 'Cheque', label: 'เช็ค' },
  { value: 'Others', label: 'อื่นๆ' },
]

// --- Filters ---
const searchInvoice = ref('')
const filterInvoiceStatus = ref('')
const filteredInvoices = computed(() => store.invoices)

async function loadInvoices(page = 1) {
  await store.fetchInvoices({
    searchTerm: searchInvoice.value.trim() || undefined,
    status: filterInvoiceStatus.value || undefined,
    pageNumber: page,
  })
}

watch([searchInvoice, filterInvoiceStatus], () => loadInvoices(1))

// --- Modal state ---
const modalLoading = ref(false)
const modalError = ref('')

// Invoice Modal
const showInvoiceModal = ref(false)
const editingInvoice = ref<Invoice | null>(null)
const invoiceForm = reactive({
  invoiceNumber: '',
  customerId: '',
  supplierId: '',
  purchaseOrderId: '',
  description: '',
  totalAmount: 0,
  amountDue: 0,
  invoiceDate: '',
  dueDate: '',
  status: 'Issued',
})

function openInvoiceModal() {
  editingInvoice.value = null
  modalError.value = ''
  const today = new Date().toISOString().split('T')[0]
  const due = new Date(Date.now() + 30 * 86400000).toISOString().split('T')[0]
  Object.assign(invoiceForm, {
    invoiceNumber: '',
    customerId: '',
    supplierId: '',
    purchaseOrderId: '',
    description: '',
    amountDue: 0,
    invoiceDate: today,
    dueDate: due,
    status: 'Issued',
  })
  showInvoiceModal.value = true
}

function openEditInvoiceModal(inv: Invoice) {
  editingInvoice.value = inv
  modalError.value = ''
  Object.assign(invoiceForm, {
    invoiceNumber: inv.invoiceNumber,
    description: inv.description ?? '',
    totalAmount: inv.totalAmount,
    amountDue: inv.amountDue,
    invoiceDate: inv.invoiceDate.split('T')[0],
    dueDate: inv.dueDate.split('T')[0],
    status: inv.status,
  })
  showInvoiceModal.value = true
}

async function submitInvoice() {
  if (
    !invoiceForm.totalAmount ||
    !invoiceForm.amountDue ||
    !invoiceForm.invoiceDate ||
    !invoiceForm.dueDate
  ) {
    modalError.value = 'กรุณากรอกข้อมูลให้ครบ'
    return
  }

  if (!editingInvoice.value && !invoiceForm.customerId && !invoiceForm.supplierId) {
    modalError.value = 'กรุณาระบุ Customer ID หรือ Supplier ID อย่างใดอย่างหนึ่ง'
    return
  }

  modalLoading.value = true
  modalError.value = ''
  try {
    if (editingInvoice.value) {
      await store.updateInvoice(editingInvoice.value.id, {
        amountDue: invoiceForm.amountDue,
        dueDate: invoiceForm.dueDate,
        description: invoiceForm.description,
        status: invoiceForm.status,
      })
    } else {
      if (!invoiceForm.invoiceNumber.trim()) {
        modalError.value = 'กรุณากรอกเลข Invoice'
        return
      }
      if (invoiceForm.totalAmount <= 0) {
        modalError.value = 'กรุณากรอกยอดรวม'
        return
      }
      if (invoiceForm.amountDue <= 0) {
        modalError.value = 'กรุณากรอกยอดค้างชำระ'
        return
      }
      if (invoiceForm.amountDue > invoiceForm.totalAmount) {
        modalError.value = 'ยอดค้างชำระต้องไม่เกินยอดรวม'
        return
      }
      await store.createInvoice({
        invoiceNumber: invoiceForm.invoiceNumber,
        customerId: invoiceForm.customerId || undefined,
        supplierId: invoiceForm.supplierId || undefined,
        description: invoiceForm.description,
        totalAmount: invoiceForm.totalAmount,
        amountDue: invoiceForm.amountDue,
        invoiceDate: invoiceForm.invoiceDate,
        dueDate: invoiceForm.dueDate,
      })
    }
    showInvoiceModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// Payment Modal
const showPaymentModal = ref(false)
const payingInvoice = ref<Invoice | null>(null)
const paymentForm = reactive({
  accountId: '',
  amountPaid: 0,
  paymentDate: '',
  paymentMethod: '',
  referenceNumber: '',
})

function openPaymentModal(inv: Invoice) {
  payingInvoice.value = inv
  modalError.value = ''
  paymentForm.accountId = ''
  paymentForm.amountPaid = inv.amountDue
  paymentForm.paymentDate = new Date().toISOString().split('T')[0] as string
  paymentForm.paymentMethod = ''
  paymentForm.referenceNumber = ''
  showPaymentModal.value = true
}

async function submitPayment() {
  if (!payingInvoice.value) return
  if (
    !paymentForm.accountId ||
    !paymentForm.amountPaid ||
    !paymentForm.paymentDate ||
    !paymentForm.paymentMethod
  ) {
    modalError.value = 'กรุณากรอกข้อมูลให้ครบ'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    await store.createPayment({
      invoiceId: payingInvoice.value.id,
      accountId: paymentForm.accountId,
      amountPaid: paymentForm.amountPaid,
      paymentDate: paymentForm.paymentDate,
      paymentMethod: paymentForm.paymentMethod,
      referenceNumber: paymentForm.referenceNumber || undefined,
    })
    showPaymentModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// Account Modal
const showAccountModal = ref(false)
const accountForm = reactive({ accountName: '', accountCode: '', balance: 0 })

function openAccountModal() {
  modalError.value = ''
  Object.assign(accountForm, { accountName: '', accountCode: '', balance: 0 })
  showAccountModal.value = true
}

async function submitAccount() {
  if (!accountForm.accountName.trim() || !accountForm.accountCode.trim()) {
    modalError.value = 'กรุณากรอกชื่อและรหัสบัญชี'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    await store.createAccount({ ...accountForm })
    showAccountModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// Helpers
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
function isOverdue(inv: Invoice) {
  return inv.status !== 'Paid' && inv.status !== 'Cancelled' && new Date(inv.dueDate) < new Date()
}
function invoiceStatusLabel(s: string) {
  return invoiceStatuses.find((x) => x.value === s)?.label ?? s
}
function invoiceStatusClass(s: string) {
  const map: Record<string, string> = {
    Draft: 'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-500',
    Issued: 'inline-flex rounded-full bg-blue-50 px-2 py-0.5 text-xs font-semibold text-blue-600',
    Paid: 'inline-flex rounded-full bg-emerald-50 px-2 py-0.5 text-xs font-semibold text-emerald-600',
    Overdue: 'inline-flex rounded-full bg-rose-50 px-2 py-0.5 text-xs font-semibold text-rose-600',
    Cancelled:
      'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-400',
  }
  return (
    map[s] ??
    'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-500'
  )
}

onMounted(async () => {
  await Promise.all([loadInvoices(), store.fetchAccounts()])
})
</script>
