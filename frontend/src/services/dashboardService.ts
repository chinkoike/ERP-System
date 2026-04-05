import http from './http'
import type { DashboardSummary } from '@/types/dashboard'

export const dashboardService = {
  async getSummary(): Promise<DashboardSummary> {
    const res = await http.get<DashboardSummary>('/api/dashboard/summary')
    return res.data
  },
}
