<template>
  <div class="min-h-screen bg-slate-50">
    <main class="px-8 py-8 max-w-7xl mx-auto">
      <!-- Heading -->
      <div class="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between mb-6">
        <div>
          <h1 class="text-3xl font-semibold tracking-tight text-slate-900">Users & Roles</h1>
          <p class="mt-2 text-sm text-slate-500">จัดการผู้ใช้งานและสิทธิ์การเข้าถึง</p>
        </div>
        <button
          v-if="activeTab === 'users'"
          @click="openUserModal()"
          class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
        >
          <span class="text-lg leading-none">+</span> เพิ่มผู้ใช้
        </button>
        <button
          v-if="activeTab === 'roles'"
          @click="openRoleModal()"
          class="inline-flex items-center gap-2 rounded-2xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
        >
          <span class="text-lg leading-none">+</span> เพิ่ม Role
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
        <!-- Users Tab -->
        <div v-if="activeTab === 'users'">
          <UserTable
            :users="store.users"
            :roles="store.roles"
            :is-searching="isSearching"
            :total-items="store.totalItems"
            :current-page="store.currentPage"
            :total-pages="store.totalPages"
            v-model:searchUser="searchUser"
            v-model:filterRole="filterRole"
            v-model:filterActive="filterActive"
            @edit="openUserModal"
            @delete="confirmDeleteUser"
            @pageChange="loadUsers"
          />
        </div>

        <!-- Roles Tab -->
        <div v-if="activeTab === 'roles'">
          <RoleList :roles="store.roles" @edit="openRoleModal" @delete="confirmDeleteRole" />
        </div>
      </template>
    </main>

    <!-- User Modal -->
    <UserModal
      :show="showUserModal"
      :editing-user="editingUser"
      :roles="store.roles"
      :loading="modalLoading"
      :error="modalError"
      @close="showUserModal = false"
      @submit="submitUser"
    />

    <!-- Role Modal -->
    <RoleModal
      :show="showRoleModal"
      :editing-role="editingRole"
      :loading="modalLoading"
      :error="modalError"
      @close="showRoleModal = false"
      @submit="submitRole"
    />

    <!-- Confirm Modal -->
    <ConfirmModal
      :show="showConfirmModal"
      :title="confirmTitle"
      :message="confirmMessage"
      :confirm-label="confirmBtn"
      :loading="modalLoading"
      @close="showConfirmModal = false"
      @confirm="runConfirm"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useIdentityStore } from '@/stores/identityStore'
import type {
  User,
  Role,
  CreateUserPayload,
  UpdateUserPayload,
  CreateRolePayload,
  UpdateRolePayload,
} from '@/types/identity'

import UserTable from '@/components/users/UserTable.vue'
import RoleList from '@/components/users/RoleList.vue'
import UserModal from '@/components/users/UserModal.vue'
import RoleModal from '@/components/users/RoleModal.vue'
import ConfirmModal from '@/components/common/ConfirmModal.vue'

const store = useIdentityStore()

const activeTab = ref<'users' | 'roles'>('users')
const tabs = [
  { key: 'users', label: 'ผู้ใช้งาน' },
  { key: 'roles', label: 'Roles' },
] as const

// ─── Users ─────────────────────────────────────────────────────
const searchUser = ref('')
const filterRole = ref('')
const filterActive = ref('')
const loading = ref(true)
const isSearching = ref(false)
async function loadUsers(page = 1) {
  isSearching.value = true
  try {
    await store.fetchUsers({
      searchTerm: searchUser.value.trim() || undefined,
      roleId: filterRole.value || undefined,
      isActive: filterActive.value === '' ? undefined : filterActive.value === 'true',
      pageNumber: page,
    })
  } finally {
    loading.value = false
    isSearching.value = false
  }
}

watch([searchUser, filterRole, filterActive], () => loadUsers(1))

// ─── Shared modal state ────────────────────────────────────────
const modalLoading = ref(false)
const modalError = ref('')

// ─── User Modal ────────────────────────────────────────────────
const showUserModal = ref(false)
const editingUser = ref<User | null>(null)

function openUserModal(u?: User) {
  editingUser.value = u ?? null
  modalError.value = ''
  showUserModal.value = true
}

async function submitUser(payload: CreateUserPayload | UpdateUserPayload) {
  if (
    !('email' in payload && payload.email?.trim()) ||
    !('firstName' in payload && payload.firstName?.trim())
  ) {
    modalError.value = 'กรุณากรอกชื่อและ Email'
    return
  }
  if (!editingUser.value) {
    const p = payload as CreateUserPayload
    if (!p.username?.trim() || !p.password?.trim()) {
      modalError.value = 'กรุณากรอก Username และ Password'
      return
    }
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    if (editingUser.value) {
      await store.updateUser(editingUser.value.id, payload as UpdateUserPayload)
    } else {
      await store.createUser(payload as CreateUserPayload)
    }
    showUserModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// ─── Role Modal ────────────────────────────────────────────────
const showRoleModal = ref(false)
const editingRole = ref<Role | null>(null)

function openRoleModal(r?: Role) {
  editingRole.value = r ?? null
  modalError.value = ''
  showRoleModal.value = true
}

async function submitRole(payload: CreateRolePayload | UpdateRolePayload) {
  if (!payload.name?.trim()) {
    modalError.value = 'กรุณากรอกชื่อ Role'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    if (editingRole.value) {
      await store.updateRole(editingRole.value.id, payload as UpdateRolePayload)
    } else {
      await store.createRole(payload as CreateRolePayload)
    }
    showRoleModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// ─── Confirm Modal ─────────────────────────────────────────────
const showConfirmModal = ref(false)
const confirmTitle = ref('')
const confirmMessage = ref('')
const confirmBtn = ref('')
const confirmAction = ref<() => Promise<void>>(async () => {})

function confirmDeleteUser(u: User) {
  confirmTitle.value = 'ยืนยันการลบผู้ใช้'
  confirmMessage.value = `ต้องการลบ "${u.fullName || u.username}" ใช่หรือไม่?`
  confirmBtn.value = 'ลบ'
  confirmAction.value = async () => {
    await store.deleteUser(u.id)
    showConfirmModal.value = false
  }
  showConfirmModal.value = true
}

function confirmDeleteRole(r: Role) {
  confirmTitle.value = 'ยืนยันการลบ Role'
  confirmMessage.value = `ต้องการลบ "${r.name}" ใช่หรือไม่? User ที่มี Role นี้จะได้รับผลกระทบ`
  confirmBtn.value = 'ลบ'
  confirmAction.value = async () => {
    await store.deleteRole(r.id)
    showConfirmModal.value = false
  }
  showConfirmModal.value = true
}

async function runConfirm() {
  modalLoading.value = true
  try {
    await confirmAction.value()
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'เกิดข้อผิดพลาด'
  } finally {
    modalLoading.value = false
  }
}

onMounted(async () => {
  await Promise.all([loadUsers(), store.fetchRoles()])
})
</script>
