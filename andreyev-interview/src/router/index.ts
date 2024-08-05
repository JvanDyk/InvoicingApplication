import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'Invoices',
    component: () => import('../views/Invoices.vue')
  },
  {
    path: '/:id',
    name: 'Invoice',
    component: () => import('../views/Invoice.vue'),
    props: true
  },
  {
    path: '/Clients/:id',
    name: 'GetClient',
    component: () => import('../views/Client.vue'),
    props: true
  },
  {
    path: '/Clients',
    name: 'CreateClient',
    component: () => import('../views/Client.vue'),
  },
  
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
