using System.Text.Json.Serialization;

namespace TrabajoFinal_ProgramacionIII.Models
{
    public class Transaccion
    {
        public int Id { get; set; }

        public string CodigoCripto { get; set; }

        public string Accion { get; set; }

        public decimal Cantidad { get; set; }

        public decimal Dinero { get; set; }

        public DateTime Fecha { get; set; }

        public int UsuarioId { get; set; }

        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}
