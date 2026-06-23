using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrabajoFinal_ProgramacionIII.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPesosUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Pesos",
                table: "Usuarios",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pesos",
                table: "Usuarios");
        }
    }
}
