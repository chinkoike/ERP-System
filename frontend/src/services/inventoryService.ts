import http from './http'
import type { PagedResult } from '@/types/pagination'
import type {
  Product,
  Category,
  CreateProductPayload,
  UpdateProductPayload,
  UpdateStockPayload,
  CreateCategoryPayload,
  UpdateCategoryPayload,
} from '@/types/inventory'

export const inventoryService = {
  // --- Products ---
  async getProducts(): Promise<Product[]> {
    const res = await http.get<Product[]>('/api/products')
    return res.data
  },
  async searchProducts(filter: {
    searchTerm?: string
    categoryId?: string
    pageNumber?: number
    pageSize?: number
  }): Promise<PagedResult<Product>> {
    const res = await http.get<PagedResult<Product>>('/api/products/search', { params: filter })
    return res.data
  },
  async getProductById(id: string): Promise<Product> {
    const res = await http.get<Product>(`/api/products/${id}`)
    return res.data
  },
  async getProductsByCategoryId(categoryId: string): Promise<Product[]> {
    const res = await http.get<Product[]>(`/api/products/by-category/${categoryId}`)
    return res.data
  },
  async getLowStockProducts(threshold = 10): Promise<Product[]> {
    const res = await http.get<Product[]>(`/api/products/low-stock?threshold=${threshold}`)
    return res.data
  },
  async createProduct(payload: CreateProductPayload): Promise<{ id: string }> {
    const res = await http.post<{ id: string }>('/api/products', payload)
    return res.data
  },
  async updateProduct(id: string, payload: UpdateProductPayload): Promise<void> {
    await http.put(`/api/products/${id}`, payload)
  },
  async updateStock(id: string, payload: UpdateStockPayload): Promise<void> {
    await http.patch(`/api/products/${id}/stock`, payload)
  },
  async deleteProduct(id: string): Promise<void> {
    await http.delete(`/api/products/${id}`)
  },

  // --- Categories ---
  async getCategories(): Promise<Category[]> {
    const res = await http.get<Category[]>('/api/categories')
    return res.data
  },

  async searchCategories(filter: {
    searchTerm?: string
    pageNumber?: number
    pageSize?: number
  }): Promise<PagedResult<Category>> {
    const res = await http.get<PagedResult<Category>>('/api/categories/search', { params: filter })
    return res.data
  },

  async createCategory(payload: CreateCategoryPayload): Promise<{ id: string }> {
    const res = await http.post<{ id: string }>('/api/categories', payload)
    return res.data
  },
  async updateCategory(id: string, payload: UpdateCategoryPayload): Promise<void> {
    await http.put(`/api/categories/${id}`, payload)
  },
  async deleteCategory(id: string): Promise<void> {
    await http.delete(`/api/categories/${id}`)
  },
}
