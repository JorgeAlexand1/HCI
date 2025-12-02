using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentesFISEI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEscalacionSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NivelSoporte",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EscaladoAutomaticamente",
                table: "Incidentes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaEscalacion",
                table: "Incidentes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NivelActual",
                table: "Incidentes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumeroEscalaciones",
                table: "Incidentes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RazonEscalacion",
                table: "Incidentes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HistorialEscalaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidenteId = table.Column<int>(type: "int", nullable: false),
                    NivelOrigen = table.Column<int>(type: "int", nullable: false),
                    NivelDestino = table.Column<int>(type: "int", nullable: false),
                    TecnicoOrigenId = table.Column<int>(type: "int", nullable: true),
                    TecnicoDestinoId = table.Column<int>(type: "int", nullable: true),
                    Razon = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FueAutomatico = table.Column<bool>(type: "bit", nullable: false),
                    FechaEscalacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialEscalaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialEscalaciones_Incidentes_IncidenteId",
                        column: x => x.IncidenteId,
                        principalTable: "Incidentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialEscalaciones_Usuarios_TecnicoDestinoId",
                        column: x => x.TecnicoDestinoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialEscalaciones_Usuarios_TecnicoOrigenId",
                        column: x => x.TecnicoOrigenId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(1704));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(1779));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(1782));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(1794));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(1796));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(7846));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(7848));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(7850));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(7852));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "NivelSoporte",
                value: null);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "NivelSoporte",
                value: null);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                column: "NivelSoporte",
                value: null);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4,
                column: "NivelSoporte",
                value: null);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 5,
                column: "NivelSoporte",
                value: null);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 6,
                column: "NivelSoporte",
                value: null);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 7,
                column: "NivelSoporte",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEscalaciones_IncidenteId",
                table: "HistorialEscalaciones",
                column: "IncidenteId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEscalaciones_TecnicoDestinoId",
                table: "HistorialEscalaciones",
                column: "TecnicoDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEscalaciones_TecnicoOrigenId",
                table: "HistorialEscalaciones",
                column: "TecnicoOrigenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialEscalaciones");

            migrationBuilder.DropColumn(
                name: "NivelSoporte",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "EscaladoAutomaticamente",
                table: "Incidentes");

            migrationBuilder.DropColumn(
                name: "FechaUltimaEscalacion",
                table: "Incidentes");

            migrationBuilder.DropColumn(
                name: "NivelActual",
                table: "Incidentes");

            migrationBuilder.DropColumn(
                name: "NumeroEscalaciones",
                table: "Incidentes");

            migrationBuilder.DropColumn(
                name: "RazonEscalacion",
                table: "Incidentes");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 21, 52, 36, 621, DateTimeKind.Utc).AddTicks(6367));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 21, 52, 36, 621, DateTimeKind.Utc).AddTicks(6441));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 21, 52, 36, 621, DateTimeKind.Utc).AddTicks(6443));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 21, 52, 36, 621, DateTimeKind.Utc).AddTicks(6445));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 21, 52, 36, 621, DateTimeKind.Utc).AddTicks(6447));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 21, 52, 36, 622, DateTimeKind.Utc).AddTicks(4426));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 21, 52, 36, 622, DateTimeKind.Utc).AddTicks(4428));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 21, 52, 36, 622, DateTimeKind.Utc).AddTicks(4430));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 21, 52, 36, 622, DateTimeKind.Utc).AddTicks(4431));
        }
    }
}
