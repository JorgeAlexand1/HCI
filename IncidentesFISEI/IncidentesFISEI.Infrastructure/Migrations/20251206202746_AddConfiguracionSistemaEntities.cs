using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentesFISEI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracionSistemaEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionesServiciosExternos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ConfiguracionJSON = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EstaConectado = table.Column<bool>(type: "bit", nullable: false),
                    UltimaConexion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MensajeError = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesServiciosExternos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionesSistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EmailSoporte = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TiempoRespuestaDefecto = table.Column<int>(type: "int", nullable: false),
                    TiempoResolucionDefecto = table.Column<int>(type: "int", nullable: false),
                    PermitirEscalacionAutomatica = table.Column<bool>(type: "bit", nullable: false),
                    PortcentajeEscalacion = table.Column<int>(type: "int", nullable: false),
                    NotificacionesHabilitadas = table.Column<bool>(type: "bit", nullable: false),
                    NotificacionesPorEmail = table.Column<bool>(type: "bit", nullable: false),
                    NotificacionesPorSistema = table.Column<bool>(type: "bit", nullable: false),
                    AuditoriaHabilitada = table.Column<bool>(type: "bit", nullable: false),
                    DiasRetencionLogs = table.Column<int>(type: "int", nullable: false),
                    ZonaHoraria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualizadoPor = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionesSLA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TiempoRespuestaCritica = table.Column<int>(type: "int", nullable: false),
                    TiempoRespuestaAlta = table.Column<int>(type: "int", nullable: false),
                    TiempoRespuestaMedia = table.Column<int>(type: "int", nullable: false),
                    TiempoRespuestaBaja = table.Column<int>(type: "int", nullable: false),
                    TiempoResolucionCritica = table.Column<int>(type: "int", nullable: false),
                    TiempoResolucionAlta = table.Column<int>(type: "int", nullable: false),
                    TiempoResolucionMedia = table.Column<int>(type: "int", nullable: false),
                    TiempoResolucionBaja = table.Column<int>(type: "int", nullable: false),
                    PermitirEscalacion = table.Column<bool>(type: "bit", nullable: false),
                    MinutosAntesDeLlegar = table.Column<int>(type: "int", nullable: false),
                    NotificarAlEscalar = table.Column<bool>(type: "bit", nullable: false),
                    NotificarAlVencer = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesSLA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PoliticasSeguridad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LongitudMinimaContraseña = table.Column<int>(type: "int", nullable: false),
                    RequerirMayusculas = table.Column<bool>(type: "bit", nullable: false),
                    RequerirMinusculas = table.Column<bool>(type: "bit", nullable: false),
                    RequerirNumeros = table.Column<bool>(type: "bit", nullable: false),
                    RequerirCaracteresEspeciales = table.Column<bool>(type: "bit", nullable: false),
                    DiasExpiracionContraseña = table.Column<int>(type: "int", nullable: false),
                    IntentosLoginMaximos = table.Column<int>(type: "int", nullable: false),
                    MinutosBloqueoDespuesIntentos = table.Column<int>(type: "int", nullable: false),
                    MinutosTimeoutSesion = table.Column<int>(type: "int", nullable: false),
                    CerrarSesionAlCambiarContraseña = table.Column<bool>(type: "bit", nullable: false),
                    PermitirMultiplesSesiones = table.Column<bool>(type: "bit", nullable: false),
                    RequiereVerificacionDosFactores = table.Column<bool>(type: "bit", nullable: false),
                    PermitirAccesoDesdeIPsDiferentes = table.Column<bool>(type: "bit", nullable: false),
                    AuditoriaActividadAdmin = table.Column<bool>(type: "bit", nullable: false),
                    AuditoriaLecturaIncidentes = table.Column<bool>(type: "bit", nullable: false),
                    AuditoriaModificacionIncidentes = table.Column<bool>(type: "bit", nullable: false),
                    AuditoriaAccesoSistema = table.Column<bool>(type: "bit", nullable: false),
                    EncriptarDatosEnTransito = table.Column<bool>(type: "bit", nullable: false),
                    EncriptarDatosEnReposo = table.Column<bool>(type: "bit", nullable: false),
                    NormasAplicables = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    UltimaRevision = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevisadoPor = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoliticasSeguridad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosAuditoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Accion = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DireccionIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DetallesJSON = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Exitoso = table.Column<bool>(type: "bit", nullable: false),
                    MensajeError = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosAuditoria", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionesServiciosExternos");

            migrationBuilder.DropTable(
                name: "ConfiguracionesSistema");

            migrationBuilder.DropTable(
                name: "ConfiguracionesSLA");

            migrationBuilder.DropTable(
                name: "PoliticasSeguridad");

            migrationBuilder.DropTable(
                name: "RegistrosAuditoria");

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
        }
    }
}
