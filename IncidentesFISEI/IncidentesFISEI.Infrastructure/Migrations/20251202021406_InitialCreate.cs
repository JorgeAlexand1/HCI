using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IncidentesFISEI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    Icono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    TiempoRespuestaMinutos = table.Column<int>(type: "int", nullable: true),
                    TiempoResolucionMinutos = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categorias_Categorias_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SLAs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TiempoRespuesta = table.Column<int>(type: "int", nullable: false),
                    TiempoResolucion = table.Column<int>(type: "int", nullable: false),
                    Prioridad = table.Column<int>(type: "int", nullable: false),
                    Impacto = table.Column<int>(type: "int", nullable: false),
                    Urgencia = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SLAs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TipoUsuario = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsEmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Especialidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AñosExperiencia = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticulosConocimiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Resumen = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Visualizaciones = table.Column<int>(type: "int", nullable: false),
                    VotosPositivos = table.Column<int>(type: "int", nullable: false),
                    VotosNegativos = table.Column<int>(type: "int", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaRevision = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EsSolucionValidada = table.Column<bool>(type: "bit", nullable: false),
                    PasosDetallados = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    Prerequisites = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Limitaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AutorId = table.Column<int>(type: "int", nullable: false),
                    RevisadoPorId = table.Column<int>(type: "int", nullable: true),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticulosConocimiento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticulosConocimiento_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticulosConocimiento_Usuarios_AutorId",
                        column: x => x.AutorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticulosConocimiento_Usuarios_RevisadoPorId",
                        column: x => x.RevisadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ComentariosArticulo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Contenido = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Puntuacion = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ArticuloId = table.Column<int>(type: "int", nullable: false),
                    AutorId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentariosArticulo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComentariosArticulo_ArticulosConocimiento_ArticuloId",
                        column: x => x.ArticuloId,
                        principalTable: "ArticulosConocimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComentariosArticulo_Usuarios_AutorId",
                        column: x => x.AutorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Incidentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroIncidente = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Prioridad = table.Column<int>(type: "int", nullable: false),
                    Impacto = table.Column<int>(type: "int", nullable: false),
                    Urgencia = table.Column<int>(type: "int", nullable: false),
                    FechaReporte = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaInicioTrabajo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Solucion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CausaRaiz = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PasosReproducir = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ImpactoDetallado = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ActivosAfectados = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReportadoPorId = table.Column<int>(type: "int", nullable: false),
                    AsignadoAId = table.Column<int>(type: "int", nullable: true),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    ArticuloConocimientoId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incidentes_ArticulosConocimiento_ArticuloConocimientoId",
                        column: x => x.ArticuloConocimientoId,
                        principalTable: "ArticulosConocimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Incidentes_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidentes_Usuarios_AsignadoAId",
                        column: x => x.AsignadoAId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Incidentes_Usuarios_ReportadoPorId",
                        column: x => x.ReportadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VotacionesArticulo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EsPositivo = table.Column<bool>(type: "bit", nullable: false),
                    ArticuloId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotacionesArticulo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VotacionesArticulo_ArticulosConocimiento_ArticuloId",
                        column: x => x.ArticuloId,
                        principalTable: "ArticulosConocimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VotacionesArticulo_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArchivosAdjuntos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreOriginal = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TipoMime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TamañoBytes = table.Column<long>(type: "bigint", nullable: false),
                    IncidenteId = table.Column<int>(type: "int", nullable: true),
                    ArticuloId = table.Column<int>(type: "int", nullable: true),
                    SubidoPorId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivosAdjuntos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivosAdjuntos_ArticulosConocimiento_ArticuloId",
                        column: x => x.ArticuloId,
                        principalTable: "ArticulosConocimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArchivosAdjuntos_Incidentes_IncidenteId",
                        column: x => x.IncidenteId,
                        principalTable: "Incidentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArchivosAdjuntos_Usuarios_SubidoPorId",
                        column: x => x.SubidoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComentariosIncidente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Contenido = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    EsInterno = table.Column<bool>(type: "bit", nullable: false),
                    IncidenteId = table.Column<int>(type: "int", nullable: false),
                    AutorId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentariosIncidente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComentariosIncidente_Incidentes_IncidenteId",
                        column: x => x.IncidenteId,
                        principalTable: "Incidentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComentariosIncidente_Usuarios_AutorId",
                        column: x => x.AutorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EscalacionesSLA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SLAId = table.Column<int>(type: "int", nullable: false),
                    IncidenteId = table.Column<int>(type: "int", nullable: false),
                    FechaEscalacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FueNotificado = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscalacionesSLA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EscalacionesSLA_Incidentes_IncidenteId",
                        column: x => x.IncidenteId,
                        principalTable: "Incidentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EscalacionesSLA_SLAs_SLAId",
                        column: x => x.SLAId,
                        principalTable: "SLAs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncidentesRelacionados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentePrincipalId = table.Column<int>(type: "int", nullable: false),
                    IncidenteRelacionadoId = table.Column<int>(type: "int", nullable: false),
                    TipoRelacion = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentesRelacionados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentesRelacionados_Incidentes_IncidentePrincipalId",
                        column: x => x.IncidentePrincipalId,
                        principalTable: "Incidentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentesRelacionados_Incidentes_IncidenteRelacionadoId",
                        column: x => x.IncidenteRelacionadoId,
                        principalTable: "Incidentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosTiempo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TiempoTranscurrido = table.Column<TimeSpan>(type: "time", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TipoActividad = table.Column<int>(type: "int", nullable: false),
                    IncidenteId = table.Column<int>(type: "int", nullable: false),
                    TecnicoId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosTiempo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosTiempo_Incidentes_IncidenteId",
                        column: x => x.IncidenteId,
                        principalTable: "Incidentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrosTiempo_Usuarios_TecnicoId",
                        column: x => x.TecnicoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Color", "CreatedAt", "Descripcion", "Icono", "IsActive", "IsDeleted", "Nombre", "ParentCategoryId", "TiempoResolucionMinutos", "TiempoRespuestaMinutos", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "#dc3545", new DateTime(2025, 12, 2, 2, 14, 5, 891, DateTimeKind.Utc).AddTicks(8295), "Problemas relacionados con hardware", "fas fa-desktop", true, false, "Hardware", null, 240, 30, null },
                    { 2, "#007bff", new DateTime(2025, 12, 2, 2, 14, 5, 891, DateTimeKind.Utc).AddTicks(8527), "Problemas relacionados con software", "fas fa-code", true, false, "Software", null, 480, 60, null },
                    { 3, "#28a745", new DateTime(2025, 12, 2, 2, 14, 5, 891, DateTimeKind.Utc).AddTicks(8530), "Problemas de conectividad y red", "fas fa-network-wired", true, false, "Red", null, 120, 15, null },
                    { 4, "#ffc107", new DateTime(2025, 12, 2, 2, 14, 5, 891, DateTimeKind.Utc).AddTicks(8533), "Problemas de autenticación y permisos", "fas fa-lock", true, false, "Acceso", null, 180, 45, null },
                    { 5, "#17a2b8", new DateTime(2025, 12, 2, 2, 14, 5, 891, DateTimeKind.Utc).AddTicks(8535), "Problemas con correo electrónico", "fas fa-envelope", true, false, "Correo", null, 360, 60, null }
                });

            migrationBuilder.InsertData(
                table: "SLAs",
                columns: new[] { "Id", "CreatedAt", "Descripcion", "Impacto", "IsActive", "IsDeleted", "Nombre", "Prioridad", "TiempoResolucion", "TiempoRespuesta", "UpdatedAt", "Urgencia" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 2, 2, 14, 5, 892, DateTimeKind.Utc).AddTicks(8361), "SLA para incidentes críticos", 4, true, false, "Crítico", 4, 60, 15, null, 4 },
                    { 2, new DateTime(2025, 12, 2, 2, 14, 5, 892, DateTimeKind.Utc).AddTicks(8366), "SLA para incidentes de alta prioridad", 3, true, false, "Alto", 3, 120, 30, null, 3 },
                    { 3, new DateTime(2025, 12, 2, 2, 14, 5, 892, DateTimeKind.Utc).AddTicks(8368), "SLA para incidentes de prioridad media", 2, true, false, "Medio", 2, 480, 60, null, 2 },
                    { 4, new DateTime(2025, 12, 2, 2, 14, 5, 892, DateTimeKind.Utc).AddTicks(8370), "SLA para incidentes de baja prioridad", 1, true, false, "Bajo", 1, 2880, 240, null, 1 }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "AñosExperiencia", "CreatedAt", "Department", "Email", "Especialidad", "FirstName", "IsActive", "IsDeleted", "IsEmailConfirmed", "LastLoginAt", "LastName", "PasswordHash", "Phone", "TipoUsuario", "UpdatedAt", "Username" },
                values: new object[] { 1, null, new DateTime(2025, 12, 2, 2, 14, 5, 892, DateTimeKind.Utc).AddTicks(6610), "FISEI", "admin@fisei.uta.edu.ec", null, "Administrador", true, false, true, null, "Sistema", "$2a$11$9f5rP2P5qP5qP5qP5qP5qOG5rP2P5qP5qP5qP5qP5qP5qP5qP5qP5q", null, 4, null, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivosAdjuntos_ArticuloId",
                table: "ArchivosAdjuntos",
                column: "ArticuloId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivosAdjuntos_IncidenteId",
                table: "ArchivosAdjuntos",
                column: "IncidenteId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivosAdjuntos_SubidoPorId",
                table: "ArchivosAdjuntos",
                column: "SubidoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticulosConocimiento_AutorId",
                table: "ArticulosConocimiento",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticulosConocimiento_CategoriaId",
                table: "ArticulosConocimiento",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticulosConocimiento_RevisadoPorId",
                table: "ArticulosConocimiento",
                column: "RevisadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_ParentCategoryId",
                table: "Categorias",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosArticulo_ArticuloId",
                table: "ComentariosArticulo",
                column: "ArticuloId");

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosArticulo_AutorId",
                table: "ComentariosArticulo",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosIncidente_AutorId",
                table: "ComentariosIncidente",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosIncidente_IncidenteId",
                table: "ComentariosIncidente",
                column: "IncidenteId");

            migrationBuilder.CreateIndex(
                name: "IX_EscalacionesSLA_IncidenteId",
                table: "EscalacionesSLA",
                column: "IncidenteId");

            migrationBuilder.CreateIndex(
                name: "IX_EscalacionesSLA_SLAId",
                table: "EscalacionesSLA",
                column: "SLAId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidentes_ArticuloConocimientoId",
                table: "Incidentes",
                column: "ArticuloConocimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidentes_AsignadoAId",
                table: "Incidentes",
                column: "AsignadoAId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidentes_CategoriaId",
                table: "Incidentes",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidentes_NumeroIncidente",
                table: "Incidentes",
                column: "NumeroIncidente",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidentes_ReportadoPorId",
                table: "Incidentes",
                column: "ReportadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentesRelacionados_IncidentePrincipalId",
                table: "IncidentesRelacionados",
                column: "IncidentePrincipalId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentesRelacionados_IncidenteRelacionadoId",
                table: "IncidentesRelacionados",
                column: "IncidenteRelacionadoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosTiempo_IncidenteId",
                table: "RegistrosTiempo",
                column: "IncidenteId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosTiempo_TecnicoId",
                table: "RegistrosTiempo",
                column: "TecnicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Username",
                table: "Usuarios",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VotacionesArticulo_ArticuloId_UsuarioId",
                table: "VotacionesArticulo",
                columns: new[] { "ArticuloId", "UsuarioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VotacionesArticulo_UsuarioId",
                table: "VotacionesArticulo",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivosAdjuntos");

            migrationBuilder.DropTable(
                name: "ComentariosArticulo");

            migrationBuilder.DropTable(
                name: "ComentariosIncidente");

            migrationBuilder.DropTable(
                name: "EscalacionesSLA");

            migrationBuilder.DropTable(
                name: "IncidentesRelacionados");

            migrationBuilder.DropTable(
                name: "RegistrosTiempo");

            migrationBuilder.DropTable(
                name: "VotacionesArticulo");

            migrationBuilder.DropTable(
                name: "SLAs");

            migrationBuilder.DropTable(
                name: "Incidentes");

            migrationBuilder.DropTable(
                name: "ArticulosConocimiento");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
