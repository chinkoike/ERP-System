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
          meta: { roles: ['Admin', 'Manager'] },
        },
        {
          path: 'inventory',
          name: 'inventory',
          component: () => import('@/views/InventoryView.vue'),
          meta: { roles: ['Admin', 'Manager', 'User'] },
        },
        {
          path: 'sales',
          name: 'sales',
          component: () => import('@/views/SalesView.vue'),
          meta: { roles: ['Admin', 'Manager', 'User'] },
        },
        {
          path: 'purchasing',
          name: 'purchasing',
          component: () => import('@/views/PurchasingView.vue'),
          meta: { roles: ['Admin', 'Manager', 'User'] },
        },
        {
          path: 'finance',
          name: 'finance',
          component: () => import('@/views/FinanceView.vue'),
          meta: { roles: ['Admin', 'Manager'] },
        },
        {
          path: 'report',
          name: 'report',
          component: () => import('@/views/ReportView.vue'),
          meta: { roles: ['Admin', 'Manager'] },
        },
        {
          path: 'users',
          name: 'users',
          component: () => import('@/views/UsersView.vue'),
          meta: { roles: ['Admin', 'Manager'] },
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
  if (to.name === 'login' && authStore.isAuthenticated && authStore.user) {
    return { name: 'dashboard' }
  }

  const allowedRoles = to.meta.roles as string[] | undefined
  if (allowedRoles && allowedRoles.length > 0 && !authStore.hasAnyRole(allowedRoles)) {
    return { name: authStore.hasAnyRole(['Admin', 'Manager', 'User']) ? 'inventory' : 'login' }
  }
})

export default router
