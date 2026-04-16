import http from './http'
import type { PagedResult } from '@/types/pagination'
import type {
  User,
  Role,
  CreateUserPayload,
  UpdateUserPayload,
  CreateRolePayload,
  UpdateRolePayload,
} from '@/types/identity'

export const identityService = {
  // --- Users ---
  async getUsers(): Promise<User[]> {
    const res = await http.get<User[]>('/api/users')
    return res.data
  },
  async searchUsers(filter: {
    searchTerm?: string
    isActive?: boolean | null
    pageNumber?: number
    pageSize?: number
  }): Promise<PagedResult<User>> {
    const res = await http.get<PagedResult<User>>('/api/users/search', { params: filter })
    return res.data
  },
  async getUserById(id: string): Promise<User> {
    const res = await http.get<User>(`/api/users/${id}`)
    return res.data
  },
  async createUser(payload: CreateUserPayload): Promise<{ id: string }> {
    const res = await http.post<{ id: string }>('/api/users', payload)
    return res.data
  },
  async updateUser(id: string, payload: UpdateUserPayload): Promise<void> {
    await http.put(`/api/users/${id}`, payload)
  },
  async deleteUser(id: string): Promise<void> {
    await http.delete(`/api/users/${id}`)
  },
  async assignRole(userId: string, roleId: string): Promise<void> {
    await http.post(`/api/users/${userId}/assign-role/${roleId}`, {})
  },

  // --- Roles ---
  async getRoles(): Promise<Role[]> {
    const res = await http.get<Role[]>('/api/roles')
    return res.data
  },
  async createRole(payload: CreateRolePayload): Promise<{ id: string }> {
    const res = await http.post<{ id: string }>('/api/roles', payload)
    return res.data
  },
  async updateRole(id: string, payload: UpdateRolePayload): Promise<void> {
    await http.put(`/api/roles/${id}`, payload)
  },
  async deleteRole(id: string): Promise<void> {
    await http.delete(`/api/roles/${id}`)
  },
}
