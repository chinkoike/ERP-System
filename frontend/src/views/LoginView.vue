<template>
  <div class="min-h-screen bg-slate-50 flex items-center justify-center p-4">
    <div class="w-full max-w-sm">
      <!-- Brand -->
      <div class="mb-8">
        <div class="flex items-center gap-2.5 mb-6">
          <div class="flex h-8 w-8 items-center justify-center rounded-xl bg-slate-900">
            <svg
              width="14"
              height="14"
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
          <span class="text-sm font-semibold text-slate-900 tracking-tight">ERP System</span>
        </div>
        <h1 class="text-3xl font-semibold tracking-tight text-slate-900">Welcome back</h1>
        <p class="mt-2 text-sm text-slate-500">Sign in to continue to your workspace</p>
      </div>

      <!-- Card -->
      <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
        <!-- Error -->
        <div
          v-if="errorMessage"
          class="mb-4 flex items-start gap-2.5 rounded-2xl bg-rose-50 px-4 py-3 text-sm text-rose-700"
        >
          <svg class="mt-0.5 h-4 w-4 shrink-0" fill="currentColor" viewBox="0 0 20 20">
            <path
              fill-rule="evenodd"
              d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
              clip-rule="evenodd"
            />
          </svg>
          {{ errorMessage }}
        </div>

        <form @submit.prevent="handleLogin" novalidate class="space-y-4">
          <!-- Username -->
          <div>
            <label for="username" class="mb-1.5 block text-sm font-medium text-slate-700"
              >Username</label
            >
            <input
              id="username"
              v-model="form.username"
              type="text"
              autocomplete="username"
              placeholder="your.username"
              :class="
                fieldError.username
                  ? 'border-rose-300 bg-rose-50 focus:ring-rose-200'
                  : 'border-slate-200 focus:ring-slate-200'
              "
              class="w-full rounded-2xl border bg-white px-3 py-2 text-sm text-slate-900 placeholder-slate-300 outline-none focus:ring-2 transition"
            />
            <p v-if="fieldError.username" class="mt-1 text-xs text-rose-500">
              {{ fieldError.username }}
            </p>
          </div>

          <!-- Password -->
          <div>
            <label for="password" class="mb-1.5 block text-sm font-medium text-slate-700"
              >Password</label
            >
            <div class="relative">
              <input
                id="password"
                v-model="form.password"
                :type="showPassword ? 'text' : 'password'"
                autocomplete="current-password"
                placeholder="••••••••"
                :class="
                  fieldError.password
                    ? 'border-rose-300 bg-rose-50 focus:ring-rose-200'
                    : 'border-slate-200 focus:ring-slate-200'
                "
                class="w-full rounded-2xl border bg-white px-3 py-2 pr-10 text-sm text-slate-900 placeholder-slate-300 outline-none focus:ring-2 transition"
              />
              <button
                type="button"
                @click="showPassword = !showPassword"
                class="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-slate-600 transition"
              >
                <svg
                  v-if="!showPassword"
                  class="h-4 w-4"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
                  />
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"
                  />
                </svg>
                <svg v-else class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21"
                  />
                </svg>
              </button>
            </div>
            <p v-if="fieldError.password" class="mt-1 text-xs text-rose-500">
              {{ fieldError.password }}
            </p>
          </div>

          <!-- Submit -->
          <button
            type="submit"
            :disabled="authStore.loading"
            class="mt-2 flex w-full items-center justify-center gap-2 rounded-2xl bg-slate-900 px-4 py-2.5 text-sm font-medium text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-50"
          >
            <svg
              v-if="authStore.loading"
              class="h-4 w-4 animate-spin"
              fill="none"
              viewBox="0 0 24 24"
            >
              <circle
                class="opacity-25"
                cx="12"
                cy="12"
                r="10"
                stroke="currentColor"
                stroke-width="4"
              />
              <path
                class="opacity-75"
                fill="currentColor"
                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"
              />
            </svg>
            {{ authStore.loading ? 'Signing in...' : 'Sign in' }}
          </button>
          <!-- Demo Login  -->
          <div class="relative my-4">
            <div class="absolute inset-0 flex items-center">
              <span class="w-full border-t border-slate-100"></span>
            </div>
            <div class="relative flex justify-center text-xs uppercase">
              <span class="bg-green-100 px-2 text-slate-800 rounded-3xl">Or testing?</span>
            </div>
          </div>

          <button
            type="button"
            @click="handleDemoLogin"
            class="flex w-full items-center justify-center gap-2 rounded-2xl border border-slate-200 bg-slate-100 px-4 py-2.5 text-sm font-medium text-slate-800 transition hover:bg-slate-200 hover:border-slate-300"
          >
            Demo Login (Click & See)
          </button>
        </form>
      </div>

      <p class="mt-6 text-center text-xs text-slate-400">
        ERP System &copy; {{ new Date().getFullYear() }}
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

const router = useRouter()
const authStore = useAuthStore()

const showPassword = ref(false)
const errorMessage = ref('')
const form = reactive({ username: '', password: '' })
const fieldError = reactive({ username: '', password: '' })

function validate(): boolean {
  fieldError.username = ''
  fieldError.password = ''
  let valid = true
  if (!form.username.trim()) {
    fieldError.username = 'Username is required'
    valid = false
  }
  if (!form.password) {
    fieldError.password = 'Password is required'
    valid = false
  } else if (form.password.length < 6) {
    fieldError.password = 'Password must be at least 6 characters'
    valid = false
  }
  return valid
}

async function handleLogin() {
  errorMessage.value = ''
  if (!validate()) return
  try {
    await authStore.login({ username: form.username, password: form.password })
    router.push({ name: 'dashboard' })
  } catch (e: unknown) {
    errorMessage.value = e instanceof Error ? e.message : 'Invalid username or password'
  }
}

async function handleDemoLogin() {
  form.username = 'admin'
  form.password = 'Password123!'

  await handleLogin()
}
</script>
