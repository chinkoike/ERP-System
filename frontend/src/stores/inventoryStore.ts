import { defineStore } from 'pinia'
import { ref } from 'vue'
import { inventoryService } from '@/services/inventoryService'
import type { Product, Category, CreateProductPayload, UpdateProductPayload, UpdateStockPayload, CreateCategoryPayload, UpdateCategoryPayload } from '@/types/inventory'

export const useInventoryStore = defineStore('inventory', () => {
  const products = ref<Product[]>([])
  const categories = ref<Category[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchProducts() {
    loading.value = true
    error.value = null
    try {
      products.value = await inventoryService.getProducts()
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'โหลดสินค้าไม่สำเร็จ'
    } finally {
      loading.value = false
    }
  }

  async function fetchCategories() {
    try {
      categories.value = await inventoryService.getCategories()
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'โหลดหมวดหมู่ไม่สำเร็จ'
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
    products.value = products.value.filter(p => p.id !== id)
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
    categories.value = categories.value.filter(c => c.id !== id)
  }

  return {
    products, categories, loading, error,
    fetchProducts, fetchCategories,
    createProduct, updateProduct, updateStock, deleteProduct,
    createCategory, updateCategory, deleteCategory,
  }
})
