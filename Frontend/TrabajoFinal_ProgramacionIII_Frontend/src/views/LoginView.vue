<script setup>
import { ref } from 'vue'
import api from '../services/api'

const nombre = ref('')
const password = ref('')
const mensaje = ref('')

const login = async () => {
  try {
    const response = await api.post('/usuarios/login', {
      nombre: nombre.value,
      password: password.value
    })

    localStorage.setItem('usuario', JSON.stringify(response.data))
    window.location.href = '/historial'
  }
  catch (error) {
    if (error.response) {
      mensaje.value = error.response.data
    } else {
      mensaje.value = 'No se pudo conectar con la API'
    }
  }
}

const registrar = async () => {
  try {
    const response = await api.post('/usuarios/registrar', {
      nombre: nombre.value,
      password: password.value
    })

    mensaje.value = response.data
  }
  catch (error) {
    if (error.response) {
      mensaje.value = error.response.data
    } else {
      mensaje.value = 'No se pudo registrar el usuario'
    }
  }
}
</script>

<template>
  <div class="row justify-content-center">
    <div class="col-md-5">
      <div class="card shadow-lg bg-dark text-white border-secondary">
        <div class="card-body">
          <h2 class="text-center mb-4">Iniciar sesión</h2>

          <div class="mb-3">
            <label class="form-label">Usuario</label>
            <input v-model="nombre" class="form-control" />
          </div>

          <div class="mb-3">
            <label class="form-label">Contraseña</label>
            <input type="password" v-model="password" class="form-control" />
          </div>

          <button @click="login" class="btn btn-primary w-100">
            Ingresar
          </button>
          
          <div class="d-grid gap-2 mt-2">
            <button
              @click="registrar"
              class="btn btn-outline-light"
            >
              Registrarse
            </button>
          </div>

          <div v-if="mensaje" class="alert alert-danger mt-3">
            {{ mensaje }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>