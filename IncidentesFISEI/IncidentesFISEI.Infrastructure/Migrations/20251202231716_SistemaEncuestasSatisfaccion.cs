using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IncidentesFISEI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SistemaEncuestasSatisfaccion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlantillasEncuesta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EsActiva = table.Column<bool>(type: "bit", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    MostrarAutomaticamente = table.Column<bool>(type: "bit", nullable: false),
                    DiasVigencia = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantillasEncuesta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Encuestas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidenteId = table.Column<int>(type: "int", nullable: false),
                    PlantillaEncuestaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EsRespondida = table.Column<bool>(type: "bit", nullable: false),
                    EsVencida = table.Column<bool>(type: "bit", nullable: false),
                    CalificacionPromedio = table.Column<double>(type: "float", nullable: true),
                    ComentariosGenerales = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Encuestas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Encuestas_Incidentes_IncidenteId",
                        column: x => x.IncidenteId,
                        principalTable: "Incidentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Encuestas_PlantillasEncuesta_PlantillaEncuestaId",
                        column: x => x.PlantillaEncuestaId,
                        principalTable: "PlantillasEncuesta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Encuestas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreguntasEncuesta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantillaEncuestaId = table.Column<int>(type: "int", nullable: false),
                    TextoPregunta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    EsObligatoria = table.Column<bool>(type: "bit", nullable: false),
                    OpcionesJson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ValorMinimo = table.Column<int>(type: "int", nullable: true),
                    ValorMaximo = table.Column<int>(type: "int", nullable: true),
                    EtiquetaMinimo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EtiquetaMaximo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreguntasEncuesta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreguntasEncuesta_PlantillasEncuesta_PlantillaEncuestaId",
                        column: x => x.PlantillaEncuestaId,
                        principalTable: "PlantillasEncuesta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RespuestasEncuesta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EncuestaId = table.Column<int>(type: "int", nullable: false),
                    PreguntaEncuestaId = table.Column<int>(type: "int", nullable: false),
                    RespuestaTexto = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RespuestaNumero = table.Column<int>(type: "int", nullable: true),
                    RespuestaBooleana = table.Column<bool>(type: "bit", nullable: true),
                    RespuestaFecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RespuestasSeleccionJson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespuestasEncuesta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RespuestasEncuesta_Encuestas_EncuestaId",
                        column: x => x.EncuestaId,
                        principalTable: "Encuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RespuestasEncuesta_PreguntasEncuesta_PreguntaEncuestaId",
                        column: x => x.PreguntaEncuestaId,
                        principalTable: "PreguntasEncuesta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 191, DateTimeKind.Utc).AddTicks(8772));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 191, DateTimeKind.Utc).AddTicks(8903));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 191, DateTimeKind.Utc).AddTicks(8907));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 191, DateTimeKind.Utc).AddTicks(8930));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 191, DateTimeKind.Utc).AddTicks(8933));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6487));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6492));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6495));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6506));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6508));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6510));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6512));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6514));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6516));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 204, DateTimeKind.Utc).AddTicks(6518));

            migrationBuilder.InsertData(
                table: "PlantillasEncuesta",
                columns: new[] { "Id", "CreatedAt", "Descripcion", "DiasVigencia", "EsActiva", "IsDeleted", "MostrarAutomaticamente", "Nombre", "Tipo", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2025, 12, 2, 23, 17, 15, 211, DateTimeKind.Utc).AddTicks(1809), "Encuesta estándar enviada automáticamente al cerrar un incidente", 7, true, false, true, "Encuesta de Satisfacción Post-Cierre", 1, null });

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 192, DateTimeKind.Utc).AddTicks(9013));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 192, DateTimeKind.Utc).AddTicks(9018));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 192, DateTimeKind.Utc).AddTicks(9042));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 192, DateTimeKind.Utc).AddTicks(9045));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(1942));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2082));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2112));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2117));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2122));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2265));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2940));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2947));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2952));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 23, 17, 15, 193, DateTimeKind.Utc).AddTicks(2957));

            migrationBuilder.InsertData(
                table: "PreguntasEncuesta",
                columns: new[] { "Id", "CreatedAt", "EsObligatoria", "EtiquetaMaximo", "EtiquetaMinimo", "IsDeleted", "OpcionesJson", "Orden", "PlantillaEncuestaId", "TextoPregunta", "Tipo", "UpdatedAt", "ValorMaximo", "ValorMinimo" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 2, 23, 17, 15, 211, DateTimeKind.Utc).AddTicks(4150), true, "Muy satisfecho", "Muy insatisfecho", false, null, 1, 1, "¿Qué tan satisfecho está con la resolución del incidente?", 8, null, 5, 1 },
                    { 2, new DateTime(2025, 12, 2, 23, 17, 15, 211, DateTimeKind.Utc).AddTicks(4156), true, null, null, false, null, 2, 1, "¿El tiempo de respuesta fue adecuado?", 6, null, null, null },
                    { 3, new DateTime(2025, 12, 2, 23, 17, 15, 211, DateTimeKind.Utc).AddTicks(4161), true, "Muy probable", "Nada probable", false, null, 3, 1, "¿Qué tan probable es que recomiende nuestro servicio de soporte?", 3, null, 10, 0 },
                    { 4, new DateTime(2025, 12, 2, 23, 17, 15, 211, DateTimeKind.Utc).AddTicks(4164), true, "Excelente", "Muy malo", false, null, 4, 1, "¿El técnico fue profesional y cortés?", 8, null, 5, 1 },
                    { 5, new DateTime(2025, 12, 2, 23, 17, 15, 211, DateTimeKind.Utc).AddTicks(4167), false, null, null, false, null, 5, 1, "Comentarios adicionales o sugerencias", 2, null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Encuestas_EsRespondida",
                table: "Encuestas",
                column: "EsRespondida");

            migrationBuilder.CreateIndex(
                name: "IX_Encuestas_EsVencida",
                table: "Encuestas",
                column: "EsVencida");

            migrationBuilder.CreateIndex(
                name: "IX_Encuestas_FechaEnvio",
                table: "Encuestas",
                column: "FechaEnvio");

            migrationBuilder.CreateIndex(
                name: "IX_Encuestas_FechaVencimiento",
                table: "Encuestas",
                column: "FechaVencimiento");

            migrationBuilder.CreateIndex(
                name: "IX_Encuestas_IncidenteId",
                table: "Encuestas",
                column: "IncidenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Encuestas_PlantillaEncuestaId",
                table: "Encuestas",
                column: "PlantillaEncuestaId");

            migrationBuilder.CreateIndex(
                name: "IX_Encuestas_UsuarioId",
                table: "Encuestas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasEncuesta_EsActiva",
                table: "PlantillasEncuesta",
                column: "EsActiva");

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasEncuesta_Tipo",
                table: "PlantillasEncuesta",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_PreguntasEncuesta_Orden",
                table: "PreguntasEncuesta",
                column: "Orden");

            migrationBuilder.CreateIndex(
                name: "IX_PreguntasEncuesta_PlantillaEncuestaId",
                table: "PreguntasEncuesta",
                column: "PlantillaEncuestaId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasEncuesta_EncuestaId",
                table: "RespuestasEncuesta",
                column: "EncuestaId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasEncuesta_PreguntaEncuestaId",
                table: "RespuestasEncuesta",
                column: "PreguntaEncuestaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RespuestasEncuesta");

            migrationBuilder.DropTable(
                name: "Encuestas");

            migrationBuilder.DropTable(
                name: "PreguntasEncuesta");

            migrationBuilder.DropTable(
                name: "PlantillasEncuesta");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 715, DateTimeKind.Utc).AddTicks(6456));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 715, DateTimeKind.Utc).AddTicks(6593));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 715, DateTimeKind.Utc).AddTicks(6597));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 715, DateTimeKind.Utc).AddTicks(6600));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 715, DateTimeKind.Utc).AddTicks(6603));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2326));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2334));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2337));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2339));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2341));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2343));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2345));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2347));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2349));

            migrationBuilder.UpdateData(
                table: "EtiquetasConocimiento",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 730, DateTimeKind.Utc).AddTicks(2351));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 716, DateTimeKind.Utc).AddTicks(8369));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 716, DateTimeKind.Utc).AddTicks(8375));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 716, DateTimeKind.Utc).AddTicks(8378));

            migrationBuilder.UpdateData(
                table: "SLAs",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 716, DateTimeKind.Utc).AddTicks(8381));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(1150));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(1296));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(1304));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(1309));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(1315));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(1444));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(2102));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(2109));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(2115));

            migrationBuilder.UpdateData(
                table: "ServiciosDITIC",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 2, 22, 55, 7, 717, DateTimeKind.Utc).AddTicks(2120));
        }
    }
}
