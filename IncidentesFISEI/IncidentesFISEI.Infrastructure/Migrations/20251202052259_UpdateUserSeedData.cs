using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IncidentesFISEI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 5, 22, 58, 675, DateTimeKind.Utc).AddTicks(3738));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 5, 22, 58, 675, DateTimeKind.Utc).AddTicks(3889));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 5, 22, 58, 675, DateTimeKind.Utc).AddTicks(3892));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 5, 22, 58, 675, DateTimeKind.Utc).AddTicks(3894));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 5, 22, 58, 675, DateTimeKind.Utc).AddTicks(3896));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 5, 22, 58, 676, DateTimeKind.Utc).AddTicks(3158));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 5, 22, 58, 676, DateTimeKind.Utc).AddTicks(3162));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 5, 22, 58, 676, DateTimeKind.Utc).AddTicks(3164));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 5, 22, 58, 676, DateTimeKind.Utc).AddTicks(3166));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$KjLnhQkoD9HBy0sZQaxvKe5sBoELZbldjM1ywnR.wn9aAgOOXQr1K" });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "AñosExperiencia", "CreatedAt", "Department", "Email", "Especialidad", "FirstName", "IsActive", "IsDeleted", "IsEmailConfirmed", "LastLoginAt", "LastName", "PasswordHash", "Phone", "TipoUsuario", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { 2, 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Soporte Técnico", "supervisor@fisei.uta.edu.ec", "Redes y Sistemas", "Carlos", true, false, true, null, "Mendoza", "$2a$11$r9PuDRz5A1J/7FHn3AXd5.Zg/AOUy0Ok/8sDSxJEKVGJYtFRlUdOK", null, 3, null, "supervisor1" },
                    { 3, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Soporte Técnico", "tecnico1@fisei.uta.edu.ec", "Hardware y Software", "María", true, false, true, null, "González", "$2a$11$wxqje4Ox3VO3RGljJA9Vp.9WByK3DpGbgDt2Bfde5awiLFBlY7.0e", null, 2, null, "tecnico1" },
                    { 4, 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Soporte Técnico", "tecnico2@fisei.uta.edu.ec", "Redes y Conectividad", "Luis", true, false, true, null, "Ramírez", "$2a$11$9f5rP2P5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5q", null, 2, null, "tecnico2" },
                    { 5, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ingeniería en Sistemas", "docente1@fisei.uta.edu.ec", null, "Ana", true, false, true, null, "Pérez", "$2a$11$fS7059bDQD23mvMoKYCjoOvs7umhzDvu5tOW.lGwA7y6gzUWaNGo.", "0998123456", 1, null, "docente1" },
                    { 6, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ingeniería en Sistemas", "estudiante1@fisei.uta.edu.ec", null, "José", false, false, false, null, "Morales", "$2a$11$9f5rP2P5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5q", null, 1, null, "estudiante1" },
                    { 7, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ingeniería Industrial", "estudiante2@fisei.uta.edu.ec", null, "Carmen", false, false, false, null, "Torres", "$2a$11$9f5rP2P5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5q", null, 1, null, "estudiante2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 2, 14, 5, 891, DateTimeKind.Utc).AddTicks(8295));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 2, 14, 5, 891, DateTimeKind.Utc).AddTicks(8527));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 2, 14, 5, 891, DateTimeKind.Utc).AddTicks(8530));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 2, 14, 5, 891, DateTimeKind.Utc).AddTicks(8533));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 2, 14, 5, 891, DateTimeKind.Utc).AddTicks(8535));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 2, 14, 5, 892, DateTimeKind.Utc).AddTicks(8361));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 2, 14, 5, 892, DateTimeKind.Utc).AddTicks(8366));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 2, 14, 5, 892, DateTimeKind.Utc).AddTicks(8368));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 2, 14, 5, 892, DateTimeKind.Utc).AddTicks(8370));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 2, 2, 14, 5, 892, DateTimeKind.Utc).AddTicks(6610), "$2a$11$9f5rP2P5qP5qP5qP5qP5qOG5rP2P5qP5qP5qP5qP5qP5qP5qP5qP5q" });
        }
    }
}
