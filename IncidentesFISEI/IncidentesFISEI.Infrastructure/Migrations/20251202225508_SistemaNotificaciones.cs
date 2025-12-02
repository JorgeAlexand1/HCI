using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentesFISEI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SistemaNotificaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    IncidenteId = table.Column<int>(type: "int", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Leida = table.Column<bool>(type: "bit", nullable: false),
                    FechaLectura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EnviadaPorEmail = table.Column<bool>(type: "bit", nullable: false),
                    FechaEnvioEmail = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrupoNotificacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Incidentes_IncidenteId",
                        column: x => x.IncidenteId,
                        principalTable: "Incidentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 715, DateTimeKind.Utc).AddTicks(6456));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 715, DateTimeKind.Utc).AddTicks(6593));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 715, DateTimeKind.Utc).AddTicks(6597));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 715, DateTimeKind.Utc).AddTicks(6600));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 715, DateTimeKind.Utc).AddTicks(6603));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2326));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2334));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2337));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2339));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2341));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2343));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2345));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2347));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2349));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2351));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 716, DateTimeKind.Utc).AddTicks(8369));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 716, DateTimeKind.Utc).AddTicks(8375));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 716, DateTimeKind.Utc).AddTicks(8378));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 716, DateTimeKind.Utc).AddTicks(8381));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(1150));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(1296));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(1304));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(1309));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(1315));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(1444));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(2102));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(2109));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(2115));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(2120));

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_CreatedAt",
                table: "Notificaciones",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_IncidenteId",
                table: "Notificaciones",
                column: "IncidenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_Leida",
                table: "Notificaciones",
                column: "Leida");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioId",
                table: "Notificaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioId_Leida",
                table: "Notificaciones",
                columns: new[] { "UsuarioId", "Leida" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 31, 988, DateTimeKind.Utc).AddTicks(6149));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 31, 988, DateTimeKind.Utc).AddTicks(6292));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 31, 988, DateTimeKind.Utc).AddTicks(6297));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 31, 988, DateTimeKind.Utc).AddTicks(6316));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 31, 988, DateTimeKind.Utc).AddTicks(6319));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4105));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4113));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4115));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4117));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4119));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4122));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4124));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4126));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4128));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4130));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 2, DateTimeKind.Utc).AddTicks(6797));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 2, DateTimeKind.Utc).AddTicks(6805));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 2, DateTimeKind.Utc).AddTicks(6822));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 2, DateTimeKind.Utc).AddTicks(6825));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 2, DateTimeKind.Utc).AddTicks(9742));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 2, DateTimeKind.Utc).AddTicks(9892));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 2, DateTimeKind.Utc).AddTicks(9900));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 2, DateTimeKind.Utc).AddTicks(9905));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 2, DateTimeKind.Utc).AddTicks(9910));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 3, DateTimeKind.Utc).AddTicks(64));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 3, DateTimeKind.Utc).AddTicks(859));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 3, DateTimeKind.Utc).AddTicks(868));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 3, DateTimeKind.Utc).AddTicks(873));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 38, 32, 3, DateTimeKind.Utc).AddTicks(878));
        }
    }
}
