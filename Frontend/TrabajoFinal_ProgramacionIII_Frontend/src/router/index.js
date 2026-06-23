import { createRouter, createWebHistory } from 'vue-router'

import LoginView from '../views/LoginView.vue'
import HistorialView from '../views/HistorialView.vue'
import NuevaTransaccionView from '../views/NuevaTransaccionView.vue'
import CarteraView from '../views/CarteraView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'login',
      component: LoginView,
    },
    {
      path: '/historial',
      name: 'historial',
      component: HistorialView,
    },
    {
      path: '/nueva',
      name: 'nueva',
      component: NuevaTransaccionView,
    },
    {
      path: '/cartera',
      name: 'cartera',
      component: CarteraView,
    },
    {
      path: '/editar/:id',
      name: 'editar',
      component: NuevaTransaccionView,
    },
  ],
})

export default router