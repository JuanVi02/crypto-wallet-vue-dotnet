using Microsoft.AspNetCore.Mvc;
using TrabajoFinal_ProgramacionIII.Data;
using TrabajoFinal_ProgramacionIII.Models;

namespace TrabajoFinal_ProgramacionIII.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario usuario)
        {
            var usuarioEncontrado = _context.Usuarios.FirstOrDefault(u =>
                u.Nombre == usuario.Nombre &&
                u.Password == usuario.Password);

            if (usuarioEncontrado == null)
            {
                return Unauthorized("Usuario o contraseña incorrectos");
            }

            return Ok(new
            {
                usuarioEncontrado.Id,
                usuarioEncontrado.Nombre,
                EsAdmin = usuarioEncontrado.Nombre.ToLower() == "admin"
            });
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerUsuario(int id)
        {
            var usuario = _context.Usuarios.Find(id);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            return Ok(new
            {
                usuario.Id,
                usuario.Nombre,
                EsAdmin = usuario.Nombre == "admin"
            });
        }

        [HttpPost("registrar")]
        public IActionResult Registrar([FromBody] Usuario usuario)
        {
            if (_context.Usuarios.Any(u => u.Nombre == usuario.Nombre))
            {
                return BadRequest("Ya existe un usuario con ese nombre");
            }

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return Ok("Usuario registrado correctamente");
        }
    }
}