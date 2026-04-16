import { defineStore } from 'pinia'
import { ref } from 'vue'
import { inventoryService } from '@/services/inventoryService'
import type {
  Product,
  Category,
  CreateProductPayload,
  UpdateProductPayload,
  UpdateStockPayload,
  CreateCategoryPayload,
  UpdateCategoryPayload,
} from '@/types/inventory'

export const useInventoryStore = defineStore('inventory', () => {
  const products = ref<Product[]>([])
  const currentPage = ref(1)
  const pageSize = ref(10)
  const totalItems = ref(0)
  const totalPages = ref(1)

  // --- Category State ---
  const categories = ref<Category[]>([])
  const categoryPage = ref(1)
  const categoryPageSize = ref(10)
  const categoryTotalItems = ref(0)
  const categoryTotalPages = ref(1)

  // --- Global State ---
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchProducts(
    filter: {
      searchTerm?: string
      categoryId?: string
      pageNumber?: number
      pageSize?: number
    } = {},
  ) {
    loading.value = true
    error.value = null
    try {
      const result = await inventoryService.searchProducts({
        searchTerm: filter.searchTerm,
        categoryId: filter.categoryId,
        pageNumber: filter.pageNumber ?? currentPage.value,
        pageSize: filter.pageSize ?? pageSize.value,
      })
      products.value = result.items
      currentPage.value = result.pageNumber
      pageSize.value = result.pageSize
      totalItems.value = result.totalCount
      totalPages.value = Math.max(1, result.totalPages)
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'โหลดสินค้าไม่สำเร็จ'
    } finally {
      loading.value = false
    }
  }

  async function fetchCategories(
    filter: {
      searchTerm?: string
      pageNumber?: number
      pageSize?: number
    } = {},
  ) {
    loading.value = true
    error.value = null
    try {
      const result = await inventoryService.searchCategories({
        searchTerm: filter.searchTerm,
        pageNumber: filter.pageNumber ?? categoryPage.value,
        pageSize: filter.pageSize ?? categoryPageSize.value,
      })

      categories.value = result.items
      categoryPage.value = result.pageNumber
      categoryPageSize.value = result.pageSize
      categoryTotalItems.value = result.totalCount
      categoryTotalPages.value = Math.max(1, result.totalPages)
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'โหลดหมวดหมู่ไม่สำเร็จ'
    } finally {
      loading.value = false
    }
  }

  async function createProduct(payload: CreateProductPayload) {
    await inventoryService.createProduct(payload)
    await fetchProducts()
  }

  async function updateProduct(id: string, payload: UpdateProductPayload) {
    await inventoryService.updateProduct(id, payload)
    await fetchProducts()
  }

  async function updateStock(id: string, payload: UpdateStockPayload) {
    await inventoryService.updateStock(id, payload)
    await fetchProducts()
  }

  async function deleteProduct(id: string) {
    await inventoryService.deleteProduct(id)
    products.value = products.value.filter((p) => p.id !== id)
  }

  async function createCategory(payload: CreateCategoryPayload) {
    await inventoryService.createCategory(payload)
    await fetchCategories()
  }

  async function updateCategory(id: string, payload: UpdateCategoryPayload) {
    await inventoryService.updateCategory(id, payload)
    await fetchCategories()
  }

  async function deleteCategory(id: string) {
    await inventoryService.deleteCategory(id)
    categories.value = categories.value.filter((c) => c.id !== id)
  }

  return {
    products,
    currentPage,
    pageSize,
    totalItems,
    totalPages,
    // Category State & Pagination
    categories,
    categoryPage,
    categoryPageSize,
    categoryTotalItems,
    categoryTotalPages,
    loading,
    error,
    fetchProducts,
    fetchCategories,
    createProduct,
    updateProduct,
    updateStock,
    deleteProduct,
    createCategory,
    updateCategory,
    deleteCategory,
  }
})
