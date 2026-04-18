<template>
  <TransitionRoot appear :show="show" as="template">
    <Dialog as="div" class="relative z-50" @close="$emit('close')">
      <div class="fixed inset-0 bg-black/30" aria-hidden="true" />

      <div class="fixed inset-0 flex items-center justify-center p-4">
        <TransitionChild
          as="template"
          enter="ease-out duration-200"
          enter-from="opacity-0 scale-95"
          enter-to="opacity-100 scale-100"
          leave="ease-in duration-150"
          leave-from="opacity-100 scale-100"
          leave-to="opacity-0 scale-95"
        >
          <DialogPanel class="w-full max-w-lg overflow-hidden rounded-2xl bg-white shadow-2xl">
            <div class="flex items-center justify-between border-b border-slate-200 px-6 py-5">
              <DialogTitle class="text-base font-semibold text-slate-900">
                {{ editingUser ? 'แก้ไขผู้ใช้' : 'เพิ่มผู้ใช้ใหม่' }}
              </DialogTitle>
              <button @click="$emit('close')" class="text-slate-500 hover:text-slate-700 text-2xl leading-none">×</button>
            </div>

            <div class="space-y-4 p-6">
              <!-- Create-only: username + password -->
              <template v-if="!editingUser">
                <div class="grid grid-cols-2 gap-3">
                  <div>
                    <label class="mb-1.5 block text-sm font-medium text-slate-600">Username *</label>
                    <input
                      v-model="local.username"
                      type="text"
                      class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                    />
                  </div>
                  <div>
                    <label class="mb-1.5 block text-sm font-medium text-slate-600">Password *</label>
                    <input
                      v-model="local.password"
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
                    v-model="local.firstName"
                    type="text"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                </div>
                <div>
                  <label class="mb-1.5 block text-sm font-medium text-slate-600">นามสกุล *</label>
                  <input
                    v-model="local.lastName"
                    type="text"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                </div>
              </div>

              <div>
                <label class="mb-1.5 block text-sm font-medium text-slate-600">Email *</label>
                <input
                  v-model="local.email"
                  type="email"
                  class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                />
              </div>

              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="mb-1.5 block text-sm font-medium text-slate-600">ตำแหน่ง</label>
                  <input
                    v-model="local.jobTitle"
                    type="text"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                </div>
                <div>
                  <label class="mb-1.5 block text-sm font-medium text-slate-600">แผนก</label>
                  <input
                    v-model="local.department"
                    type="text"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  />
                </div>
              </div>

              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="mb-1.5 block text-sm font-medium text-slate-600">Role</label>
                  <select
                    v-model="local.roleId"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  >
                    <option value="">ไม่ระบุ</option>
                    <option v-for="r in roles" :key="r.id" :value="r.id">{{ r.name }}</option>
                  </select>
                </div>
                <!-- สถานะ: edit mode เท่านั้น -->
                <div v-if="editingUser">
                  <label class="mb-1.5 block text-sm font-medium text-slate-600">สถานะ</label>
                  <select
                    v-model="local.isActive"
                    class="w-full rounded-2xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 outline-none focus:ring-2 focus:ring-slate-300"
                  >
                    <option :value="true">Active</option>
                    <option :value="false">Inactive</option>
                  </select>
                </div>
              </div>

              <div v-if="error" class="rounded-2xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{{ error }}</div>
            </div>

            <div class="flex justify-end gap-2 border-t border-slate-200 bg-slate-50 px-4 py-4">
              <button
                @click="$emit('close')"
                class="rounded-2xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50"
              >
                ยกเลิก
              </button>
              <button
                @click="handleSubmit"
                :disabled="loading"
                class="rounded-2xl bg-slate-900 px-5 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {{ loading ? 'กำลังบันทึก...' : 'บันทึก' }}
              </button>
            </div>
          </DialogPanel>
        </TransitionChild>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script setup lang="ts">
import { reactive, watch } from 'vue'
import { Dialog, DialogPanel, DialogTitle, TransitionRoot, TransitionChild } from '@headlessui/vue'
import type { User, Role, CreateUserPayload, UpdateUserPayload } from '@/types/identity'

const props = defineProps<{
  show: boolean
  editingUser: User | null
  roles: Role[]
  loading: boolean
  error: string
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit', payload: CreateUserPayload | UpdateUserPayload): void
}>()

const local = reactive({
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

watch(
  () => props.show,
  (opened) => {
    if (!opened) return
    const u = props.editingUser
    Object.assign(local, {
      username: u?.username ?? '',
      password: '',
      email: u?.email ?? '',
      firstName: u?.firstName ?? '',
      lastName: u?.lastName ?? '',
      jobTitle: u?.jobTitle ?? '',
      department: u?.department ?? '',
      roleId: '',
      isActive: u?.isActive ?? true,
    })
  }
)

function handleSubmit() {
  if (props.editingUser) {
    emit('submit', {
      email: local.email,
      firstName: local.firstName,
      lastName: local.lastName,
      isActive: local.isActive,
      roleId: local.roleId || undefined,
      jobTitle: local.jobTitle || undefined,
      department: local.department || undefined,
    } satisfies UpdateUserPayload)
  } else {
    emit('submit', {
      username: local.username,
      password: local.password,
      email: local.email,
      firstName: local.firstName,
      lastName: local.lastName,
      jobTitle: local.jobTitle || undefined,
      department: local.department || undefined,
      roleId: local.roleId || undefined,
    } satisfies CreateUserPayload)
  }
}
</script>
