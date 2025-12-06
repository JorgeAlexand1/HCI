using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentesFISEI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiciosTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServicioId",
                table: "Incidentes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    ResponsableArea = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactoTecnico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TiempoRespuestaMinutos = table.Column<int>(type: "int", nullable: true),
                    TiempoResolucionMinutos = table.Column<int>(type: "int", nullable: true),
                    Instrucciones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EscalacionProcedure = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequiereAprobacion = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servicios_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 2, 56, 26, 530, DateTimeKind.Utc).AddTicks(2172));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 2, 56, 26, 530, DateTimeKind.Utc).AddTicks(2313));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 2, 56, 26, 530, DateTimeKind.Utc).AddTicks(2316));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 2, 56, 26, 530, DateTimeKind.Utc).AddTicks(2318));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 2, 56, 26, 530, DateTimeKind.Utc).AddTicks(2320));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 2, 56, 26, 531, DateTimeKind.Utc).AddTicks(1557));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 2, 56, 26, 531, DateTimeKind.Utc).AddTicks(1561));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 2, 56, 26, 531, DateTimeKind.Utc).AddTicks(1563));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 2, 56, 26, 531, DateTimeKind.Utc).AddTicks(1565));

            migrationBuilder.CreateIndex(
                name: "IX_Incidentes_ServicioId",
                table: "Incidentes",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_CategoriaId",
                table: "Servicios",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_Codigo",
                table: "Servicios",
                column: "Codigo",
                unique: true,
                filter: "[Codigo] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidentes_Servicios_ServicioId",
                table: "Incidentes",
                column: "ServicioId",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidentes_Servicios_ServicioId",
                table: "Incidentes");

            migrationBuilder.DropTable(
                name: "Servicios");

            migrationBuilder.DropIndex(
                name: "IX_Incidentes_ServicioId",
                table: "Incidentes");

            migrationBuilder.DropColumn(
                name: "ServicioId",
                table: "Incidentes");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 44, 539, DateTimeKind.Utc).AddTicks(9780));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 44, 539, DateTimeKind.Utc).AddTicks(9928));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 44, 539, DateTimeKind.Utc).AddTicks(9931));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 44, 539, DateTimeKind.Utc).AddTicks(9933));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 44, 539, DateTimeKind.Utc).AddTicks(9936));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 44, 540, DateTimeKind.Utc).AddTicks(9734));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 44, 540, DateTimeKind.Utc).AddTicks(9738));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 44, 540, DateTimeKind.Utc).AddTicks(9740));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 44, 540, DateTimeKind.Utc).AddTicks(9742));
        }
    }
}
