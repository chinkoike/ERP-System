import http from './http'
import type { LoginRequest, AuthResponse, RefreshTokenRequest } from '@/types/auth'

export const authService = {
  async login(data: LoginRequest): Promise<AuthResponse> {
    const res = await http.post<AuthResponse>('/api/users/login', data)
    return res.data
  },

  async refresh(data: RefreshTokenRequest): Promise<AuthResponse> {
    const res = await http.post<AuthResponse>('/api/users/refresh-token', data)
    return res.data
  },

  async logout(refreshToken: string): Promise<void> {
    await http.post('/api/users/logout', { refreshToken })
  },
}
