export interface Category {
  id: string
  name: string
  description?: string
  createdAt: string
  createdBy: string
  updatedAt?: string
  updatedBy?: string
}

export interface CreateCategoryPayload {
  name: string
  description?: string
}

export interface UpdateCategoryPayload {
  name?: string
  description?: string
}

export interface Product {
  id: string
  name: string
  sku: string
  imageUrl?: string
  description?: string
  basePrice: number
  price: number
  currentStock: number
  categoryId: string
  categoryName: string
  createdAt: string
  createdBy: string
  updatedAt?: string
  updatedBy?: string
}

export interface CreateProductPayload {
  name: string
  sku: string
  imageUrl?: string
  description?: string
  basePrice: number
  initialStock: number
  categoryId: string
}

export interface UpdateProductPayload {
  name: string
  description?: string
  BasePrice: number
  categoryId: string
}

export interface UpdateStockPayload {
  productId: string
  quantityChange: number
  note?: string
}
