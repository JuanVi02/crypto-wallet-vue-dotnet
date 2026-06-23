using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabajoFinal_ProgramacionIII.Data;
using TrabajoFinal_ProgramacionIII.Models;
using System.Text.Json;

namespace TrabajoFinal_ProgramacionIII.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaccionesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public TransaccionesController(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public IActionResult ObtenerTransacciones()
        {
            var transacciones = _context.Transacciones
                .OrderByDescending(t => t.Fecha)
                .ToList();

            return Ok(transacciones);
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerTransaccion(int id)
        {
            var transaccion = _context.Transacciones.Find(id);

            if (transaccion == null)
            {
                return NotFound("Transacción no encontrada");
            }

            return Ok(transaccion);
        }

        [HttpPost]
        public async Task<IActionResult> CrearTransaccion(Transaccion transaccion)
        {
            if (transaccion.Cantidad <= 0)
            {
                return BadRequest("La cantidad debe ser mayor a 0");
            }

            if (transaccion.Accion != "compra" && transaccion.Accion != "venta")
            {
                return BadRequest("La acción debe ser compra o venta");
            }

            if (transaccion.Accion == "venta")
            {
                var totalComprado = _context.Transacciones
                    .Where(t => t.UsuarioId == transaccion.UsuarioId
                             && t.CodigoCripto == transaccion.CodigoCripto
                             && t.Accion == "compra")
                    .Sum(t => t.Cantidad);

                var totalVendido = _context.Transacciones
                    .Where(t => t.UsuarioId == transaccion.UsuarioId
                             && t.CodigoCripto == transaccion.CodigoCripto
                             && t.Accion == "venta")
                    .Sum(t => t.Cantidad);

                var saldoDisponible = totalComprado - totalVendido;

                if (transaccion.Cantidad > saldoDisponible)
                {
                    return BadRequest("No tenés suficiente saldo de esa criptomoneda para vender");
                }
            }

            try
            {
                var precio = await ObtenerPrecioCripto(
                    transaccion.CodigoCripto,
                    transaccion.Accion
                );

                transaccion.Dinero = transaccion.Cantidad * precio;
            }
            catch
            {
                return BadRequest("No se pudo obtener el precio de la criptomoneda");
            }

            var usuario = _context.Usuarios.Find(transaccion.UsuarioId);

            if (usuario == null)
            {
                return BadRequest("Usuario no encontrado");
            }

            if (transaccion.Accion == "compra")
            {
                if (usuario.Pesos < transaccion.Dinero)
                {
                    return BadRequest("No tenés pesos suficientes para realizar la compra");
                }

                usuario.Pesos -= transaccion.Dinero;
            }

            if (transaccion.Accion == "venta")
            {
                usuario.Pesos += transaccion.Dinero;
            }

            _context.Transacciones.Add(transaccion);
            _context.SaveChanges();

            return Ok(transaccion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarTransaccion(int id, Transaccion transaccion, string nombreUsuario)
        {
            if (nombreUsuario != "admin")
            {
                return StatusCode(403, "Solo el usuario admin puede editar transacciones");
            }

            var transaccionExistente = _context.Transacciones.Find(id);

            if (transaccionExistente == null)
            {
                return NotFound("Transacción no encontrada");
            }

            transaccionExistente.CodigoCripto = transaccion.CodigoCripto;
            transaccionExistente.Accion = transaccion.Accion;
            transaccionExistente.Cantidad = transaccion.Cantidad;
            var precio = await ObtenerPrecioCripto(
                transaccion.CodigoCripto,
                transaccion.Accion
            );

            transaccionExistente.Dinero = transaccion.Cantidad * precio;
            transaccionExistente.Fecha = transaccion.Fecha;
            transaccionExistente.UsuarioId = transaccion.UsuarioId;

            _context.SaveChanges();

            return Ok(transaccionExistente);
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarTransaccion(int id, string nombreUsuario)
        {
            if (nombreUsuario != "admin")
            {
                return StatusCode(403, "Solo el usuario admin puede eliminar transacciones");
            }

            var transaccion = _context.Transacciones.Find(id);

            if (transaccion == null)
            {
                return NotFound("Transacción no encontrada");
            }

            _context.Transacciones.Remove(transaccion);
            _context.SaveChanges();

            return Ok("Transacción eliminada correctamente");
        }

        [HttpGet("estado/{usuarioId}")]
        public async Task<IActionResult> ObtenerEstadoCartera(int usuarioId)
        {
            var codigos = _context.Transacciones
                .Where(t => t.UsuarioId == usuarioId)
                .Select(t => t.CodigoCripto)
                .Distinct()
                .ToList();

            var resultado = new List<object>();
            decimal totalGeneral = 0;

            foreach (var codigo in codigos)
            {
                var totalComprado = _context.Transacciones
                    .Where(t => t.UsuarioId == usuarioId
                             && t.CodigoCripto == codigo
                             && t.Accion == "compra")
                    .Sum(t => t.Cantidad);

                var totalVendido = _context.Transacciones
                    .Where(t => t.UsuarioId == usuarioId
                             && t.CodigoCripto == codigo
                             && t.Accion == "venta")
                    .Sum(t => t.Cantidad);

                var cantidadActual = totalComprado - totalVendido;

                if (cantidadActual > 0)
                {
                    var precioActual = await ObtenerPrecioCripto(codigo, "venta");
                    var dineroActual = cantidadActual * precioActual;

                    totalGeneral += dineroActual;

                    resultado.Add(new
                    {
                        CodigoCripto = codigo,
                        Cantidad = cantidadActual,
                        PrecioActual = precioActual,
                        DineroActual = dineroActual
                    });
                }
            }

            return Ok(new
            {
                Criptomonedas = resultado,
                TotalGeneral = totalGeneral
            });
        }

        private async Task<decimal> ObtenerPrecioCripto(string codigoCripto, string accion)
        {
            string codigoApi = codigoCripto.ToLower();

            string url = $"https://criptoya.com/api/binance/{codigoApi}/ars/1";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {    
                throw new Exception("Error al obtener el precio de CryptoYa");
            }

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            if (accion == "compra")
            {
                return document.RootElement.GetProperty("ask").GetDecimal();
            }

            return document.RootElement.GetProperty("bid").GetDecimal();
        }
    }
}