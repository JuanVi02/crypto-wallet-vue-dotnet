<script setup>
import { ref, onMounted, computed } from 'vue'
import api from '../services/api'

import { Pie } from 'vue-chartjs'
import {
  Chart as ChartJS,
  ArcElement,
  Tooltip,
  Legend
} from 'chart.js'

ChartJS.register(ArcElement, Tooltip, Legend)

const usuario = JSON.parse(localStorage.getItem('usuario'))

const criptomonedas = ref([])
const totalGeneral = ref(0)
const mensaje = ref('')

const cargarCartera = async () => {
  try {
    const response = await api.get(`/transacciones/estado/${usuario.id}`)

    criptomonedas.value = response.data.criptomonedas
    totalGeneral.value = response.data.totalGeneral
  }
  catch {
    mensaje.value = 'Error al cargar la cartera'
  }
}

const calcularPorcentaje = (dineroActual) => {
  if (totalGeneral.value === 0) return 0

  return (dineroActual / totalGeneral.value) * 100
}

const datosGrafico = computed(() => {
  return {
    labels: criptomonedas.value.map(c => c.codigoCripto.toUpperCase()),
    datasets: [
      {
        data: criptomonedas.value.map(c => c.dineroActual),
        backgroundColor: [
          '#0d6efd',
          '#198754',
          '#ffc107',
          '#dc3545',
          '#6f42c1'
        ]
      }
    ]
  }
})

const opcionesGrafico = {
  responsive: true,
  plugins: {
    legend: {
      position: 'bottom'
    }
  }
}

onMounted(() => {
  cargarCartera()
})
</script>

<template>
  <div>
    <h1 class="titulo-pagina mb-4">Estado de la cartera</h1>

    <div class="row">
      <div
        class="col-md-4 mb-3"
        v-for="c in criptomonedas"
        :key="c.codigoCripto"
      >
        <div class="card shadow h-100">
          <div class="card-body">
            <h4 class="card-title">
              {{ c.codigoCripto.toUpperCase() }}
            </h4>

            <hr>

            <p>
              <strong>Cantidad:</strong>
              {{ c.cantidad }}
            </p>

            <p>
              <strong>Precio actual:</strong>
              ${{ c.precioActual.toFixed(2) }}
            </p>

            <p>
              <strong>Valor actual:</strong>
              ${{ c.dineroActual.toFixed(2) }}
            </p>

            <p>
              <strong>Porcentaje:</strong>
              {{ calcularPorcentaje(c.dineroActual).toFixed(2) }}%
            </p>
          </div>
        </div>
      </div>
    </div>

    <div class="row mt-4" v-if="criptomonedas.length > 0">
      <div class="col-md-6">
        <div class="card shadow h-100">
          <div class="card-body text-center">
            <h3>Total General</h3>

            <h2 class="text-success">
              ${{ totalGeneral.toFixed(2) }}
            </h2>
          </div>
        </div>
      </div>

      <div class="col-md-6">
        <div class="card shadow h-100">
          <div class="card-body">
            <h3 class="text-center mb-3">
              Distribución de cartera
            </h3>

            <div style="max-width: 320px; margin: 0 auto;">
              <Pie
                :data="datosGrafico"
                :options="opcionesGrafico"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <div
      v-if="criptomonedas.length === 0"
      class="alert alert-warning mt-3"
    >
      No posee criptomonedas actualmente.
    </div>

    <div v-if="mensaje" class="alert alert-danger mt-3">
      {{ mensaje }}
    </div>
  </div>
</template>