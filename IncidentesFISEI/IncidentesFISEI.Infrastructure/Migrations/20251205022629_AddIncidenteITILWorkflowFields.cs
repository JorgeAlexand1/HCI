using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentesFISEI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidenteITILWorkflowFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CerradoPorId",
                table: "Incidentes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComentarioCierre",
                table: "Incidentes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaReapertura",
                table: "Incidentes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoReapertura",
                table: "Incidentes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReabiertoPorId",
                table: "Incidentes",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 5, 2, 26, 28, 641, DateTimeKind.Utc).AddTicks(3339));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 5, 2, 26, 28, 641, DateTimeKind.Utc).AddTicks(3461));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 5, 2, 26, 28, 641, DateTimeKind.Utc).AddTicks(3464));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 5, 2, 26, 28, 641, DateTimeKind.Utc).AddTicks(3466));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 5, 2, 26, 28, 641, DateTimeKind.Utc).AddTicks(3468));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 5, 2, 26, 28, 642, DateTimeKind.Utc).AddTicks(1873));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 5, 2, 26, 28, 642, DateTimeKind.Utc).AddTicks(1876));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 5, 2, 26, 28, 642, DateTimeKind.Utc).AddTicks(1878));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 5, 2, 26, 28, 642, DateTimeKind.Utc).AddTicks(1880));

            migrationBuilder.CreateIndex(
                name: "IX_Incidentes_CerradoPorId",
                table: "Incidentes",
                column: "CerradoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidentes_ReabiertoPorId",
                table: "Incidentes",
                column: "ReabiertoPorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidentes_Usuarios_CerradoPorId",
                table: "Incidentes",
                column: "CerradoPorId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidentes_Usuarios_ReabiertoPorId",
                table: "Incidentes",
                column: "ReabiertoPorId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidentes_Usuarios_CerradoPorId",
                table: "Incidentes");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidentes_Usuarios_ReabiertoPorId",
                table: "Incidentes");

            migrationBuilder.DropIndex(
                name: "IX_Incidentes_CerradoPorId",
                table: "Incidentes");

            migrationBuilder.DropIndex(
                name: "IX_Incidentes_ReabiertoPorId",
                table: "Incidentes");

            migrationBuilder.DropColumn(
                name: "CerradoPorId",
                table: "Incidentes");

            migrationBuilder.DropColumn(
                name: "ComentarioCierre",
                table: "Incidentes");

            migrationBuilder.DropColumn(
                name: "FechaReapertura",
                table: "Incidentes");

            migrationBuilder.DropColumn(
                name: "MotivoReapertura",
                table: "Incidentes");

            migrationBuilder.DropColumn(
                name: "ReabiertoPorId",
                table: "Incidentes");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 3, 49, 23, 816, DateTimeKind.Utc).AddTicks(3099));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 3, 49, 23, 816, DateTimeKind.Utc).AddTicks(3244));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 3, 49, 23, 816, DateTimeKind.Utc).AddTicks(3247));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 3, 49, 23, 816, DateTimeKind.Utc).AddTicks(3325));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 3, 49, 23, 816, DateTimeKind.Utc).AddTicks(3327));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 3, 49, 23, 817, DateTimeKind.Utc).AddTicks(3323));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 3, 49, 23, 817, DateTimeKind.Utc).AddTicks(3328));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 3, 49, 23, 817, DateTimeKind.Utc).AddTicks(3330));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 3, 49, 23, 817, DateTimeKind.Utc).AddTicks(3332));
        }
    }
}
