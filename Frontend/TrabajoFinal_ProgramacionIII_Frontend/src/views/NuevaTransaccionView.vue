<script setup>
import { ref, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import api from '../services/api'

const route = useRoute()

const usuario = JSON.parse(localStorage.getItem('usuario'))

const codigoCripto = ref('btc')
const accion = ref('compra')
const cantidad = ref(0)
const fecha = ref('')
const mensaje = ref('')
const precioUnitario = ref(0)
const valorEstimado = ref(0)

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

const actualizarUsuarioLocal = async () => {
  const response = await api.get(`/usuarios/${usuario.id}`)
  localStorage.setItem('usuario', JSON.stringify(response.data))
}

const guardar = async () => {

  if (!fecha.value) {
    mensaje.value = 'Debe ingresar una fecha'
    return
  }

  if (cantidad.value <= 0) {
    mensaje.value = 'La cantidad debe ser mayor a 0'
    return
  }

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

    await actualizarUsuarioLocal()

    window.location.href = '/historial'
  }
  catch (error) {
    if (error.response) {
      mensaje.value = error.response.data
    } else {
      mensaje.value = 'Error al guardar la transacción'
    }
  }
}

const obtenerCotizacion = async () => {
  try {
    const codigo = codigoCripto.value.toLowerCase()
    const response = await fetch(`https://criptoya.com/api/binance/${codigo}/ars/1`)
    const data = await response.json()

    if (accion.value === 'compra') {
      precioUnitario.value = data.ask
    } else {
      precioUnitario.value = data.bid
    }

    valorEstimado.value = cantidad.value * precioUnitario.value
  }
  catch {
    precioUnitario.value = 0
    valorEstimado.value = 0
  }
}

watch([codigoCripto, accion, cantidad], () => {
  obtenerCotizacion()
})

onMounted(async () => {
  await cargarTransaccion()
  obtenerCotizacion()
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

          <div class="alert alert-info mt-3">
            <p class="mb-1">
              <strong>Precio de 1 {{ codigoCripto.toUpperCase() }}:</strong>
              ${{ precioUnitario.toFixed(2) }} ARS
            </p>

            <p class="mb-0">
              <strong>Valor estimado:</strong>
              ${{ valorEstimado.toFixed(2) }} ARS
            </p>
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