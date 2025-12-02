using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentesFISEI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SistemaAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    UsuarioNombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DireccionIP = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TipoAccion = table.Column<int>(type: "int", nullable: false),
                    TipoEntidad = table.Column<int>(type: "int", nullable: false),
                    EntidadId = table.Column<int>(type: "int", nullable: true),
                    EntidadDescripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ValoresAnteriores = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ValoresNuevos = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NivelSeveridad = table.Column<int>(type: "int", nullable: false),
                    MetadataJson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EsExitoso = table.Column<bool>(type: "bit", nullable: false),
                    MensajeError = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CantidadRegistros = table.Column<int>(type: "int", nullable: true),
                    FiltrosAplicados = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modulo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Endpoint = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 832, DateTimeKind.Utc).AddTicks(6608));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 832, DateTimeKind.Utc).AddTicks(6749));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 832, DateTimeKind.Utc).AddTicks(6753));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 832, DateTimeKind.Utc).AddTicks(6757));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 832, DateTimeKind.Utc).AddTicks(6760));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 844, DateTimeKind.Utc).AddTicks(7924));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 844, DateTimeKind.Utc).AddTicks(7929));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 844, DateTimeKind.Utc).AddTicks(7932));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 844, DateTimeKind.Utc).AddTicks(7934));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 844, DateTimeKind.Utc).AddTicks(7936));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 844, DateTimeKind.Utc).AddTicks(7938));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 844, DateTimeKind.Utc).AddTicks(7986));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 844, DateTimeKind.Utc).AddTicks(7988));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 844, DateTimeKind.Utc).AddTicks(7990));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 844, DateTimeKind.Utc).AddTicks(7992));

            migrationBuilder.UpdateData(
                table: "PlantillasEncuesta",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 851, DateTimeKind.Utc).AddTicks(5307));

            migrationBuilder.UpdateData(
                table: "PreguntasEncuesta",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 851, DateTimeKind.Utc).AddTicks(7536));

            migrationBuilder.UpdateData(
                table: "PreguntasEncuesta",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 851, DateTimeKind.Utc).AddTicks(7543));

            migrationBuilder.UpdateData(
                table: "PreguntasEncuesta",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 851, DateTimeKind.Utc).AddTicks(7547));

            migrationBuilder.UpdateData(
                table: "PreguntasEncuesta",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 851, DateTimeKind.Utc).AddTicks(7551));

            migrationBuilder.UpdateData(
                table: "PreguntasEncuesta",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 851, DateTimeKind.Utc).AddTicks(7554));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 834, DateTimeKind.Utc).AddTicks(677));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 834, DateTimeKind.Utc).AddTicks(683));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 834, DateTimeKind.Utc).AddTicks(686));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 834, DateTimeKind.Utc).AddTicks(688));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 834, DateTimeKind.Utc).AddTicks(3173));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 834, DateTimeKind.Utc).AddTicks(3328));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 834, DateTimeKind.Utc).AddTicks(3336));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 834, DateTimeKind.Utc).AddTicks(3342));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 834, DateTimeKind.Utc).AddTicks(3347));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 834, DateTimeKind.Utc).AddTicks(3473));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 834, DateTimeKind.Utc).AddTicks(4194));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 834, DateTimeKind.Utc).AddTicks(4216));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 834, DateTimeKind.Utc).AddTicks(4222));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 26, 42, 834, DateTimeKind.Utc).AddTicks(4227));

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EsExitoso",
                table: "AuditLogs",
                column: "EsExitoso");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_FechaHora",
                table: "AuditLogs",
                column: "FechaHora");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Modulo",
                table: "AuditLogs",
                column: "Modulo");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_NivelSeveridad",
                table: "AuditLogs",
                column: "NivelSeveridad");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TipoAccion",
                table: "AuditLogs",
                column: "TipoAccion");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TipoEntidad",
                table: "AuditLogs",
                column: "TipoEntidad");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TipoEntidad_EntidadId",
                table: "AuditLogs",
                columns: new[] { "TipoEntidad", "EntidadId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UsuarioId",
                table: "AuditLogs",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 191, DateTimeKind.Utc).AddTicks(8772));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 191, DateTimeKind.Utc).AddTicks(8903));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 191, DateTimeKind.Utc).AddTicks(8907));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 191, DateTimeKind.Utc).AddTicks(8930));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 191, DateTimeKind.Utc).AddTicks(8933));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6487));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6492));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6495));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6506));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6508));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6510));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6512));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6514));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6516));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6518));

            migrationBuilder.UpdateData(
                table: "PlantillasEncuesta",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 211, DateTimeKind.Utc).AddTicks(1809));

            migrationBuilder.UpdateData(
                table: "PreguntasEncuesta",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 211, DateTimeKind.Utc).AddTicks(4150));

            migrationBuilder.UpdateData(
                table: "PreguntasEncuesta",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 211, DateTimeKind.Utc).AddTicks(4156));

            migrationBuilder.UpdateData(
                table: "PreguntasEncuesta",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 211, DateTimeKind.Utc).AddTicks(4161));

            migrationBuilder.UpdateData(
                table: "PreguntasEncuesta",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 211, DateTimeKind.Utc).AddTicks(4164));

            migrationBuilder.UpdateData(
                table: "PreguntasEncuesta",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 211, DateTimeKind.Utc).AddTicks(4167));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 192, DateTimeKind.Utc).AddTicks(9013));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 192, DateTimeKind.Utc).AddTicks(9018));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 192, DateTimeKind.Utc).AddTicks(9042));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 192, DateTimeKind.Utc).AddTicks(9045));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(1942));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2082));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2112));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2117));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2122));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2265));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2940));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2947));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2952));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2957));
        }
    }
}
