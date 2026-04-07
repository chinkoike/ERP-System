import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authService } from '@/services/authService'
import type { AuthResponse, AuthUser, LoginRequest } from '@/types/auth'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const refreshToken = ref<string | null>(localStorage.getItem('refreshToken'))
  const user = ref<AuthUser | null>(JSON.parse(localStorage.getItem('user') ?? 'null'))
  const loading = ref(false)
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => !!token.value)
  const isAdmin = computed(() => user.value?.roles.includes('Admin') ?? false)

  function setSession(
    data: AuthResponse & { username?: string; email?: string; roles?: string[] },
  ) {
    token.value = data.token
    refreshToken.value = data.refreshToken

    if (data.user) {
      user.value = data.user
    } else if (data.username && data.email && data.roles) {
      user.value = { username: data.username, email: data.email, roles: data.roles }
    }

    if (user.value) {
      localStorage.setItem('user', JSON.stringify(user.value))
    }

    localStorage.setItem('token', data.token)
    localStorage.setItem('refreshToken', data.refreshToken)
  }

  function clearSession() {
    token.value = null
    refreshToken.value = null
    user.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('user')
  }

  async function login(credentials: LoginRequest) {
    loading.value = true
    error.value = null
    try {
      const res = await authService.login(credentials)
      setSession(res)
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'Login failed'
      throw e
    } finally {
      loading.value = false
    }
  }

  async function logout() {
    if (refreshToken.value) {
      await authService.logout(refreshToken.value).catch(() => {})
    }
    clearSession()
  }

  async function refresh() {
    if (!refreshToken.value) throw new Error('No refresh token')
    if (!token.value) throw new Error('No access token')
    const res = await authService.refresh({
      refreshToken: refreshToken.value,
      accessToken: token.value,
    })
    setSession(res)
  }

  return { token, user, loading, error, isAuthenticated, isAdmin, login, logout, refresh }
})
