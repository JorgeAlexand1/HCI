using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentesFISEI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServiciosUtf8Collation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase(
                collation: "Modern_Spanish_CI_AS");

            migrationBuilder.AlterColumn<string>(
                name: "ResponsableArea",
                table: "Servicios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                collation: "Modern_Spanish_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Servicios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                collation: "Modern_Spanish_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Instrucciones",
                table: "Servicios",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                collation: "Modern_Spanish_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EscalacionProcedure",
                table: "Servicios",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                collation: "Modern_Spanish_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Servicios",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                collation: "Modern_Spanish_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase(
                oldCollation: "Modern_Spanish_CI_AS");

            migrationBuilder.AlterColumn<string>(
                name: "ResponsableArea",
                table: "Servicios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldCollation: "Modern_Spanish_CI_AS");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Servicios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldCollation: "Modern_Spanish_CI_AS");

            migrationBuilder.AlterColumn<string>(
                name: "Instrucciones",
                table: "Servicios",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true,
                oldCollation: "Modern_Spanish_CI_AS");

            migrationBuilder.AlterColumn<string>(
                name: "EscalacionProcedure",
                table: "Servicios",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldCollation: "Modern_Spanish_CI_AS");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Servicios",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldCollation: "Modern_Spanish_CI_AS");

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
        }
    }
}
