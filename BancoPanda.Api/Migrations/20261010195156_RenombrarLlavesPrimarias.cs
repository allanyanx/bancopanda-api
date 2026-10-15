using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BancoPanda.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarLlavesPrimarias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "usuarios",
                newName: "usuario_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "tarjetas",
                newName: "tarjeta_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "movimientos",
                newName: "movimiento_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "cuentas",
                newName: "cuenta_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "usuarios",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "tarjeta_id",
                table: "tarjetas",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "movimiento_id",
                table: "movimientos",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "cuenta_id",
                table: "cuentas",
                newName: "id");
        }
    }
}
