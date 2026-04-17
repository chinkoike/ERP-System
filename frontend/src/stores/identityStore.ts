import { defineStore } from 'pinia'
import { ref } from 'vue'
import { identityService } from '@/services/identityService'
import type {
  User,
  Role,
  CreateUserPayload,
  UpdateUserPayload,
  CreateRolePayload,
  UpdateRolePayload,
} from '@/types/identity'

export const useIdentityStore = defineStore('identity', () => {
  const users = ref<User[]>([])
  const currentPage = ref(1)
  const pageSize = ref(10)
  const totalItems = ref(0)
  const totalPages = ref(1)
  const roles = ref<Role[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchUsers(
    filter: {
      searchTerm?: string
      roleId?: string
      isActive?: boolean | null
      pageNumber?: number
      pageSize?: number
    } = {},
  ) {
    loading.value = true
    error.value = null
    try {
      const result = await identityService.searchUsers({
        searchTerm: filter.searchTerm,
        roleId: filter.roleId,
        isActive: filter.isActive,
        pageNumber: filter.pageNumber ?? currentPage.value,
        pageSize: filter.pageSize ?? pageSize.value,
      })
      users.value = result.items
      currentPage.value = result.pageNumber
      pageSize.value = result.pageSize
      totalItems.value = result.totalCount
      totalPages.value = Math.max(1, result.totalPages)
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'โหลด user ไม่สำเร็จ'
    } finally {
      loading.value = false
    }
  }

  async function fetchRoles() {
    try {
      roles.value = await identityService.getRoles()
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'โหลด role ไม่สำเร็จ'
    }
  }

  async function createUser(payload: CreateUserPayload) {
    await identityService.createUser(payload)
    await fetchUsers()
  }

  async function updateUser(id: string, payload: UpdateUserPayload) {
    await identityService.updateUser(id, payload)
    await fetchUsers()
  }

  async function deleteUser(id: string) {
    await identityService.deleteUser(id)
    users.value = users.value.filter((u) => u.id !== id)
  }

  async function createRole(payload: CreateRolePayload) {
    await identityService.createRole(payload)
    await fetchRoles()
  }

  async function updateRole(id: string, payload: UpdateRolePayload) {
    await identityService.updateRole(id, payload)
    await fetchRoles()
  }

  async function deleteRole(id: string) {
    await identityService.deleteRole(id)
    roles.value = roles.value.filter((r) => r.id !== id)
  }

  return {
    users,
    currentPage,
    pageSize,
    totalItems,
    totalPages,
    roles,
    loading,
    error,
    fetchUsers,
    fetchRoles,
    createUser,
    updateUser,
    deleteUser,
    createRole,
    updateRole,
    deleteRole,
  }
})
