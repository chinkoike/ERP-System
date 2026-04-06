<template>
  <div class="min-h-screen" style="background: #f7f6f3; font-family: 'DM Sans', sans-serif">
    <!-- Top bar -->
    <header
      style="background: white; border-bottom: 0.5px solid rgba(0, 0, 0, 0.08)"
      class="px-8 py-3 flex items-center justify-between"
    >
      <div class="flex items-center gap-2">
        <div
          style="width: 26px; height: 26px; background: #0f0f0f; border-radius: 6px"
          class="flex items-center justify-center"
        >
          <svg
            width="12"
            height="12"
            fill="none"
            viewBox="0 0 24 24"
            stroke="white"
            stroke-width="2"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10"
            />
          </svg>
        </div>
        <span style="font-size: 13px; font-weight: 500">ERP System</span>
        <span style="color: rgba(0, 0, 0, 0.2); margin: 0 4px">/</span>
        <span style="font-size: 13px; color: #9a9a9a">Inventory</span>
      </div>
      <div class="flex items-center gap-3">
        <div
          style="
            width: 28px;
            height: 28px;
            border-radius: 50%;
            background: #f0efe9;
            border: 0.5px solid rgba(0, 0, 0, 0.1);
          "
          class="flex items-center justify-center"
        >
          <span style="font-size: 11px; font-weight: 500; color: #5a5a5a">{{ userInitial }}</span>
        </div>
        <button @click="handleLogout" style="font-size: 13px; color: #9a9a9a">Sign out</button>
      </div>
    </header>

    <main class="px-8 py-8 max-w-7xl mx-auto">
      <!-- Page heading -->
      <div class="flex items-end justify-between mb-6">
        <div>
          <h1 style="font-size: 22px; font-weight: 300; letter-spacing: -0.03em; color: #0f0f0f">
            Inventory
          </h1>
          <p style="font-size: 13px; color: #9a9a9a; margin-top: 2px">จัดการสินค้าและหมวดหมู่</p>
        </div>
        <button
          @click="openProductModal()"
          style="
            background: #0f0f0f;
            color: white;
            border: none;
            border-radius: 8px;
            padding: 9px 16px;
            font-size: 13px;
            font-weight: 500;
            cursor: pointer;
            display: flex;
            align-items: center;
            gap: 6px;
          "
        >
          <span style="font-size: 16px; line-height: 1">+</span> เพิ่มสินค้า
        </button>
      </div>

      <!-- Tabs -->
      <div
        style="
          display: flex;
          gap: 0;
          background: white;
          border: 0.5px solid rgba(0, 0, 0, 0.08);
          border-radius: 10px;
          padding: 4px;
          width: fit-content;
          margin-bottom: 1.5rem;
        "
      >
        <button
          v-for="tab in tabs"
          :key="tab.key"
          @click="activeTab = tab.key"
          :style="
            activeTab === tab.key
              ? 'background:#0f0f0f;color:white;border-radius:7px;'
              : 'background:transparent;color:#9a9a9a;'
          "
          style="
            border: none;
            padding: 7px 18px;
            font-size: 13px;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.15s;
          "
        >
          {{ tab.label }}
        </button>
      </div>

      <!-- Loading -->
      <div v-if="store.loading" class="flex items-center justify-center py-24">
        <svg
          class="animate-spin"
          style="width: 20px; height: 20px; color: #d1d0cb"
          fill="none"
          viewBox="0 0 24 24"
        >
          <circle
            style="opacity: 0.25"
            cx="12"
            cy="12"
            r="10"
            stroke="currentColor"
            stroke-width="4"
          />
          <path
            style="opacity: 0.75"
            fill="currentColor"
            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"
          />
        </svg>
      </div>

      <template v-else>
        <!-- PRODUCTS TAB -->
        <div v-if="activeTab === 'products'">
          <!-- Filter row -->
          <div class="flex items-center gap-3 mb-4">
            <div style="position: relative; flex: 1; max-width: 320px">
              <svg
                style="
                  position: absolute;
                  left: 10px;
                  top: 50%;
                  transform: translateY(-50%);
                  width: 14px;
                  height: 14px;
                  color: #9a9a9a;
                "
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0"
                />
              </svg>
              <input
                v-model="searchProduct"
                type="text"
                placeholder="ค้นหาสินค้า..."
                style="
                  width: 100%;
                  padding: 8px 12px 8px 32px;
                  border: 0.5px solid rgba(0, 0, 0, 0.1);
                  border-radius: 8px;
                  font-size: 13px;
                  background: white;
                  outline: none;
                  color: #0f0f0f;
                "
              />
            </div>
            <select
              v-model="filterCategory"
              style="
                padding: 8px 12px;
                border: 0.5px solid rgba(0, 0, 0, 0.1);
                border-radius: 8px;
                font-size: 13px;
                background: white;
                color: #5a5a5a;
                outline: none;
                cursor: pointer;
              "
            >
              <option value="">ทุกหมวดหมู่</option>
              <option v-for="c in store.categories" :key="c.id" :value="c.id">{{ c.name }}</option>
            </select>
          </div>

          <!-- Products table -->
          <div
            style="
              background: white;
              border-radius: 12px;
              border: 0.5px solid rgba(0, 0, 0, 0.08);
              overflow: hidden;
            "
          >
            <table style="width: 100%; border-collapse: collapse">
              <thead>
                <tr style="border-bottom: 0.5px solid rgba(0, 0, 0, 0.06)">
                  <th
                    style="
                      text-align: left;
                      padding: 10px 16px;
                      font-size: 11px;
                      font-weight: 500;
                      color: #9a9a9a;
                      letter-spacing: 0.05em;
                      text-transform: uppercase;
                    "
                  >
                    สินค้า
                  </th>
                  <th
                    style="
                      text-align: left;
                      padding: 10px 16px;
                      font-size: 11px;
                      font-weight: 500;
                      color: #9a9a9a;
                      letter-spacing: 0.05em;
                      text-transform: uppercase;
                    "
                  >
                    SKU
                  </th>
                  <th
                    style="
                      text-align: left;
                      padding: 10px 16px;
                      font-size: 11px;
                      font-weight: 500;
                      color: #9a9a9a;
                      letter-spacing: 0.05em;
                      text-transform: uppercase;
                    "
                  >
                    หมวดหมู่
                  </th>
                  <th
                    style="
                      text-align: right;
                      padding: 10px 16px;
                      font-size: 11px;
                      font-weight: 500;
                      color: #9a9a9a;
                      letter-spacing: 0.05em;
                      text-transform: uppercase;
                    "
                  >
                    ราคา
                  </th>
                  <th
                    style="
                      text-align: center;
                      padding: 10px 16px;
                      font-size: 11px;
                      font-weight: 500;
                      color: #9a9a9a;
                      letter-spacing: 0.05em;
                      text-transform: uppercase;
                    "
                  >
                    Stock
                  </th>
                  <th
                    style="
                      text-align: center;
                      padding: 10px 16px;
                      font-size: 11px;
                      font-weight: 500;
                      color: #9a9a9a;
                      letter-spacing: 0.05em;
                      text-transform: uppercase;
                    "
                  >
                    สถานะ
                  </th>
                  <th style="padding: 10px 16px"></th>
                </tr>
              </thead>
              <tbody>
                <tr v-if="filteredProducts.length === 0">
                  <td
                    colspan="7"
                    style="text-align: center; padding: 3rem; font-size: 13px; color: #9a9a9a"
                  >
                    ไม่พบสินค้า
                  </td>
                </tr>
                <tr
                  v-for="(p, i) in filteredProducts"
                  :key="p.id"
                  :style="i % 2 === 1 ? 'background:#fafaf8;' : ''"
                  style="border-bottom: 0.5px solid rgba(0, 0, 0, 0.04)"
                >
                  <td style="padding: 12px 16px">
                    <div style="font-size: 13px; font-weight: 500; color: #0f0f0f">
                      {{ p.name }}
                    </div>
                    <div
                      v-if="p.description"
                      style="
                        font-size: 11px;
                        color: #9a9a9a;
                        margin-top: 2px;
                        white-space: nowrap;
                        overflow: hidden;
                        text-overflow: ellipsis;
                        max-width: 200px;
                      "
                    >
                      {{ p.description }}
                    </div>
                  </td>
                  <td
                    style="
                      padding: 12px 16px;
                      font-family: 'DM Mono', monospace;
                      font-size: 12px;
                      color: #5a5a5a;
                    "
                  >
                    {{ p.sku }}
                  </td>
                  <td style="padding: 12px 16px; font-size: 13px; color: #5a5a5a">
                    {{ p.categoryName }}
                  </td>
                  <td
                    style="
                      padding: 12px 16px;
                      font-size: 13px;
                      font-weight: 500;
                      color: #0f0f0f;
                      text-align: right;
                    "
                  >
                    {{ formatCurrency(p.basePrice) }}
                  </td>
                  <td style="padding: 12px 16px; text-align: center">
                    <span
                      style="font-size: 13px; font-weight: 500"
                      :style="p.currentStock <= 10 ? 'color:#dc2626;' : 'color:#0f0f0f;'"
                      >{{ p.currentStock }}</span
                    >
                  </td>
                  <td style="padding: 12px 16px; text-align: center">
                    <span :style="stockStatusStyle(p.currentStock)">{{
                      stockStatusLabel(p.currentStock)
                    }}</span>
                  </td>
                  <td style="padding: 12px 16px">
                    <div class="flex items-center gap-2 justify-end">
                      <button
                        @click="openStockModal(p)"
                        title="ปรับ Stock"
                        style="
                          border: 0.5px solid rgba(0, 0, 0, 0.1);
                          background: white;
                          border-radius: 6px;
                          padding: 5px 8px;
                          font-size: 12px;
                          cursor: pointer;
                          color: #5a5a5a;
                        "
                      >
                        ±Stock
                      </button>
                      <button
                        @click="openProductModal(p)"
                        style="
                          border: 0.5px solid rgba(0, 0, 0, 0.1);
                          background: white;
                          border-radius: 6px;
                          padding: 5px 8px;
                          font-size: 12px;
                          cursor: pointer;
                          color: #5a5a5a;
                        "
                      >
                        แก้ไข
                      </button>
                      <button
                        @click="confirmDelete('product', p.id, p.name)"
                        style="
                          border: 0.5px solid rgba(220, 38, 38, 0.2);
                          background: white;
                          border-radius: 6px;
                          padding: 5px 8px;
                          font-size: 12px;
                          cursor: pointer;
                          color: #dc2626;
                        "
                      >
                        ลบ
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- CATEGORIES TAB -->
        <div v-if="activeTab === 'categories'">
          <div class="flex justify-end mb-4">
            <button
              @click="openCategoryModal()"
              style="
                background: #0f0f0f;
                color: white;
                border: none;
                border-radius: 8px;
                padding: 9px 16px;
                font-size: 13px;
                font-weight: 500;
                cursor: pointer;
                display: flex;
                align-items: center;
                gap: 6px;
              "
            >
              <span style="font-size: 16px; line-height: 1">+</span> เพิ่มหมวดหมู่
            </button>
          </div>
          <div
            style="
              background: white;
              border-radius: 12px;
              border: 0.5px solid rgba(0, 0, 0, 0.08);
              overflow: hidden;
            "
          >
            <table style="width: 100%; border-collapse: collapse">
              <thead>
                <tr style="border-bottom: 0.5px solid rgba(0, 0, 0, 0.06)">
                  <th
                    style="
                      text-align: left;
                      padding: 10px 16px;
                      font-size: 11px;
                      font-weight: 500;
                      color: #9a9a9a;
                      letter-spacing: 0.05em;
                      text-transform: uppercase;
                    "
                  >
                    ชื่อหมวดหมู่
                  </th>
                  <th
                    style="
                      text-align: left;
                      padding: 10px 16px;
                      font-size: 11px;
                      font-weight: 500;
                      color: #9a9a9a;
                      letter-spacing: 0.05em;
                      text-transform: uppercase;
                    "
                  >
                    คำอธิบาย
                  </th>
                  <th
                    style="
                      text-align: center;
                      padding: 10px 16px;
                      font-size: 11px;
                      font-weight: 500;
                      color: #9a9a9a;
                      letter-spacing: 0.05em;
                      text-transform: uppercase;
                    "
                  >
                    สร้างเมื่อ
                  </th>
                  <th style="padding: 10px 16px"></th>
                </tr>
              </thead>
              <tbody>
                <tr v-if="store.categories.length === 0">
                  <td
                    colspan="4"
                    style="text-align: center; padding: 3rem; font-size: 13px; color: #9a9a9a"
                  >
                    ไม่พบหมวดหมู่
                  </td>
                </tr>
                <tr
                  v-for="(c, i) in store.categories"
                  :key="c.id"
                  :style="i % 2 === 1 ? 'background:#fafaf8;' : ''"
                  style="border-bottom: 0.5px solid rgba(0, 0, 0, 0.04)"
                >
                  <td style="padding: 12px 16px; font-size: 13px; font-weight: 500; color: #0f0f0f">
                    {{ c.name }}
                  </td>
                  <td style="padding: 12px 16px; font-size: 13px; color: #9a9a9a">
                    {{ c.description ?? '—' }}
                  </td>
                  <td
                    style="padding: 12px 16px; font-size: 12px; color: #9a9a9a; text-align: center"
                  >
                    {{ formatDate(c.createdAt) }}
                  </td>
                  <td style="padding: 12px 16px">
                    <div class="flex items-center gap-2 justify-end">
                      <button
                        @click="openCategoryModal(c)"
                        style="
                          border: 0.5px solid rgba(0, 0, 0, 0.1);
                          background: white;
                          border-radius: 6px;
                          padding: 5px 8px;
                          font-size: 12px;
                          cursor: pointer;
                          color: #5a5a5a;
                        "
                      >
                        แก้ไข
                      </button>
                      <button
                        @click="confirmDelete('category', c.id, c.name)"
                        style="
                          border: 0.5px solid rgba(220, 38, 38, 0.2);
                          background: white;
                          border-radius: 6px;
                          padding: 5px 8px;
                          font-size: 12px;
                          cursor: pointer;
                          color: #dc2626;
                        "
                      >
                        ลบ
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </template>
    </main>

    <!-- ===== PRODUCT MODAL ===== -->
    <Teleport to="body">
      <div
        v-if="showProductModal"
        style="
          position: fixed;
          inset: 0;
          background: rgba(0, 0, 0, 0.3);
          display: flex;
          align-items: center;
          justify-content: center;
          z-index: 50;
          padding: 1rem;
        "
      >
        <div
          style="
            background: white;
            border-radius: 16px;
            width: 100%;
            max-width: 480px;
            overflow: hidden;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.12);
          "
        >
          <div
            style="
              padding: 1.25rem 1.5rem;
              border-bottom: 0.5px solid rgba(0, 0, 0, 0.08);
              display: flex;
              justify-content: space-between;
              align-items: center;
            "
          >
            <span style="font-size: 15px; font-weight: 500; color: #0f0f0f">{{
              editingProduct ? 'แก้ไขสินค้า' : 'เพิ่มสินค้า'
            }}</span>
            <button
              @click="showProductModal = false"
              style="
                border: none;
                background: none;
                cursor: pointer;
                color: #9a9a9a;
                font-size: 18px;
              "
            >
              ×
            </button>
          </div>
          <div style="padding: 1.5rem; display: flex; flex-direction: column; gap: 1rem">
            <div>
              <label
                style="
                  font-size: 12px;
                  font-weight: 500;
                  color: #5a5a5a;
                  display: block;
                  margin-bottom: 5px;
                "
                >ชื่อสินค้า *</label
              >
              <input
                v-model="productForm.name"
                type="text"
                style="
                  width: 100%;
                  padding: 9px 12px;
                  border: 0.5px solid rgba(0, 0, 0, 0.12);
                  border-radius: 8px;
                  font-size: 13px;
                  outline: none;
                  color: #0f0f0f;
                "
              />
            </div>
            <div v-if="!editingProduct">
              <label
                style="
                  font-size: 12px;
                  font-weight: 500;
                  color: #5a5a5a;
                  display: block;
                  margin-bottom: 5px;
                "
                >SKU *</label
              >
              <input
                v-model="productForm.sku"
                type="text"
                style="
                  width: 100%;
                  padding: 9px 12px;
                  border: 0.5px solid rgba(0, 0, 0, 0.12);
                  border-radius: 8px;
                  font-size: 13px;
                  font-family: 'DM Mono', monospace;
                  outline: none;
                  color: #0f0f0f;
                "
              />
            </div>
            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 12px">
              <div>
                <label
                  style="
                    font-size: 12px;
                    font-weight: 500;
                    color: #5a5a5a;
                    display: block;
                    margin-bottom: 5px;
                  "
                  >ราคา (บาท) *</label
                >
                <input
                  v-model.number="productForm.basePrice"
                  type="number"
                  min="0"
                  style="
                    width: 100%;
                    padding: 9px 12px;
                    border: 0.5px solid rgba(0, 0, 0, 0.12);
                    border-radius: 8px;
                    font-size: 13px;
                    outline: none;
                    color: #0f0f0f;
                  "
                />
              </div>
              <div v-if="!editingProduct">
                <label
                  style="
                    font-size: 12px;
                    font-weight: 500;
                    color: #5a5a5a;
                    display: block;
                    margin-bottom: 5px;
                  "
                  >Stock เริ่มต้น</label
                >
                <input
                  v-model.number="productForm.initialStock"
                  type="number"
                  min="0"
                  style="
                    width: 100%;
                    padding: 9px 12px;
                    border: 0.5px solid rgba(0, 0, 0, 0.12);
                    border-radius: 8px;
                    font-size: 13px;
                    outline: none;
                    color: #0f0f0f;
                  "
                />
              </div>
            </div>
            <div>
              <label
                style="
                  font-size: 12px;
                  font-weight: 500;
                  color: #5a5a5a;
                  display: block;
                  margin-bottom: 5px;
                "
                >หมวดหมู่ *</label
              >
              <select
                v-model="productForm.categoryId"
                style="
                  width: 100%;
                  padding: 9px 12px;
                  border: 0.5px solid rgba(0, 0, 0, 0.12);
                  border-radius: 8px;
                  font-size: 13px;
                  outline: none;
                  background: white;
                  color: #0f0f0f;
                "
              >
                <option value="">เลือกหมวดหมู่</option>
                <option v-for="c in store.categories" :key="c.id" :value="c.id">
                  {{ c.name }}
                </option>
              </select>
            </div>
            <div>
              <label
                style="
                  font-size: 12px;
                  font-weight: 500;
                  color: #5a5a5a;
                  display: block;
                  margin-bottom: 5px;
                "
                >คำอธิบาย</label
              >
              <textarea
                v-model="productForm.description"
                rows="2"
                style="
                  width: 100%;
                  padding: 9px 12px;
                  border: 0.5px solid rgba(0, 0, 0, 0.12);
                  border-radius: 8px;
                  font-size: 13px;
                  outline: none;
                  resize: vertical;
                  color: #0f0f0f;
                "
              ></textarea>
            </div>
            <div
              v-if="modalError"
              style="
                font-size: 12px;
                color: #dc2626;
                background: #fef2f2;
                padding: 8px 12px;
                border-radius: 6px;
              "
            >
              {{ modalError }}
            </div>
          </div>
          <div
            style="
              padding: 1rem 1.5rem;
              border-top: 0.5px solid rgba(0, 0, 0, 0.06);
              display: flex;
              gap: 8px;
              justify-content: flex-end;
            "
          >
            <button
              @click="showProductModal = false"
              style="
                border: 0.5px solid rgba(0, 0, 0, 0.1);
                background: white;
                border-radius: 8px;
                padding: 8px 16px;
                font-size: 13px;
                cursor: pointer;
                color: #5a5a5a;
              "
            >
              ยกเลิก
            </button>
            <button
              @click="submitProduct"
              :disabled="modalLoading"
              style="
                background: #0f0f0f;
                color: white;
                border: none;
                border-radius: 8px;
                padding: 8px 20px;
                font-size: 13px;
                font-weight: 500;
                cursor: pointer;
                opacity: 1;
              "
              :style="modalLoading ? 'opacity:0.5;cursor:not-allowed;' : ''"
            >
              {{ modalLoading ? 'กำลังบันทึก...' : 'บันทึก' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- ===== STOCK MODAL ===== -->
    <Teleport to="body">
      <div
        v-if="showStockModal"
        style="
          position: fixed;
          inset: 0;
          background: rgba(0, 0, 0, 0.3);
          display: flex;
          align-items: center;
          justify-content: center;
          z-index: 50;
          padding: 1rem;
        "
      >
        <div
          style="
            background: white;
            border-radius: 16px;
            width: 100%;
            max-width: 360px;
            overflow: hidden;
          "
        >
          <div
            style="
              padding: 1.25rem 1.5rem;
              border-bottom: 0.5px solid rgba(0, 0, 0, 0.08);
              display: flex;
              justify-content: space-between;
              align-items: center;
            "
          >
            <span style="font-size: 15px; font-weight: 500; color: #0f0f0f">ปรับ Stock</span>
            <button
              @click="showStockModal = false"
              style="
                border: none;
                background: none;
                cursor: pointer;
                color: #9a9a9a;
                font-size: 18px;
              "
            >
              ×
            </button>
          </div>
          <div style="padding: 1.5rem; display: flex; flex-direction: column; gap: 1rem">
            <div style="background: #f7f6f3; border-radius: 8px; padding: 12px">
              <div style="font-size: 13px; font-weight: 500; color: #0f0f0f">
                {{ stockTarget?.name }}
              </div>
              <div style="font-size: 12px; color: #9a9a9a; margin-top: 2px">
                Stock ปัจจุบัน:
                <strong style="color: #0f0f0f">{{ stockTarget?.currentStock }}</strong>
              </div>
            </div>
            <div>
              <label
                style="
                  font-size: 12px;
                  font-weight: 500;
                  color: #5a5a5a;
                  display: block;
                  margin-bottom: 5px;
                "
                >จำนวนที่เปลี่ยน (+ เพิ่ม / - ลด)</label
              >
              <input
                v-model.number="stockForm.quantityChange"
                type="number"
                style="
                  width: 100%;
                  padding: 9px 12px;
                  border: 0.5px solid rgba(0, 0, 0, 0.12);
                  border-radius: 8px;
                  font-size: 13px;
                  outline: none;
                  color: #0f0f0f;
                "
              />
              <div v-if="stockTarget" style="font-size: 11px; color: #9a9a9a; margin-top: 4px">
                Stock ใหม่:
                <strong
                  :style="
                    stockTarget.currentStock + stockForm.quantityChange < 0
                      ? 'color:#dc2626'
                      : 'color:#0f0f0f'
                  "
                  >{{ stockTarget.currentStock + stockForm.quantityChange }}</strong
                >
              </div>
            </div>
            <div>
              <label
                style="
                  font-size: 12px;
                  font-weight: 500;
                  color: #5a5a5a;
                  display: block;
                  margin-bottom: 5px;
                "
                >หมายเหตุ</label
              >
              <input
                v-model="stockForm.note"
                type="text"
                placeholder="เช่น รับสินค้าใหม่, ของเสียหาย"
                style="
                  width: 100%;
                  padding: 9px 12px;
                  border: 0.5px solid rgba(0, 0, 0, 0.12);
                  border-radius: 8px;
                  font-size: 13px;
                  outline: none;
                  color: #0f0f0f;
                "
              />
            </div>
            <div
              v-if="modalError"
              style="
                font-size: 12px;
                color: #dc2626;
                background: #fef2f2;
                padding: 8px 12px;
                border-radius: 6px;
              "
            >
              {{ modalError }}
            </div>
          </div>
          <div
            style="
              padding: 1rem 1.5rem;
              border-top: 0.5px solid rgba(0, 0, 0, 0.06);
              display: flex;
              gap: 8px;
              justify-content: flex-end;
            "
          >
            <button
              @click="showStockModal = false"
              style="
                border: 0.5px solid rgba(0, 0, 0, 0.1);
                background: white;
                border-radius: 8px;
                padding: 8px 16px;
                font-size: 13px;
                cursor: pointer;
                color: #5a5a5a;
              "
            >
              ยกเลิก
            </button>
            <button
              @click="submitStock"
              :disabled="modalLoading"
              style="
                background: #0f0f0f;
                color: white;
                border: none;
                border-radius: 8px;
                padding: 8px 20px;
                font-size: 13px;
                font-weight: 500;
                cursor: pointer;
              "
              :style="modalLoading ? 'opacity:0.5;cursor:not-allowed;' : ''"
            >
              {{ modalLoading ? 'กำลังบันทึก...' : 'บันทึก' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- ===== CATEGORY MODAL ===== -->
    <Teleport to="body">
      <div
        v-if="showCategoryModal"
        style="
          position: fixed;
          inset: 0;
          background: rgba(0, 0, 0, 0.3);
          display: flex;
          align-items: center;
          justify-content: center;
          z-index: 50;
          padding: 1rem;
        "
      >
        <div
          style="
            background: white;
            border-radius: 16px;
            width: 100%;
            max-width: 400px;
            overflow: hidden;
          "
        >
          <div
            style="
              padding: 1.25rem 1.5rem;
              border-bottom: 0.5px solid rgba(0, 0, 0, 0.08);
              display: flex;
              justify-content: space-between;
              align-items: center;
            "
          >
            <span style="font-size: 15px; font-weight: 500; color: #0f0f0f">{{
              editingCategory ? 'แก้ไขหมวดหมู่' : 'เพิ่มหมวดหมู่'
            }}</span>
            <button
              @click="showCategoryModal = false"
              style="
                border: none;
                background: none;
                cursor: pointer;
                color: #9a9a9a;
                font-size: 18px;
              "
            >
              ×
            </button>
          </div>
          <div style="padding: 1.5rem; display: flex; flex-direction: column; gap: 1rem">
            <div>
              <label
                style="
                  font-size: 12px;
                  font-weight: 500;
                  color: #5a5a5a;
                  display: block;
                  margin-bottom: 5px;
                "
                >ชื่อหมวดหมู่ *</label
              >
              <input
                v-model="categoryForm.name"
                type="text"
                style="
                  width: 100%;
                  padding: 9px 12px;
                  border: 0.5px solid rgba(0, 0, 0, 0.12);
                  border-radius: 8px;
                  font-size: 13px;
                  outline: none;
                  color: #0f0f0f;
                "
              />
            </div>
            <div>
              <label
                style="
                  font-size: 12px;
                  font-weight: 500;
                  color: #5a5a5a;
                  display: block;
                  margin-bottom: 5px;
                "
                >คำอธิบาย</label
              >
              <textarea
                v-model="categoryForm.description"
                rows="2"
                style="
                  width: 100%;
                  padding: 9px 12px;
                  border: 0.5px solid rgba(0, 0, 0, 0.12);
                  border-radius: 8px;
                  font-size: 13px;
                  outline: none;
                  resize: vertical;
                  color: #0f0f0f;
                "
              ></textarea>
            </div>
            <div
              v-if="modalError"
              style="
                font-size: 12px;
                color: #dc2626;
                background: #fef2f2;
                padding: 8px 12px;
                border-radius: 6px;
              "
            >
              {{ modalError }}
            </div>
          </div>
          <div
            style="
              padding: 1rem 1.5rem;
              border-top: 0.5px solid rgba(0, 0, 0, 0.06);
              display: flex;
              gap: 8px;
              justify-content: flex-end;
            "
          >
            <button
              @click="showCategoryModal = false"
              style="
                border: 0.5px solid rgba(0, 0, 0, 0.1);
                background: white;
                border-radius: 8px;
                padding: 8px 16px;
                font-size: 13px;
                cursor: pointer;
                color: #5a5a5a;
              "
            >
              ยกเลิก
            </button>
            <button
              @click="submitCategory"
              :disabled="modalLoading"
              style="
                background: #0f0f0f;
                color: white;
                border: none;
                border-radius: 8px;
                padding: 8px 20px;
                font-size: 13px;
                font-weight: 500;
                cursor: pointer;
              "
              :style="modalLoading ? 'opacity:0.5;cursor:not-allowed;' : ''"
            >
              {{ modalLoading ? 'กำลังบันทึก...' : 'บันทึก' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- ===== CONFIRM DELETE MODAL ===== -->
    <Teleport to="body">
      <div
        v-if="showDeleteModal"
        style="
          position: fixed;
          inset: 0;
          background: rgba(0, 0, 0, 0.3);
          display: flex;
          align-items: center;
          justify-content: center;
          z-index: 50;
          padding: 1rem;
        "
      >
        <div
          style="
            background: white;
            border-radius: 16px;
            width: 100%;
            max-width: 360px;
            padding: 1.5rem;
          "
        >
          <div style="font-size: 15px; font-weight: 500; color: #0f0f0f; margin-bottom: 8px">
            ยืนยันการลบ
          </div>
          <div style="font-size: 13px; color: #5a5a5a; margin-bottom: 1.5rem">
            คุณต้องการลบ <strong style="color: #0f0f0f">{{ deleteTarget.name }}</strong> ใช่หรือไม่?
            ไม่สามารถยกเลิกได้
          </div>
          <div style="display: flex; gap: 8px; justify-content: flex-end">
            <button
              @click="showDeleteModal = false"
              style="
                border: 0.5px solid rgba(0, 0, 0, 0.1);
                background: white;
                border-radius: 8px;
                padding: 8px 16px;
                font-size: 13px;
                cursor: pointer;
                color: #5a5a5a;
              "
            >
              ยกเลิก
            </button>
            <button
              @click="submitDelete"
              :disabled="modalLoading"
              style="
                background: #dc2626;
                color: white;
                border: none;
                border-radius: 8px;
                padding: 8px 20px;
                font-size: 13px;
                font-weight: 500;
                cursor: pointer;
              "
              :style="modalLoading ? 'opacity:0.5;cursor:not-allowed;' : ''"
            >
              {{ modalLoading ? 'กำลังลบ...' : 'ลบ' }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import { useInventoryStore } from '@/stores/inventoryStore'
import type { Product, Category } from '@/types/inventory'

const router = useRouter()
const authStore = useAuthStore()
const store = useInventoryStore()

const userInitial = computed(() => authStore.user?.username?.charAt(0).toUpperCase() ?? 'U')
const activeTab = ref<'products' | 'categories'>('products')
const tabs = [
  { key: 'products', label: 'สินค้า' },
  { key: 'categories', label: 'หมวดหมู่' },
] as const

// --- Filter ---
const searchProduct = ref('')
const filterCategory = ref('')
const filteredProducts = computed(() =>
  store.products.filter((p) => {
    const matchSearch =
      p.name.toLowerCase().includes(searchProduct.value.toLowerCase()) ||
      p.sku.toLowerCase().includes(searchProduct.value.toLowerCase())
    const matchCat = !filterCategory.value || p.categoryId === filterCategory.value
    return matchSearch && matchCat
  }),
)

// --- Modal state ---
const modalLoading = ref(false)
const modalError = ref('')

// Product modal
const showProductModal = ref(false)
const editingProduct = ref<Product | null>(null)
const productForm = reactive({
  name: '',
  sku: '',
  basePrice: 0,
  initialStock: 0,
  categoryId: '',
  description: '',
})

function openProductModal(p?: Product) {
  editingProduct.value = p ?? null
  modalError.value = ''
  if (p) {
    productForm.name = p.name
    productForm.sku = p.sku
    productForm.basePrice = p.basePrice
    productForm.initialStock = 0
    productForm.categoryId = p.categoryId
    productForm.description = p.description ?? ''
  } else {
    Object.assign(productForm, {
      name: '',
      sku: '',
      basePrice: 0,
      initialStock: 0,
      categoryId: '',
      description: '',
    })
  }
  showProductModal.value = true
}

async function submitProduct() {
  if (!productForm.name.trim() || !productForm.categoryId) {
    modalError.value = 'กรุณากรอกชื่อสินค้าและเลือกหมวดหมู่'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    if (editingProduct.value) {
      await store.updateProduct(editingProduct.value.id, {
        name: productForm.name,
        description: productForm.description,
        BasePrice: productForm.basePrice,
        categoryId: productForm.categoryId,
      })
    } else {
      if (!productForm.sku.trim()) {
        modalError.value = 'กรุณากรอก SKU'
        return
      }
      await store.createProduct({ ...productForm })
    }
    showProductModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// Stock modal
const showStockModal = ref(false)
const stockTarget = ref<Product | null>(null)
const stockForm = reactive({ quantityChange: 0, note: '' })

function openStockModal(p: Product) {
  stockTarget.value = p
  stockForm.quantityChange = 0
  stockForm.note = ''
  modalError.value = ''
  showStockModal.value = true
}

async function submitStock() {
  if (!stockTarget.value) return
  if (stockTarget.value.currentStock + stockForm.quantityChange < 0) {
    modalError.value = 'Stock ไม่สามารถติดลบได้'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    await store.updateStock(stockTarget.value.id, {
      productId: stockTarget.value.id,
      quantityChange: stockForm.quantityChange,
      note: stockForm.note,
    })
    showStockModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// Category modal
const showCategoryModal = ref(false)
const editingCategory = ref<Category | null>(null)
const categoryForm = reactive({ name: '', description: '' })

function openCategoryModal(c?: Category) {
  editingCategory.value = c ?? null
  modalError.value = ''
  categoryForm.name = c?.name ?? ''
  categoryForm.description = c?.description ?? ''
  showCategoryModal.value = true
}

async function submitCategory() {
  if (!categoryForm.name.trim()) {
    modalError.value = 'กรุณากรอกชื่อหมวดหมู่'
    return
  }
  modalLoading.value = true
  modalError.value = ''
  try {
    if (editingCategory.value) {
      await store.updateCategory(editingCategory.value.id, {
        name: categoryForm.name,
        description: categoryForm.description,
      })
    } else {
      await store.createCategory({ name: categoryForm.name, description: categoryForm.description })
    }
    showCategoryModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'บันทึกไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// Delete modal
const showDeleteModal = ref(false)
const deleteTarget = reactive({ type: '' as 'product' | 'category', id: '', name: '' })

function confirmDelete(type: 'product' | 'category', id: string, name: string) {
  deleteTarget.type = type
  deleteTarget.id = id
  deleteTarget.name = name
  showDeleteModal.value = true
}

async function submitDelete() {
  modalLoading.value = true
  try {
    if (deleteTarget.type === 'product') await store.deleteProduct(deleteTarget.id)
    else await store.deleteCategory(deleteTarget.id)
    showDeleteModal.value = false
  } catch (e: unknown) {
    modalError.value = e instanceof Error ? e.message : 'ลบไม่สำเร็จ'
  } finally {
    modalLoading.value = false
  }
}

// --- Helpers ---
function formatCurrency(v: number) {
  return new Intl.NumberFormat('th-TH', {
    style: 'currency',
    currency: 'THB',
    maximumFractionDigits: 0,
  }).format(v)
}
function formatDate(d: string) {
  return new Date(d).toLocaleDateString('th-TH', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  })
}
function stockStatusLabel(stock: number) {
  if (stock === 0) return 'หมด'
  if (stock <= 10) return 'ใกล้หมด'
  return 'ปกติ'
}
function stockStatusStyle(stock: number) {
  if (stock === 0)
    return 'font-size:11px;font-weight:500;background:#fef2f2;color:#dc2626;padding:3px 8px;border-radius:100px;'
  if (stock <= 10)
    return 'font-size:11px;font-weight:500;background:#fffbeb;color:#d97706;padding:3px 8px;border-radius:100px;'
  return 'font-size:11px;font-weight:500;background:#f0fdf4;color:#16a34a;padding:3px 8px;border-radius:100px;'
}

async function handleLogout() {
  await authStore.logout()
  router.push({ name: 'login' })
}

onMounted(async () => {
  await Promise.all([store.fetchProducts(), store.fetchCategories()])
})
</script>
