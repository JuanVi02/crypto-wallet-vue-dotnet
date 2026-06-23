<script setup>
import { ref } from 'vue'
import { RouterLink, RouterView } from 'vue-router'

const usuario = ref(JSON.parse(localStorage.getItem('usuario')))

const cerrarSesion = () => {
  localStorage.removeItem('usuario')
  window.location.href = '/'
}
</script>

<template>
  <div>
    <nav class="navbar navbar-expand-lg navbar-dark shadow" style="background-color:#111827;">
      <div class="container">

        <span class="navbar-brand">
          💰 Wallet Cripto
        </span>

        <div v-if="usuario" class="d-flex align-items-center ms-auto">

          <span class="text-light me-4">
            Hola, {{ usuario.nombre }}
          </span>

          <div class="navbar-nav">

            <RouterLink
              to="/historial"
              class="nav-link"
            >
              Historial
            </RouterLink>

            <RouterLink
              to="/nueva"
              class="nav-link"
            >
              Nueva Transacción
            </RouterLink>

            <RouterLink
              to="/cartera"
              class="nav-link"
            >
              Cartera
            </RouterLink>

          </div>

          <button
            class="btn btn-outline-light ms-3"
            @click="cerrarSesion"
          >
            Cerrar sesión
          </button>

        </div>

      </div>
    </nav>

    <div class="container mt-4">
      <RouterView />
    </div>
  </div>
</template>
