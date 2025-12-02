using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IncidentesFISEI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CatalogoDITICCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServicioDITICId",
                table: "Incidentes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiciosDITIC",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DescripcionDetallada = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TipoServicio = table.Column<int>(type: "int", nullable: false),
                    EsServicioEsencial = table.Column<bool>(type: "bit", nullable: false),
                    HorarioDisponibilidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PorcentajeDisponibilidad = table.Column<double>(type: "float", nullable: false),
                    SLAId = table.Column<int>(type: "int", nullable: true),
                    CategoriaId = table.Column<int>(type: "int", nullable: true),
                    ResponsableTecnicoId = table.Column<int>(type: "int", nullable: true),
                    ResponsableNegocioId = table.Column<int>(type: "int", nullable: true),
                    Requisitos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Limitaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DocumentacionURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CostoEstimado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UnidadCosto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EstaActivo = table.Column<bool>(type: "bit", nullable: false),
                    FechaInicioServicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFinServicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosDITIC", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiciosDITIC_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ServiciosDITIC_SLAs_SLAId",
                        column: x => x.SLAId,
                        principalTable: "SLAs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ServiciosDITIC_Usuarios_ResponsableNegocioId",
                        column: x => x.ResponsableNegocioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiciosDITIC_Usuarios_ResponsableTecnicoId",
                        column: x => x.ResponsableTecnicoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.InsertData(
                table: "ServiciosDITIC",
                columns: new[] { "Id", "CategoriaId", "Codigo", "CostoEstimado", "CreatedAt", "Descripcion", "DescripcionDetallada", "DocumentacionURL", "EsServicioEsencial", "EstaActivo", "FechaFinServicio", "FechaInicioServicio", "HorarioDisponibilidad", "IsDeleted", "Limitaciones", "Nombre", "PorcentajeDisponibilidad", "Requisitos", "ResponsableNegocioId", "ResponsableTecnicoId", "SLAId", "TipoServicio", "UnidadCosto", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 3, "SRV-001", null, new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2031), "Conectividad WiFi en todas las instalaciones de FISEI", "Servicio de acceso inalámbrico a internet para estudiantes, docentes y personal administrativo en todo el campus de FISEI.", null, true, true, null, new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "24/7", false, null, "Acceso a Internet WiFi", 99.0, "Credenciales institucionales UTA", null, 2, 2, 10, null, null },
                    { 2, 5, "SRV-002", null, new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2115), "Servicio de correo electrónico @fisei.uta.edu.ec", "Cuentas de correo institucional con 50GB de almacenamiento, integración con Office 365 y servicios en la nube.", null, true, true, null, new DateTime(2019, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "24/7", false, "Cuota de 50GB, políticas anti-spam aplicables", "Correo Institucional", 99.900000000000006, "Ser estudiante, docente o administrativo activo", null, 2, 1, 5, null, null },
                    { 3, 1, "SRV-003", null, new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2119), "Acceso a laboratorios con equipos especializados", "5 laboratorios equipados con 150+ computadoras con software especializado para ingeniería (MATLAB, AutoCAD, IDEs, etc.)", null, true, true, null, new DateTime(2018, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lunes a Viernes 7:00-21:00, Sábados 8:00-13:00", false, "Capacidad limitada, requiere reserva para uso fuera de horario de clase", "Laboratorios de Computación", 98.0, "Horario asignado por materia o reserva previa", null, 3, 3, 9, null, null },
                    { 4, null, "SRV-004", null, new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2122), "Mesa de ayuda para problemas técnicos", "Atención personalizada para resolver problemas de hardware, software, acceso a sistemas y otros inconvenientes técnicos.", null, true, true, null, new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lunes a Viernes 8:00-17:00", false, null, "Soporte Técnico Help Desk", 100.0, "Ninguno - servicio abierto a toda la comunidad FISEI", null, 2, 2, 3, null, null },
                    { 5, 2, "SRV-005", null, new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2125), "Plataforma para registro de calificaciones y gestión académica", "Sistema integral para gestión de matrículas, calificaciones, horarios, y seguimiento académico de estudiantes.", null, true, true, null, new DateTime(2017, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "24/7", false, "Mantenimiento programado los domingos 2:00-4:00 AM", "Sistema de Gestión Académica", 99.5, "Credenciales institucionales", null, 2, 1, 2, null, null },
                    { 6, 2, "SRV-006", null, new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2201), "Plataforma de aprendizaje en línea", "Sistema LMS para gestión de cursos virtuales, tareas, foros, evaluaciones y recursos educativos digitales.", "https://aulavirtual.fisei.uta.edu.ec/ayuda", true, true, null, new DateTime(2016, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "24/7", false, null, "Aula Virtual (Moodle)", 99.0, "Estar matriculado o ser docente activo", null, 2, 2, 2, null, null },
                    { 7, 1, "SRV-007", 0.05m, new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2591), "Servicio de impresión para trabajos académicos", "Red de impresoras multifunción disponibles para estudiantes y docentes. Incluye impresión, escaneo y fotocopiado.", null, false, true, null, new DateTime(2019, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lunes a Viernes 8:00-18:00", false, "Máximo 50 páginas por día por usuario", "Impresión y Fotocopiado", 95.0, "Credenciales institucionales, cuota de impresión prepagada", null, 4, 3, 9, "por página B/N", null },
                    { 8, 3, "SRV-008", null, new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2595), "Acceso remoto seguro a recursos internos", "Red privada virtual para acceder a recursos institucionales (bibliotecas digitales, sistemas internos) desde fuera del campus.", "https://ditic.uta.edu.ec/vpn", false, true, null, new DateTime(2020, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "24/7", false, "Acceso limitado a docentes e investigadores", "VPN Institucional", 98.0, "Solicitud previa, credenciales institucionales", null, 2, 3, 4, null, null },
                    { 9, 2, "SRV-009", null, new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2598), "Almacenamiento de trabajos de titulación y publicaciones", "Plataforma para almacenar, preservar y difundir la producción académica de FISEI (tesis, papers, proyectos).", "https://repositorio.uta.edu.ec", false, true, null, new DateTime(2015, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "24/7", false, null, "Repositorio Digital Institucional", 99.5, "Aprobación del tutor y defensa del trabajo", null, 2, 4, 6, null, null },
                    { 10, 2, "SRV-010", null, new DateTime(2025, 12, 2, 22, 16, 0, 331, DateTimeKind.Utc).AddTicks(2601), "Acceso a software especializado con licencia institucional", "Provisión de licencias para software de ingeniería: MATLAB, Autodesk, Microsoft Office 365, JetBrains, Visual Studio, etc.", null, true, true, null, new DateTime(2018, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "24/7 (activación)", false, "Licencias limitadas según convenios vigentes", "Licencias de Software Académico", 99.0, "Ser estudiante o docente activo, solicitud por correo institucional", null, 2, 3, 2, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Incidentes_ServicioDITICId",
                table: "Incidentes",
                column: "ServicioDITICId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosDITIC_CategoriaId",
                table: "ServiciosDITIC",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosDITIC_Codigo",
                table: "ServiciosDITIC",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosDITIC_ResponsableNegocioId",
                table: "ServiciosDITIC",
                column: "ResponsableNegocioId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosDITIC_ResponsableTecnicoId",
                table: "ServiciosDITIC",
                column: "ResponsableTecnicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiciosDITIC_SLAId",
                table: "ServiciosDITIC",
                column: "SLAId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidentes_ServiciosDITIC_ServicioDITICId",
                table: "Incidentes",
                column: "ServicioDITICId",
                principalTable: "ServiciosDITIC",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidentes_ServiciosDITIC_ServicioDITICId",
                table: "Incidentes");

            migrationBuilder.DropTable(
                name: "ServiciosDITIC");

            migrationBuilder.DropIndex(
                name: "IX_Incidentes_ServicioDITICId",
                table: "Incidentes");

            migrationBuilder.DropColumn(
                name: "ServicioDITICId",
                table: "Incidentes");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(1704));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(1779));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(1782));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(1794));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(1796));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(7846));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(7848));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(7850));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 1, 39, 618, DateTimeKind.Utc).AddTicks(7852));
        }
    }
}
