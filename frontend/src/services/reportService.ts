import http from './http'
import type { SalesReport, InventoryStatus, FinancialSummary } from '@/types/report'

export const reportService = {
  async getSalesReport(startDate?: string, endDate?: string): Promise<SalesReport> {
    const params = new URLSearchParams()
    if (startDate) params.append('startDate', startDate)
    if (endDate) params.append('endDate', endDate)
    const res = await http.get<SalesReport>(`/api/report/sales?${params}`)
    return res.data
  },

  async getInventoryStatus(): Promise<InventoryStatus[]> {
    const res = await http.get<InventoryStatus[]>('/api/report/inventory')
    return res.data
  },

  async getFinancialSummary(startDate?: string, endDate?: string): Promise<FinancialSummary> {
    const params = new URLSearchParams()
    if (startDate) params.append('startDate', startDate)
    if (endDate) params.append('endDate', endDate)
    const res = await http.get<FinancialSummary>(`/api/report/financial?${params}`)
    return res.data
  },
}
