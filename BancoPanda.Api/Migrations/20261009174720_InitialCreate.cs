using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BancoPanda.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombres = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    apellidos = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    cedula = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    correo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    telefono = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    contrasena_hash = table.Column<string>(type: "text", nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cuentas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    numero_cuenta = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    saldo = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    moneda = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    fecha_apertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cuentas", x => x.id);
                    table.ForeignKey(
                        name: "fk_cuentas_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "movimientos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    descripcion = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    monto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    tipo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    categoria = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cuenta_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movimientos", x => x.id);
                    table.ForeignKey(
                        name: "fk_movimientos_cuentas_cuenta_id",
                        column: x => x.cuenta_id,
                        principalTable: "cuentas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tarjetas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ultimos_digitos = table.Column<string>(type: "character(4)", fixedLength: true, maxLength: 4, nullable: false),
                    marca = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    fecha_expiracion = table.Column<DateOnly>(type: "date", nullable: false),
                    activa = table.Column<bool>(type: "boolean", nullable: false),
                    cuenta_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tarjetas", x => x.id);
                    table.ForeignKey(
                        name: "fk_tarjetas_cuentas_cuenta_id",
                        column: x => x.cuenta_id,
                        principalTable: "cuentas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cuentas_numero_cuenta",
                table: "cuentas",
                column: "numero_cuenta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cuentas_usuario_id",
                table: "cuentas",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_movimientos_cuenta_id_fecha",
                table: "movimientos",
                columns: new[] { "cuenta_id", "fecha" });

            migrationBuilder.CreateIndex(
                name: "ix_tarjetas_cuenta_id",
                table: "tarjetas",
                column: "cuenta_id");

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_cedula",
                table: "usuarios",
                column: "cedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_correo",
                table: "usuarios",
                column: "correo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movimientos");

            migrationBuilder.DropTable(
                name: "tarjetas");

            migrationBuilder.DropTable(
                name: "cuentas");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
