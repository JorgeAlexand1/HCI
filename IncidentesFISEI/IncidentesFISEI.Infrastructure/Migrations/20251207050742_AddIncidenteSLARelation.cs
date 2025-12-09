using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentesFISEI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidenteSLARelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConfiguracionSLAId",
                table: "Incidentes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TiempoResolucionSLA",
                table: "Incidentes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TiempoRespuestaSLA",
                table: "Incidentes",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 5, 7, 41, 371, DateTimeKind.Utc).AddTicks(5483));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 5, 7, 41, 371, DateTimeKind.Utc).AddTicks(5630));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 5, 7, 41, 371, DateTimeKind.Utc).AddTicks(5633));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 5, 7, 41, 371, DateTimeKind.Utc).AddTicks(5635));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 5, 7, 41, 371, DateTimeKind.Utc).AddTicks(5638));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 5, 7, 41, 372, DateTimeKind.Utc).AddTicks(6466));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 5, 7, 41, 372, DateTimeKind.Utc).AddTicks(6471));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 5, 7, 41, 372, DateTimeKind.Utc).AddTicks(6474));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 7, 5, 7, 41, 372, DateTimeKind.Utc).AddTicks(6475));

            migrationBuilder.CreateIndex(
                name: "IX_Incidentes_ConfiguracionSLAId",
                table: "Incidentes",
                column: "ConfiguracionSLAId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidentes_ConfiguracionesSLA_ConfiguracionSLAId",
                table: "Incidentes",
                column: "ConfiguracionSLAId",
                principalTable: "ConfiguracionesSLA",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidentes_ConfiguracionesSLA_ConfiguracionSLAId",
                table: "Incidentes");

            migrationBuilder.DropIndex(
                name: "IX_Incidentes_ConfiguracionSLAId",
                table: "Incidentes");

            migrationBuilder.DropColumn(
                name: "ConfiguracionSLAId",
                table: "Incidentes");

            migrationBuilder.DropColumn(
                name: "TiempoResolucionSLA",
                table: "Incidentes");

            migrationBuilder.DropColumn(
                name: "TiempoRespuestaSLA",
                table: "Incidentes");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 6, 20, 27, 46, 81, DateTimeKind.Utc).AddTicks(1034));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 6, 20, 27, 46, 81, DateTimeKind.Utc).AddTicks(1173));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 6, 20, 27, 46, 81, DateTimeKind.Utc).AddTicks(1176));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 6, 20, 27, 46, 81, DateTimeKind.Utc).AddTicks(1178));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 6, 20, 27, 46, 81, DateTimeKind.Utc).AddTicks(1180));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 6, 20, 27, 46, 82, DateTimeKind.Utc).AddTicks(535));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 6, 20, 27, 46, 82, DateTimeKind.Utc).AddTicks(540));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 6, 20, 27, 46, 82, DateTimeKind.Utc).AddTicks(542));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 6, 20, 27, 46, 82, DateTimeKind.Utc).AddTicks(543));
        }
    }
}
