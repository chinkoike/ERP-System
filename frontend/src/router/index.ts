import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    // ─── Public (no layout) ───
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/LoginView.vue'),
      meta: { requiresAuth: false },
    },

    // ─── Authenticated (AppLayout) ───
    {
      path: '/',
      component: () => import('@/layouts/AppLayout.vue'),
      meta: { requiresAuth: true },
      redirect: '/dashboard',
      children: [
        {
          path: 'dashboard',
          name: 'dashboard',
          component: () => import('@/views/DashboardView.vue'),
        },
        {
          path: 'inventory',
          name: 'inventory',
          component: () => import('@/views/InventoryView.vue'),
        },
        {
          path: 'sales',
          name: 'sales',
          component: () => import('@/views/SalesView.vue'),
        },
        {
          path: 'purchasing',
          name: 'purchasing',
          component: () => import('@/views/PurchasingView.vue'),
        },
        {
          path: 'finance',
          name: 'finance',
          component: () => import('@/views/FinanceView.vue'),
        },
        {
          path: 'report',
          name: 'report',
          component: () => import('@/views/ReportView.vue'),
        },
        {
          path: 'users',
          name: 'users',
          component: () => import('@/views/UsersView.vue'),
        },
      ],
    },
  ],
})

// Auth guard
router.beforeEach((to) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return { name: 'login' }
  }
  if (to.name === 'login' && authStore.isAuthenticated) {
    return { name: 'dashboard' }
  }
})

export default router
