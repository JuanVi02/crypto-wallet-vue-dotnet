<script setup>
import { ref, onMounted } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import api from '../services/api'

const router = useRouter()

const transacciones = ref([])
const usuario = JSON.parse(localStorage.getItem('usuario'))

const formatearFecha = (fecha) => {
  return new Date(fecha).toLocaleString()
}

const cargarTransacciones = async () => {
  const response = await api.get('/transacciones')
  transacciones.value = response.data.filter(
    t => t.usuarioId === usuario.id
  )
}

const editar = (id) => {
  router.push(`/editar/${id}`)
}

const eliminar = async (id) => {
  if (!confirm('¿Seguro que querés eliminar esta transacción?')) return

  await api.delete(`/transacciones/${id}?nombreUsuario=${usuario.nombre}`)
  await cargarTransacciones()
}

onMounted(() => {
  cargarTransacciones()
})
</script>

<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h1 class="titulo-pagina">Historial de movimientos</h1>

      <RouterLink to="/nueva" class="btn btn-primary">
        Nueva transacción
      </RouterLink>
    </div>

    <div class="card shadow">
      <div class="card-body">
        <table class="table table-striped table-hover align-middle">
          <thead>
            <tr>
              <th>Fecha</th>
              <th>Cripto</th>
              <th>Acción</th>
              <th>Cantidad</th>
              <th>Dinero</th>
              <th v-if="usuario?.esAdmin">Acciones</th>
            </tr>
          </thead>

          <tbody>
            <tr
              v-for="t in transacciones"
              :key="t.id"
              :class="t.accion === 'compra' ? 'table-success' : 'table-warning'"
            >
              <td>{{ formatearFecha(t.fecha) }}</td>
              <td>{{ t.codigoCripto.toUpperCase() }}</td>
              <td>
                <span
                  class="badge"
                  :class="t.accion === 'compra' ? 'bg-success' : 'bg-warning text-dark'"
                >
                  {{ t.accion }}
                </span>
              </td>
              <td>{{ t.cantidad }}</td>
              <td>${{ t.dinero.toFixed(2) }}</td>

              <td v-if="usuario?.esAdmin">
                <button
                  class="btn btn-sm btn-outline-primary me-2"
                  @click="editar(t.id)"
                >
                  Editar
                </button>

                <button
                  class="btn btn-sm btn-outline-danger"
                  @click="eliminar(t.id)"
                >
                  Eliminar
                </button>
              </td>
            </tr>
          </tbody>
        </table>

        <p v-if="transacciones.length === 0" class="text-center text-muted">
          No hay movimientos registrados.
        </p>
      </div>
    </div>
  </div>
</template>