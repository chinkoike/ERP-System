<template>
  <div class="min-h-screen bg-slate-50">
    <!-- Top bar -->
    <header class="bg-white border-b border-slate-100 px-8 py-4 flex items-center justify-between">
      <div class="flex items-center gap-2">
        <div class="flex h-7 w-7 items-center justify-center rounded-lg bg-slate-900">
          <svg
            width="12"
            height="12"
            fill="none"
            viewBox="0 0 24 24"
            stroke="white"
            stroke-width="2"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10"
            />
          </svg>
        </div>
        <span class="text-sm font-medium text-slate-900">ERP System</span>
        <span class="text-sm text-slate-300 mx-1">/</span>
        <span class="text-sm text-slate-500">Users</span>
      </div>
      <div class="flex items-center gap-3">
        <div
          class="flex h-7 w-7 items-center justify-center rounded-full bg-slate-100 border border-slate-200"
        >
          <span class="text-[11px] font-semibold text-slate-600">{{ userInitial }}</span>
        </div>
        <button
          @click="handleLogout"
          class="text-sm text-slate-500 hover:text-slate-700 transition"
        >
          Sign out
        </button>
      </div>
    </header>

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
        <!-- USERS TAB -->
        <div v-if="activeTab === 'users'">
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
                v-model="searchUser"
                type="text"
                placeholder="ค้นหา username, email..."
                class="w-full rounded-2xl border border-slate-200 bg-white py-2 pl-10 pr-4 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>
            <select
              v-model="filterActive"
              class="rounded-2xl border border-slate-200 bg-white py-2 px-4 text-sm text-slate-700 outline-none cursor-pointer focus:ring-2 focus:ring-slate-300"
            >
              <option value="">ทุกสถานะ</option>
              <option value="true">Active</option>
              <option value="false">Inactive</option>
            </select>
          </div>

          <div class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
            <table class="min-w-full border-collapse">
              <thead>
                <tr class="border-b border-slate-100">
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    ผู้ใช้
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    Email
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    ตำแหน่ง / แผนก
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    Role
                  </th>
                  <th
                    class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    สถานะ
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    Login ล่าสุด
                  </th>
                  <th class="px-6 py-3"></th>
                </tr>
              </thead>
              <tbody>
                <tr v-if="filteredUsers.length === 0">
                  <td colspan="7" class="px-6 py-20 text-center text-sm text-slate-400">
                    ไม่พบผู้ใช้
                  </td>
                </tr>
                <tr
                  v-for="(u, i) in filteredUsers"
                  :key="u.id"
                  :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
                >
                  <td class="px-6 py-4">
                    <div class="flex items-center gap-3">
                      <!-- Avatar -->
                      <div
                        class="flex h-8 w-8 flex-shrink-0 items-center justify-center rounded-full bg-slate-200"
                      >
                        <span class="text-xs font-semibold text-slate-600">{{
                          u.fullName?.charAt(0) ?? u.username.charAt(0).toUpperCase()
                        }}</span>
                      </div>
                      <div>
                        <div class="text-sm font-semibold text-slate-900">
                          {{ u.fullName || u.username }}
                        </div>
                        <div class="text-xs text-slate-400">@{{ u.username }}</div>
                      </div>
                    </div>
                  </td>
                  <td class="px-6 py-4 text-sm text-slate-600">{{ u.email }}</td>
                  <td class="px-6 py-4">
                    <div class="text-sm text-slate-600">{{ u.jobTitle ?? '—' }}</div>
                    <div class="text-xs text-slate-400">{{ u.department ?? '' }}</div>
                  </td>
                  <td class="px-6 py-4">
                    <div class="flex flex-wrap gap-1">
                      <span
                        v-for="role in u.roles"
                        :key="role"
                        :class="
                          role === 'Admin'
                            ? 'inline-flex rounded-full bg-violet-50 px-2 py-0.5 text-xs font-semibold text-violet-600'
                            : role === 'Manager'
                              ? 'inline-flex rounded-full bg-blue-50 px-2 py-0.5 text-xs font-semibold text-blue-600'
                              : 'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-600'
                        "
                      >
                        {{ role }}
                      </span>
                      <span v-if="u.roles.length === 0" class="text-xs text-slate-400">—</span>
                    </div>
                  </td>
                  <td class="px-6 py-4 text-center">
                    <span
                      :class="
                        u.isActive
                          ? 'inline-flex rounded-full bg-emerald-50 px-2 py-0.5 text-xs font-semibold text-emerald-600'
                          : 'inline-flex rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-400'
                      "
                    >
                      {{ u.isActive ? 'Active' : 'Inactive' }}
                    </span>
                  </td>
                  <td class="px-6 py-4 text-sm text-slate-500">
                    {{ u.lastLoginAt ? formatDate(u.lastLoginAt) : '—' }}
                  </td>
                  <td class="px-6 py-4">
                    <div class="flex items-center gap-2 justify-end">
                      <button
                        @click="openUserModal(u)"
                        class="rounded-2xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                      >
                        แก้ไข
                      </button>
                      <button
                        @click="confirmDeleteUser(u)"
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

        <!-- ROLES TAB -->
        <div v-if="activeTab === 'roles'">
          <div class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
            <table class="min-w-full border-collapse">
              <thead>
                <tr class="border-b border-slate-100">
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    ชื่อ Role
                  </th>
                  <th
                    class="px-6 py-3 text-left text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    คำอธิบาย
                  </th>
                  <th
                    class="px-6 py-3 text-center text-[11px] font-semibold uppercase tracking-[0.12em] text-slate-400"
                  >
                    สร้างเมื่อ
                  </th>
                  <th class="px-6 py-3"></th>
                </tr>
              </thead>
              <tbody>
                <tr v-if="store.roles.length === 0">
                  <td colspan="4" class="px-6 py-20 text-center text-sm text-slate-400">
                    ไม่พบ Role
                  </td>
                </tr>
                <tr
                  v-for="(r, i) in store.roles"
                  :key="r.id"
                  :class="[{ 'bg-slate-50': i % 2 === 1 }, 'border-b border-slate-100']"
                >
                  <td class="px-6 py-4">
                    <span
                      :class="
                        r.name === 'Admin'
                          ? 'inline-flex rounded-full bg-violet-50 px-2.5 py-1 text-sm font-semibold text-violet-600'
                          : r.name === 'Manager'
                            ? 'inline-flex rounded-full bg-blue-50 px-2.5 py-1 text-sm font-semibold text-blue-600'
                            : 'inline-flex rounded-full bg-slate-100 px-2.5 py-1 text-sm font-semibold text-slate-600'
                      "
                    >
                      {{ r.name }}
                    </span>
                  </td>
                  <td class="px-6 py-4 text-sm text-slate-500">{{ r.description ?? '—' }}</td>
                  <td class="px-6 py-4 text-center text-sm text-slate-500">
                    {{ formatDate(r.createdAt) }}
                  </td>
                  <td class="px-6 py-4">
                    <div class="flex items-center gap-2 justify-end">
                      <button
                        @click="openRoleModal(r)"
                        class="rounded-2xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-medium text-slate-600 transition hover:bg-slate-50"
                      >
                        แก้ไข
                      </button>
                      <button
                        @click="confirmDeleteRole(r)"
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

    <!-- ===== USER MODAL ===== -->
    <Teleport to="body">
      <div
        v-if="showUserModal"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-lg overflow-hidden rounded-2xl bg-white shadow-2xl">
          <div class="flex items-center justify-between border-b border-slate-200 px-6 py-5">
            <span class="text-base font-semibold text-slate-900">{{
              editingUser ? 'แก้ไขผู้ใช้' : 'เพิ่มผู้ใช้ใหม่'
            }}</span>
            <button
              @click="showUserModal = false"
              class="text-slate-500 hover:text-slate-700 text-2xl"
            >
              ×
            </button>
          </div>
          <div class="space-y-4 p-6">
            <!-- Create only fields -->
            <template v-if="!editingUser">
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="mb-1.5 block text-sm font-medium text-slate-600">Username *</label>
                  <input
                    v-model="userForm.username"
                    type="text"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                </div>
                <div>
                  <label class="mb-1.5 block text-sm font-medium text-slate-600">Password *</label>
                  <input
                    v-model="userForm.password"
                    type="password"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                </div>
              </div>
            </template>

            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">ชื่อ *</label>
                <input
                  v-model="userForm.firstName"
                  type="text"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">นามสกุล *</label>
                <input
                  v-model="userForm.lastName"
                  type="text"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
            </div>

            <div>
              <label class="mb-1.5 block text-sm font-medium text-slate-600">Email *</label>
              <input
                v-model="userForm.email"
                type="email"
                class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>

            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">ตำแหน่ง</label>
                <input
                  v-model="userForm.jobTitle"
                  type="text"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">แผนก</label>
                <input
                  v-model="userForm.department"
                  type="text"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>
            </div>

            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">Role</label>
                <select
                  v-model="userForm.roleId"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                >
                  <option value="">ไม่ระบุ</option>
                  <option v-for="r in store.roles" :key="r.id" :value="r.id">{{ r.name }}</option>
                </select>
              </div>
              <div v-if="editingUser">
                <label class="mb-1.5 block text-sm font-medium text-slate-600">สถานะ</label>
                <select
                  v-model="userForm.isActive"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                >
                  <option :value="true">Active</option>
                  <option :value="false">Inactive</option>
                </select>
              </div>
            </div>

            <div v-if="modalError" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">
              {{ modalError }}
            </div>
          </div>
          <div class="flex justify-end gap-2 border-t border-slate-200 bg-slate-50 px-4 py-4">
            <button
              @click="showUserModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="submitUser"
              :disabled="modalLoading"
              class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ modalLoading ? 'กำลังบันทึก...' : 'บันทึก' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- ===== ROLE MODAL ===== -->
    <Teleport to="body">
      <div
        v-if="showRoleModal"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/30 p-4"
      >
        <div class="w-full max-w-md overflow-hidden rounded-2xl bg-white shadow-2xl">
          <div class="flex items-center justify-between border-b border-slate-200 px-6 py-5">
            <span class="text-base font-semibold text-slate-900">{{
              editingRole ? 'แก้ไข Role' : 'เพิ่ม Role'
            }}</span>
            <button
              @click="showRoleModal = false"
              class="text-slate-500 hover:text-slate-700 text-2xl"
            >
              ×
            </button>
          </div>
          <div class="space-y-4 p-6">
            <div>
              <label class="mb-1.5 block text-sm font-medium text-slate-600">ชื่อ Role *</label>
              <input
                v-model="roleForm.name"
                type="text"
                class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
              />
            </div>
            <div>
              <label class="mb-1.5 block text-sm font-medium text-slate-600">คำอธิบาย</label>
              <textarea
                v-model="roleForm.description"
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
              @click="showRoleModal = false"
              class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
            >
              ยกเลิก
            </button>
            <button
              @click="submitRole"
              :disabled="modalLoading"
              class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed"
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
              @click="runConfirm"
              :disabled="modalLoading"
              class="rounded-2xl bg-rose-600 px-5 py-2 text-sm font-medium text-white transition hover:bg-rose-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ modalLoading ? 'กำลังดำเนินการ...' : 'ยืนยัน' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import { useIdentityStore } from '@/stores/identityStore'
import type { User, Role } from '@/types/identity'

const router = useRouter()
const authStore = useAuthStore()
const store = useIdentityStore()

const userInitial = computed(() => authStore.user?.username?.charAt(0).toUpperCase() ?? 'U')

const activeTab = ref<'users' | 'roles'>('users')
const tabs = [
  { key: 'users', label: 'ผู้ใช้งาน' },
  { key: 'roles', label: 'Roles' },
] as const

// --- Filters ---
const searchUser = ref('')
const filterActive = ref('')
const filteredUsers = computed(() =>
  store.users.filter((u) => {
    const matchSearch =
      u.username.toLowerCase().includes(searchUser.value.toLowerCase()) ||
      u.email.toLowerCase().includes(searchUser.value.toLowerCase()) ||
      (u.fullName ?? '').toLowerCase().includes(searchUser.value.toLowerCase())
    const matchActive = filterActive.value === '' || String(u.isActive) === filterActive.value
    return matchSearch && matchActive
  }),
)

// --- Modal state ---
const modalLoading = ref(false)
const modalError = ref('')

// User Modal
const showUserModal = ref(false)
const editingUser = ref<User | null>(null)
const userForm = reactive({
  username: '',
  password: '',
  email: '',
  firstName: '',
  lastName: '',
  jobTitle: '',
  department: '',
  roleId: '',
  isActive: true,
})

function openUserModal(u?: User) {
  editingUser.value = u ?? null
  modalError.value = ''
  if (u) {
    userForm.username = u.username
    userForm.password = ''
    userForm.email = u.email
    userForm.firstName = u.firstName ?? ''
    userForm.lastName = u.lastName ?? ''
    userForm.jobTitle = u.jobTitle ?? ''
    userForm.department = u.department ?? ''
    userForm.roleId = ''
    userForm.isActive = u.isActive
  } else {
    Object.assign(userForm, {
      username: '',
      password: '',
      email: '',
      firstName: '',
      lastName: '',
      jobTitle: '',
      department: '',
      roleId: '',
      isActive: true,
    })
  }
  showUserModal.value = true
}

async function submitUser() {
  if (!userForm.email.trim() || !userForm.firstName.trim()) {
    modalError.value = 'กรุณากรอกชื่อและ Email'
    return
  }
  if (!editingUser.value && (!userForm.username.trim() || !userForm.password.trim())) {
    modalError.value = 'กรุณากรอก Username และ Password'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    if (editingUser.value) {
      await store.updateUser(editingUser.value.id, {
        email: userForm.email,
        firstName: userForm.firstName,
        lastName: userForm.lastName,
        isActive: userForm.isActive,
        roleId: userForm.roleId || undefined,
        jobTitle: userForm.jobTitle || undefined,
        department: userForm.department || undefined,
      })
    } else {
      await store.createUser({
        username: userForm.username,
        password: userForm.password,
        email: userForm.email,
        firstName: userForm.firstName,
        lastName: userForm.lastName,
        jobTitle: userForm.jobTitle || undefined,
        department: userForm.department || undefined,
        roleId: userForm.roleId || undefined,
      })
    }
    showUserModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// Role Modal
const showRoleModal = ref(false)
const editingRole = ref<Role | null>(null)
const roleForm = reactive({ name: '', description: '' })

function openRoleModal(r?: Role) {
  editingRole.value = r ?? null
  modalError.value = ''
  roleForm.name = r?.name ?? ''
  roleForm.description = r?.description ?? ''
  showRoleModal.value = true
}

async function submitRole() {
  if (!roleForm.name.trim()) {
    modalError.value = 'กรุณากรอกชื่อ Role'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    if (editingRole.value) {
      await store.updateRole(editingRole.value.id, {
        name: roleForm.name,
        description: roleForm.description,
      })
    } else {
      await store.createRole({ name: roleForm.name, description: roleForm.description })
    }
    showRoleModal.value = false
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
const confirmAction = ref<() => Promise<void>>(async () => {})

function confirmDeleteUser(u: User) {
  confirmTitle.value = 'ยืนยันการลบผู้ใช้'
  confirmMessage.value = `ต้องการลบ "${u.fullName || u.username}" ใช่หรือไม่?`
  confirmAction.value = async () => {
    await store.deleteUser(u.id)
    showConfirmModal.value = false
  }
  showConfirmModal.value = true
}

function confirmDeleteRole(r: Role) {
  confirmTitle.value = 'ยืนยันการลบ Role'
  confirmMessage.value = `ต้องการลบ "${r.name}" ใช่หรือไม่? User ที่มี Role นี้จะได้รับผลกระทบ`
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

function formatDate(d: string) {
  return new Date(d).toLocaleDateString('th-TH', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  })
}

async function handleLogout() {
  await authStore.logout()
  router.push({ name: 'login' })
}

onMounted(async () => {
  await Promise.all([store.fetchUsers(), store.fetchRoles()])
})
</script>
