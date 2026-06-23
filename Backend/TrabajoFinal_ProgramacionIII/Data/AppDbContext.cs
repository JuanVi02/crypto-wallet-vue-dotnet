using Microsoft.EntityFrameworkCore;
using TrabajoFinal_ProgramacionIII.Models;

namespace TrabajoFinal_ProgramacionIII.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Transaccion> Transacciones { get; set; }
    }
}
