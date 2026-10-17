using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BancoPanda.Api.Migrations
{
    /// <inheritdoc />
    public partial class TransferenciasEfectivoMovilYTarjetas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "numero_tarjeta",
                table: "tarjetas",
                type: "character(16)",
                fixedLength: true,
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            // Las tarjetas que ya existen pasan a 16 dígitos conservando sus 4 últimos.
            migrationBuilder.Sql(
                """
                UPDATE tarjetas
                SET numero_tarjeta = (CASE marca
                        WHEN 'Visa' THEN '4' || lpad(floor(random() * 100000000000)::bigint::text, 11, '0')
                        ELSE '5' || (1 + floor(random() * 5))::int || lpad(floor(random() * 10000000000)::bigint::text, 10, '0')
                    END) || ultimos_digitos;
                """);

            migrationBuilder.DropColumn(
                name: "ultimos_digitos",
                table: "tarjetas");

            migrationBuilder.CreateTable(
                name: "bancos",
                columns: table => new
                {
                    banco_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bancos", x => x.banco_id);
                });

            migrationBuilder.CreateTable(
                name: "efectivos_moviles",
                columns: table => new
                {
                    efectivo_movil_id = table.Column<Guid>(type: "uuid", nullable: false),
                    telefono_destino = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    monto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    codigo_hash = table.Column<string>(type: "text", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_expiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    estado = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    solicitud_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cuenta_id = table.Column<Guid>(type: "uuid", nullable: false),
                    movimiento_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_efectivos_moviles", x => x.efectivo_movil_id);
                    table.ForeignKey(
                        name: "fk_efectivos_moviles_cuentas_cuenta_id",
                        column: x => x.cuenta_id,
                        principalTable: "cuentas",
                        principalColumn: "cuenta_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_efectivos_moviles_movimientos_movimiento_id",
                        column: x => x.movimiento_id,
                        principalTable: "movimientos",
                        principalColumn: "movimiento_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "beneficiarios",
                columns: table => new
                {
                    beneficiario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    numero_cuenta = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cedula = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    banco_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_beneficiarios", x => x.beneficiario_id);
                    table.ForeignKey(
                        name: "fk_beneficiarios_bancos_banco_id",
                        column: x => x.banco_id,
                        principalTable: "bancos",
                        principalColumn: "banco_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_beneficiarios_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "usuario_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transferencias",
                columns: table => new
                {
                    transferencia_id = table.Column<Guid>(type: "uuid", nullable: false),
                    referencia = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    monto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    solicitud_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cuenta_origen_id = table.Column<Guid>(type: "uuid", nullable: false),
                    beneficiario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    movimiento_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transferencias", x => x.transferencia_id);
                    table.ForeignKey(
                        name: "fk_transferencias_beneficiarios_beneficiario_id",
                        column: x => x.beneficiario_id,
                        principalTable: "beneficiarios",
                        principalColumn: "beneficiario_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_transferencias_cuentas_cuenta_origen_id",
                        column: x => x.cuenta_origen_id,
                        principalTable: "cuentas",
                        principalColumn: "cuenta_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_transferencias_movimientos_movimiento_id",
                        column: x => x.movimiento_id,
                        principalTable: "movimientos",
                        principalColumn: "movimiento_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "bancos",
                columns: new[] { "banco_id", "activo", "nombre" },
                values: new object[,]
                {
                    { new Guid("b0000000-0000-4000-8000-000000000001"), true, "Banco Panda" },
                    { new Guid("b0000000-0000-4000-8000-000000000002"), true, "Banco Guayaquil" },
                    { new Guid("b0000000-0000-4000-8000-000000000003"), true, "Banco Pichincha" },
                    { new Guid("b0000000-0000-4000-8000-000000000004"), true, "Banco Bolivariano" },
                    { new Guid("b0000000-0000-4000-8000-000000000005"), true, "Produbanco" },
                    { new Guid("b0000000-0000-4000-8000-000000000006"), true, "Banco del Pacífico" },
                    { new Guid("b0000000-0000-4000-8000-000000000007"), true, "Banco Amazonas" },
                    { new Guid("b0000000-0000-4000-8000-000000000008"), true, "Banco Internacional" },
                    { new Guid("b0000000-0000-4000-8000-000000000009"), true, "Banco del Austro" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_tarjetas_numero_tarjeta",
                table: "tarjetas",
                column: "numero_tarjeta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bancos_nombre",
                table: "bancos",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_beneficiarios_banco_id",
                table: "beneficiarios",
                column: "banco_id");

            migrationBuilder.CreateIndex(
                name: "ix_beneficiarios_usuario_id_banco_id_numero_cuenta",
                table: "beneficiarios",
                columns: new[] { "usuario_id", "banco_id", "numero_cuenta" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_efectivos_moviles_cuenta_id",
                table: "efectivos_moviles",
                column: "cuenta_id");

            migrationBuilder.CreateIndex(
                name: "ix_efectivos_moviles_movimiento_id",
                table: "efectivos_moviles",
                column: "movimiento_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_efectivos_moviles_solicitud_id",
                table: "efectivos_moviles",
                column: "solicitud_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transferencias_beneficiario_id",
                table: "transferencias",
                column: "beneficiario_id");

            migrationBuilder.CreateIndex(
                name: "ix_transferencias_cuenta_origen_id",
                table: "transferencias",
                column: "cuenta_origen_id");

            migrationBuilder.CreateIndex(
                name: "ix_transferencias_movimiento_id",
                table: "transferencias",
                column: "movimiento_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transferencias_referencia",
                table: "transferencias",
                column: "referencia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transferencias_solicitud_id",
                table: "transferencias",
                column: "solicitud_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "efectivos_moviles");

            migrationBuilder.DropTable(
                name: "transferencias");

            migrationBuilder.DropTable(
                name: "beneficiarios");

            migrationBuilder.DropTable(
                name: "bancos");

            migrationBuilder.DropIndex(
                name: "ix_tarjetas_numero_tarjeta",
                table: "tarjetas");

            migrationBuilder.AddColumn<string>(
                name: "ultimos_digitos",
                table: "tarjetas",
                type: "character(4)",
                fixedLength: true,
                maxLength: 4,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE tarjetas SET ultimos_digitos = right(numero_tarjeta, 4);");

            migrationBuilder.DropColumn(
                name: "numero_tarjeta",
                table: "tarjetas");
        }
    }
}
