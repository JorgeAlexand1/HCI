using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FISEI.Incidentes.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioRolFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdRol",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ROL",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROL", x => x.IdRol);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios",
                column: "IdRol");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_ROL_IdRol",
                table: "Usuarios",
                column: "IdRol",
                principalTable: "ROL",
                principalColumn: "IdRol",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_ROL_IdRol",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "ROL");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "IdRol",
                table: "Usuarios");
        }
    }
}
