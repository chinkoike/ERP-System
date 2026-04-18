<template>
  <div class="min-h-screen bg-slate-50">
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
      <TableSkeleton v-if="loading" :rows="6" />

      <template v-else>
        <div v-if="activeTab === 'invoices'">
          <InvoiceTable
            :invoices="store.invoices"
            :isSearching="isSearching"
            :accounts="store.accounts"
            :total-items="store.totalItems"
            :current-page="store.currentPage"
            :total-pages="store.totalPages"
            v-model:searchInvoice="searchInvoice"
            v-model:filterAccount="filterInvoiceAccount"
            v-model:filterStatus="filterInvoiceStatus"
            @pay="openPaymentModal"
            @edit="openEditInvoiceModal"
            @updateStatus="openStatusModal"
            @pageChange="loadInvoices"
          />
        </div>

        <div v-if="activeTab === 'accounts'">
          <AccountList :accounts="store.accounts" />
        </div>
      </template>
    </main>

    <!-- Invoice Modal (Create / Edit) -->
    <InvoiceModal
      :show="showInvoiceModal"
      :editing-invoice="editingInvoice"
      :loading="modalLoading"
      :error="modalError"
      @close="showInvoiceModal = false"
      @submit="submitInvoice"
    />

    <!-- Invoice Status Modal -->
    <InvoiceStatusModal
      :show="showStatusModal"
      :invoice="statusInvoice"
      :loading="modalLoading"
      :error="modalError"
      @close="showStatusModal = false"
      @submit="submitStatus"
    />

    <!-- Payment Modal -->
    <PaymentModal
      :show="showPaymentModal"
      :invoice="payingInvoice"
      :accounts="store.accounts"
      :loading="modalLoading"
      :error="modalError"
      @close="showPaymentModal = false"
      @submit="submitPayment"
    />

    <!-- Account Modal -->
    <AccountModal
      :show="showAccountModal"
      :loading="modalLoading"
      :error="modalError"
      @close="showAccountModal = false"
      @submit="submitAccount"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useFinanceStore } from '@/stores/financeStore'
import type {
  Invoice,
  InvoiceStatus,
  CreateInvoicePayload,
  UpdateInvoicePayload,
  CreatePaymentPayload,
  CreateAccountPayload,
} from '@/types/finance'

import InvoiceTable from '@/components/finanace/InvoiceTable.vue'
import AccountList from '@/components/finanace/AccountList.vue'
import InvoiceModal from '@/components/finanace/InvoiceModal.vue'
import InvoiceStatusModal from '@/components/finanace/InvoiceStatusModal.vue'
import PaymentModal from '@/components/finanace/PaymentModal.vue'
import AccountModal from '@/components/finanace/AccountModal.vue'

const store = useFinanceStore()

const activeTab = ref<'invoices' | 'accounts'>('invoices')
const tabs = [
  { key: 'invoices', label: 'Invoices' },
  { key: 'accounts', label: 'บัญชี' },
] as const

// ─── Filters ──────────────────────────────────────────────────
const searchInvoice = ref('')
const filterInvoiceAccount = ref('')
const filterInvoiceStatus = ref('')
const loading = ref(true)
const isSearching = ref(false)
async function loadInvoices(page = 1) {
  isSearching.value = true // เริ่มค้นหา
  try {
    await store.fetchInvoices({
      searchTerm: searchInvoice.value.trim() || undefined,
      accountId: filterInvoiceAccount.value || undefined,
      status: filterInvoiceStatus.value || undefined,
      pageNumber: page,
    })
  } finally {
    loading.value = false
    isSearching.value = false
  }
}
watch([searchInvoice, filterInvoiceAccount, filterInvoiceStatus], () => loadInvoices(1))

// ─── Shared modal state ────────────────────────────────────────
const modalLoading = ref(false)
const modalError = ref('')

// ─── Invoice Modal ─────────────────────────────────────────────
const showInvoiceModal = ref(false)
const editingInvoice = ref<Invoice | null>(null)

function openInvoiceModal() {
  editingInvoice.value = null
  modalError.value = ''
  showInvoiceModal.value = true
}

function openEditInvoiceModal(inv: Invoice) {
  editingInvoice.value = inv
  modalError.value = ''
  showInvoiceModal.value = true
}

async function submitInvoice(payload: CreateInvoicePayload | UpdateInvoicePayload) {
  modalLoading.value = true
  modalError.value = ''
  try {
    if (editingInvoice.value) {
      const p = payload as UpdateInvoicePayload
      if (!p.amountDue || !p.dueDate) {
        modalError.value = 'กรุณากรอกข้อมูลให้ครบ'
        return
      }
      await store.updateInvoice(editingInvoice.value.id, p)
    } else {
      const p = payload as CreateInvoicePayload
      if (!p.invoiceNumber?.trim()) {
        modalError.value = 'กรุณากรอกเลข Invoice'
        return
      }
      if (!p.customerId && !p.supplierId) {
        modalError.value = 'กรุณาระบุ Customer ID หรือ Supplier ID'
        return
      }
      if (!p.totalAmount || p.totalAmount <= 0) {
        modalError.value = 'กรุณากรอกยอดรวม'
        return
      }
      if (!p.amountDue || p.amountDue <= 0) {
        modalError.value = 'กรุณากรอกยอดค้างชำระ'
        return
      }
      if (p.amountDue > p.totalAmount) {
        modalError.value = 'ยอดค้างชำระต้องไม่เกินยอดรวม'
        return
      }
      if (!p.invoiceDate || !p.dueDate) {
        modalError.value = 'กรุณากรอกวันที่'
        return
      }
      await store.createInvoice(p)
    }
    showInvoiceModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// ─── Status Modal ──────────────────────────────────────────────
const showStatusModal = ref(false)
const statusInvoice = ref<Invoice | null>(null)

function openStatusModal(inv: Invoice) {
  statusInvoice.value = inv
  modalError.value = ''
  showStatusModal.value = true
}

async function submitStatus(newStatus: InvoiceStatus) {
  if (!statusInvoice.value) return
  modalLoading.value = true
  modalError.value = ''
  try {
    await store.updateInvoice(statusInvoice.value.id, { status: newStatus })
    showStatusModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'อัปเดตสถานะไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// ─── Payment Modal ─────────────────────────────────────────────
const showPaymentModal = ref(false)
const payingInvoice = ref<Invoice | null>(null)

function openPaymentModal(inv: Invoice) {
  payingInvoice.value = inv
  modalError.value = ''
  showPaymentModal.value = true
}

async function submitPayment(payload: CreatePaymentPayload) {
  if (!payload.accountId || !payload.amountPaid || !payload.paymentDate || !payload.paymentMethod) {
    modalError.value = 'กรุณากรอกข้อมูลให้ครบ'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    await store.createPayment(payload)
    showPaymentModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// ─── Account Modal ─────────────────────────────────────────────
const showAccountModal = ref(false)

function openAccountModal() {
  modalError.value = ''
  showAccountModal.value = true
}

async function submitAccount(payload: CreateAccountPayload) {
  if (!payload.accountName.trim() || !payload.accountCode.trim()) {
    modalError.value = 'กรุณากรอกชื่อและรหัสบัญชี'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    await store.createAccount(payload)
    showAccountModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

onMounted(async () => {
  await Promise.all([loadInvoices(), store.fetchAccounts()])
})
</script>
