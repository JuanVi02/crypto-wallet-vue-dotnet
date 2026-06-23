<script setup>
import { ref } from 'vue'
import { RouterLink, RouterView } from 'vue-router'

const usuario = ref(JSON.parse(localStorage.getItem('usuario')))

const montoPesos = ref(0)
const mostrarCargaPesos = ref(false)

const cargarPesos = async () => {
  if (montoPesos.value <= 0) {
    alert('El monto debe ser mayor a 0')
    return
  }

  const response = await fetch(
    `https://localhost:7179/api/usuarios/${usuario.value.id}/cargar-pesos`,
    {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(montoPesos.value)
    }
  )

  if (response.ok) {
    const usuarioActualizado = await response.json()

    usuario.value.pesos = usuarioActualizado.pesos
    localStorage.setItem('usuario', JSON.stringify(usuario.value))

    montoPesos.value = 0
    mostrarCargaPesos.value = false
  }
}

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
            Hola, {{ usuario.nombre }} | Saldo: ${{ (usuario.pesos ?? 0).toFixed(2) }}
          </span>

          <button
            class="btn btn-sm btn-success me-3"
            @click="mostrarCargaPesos = !mostrarCargaPesos"
          >
            Cargar pesos
          </button>          

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

    <div
      v-if="mostrarCargaPesos"
      class="container mt-3"
    >
      <div class="card shadow">
        <div class="card-body">
          <h5>Cargar pesos</h5>

          <input
            type="number"
            class="form-control mb-2"
            v-model="montoPesos"
            placeholder="Ingrese monto"
          >

          <button
            class="btn btn-success"
            @click="cargarPesos"
          >
            Confirmar carga
          </button>
        </div>
      </div>
    </div>

    <div class="container mt-4">
      <RouterView />
    </div>
  </div>
</template>
