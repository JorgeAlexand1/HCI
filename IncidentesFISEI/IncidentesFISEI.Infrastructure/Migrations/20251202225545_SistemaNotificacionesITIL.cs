using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentesFISEI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SistemaNotificacionesITIL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionesNotificacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoEvento = table.Column<int>(type: "int", nullable: false),
                    NotificarEnSistema = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NotificarPorEmail = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NotificarPorSMS = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NotificacionInmediata = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ResumenDiario = table.Column<bool>(type: "bit", nullable: false),
                    ResumenSemanal = table.Column<bool>(type: "bit", nullable: false),
                    SoloPrioridad = table.Column<int>(type: "int", nullable: true),
                    SoloIncidentesAsignados = table.Column<bool>(type: "bit", nullable: false),
                    HoraInicioSilencioso = table.Column<TimeOnly>(type: "time", nullable: true),
                    HoraFinSilencioso = table.Column<TimeOnly>(type: "time", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesNotificacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracionesNotificacion_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TipoNotificacion = table.Column<int>(type: "int", nullable: false),
                    Prioridad = table.Column<int>(type: "int", nullable: false),
                    Leida = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaLectura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DatosAdicionales = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlAccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    IncidenteId = table.Column<int>(type: "int", nullable: true),
                    NotificadoPorEmail = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NotificadoPorSMS = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaEnvioEmail = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaEnvioSMS = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorEnvio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Incidentes_IncidenteId",
                        column: x => x.IncidenteId,
                        principalTable: "Incidentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlantillasNotificacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoNotificacion = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlantillaTitulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PlantillaMensaje = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    PlantillaEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantillaSMS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    VariablesDisponibles = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantillasNotificacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogsNotificacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificacionId = table.Column<int>(type: "int", nullable: false),
                    Canal = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaIntento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DireccionDestino = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MensajeError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorDetalle = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IdExterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroReintentos = table.Column<int>(type: "int", nullable: false),
                    ProximoReintento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsNotificacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogsNotificacion_Notificaciones_NotificacionId",
                        column: x => x.NotificacionId,
                        principalTable: "Notificaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesNotificacion_UsuarioId_TipoEvento",
                table: "ConfiguracionesNotificacion",
                columns: new[] { "UsuarioId", "TipoEvento" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogsNotificacion_Estado_FechaIntento",
                table: "LogsNotificacion",
                columns: new[] { "Estado", "FechaIntento" });

            migrationBuilder.CreateIndex(
                name: "IX_LogsNotificacion_NotificacionId",
                table: "LogsNotificacion",
                column: "NotificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_IncidenteId",
                table: "Notificaciones",
                column: "IncidenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioId",
                table: "Notificaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioId_Leida",
                table: "Notificaciones",
                columns: new[] { "UsuarioId", "Leida" });

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasNotificacion_TipoNotificacion_IsActive",
                table: "PlantillasNotificacion",
                columns: new[] { "TipoNotificacion", "IsActive" },
                filter: "[IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionesNotificacion");

            migrationBuilder.DropTable(
                name: "LogsNotificacion");

            migrationBuilder.DropTable(
                name: "PlantillasNotificacion");

            migrationBuilder.DropTable(
                name: "Notificaciones");

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
        }
    }
}
