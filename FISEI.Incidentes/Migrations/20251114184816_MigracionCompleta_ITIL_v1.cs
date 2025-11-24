using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FISEI.Incidentes.Migrations
{
    /// <inheritdoc />
    public partial class MigracionCompleta_ITIL_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    IdCategoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.IdCategoria);
                });

            migrationBuilder.CreateTable(
                name: "Estados",
                columns: table => new
                {
                    IdEstado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.IdEstado);
                });

            migrationBuilder.CreateTable(
                name: "NIVEL_SOPORTE",
                columns: table => new
                {
                    IdNivelSoporte = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NIVEL_SOPORTE", x => x.IdNivelSoporte);
                });

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

            migrationBuilder.CreateTable(
                name: "SERVICIO",
                columns: table => new
                {
                    IdServicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Responsable = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SLA = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AreaDestino = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SERVICIO", x => x.IdServicio);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdRol = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Usuarios_ROL_IdRol",
                        column: x => x.IdRol,
                        principalTable: "ROL",
                        principalColumn: "IdRol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "INCIDENTE",
                columns: table => new
                {
                    IdIncidente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdCategoria = table.Column<int>(type: "int", nullable: false),
                    IdEstado = table.Column<int>(type: "int", nullable: false),
                    IdNivelSoporte = table.Column<int>(type: "int", nullable: false),
                    IdServicio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INCIDENTE", x => x.IdIncidente);
                    table.ForeignKey(
                        name: "FK_INCIDENTE_Categorias_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categorias",
                        principalColumn: "IdCategoria",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_INCIDENTE_Estados_IdEstado",
                        column: x => x.IdEstado,
                        principalTable: "Estados",
                        principalColumn: "IdEstado",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_INCIDENTE_NIVEL_SOPORTE_IdNivelSoporte",
                        column: x => x.IdNivelSoporte,
                        principalTable: "NIVEL_SOPORTE",
                        principalColumn: "IdNivelSoporte",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_INCIDENTE_SERVICIO_IdServicio",
                        column: x => x.IdServicio,
                        principalTable: "SERVICIO",
                        principalColumn: "IdServicio",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_INCIDENTE_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NOTIFICACION",
                columns: table => new
                {
                    IdNotificacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mensaje = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Leido = table.Column<bool>(type: "bit", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUsuarioDestino = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NOTIFICACION", x => x.IdNotificacion);
                    table.ForeignKey(
                        name: "FK_NOTIFICACION_Usuarios_IdUsuarioDestino",
                        column: x => x.IdUsuarioDestino,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ASIGNACION",
                columns: table => new
                {
                    IdAsignacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdIncidente = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioAsignado = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASIGNACION", x => x.IdAsignacion);
                    table.ForeignKey(
                        name: "FK_ASIGNACION_INCIDENTE_IdIncidente",
                        column: x => x.IdIncidente,
                        principalTable: "INCIDENTE",
                        principalColumn: "IdIncidente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ASIGNACION_Usuarios_IdUsuarioAsignado",
                        column: x => x.IdUsuarioAsignado,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CONOCIMIENTO",
                columns: table => new
                {
                    IdConocimiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Solucion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PalabrasClave = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdCategoria = table.Column<int>(type: "int", nullable: false),
                    IdIncidenteOrigen = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Visualizaciones = table.Column<int>(type: "int", nullable: false),
                    Calificacion = table.Column<int>(type: "int", nullable: true),
                    Aprobado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONOCIMIENTO", x => x.IdConocimiento);
                    table.ForeignKey(
                        name: "FK_CONOCIMIENTO_Categorias_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categorias",
                        principalColumn: "IdCategoria",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CONOCIMIENTO_INCIDENTE_IdIncidenteOrigen",
                        column: x => x.IdIncidenteOrigen,
                        principalTable: "INCIDENTE",
                        principalColumn: "IdIncidente",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HISTORIAL",
                columns: table => new
                {
                    IdHistorial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IdIncidente = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HISTORIAL", x => x.IdHistorial);
                    table.ForeignKey(
                        name: "FK_HISTORIAL_INCIDENTE_IdIncidente",
                        column: x => x.IdIncidente,
                        principalTable: "INCIDENTE",
                        principalColumn: "IdIncidente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HISTORIAL_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ASIGNACION_IdIncidente",
                table: "ASIGNACION",
                column: "IdIncidente");

            migrationBuilder.CreateIndex(
                name: "IX_ASIGNACION_IdUsuarioAsignado",
                table: "ASIGNACION",
                column: "IdUsuarioAsignado");

            migrationBuilder.CreateIndex(
                name: "IX_CONOCIMIENTO_IdCategoria",
                table: "CONOCIMIENTO",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_CONOCIMIENTO_IdIncidenteOrigen",
                table: "CONOCIMIENTO",
                column: "IdIncidenteOrigen");

            migrationBuilder.CreateIndex(
                name: "IX_HISTORIAL_IdIncidente",
                table: "HISTORIAL",
                column: "IdIncidente");

            migrationBuilder.CreateIndex(
                name: "IX_HISTORIAL_IdUsuario",
                table: "HISTORIAL",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_INCIDENTE_IdCategoria",
                table: "INCIDENTE",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_INCIDENTE_IdEstado",
                table: "INCIDENTE",
                column: "IdEstado");

            migrationBuilder.CreateIndex(
                name: "IX_INCIDENTE_IdNivelSoporte",
                table: "INCIDENTE",
                column: "IdNivelSoporte");

            migrationBuilder.CreateIndex(
                name: "IX_INCIDENTE_IdServicio",
                table: "INCIDENTE",
                column: "IdServicio");

            migrationBuilder.CreateIndex(
                name: "IX_INCIDENTE_IdUsuario",
                table: "INCIDENTE",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_NOTIFICACION_IdUsuarioDestino",
                table: "NOTIFICACION",
                column: "IdUsuarioDestino");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios",
                column: "IdRol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ASIGNACION");

            migrationBuilder.DropTable(
                name: "CONOCIMIENTO");

            migrationBuilder.DropTable(
                name: "HISTORIAL");

            migrationBuilder.DropTable(
                name: "NOTIFICACION");

            migrationBuilder.DropTable(
                name: "INCIDENTE");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Estados");

            migrationBuilder.DropTable(
                name: "NIVEL_SOPORTE");

            migrationBuilder.DropTable(
                name: "SERVICIO");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "ROL");
        }
    }
}
