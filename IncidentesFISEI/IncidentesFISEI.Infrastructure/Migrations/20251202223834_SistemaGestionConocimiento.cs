using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IncidentesFISEI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SistemaGestionConocimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComentariosArticulo_ArticulosConocimiento_ArticuloId",
                table: "ComentariosArticulo");

            migrationBuilder.DropForeignKey(
                name: "FK_ComentariosArticulo_Usuarios_AutorId",
                table: "ComentariosArticulo");

            migrationBuilder.DropForeignKey(
                name: "FK_VotacionesArticulo_ArticulosConocimiento_ArticuloId",
                table: "VotacionesArticulo");

            migrationBuilder.DropIndex(
                name: "IX_VotacionesArticulo_ArticuloId_UsuarioId",
                table: "VotacionesArticulo");

            migrationBuilder.DropColumn(
                name: "EsPositivo",
                table: "VotacionesArticulo");

            migrationBuilder.DropColumn(
                name: "Puntuacion",
                table: "ComentariosArticulo");

            migrationBuilder.RenameColumn(
                name: "ArticuloId",
                table: "VotacionesArticulo",
                newName: "Voto");

            migrationBuilder.RenameColumn(
                name: "AutorId",
                table: "ComentariosArticulo",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "ArticuloId",
                table: "ComentariosArticulo",
                newName: "ArticuloConocimientoId");

            migrationBuilder.RenameIndex(
                name: "IX_ComentariosArticulo_AutorId",
                table: "ComentariosArticulo",
                newName: "IX_ComentariosArticulo_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_ComentariosArticulo_ArticuloId",
                table: "ComentariosArticulo",
                newName: "IX_ComentariosArticulo_ArticuloConocimientoId");

            migrationBuilder.AddColumn<int>(
                name: "ArticuloConocimientoId",
                table: "VotacionesArticulo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Comentario",
                table: "VotacionesArticulo",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ComentarioPadreId",
                table: "ComentariosArticulo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsRespuesta",
                table: "ComentariosArticulo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "TasaExito",
                table: "ArticulosConocimiento",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoArticulo",
                table: "ArticulosConocimiento",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VecesUtilizado",
                table: "ArticulosConocimiento",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VersionActual",
                table: "ArticulosConocimiento",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EtiquetasConocimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false, defaultValue: "#007bff"),
                    VecesUsada = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtiquetasConocimiento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ValidacionesArticulo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticuloConocimientoId = table.Column<int>(type: "int", nullable: false),
                    SolicitadoPorId = table.Column<int>(type: "int", nullable: false),
                    ValidadorId = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    ComentariosValidador = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaValidacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aprobado = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidacionesArticulo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValidacionesArticulo_ArticulosConocimiento_ArticuloConocimientoId",
                        column: x => x.ArticuloConocimientoId,
                        principalTable: "ArticulosConocimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ValidacionesArticulo_Usuarios_SolicitadoPorId",
                        column: x => x.SolicitadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ValidacionesArticulo_Usuarios_ValidadorId",
                        column: x => x.ValidadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VersionesArticulo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticuloConocimientoId = table.Column<int>(type: "int", nullable: false),
                    NumeroVersion = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Resumen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CambiosRealizados = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ModificadoPorId = table.Column<int>(type: "int", nullable: false),
                    FechaVersion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VersionesArticulo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VersionesArticulo_ArticulosConocimiento_ArticuloConocimientoId",
                        column: x => x.ArticuloConocimientoId,
                        principalTable: "ArticulosConocimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VersionesArticulo_Usuarios_ModificadoPorId",
                        column: x => x.ModificadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArticulosEtiquetas",
                columns: table => new
                {
                    ArticuloConocimientoId = table.Column<int>(type: "int", nullable: false),
                    EtiquetaId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticulosEtiquetas", x => new { x.ArticuloConocimientoId, x.EtiquetaId });
                    table.ForeignKey(
                        name: "FK_ArticulosEtiquetas_ArticulosConocimiento_ArticuloConocimientoId",
                        column: x => x.ArticuloConocimientoId,
                        principalTable: "ArticulosConocimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticulosEtiquetas_EtiquetasConocimiento_EtiquetaId",
                        column: x => x.EtiquetaId,
                        principalTable: "EtiquetasConocimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.InsertData(
                table: "EtiquetasConocimiento",
                columns: new[] { "Id", "Color", "CreatedAt", "Descripcion", "IsDeleted", "Nombre", "UpdatedAt", "VecesUsada" },
                values: new object[,]
                {
                    { 1, "#0078D4", new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4105), null, false, "Windows", null, 0 },
                    { 2, "#FCC624", new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4113), null, false, "Linux", null, 0 },
                    { 3, "#00A4EF", new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4115), null, false, "Redes", null, 0 },
                    { 4, "#FF5722", new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4117), null, false, "Hardware", null, 0 },
                    { 5, "#4CAF50", new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4119), null, false, "Software", null, 0 },
                    { 6, "#F44336", new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4122), null, false, "Seguridad", null, 0 },
                    { 7, "#9C27B0", new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4124), null, false, "Base de Datos", null, 0 },
                    { 8, "#607D8B", new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4126), null, false, "Impresión", null, 0 },
                    { 9, "#FF9800", new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4128), null, false, "Email", null, 0 },
                    { 10, "#3F51B5", new DateTime(2025, 12, 2, 22, 38, 32, 15, DateTimeKind.Utc).AddTicks(4130), null, false, "VPN", null, 0 }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_VotacionesArticulo_ArticuloConocimientoId_UsuarioId",
                table: "VotacionesArticulo",
                columns: new[] { "ArticuloConocimientoId", "UsuarioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosArticulo_ComentarioPadreId",
                table: "ComentariosArticulo",
                column: "ComentarioPadreId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticulosEtiquetas_EtiquetaId",
                table: "ArticulosEtiquetas",
                column: "EtiquetaId");

            migrationBuilder.CreateIndex(
                name: "IX_EtiquetasConocimiento_Nombre",
                table: "EtiquetasConocimiento",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ValidacionesArticulo_ArticuloConocimientoId",
                table: "ValidacionesArticulo",
                column: "ArticuloConocimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_ValidacionesArticulo_SolicitadoPorId",
                table: "ValidacionesArticulo",
                column: "SolicitadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_ValidacionesArticulo_ValidadorId",
                table: "ValidacionesArticulo",
                column: "ValidadorId");

            migrationBuilder.CreateIndex(
                name: "IX_VersionesArticulo_ArticuloConocimientoId",
                table: "VersionesArticulo",
                column: "ArticuloConocimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_VersionesArticulo_ModificadoPorId",
                table: "VersionesArticulo",
                column: "ModificadoPorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComentariosArticulo_ArticulosConocimiento_ArticuloConocimientoId",
                table: "ComentariosArticulo",
                column: "ArticuloConocimientoId",
                principalTable: "ArticulosConocimiento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComentariosArticulo_ComentariosArticulo_ComentarioPadreId",
                table: "ComentariosArticulo",
                column: "ComentarioPadreId",
                principalTable: "ComentariosArticulo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComentariosArticulo_Usuarios_UsuarioId",
                table: "ComentariosArticulo",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VotacionesArticulo_ArticulosConocimiento_ArticuloConocimientoId",
                table: "VotacionesArticulo",
                column: "ArticuloConocimientoId",
                principalTable: "ArticulosConocimiento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComentariosArticulo_ArticulosConocimiento_ArticuloConocimientoId",
                table: "ComentariosArticulo");

            migrationBuilder.DropForeignKey(
                name: "FK_ComentariosArticulo_ComentariosArticulo_ComentarioPadreId",
                table: "ComentariosArticulo");

            migrationBuilder.DropForeignKey(
                name: "FK_ComentariosArticulo_Usuarios_UsuarioId",
                table: "ComentariosArticulo");

            migrationBuilder.DropForeignKey(
                name: "FK_VotacionesArticulo_ArticulosConocimiento_ArticuloConocimientoId",
                table: "VotacionesArticulo");

            migrationBuilder.DropTable(
                name: "ArticulosEtiquetas");

            migrationBuilder.DropTable(
                name: "ValidacionesArticulo");

            migrationBuilder.DropTable(
                name: "VersionesArticulo");

            migrationBuilder.DropTable(
                name: "EtiquetasConocimiento");

            migrationBuilder.DropIndex(
                name: "IX_VotacionesArticulo_ArticuloConocimientoId_UsuarioId",
                table: "VotacionesArticulo");

            migrationBuilder.DropIndex(
                name: "IX_ComentariosArticulo_ComentarioPadreId",
                table: "ComentariosArticulo");

            migrationBuilder.DropColumn(
                name: "ArticuloConocimientoId",
                table: "VotacionesArticulo");

            migrationBuilder.DropColumn(
                name: "Comentario",
                table: "VotacionesArticulo");

            migrationBuilder.DropColumn(
                name: "ComentarioPadreId",
                table: "ComentariosArticulo");

            migrationBuilder.DropColumn(
                name: "EsRespuesta",
                table: "ComentariosArticulo");

            migrationBuilder.DropColumn(
                name: "TasaExito",
                table: "ArticulosConocimiento");

            migrationBuilder.DropColumn(
                name: "TipoArticulo",
                table: "ArticulosConocimiento");

            migrationBuilder.DropColumn(
                name: "VecesUtilizado",
                table: "ArticulosConocimiento");

            migrationBuilder.DropColumn(
                name: "VersionActual",
                table: "ArticulosConocimiento");

            migrationBuilder.RenameColumn(
                name: "Voto",
                table: "VotacionesArticulo",
                newName: "ArticuloId");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "ComentariosArticulo",
                newName: "AutorId");

            migrationBuilder.RenameColumn(
                name: "ArticuloConocimientoId",
                table: "ComentariosArticulo",
                newName: "ArticuloId");

            migrationBuilder.RenameIndex(
                name: "IX_ComentariosArticulo_UsuarioId",
                table: "ComentariosArticulo",
                newName: "IX_ComentariosArticulo_AutorId");

            migrationBuilder.RenameIndex(
                name: "IX_ComentariosArticulo_ArticuloConocimientoId",
                table: "ComentariosArticulo",
                newName: "IX_ComentariosArticulo_ArticuloId");

            migrationBuilder.AddColumn<bool>(
                name: "EsPositivo",
                table: "VotacionesArticulo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Puntuacion",
                table: "ComentariosArticulo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 330, DateTimeKind.Utc).AddTicks(4240));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 330, DateTimeKind.Utc).AddTicks(4355));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 330, DateTimeKind.Utc).AddTicks(4358));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 330, DateTimeKind.Utc).AddTicks(4367));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 330, DateTimeKind.Utc).AddTicks(4369));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(512));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(515));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(525));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(526));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2031));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2115));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2119));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2122));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2125));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2201));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2591));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2595));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2598));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2601));

            migrationBuilder.CreateIndex(
                name: "IX_VotacionesArticulo_ArticuloId_UsuarioId",
                table: "VotacionesArticulo",
                columns: new[] { "ArticuloId", "UsuarioId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ComentariosArticulo_ArticulosConocimiento_ArticuloId",
                table: "ComentariosArticulo",
                column: "ArticuloId",
                principalTable: "ArticulosConocimiento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComentariosArticulo_Usuarios_AutorId",
                table: "ComentariosArticulo",
                column: "AutorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VotacionesArticulo_ArticulosConocimiento_ArticuloId",
                table: "VotacionesArticulo",
                column: "ArticuloId",
                principalTable: "ArticulosConocimiento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
