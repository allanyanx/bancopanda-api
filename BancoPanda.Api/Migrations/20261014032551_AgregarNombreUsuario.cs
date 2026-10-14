using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BancoPanda.Api.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNombreUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "nombre_usuario",
                table: "usuarios",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                "UPDATE usuarios SET nombre_usuario = 'u' || cedula WHERE nombre_usuario = '';");

            migrationBuilder.Sql(
                "UPDATE usuarios SET telefono = '+593' || substring(telefono from 2) WHERE telefono ~ '^09[0-9]{8}$';");

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_nombre_usuario",
                table: "usuarios",
                column: "nombre_usuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_usuarios_nombre_usuario",
                table: "usuarios");

            migrationBuilder.Sql(
                "UPDATE usuarios SET telefono = '0' || substring(telefono from 5) WHERE telefono ~ '^[+]5939[0-9]{8}$';");

            migrationBuilder.DropColumn(
                name: "nombre_usuario",
                table: "usuarios");
        }
    }
}
