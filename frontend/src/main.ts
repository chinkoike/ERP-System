import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'

import BaseSkeleton from './components/BaseSkeleton.vue'
import TableSkeleton from '@/components/common/TableSkeleton.vue'

const app = createApp(App)
app.component('BaseSkeleton', BaseSkeleton)
app.component('TableSkeleton', TableSkeleton)
app.use(createPinia())
app.use(router)

app.mount('#app')
