import axios from 'axios'
import { useAuthStore } from '@/stores/authStore'
import router from '@/router'

const http = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:5049',
  headers: { 'Content-Type': 'application/json' },
})

// ใส่ token ทุก request อัตโนมัติ
http.interceptors.request.use((config) => {
  const authStore = useAuthStore()
  if (authStore.token) {
    config.headers.Authorization = `Bearer ${authStore.token}`
  }
  return config
})

// flag ป้องกัน refresh ซ้ำหลายครั้งพร้อมกัน
let isRefreshing = false
let failedQueue: Array<{
  resolve: (token: string) => void
  reject: (err: unknown) => void
}> = []

function processQueue(error: unknown, token: string | null) {
  failedQueue.forEach((p) => {
    if (error) p.reject(error)
    else p.resolve(token!)
  })
  failedQueue = []
}

// ดักจับ 401 แล้ว refresh token อัตโนมัติ
http.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config

    // ถ้าไม่ใช่ 401 หรือเป็น retry แล้ว →던던던 error ออกไปเลย
    if (error.response?.status !== 401 || originalRequest._retry) {
      return Promise.reject(error)
    }

    // ถ้ากำลัง refresh อยู่ → เข้า queue รอ
    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        failedQueue.push({ resolve, reject })
      }).then((token) => {
        originalRequest.headers.Authorization = `Bearer ${token}`
        return http(originalRequest)
      })
    }

    originalRequest._retry = true
    isRefreshing = true

    try {
      const authStore = useAuthStore()
      await authStore.refresh()

      const newToken = authStore.token!
      processQueue(null, newToken)

      originalRequest.headers.Authorization = `Bearer ${newToken}`
      return http(originalRequest)
    } catch (refreshError) {
      processQueue(refreshError, null)

      // refresh token หมดอายุ → logout แล้ว redirect login
      const authStore = useAuthStore()
      await authStore.logout()
      router.push({ name: 'login' })

      return Promise.reject(refreshError)
    } finally {
      isRefreshing = false
    }
  },
)

export default http
