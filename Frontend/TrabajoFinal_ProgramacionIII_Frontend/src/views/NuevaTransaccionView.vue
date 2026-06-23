<script setup>
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import api from '../services/api'

const router = useRouter()
const route = useRoute()

const usuario = JSON.parse(localStorage.getItem('usuario'))

const codigoCripto = ref('btc')
const accion = ref('compra')
const cantidad = ref(0)
const fecha = ref('')
const mensaje = ref('')

const esEdicion = ref(false)
const idTransaccion = route.params.id

const cargarTransaccion = async () => {
  if (idTransaccion) {
    esEdicion.value = true

    const response = await api.get(`/transacciones/${idTransaccion}`)
    const t = response.data

    codigoCripto.value = t.codigoCripto
    accion.value = t.accion
    cantidad.value = t.cantidad
    fecha.value = t.fecha.substring(0, 16)
  }
}

const guardar = async () => {
  try {
    const transaccion = {
      codigoCripto: codigoCripto.value,
      accion: accion.value,
      cantidad: cantidad.value,
      fecha: fecha.value,
      usuarioId: usuario.id
    }

    if (esEdicion.value) {
      await api.put(
        `/transacciones/${idTransaccion}?nombreUsuario=${usuario.nombre}`,
        transaccion
      )
    } else {
      await api.post('/transacciones', {
        ...transaccion,
        dinero: 0
      })
    }

    router.push('/historial')
  }
  catch (error) {
    if (error.response) {
      mensaje.value = error.response.data
    } else {
      mensaje.value = 'Error al guardar la transacción'
    }
  }
}

onMounted(() => {
  cargarTransaccion()
})
</script>

<template>
  <div class="row justify-content-center">
    <div class="col-md-6">
      <div class="card shadow">
        <div class="card-body">
          <h2 class="mb-4">
            {{ esEdicion ? 'Editar transacción' : 'Nueva transacción' }}
          </h2>

          <div class="mb-3">
            <label class="form-label">Criptomoneda</label>
            <select v-model="codigoCripto" class="form-select">
              <option value="btc">Bitcoin</option>
              <option value="eth">Ethereum</option>
              <option value="usdc">USDC</option>
            </select>
          </div>

          <div class="mb-3">
            <label class="form-label">Acción</label>
            <select v-model="accion" class="form-select">
              <option value="compra">Compra</option>
              <option value="venta">Venta</option>
            </select>
          </div>

          <div class="mb-3">
            <label class="form-label">Cantidad</label>
            <input
              type="number"
              step="0.00000001"
              v-model="cantidad"
              class="form-control"
            >
          </div>

          <div class="mb-3">
            <label class="form-label">Fecha</label>
            <input
              type="datetime-local"
              v-model="fecha"
              class="form-control"
            >
          </div>

          <button @click="guardar" class="btn btn-primary w-100">
            {{ esEdicion ? 'Guardar cambios' : 'Guardar transacción' }}
          </button>

          <div v-if="mensaje" class="alert alert-danger mt-3">
            {{ mensaje }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>