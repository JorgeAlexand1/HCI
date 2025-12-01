using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FISEI.Incidentes.Migrations
{
    /// <inheritdoc />
    public partial class AddResetAndVerifyColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailVerificado",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationToken",
                table: "Usuarios",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetToken",
                table: "Usuarios",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpiry",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerificado",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "EmailVerificationToken",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ResetToken",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpiry",
                table: "Usuarios");
        }
    }
}
