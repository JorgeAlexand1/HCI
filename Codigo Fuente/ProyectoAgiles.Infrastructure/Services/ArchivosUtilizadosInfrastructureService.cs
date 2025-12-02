using Microsoft.EntityFrameworkCore;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Infrastructure.Data;

namespace ProyectoAgiles.Infrastructure.Services;

/// <summary>
/// Servicio de infraestructura para gestionar archivos utilizados en escalafones
/// Tiene acceso directo al DbContext para operaciones complejas
/// </summary>
public class ArchivosUtilizadosInfrastructureService : IArchivosUtilizadosService
{
    private readonly ApplicationDbContext _context;

    public ArchivosUtilizadosInfrastructureService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RegistrarArchivosUtilizados(int solicitudEscalafonId, string docenteCedula, string nivelOrigen, string nivelDestino)
    {
        try
        {
            Console.WriteLine($"[ARCHIVOS] Iniciando registro inteligente de archivos para solicitud {solicitudEscalafonId}");
            
            // Verificar si ya existen archivos registrados para esta solicitud
            var archivosExistentes = await _context.ArchivosUtilizadosEscalafon
                .Where(a => a.SolicitudEscalafonId == solicitudEscalafonId)
                .CountAsync();
            
            if (archivosExistentes > 0)
            {
                Console.WriteLine($"[ARCHIVOS] Ya existen {archivosExistentes} archivos registrados para la solicitud {solicitudEscalafonId}. Saltando registro.");
                return;
            }
            
            // Determinar requisitos mínimos según el nivel de destino
            var requisitos = DeterminarRequisitos(nivelOrigen, nivelDestino);
            
            var archivosUtilizados = new List<ArchivosUtilizadosEscalafon>();

            // 1. Seleccionar investigaciones necesarias (ordenadas por fecha más antigua primero)
            // IMPORTANTE: Excluir las ya utilizadas en promociones anteriores aprobadas
            if (requisitos.InvestigacionesMinimas > 0)
            {
                // Obtener IDs de investigaciones ya utilizadas
                var investigacionesYaUtilizadas = await _context.ArchivosUtilizadosEscalafon
                    .Where(a => a.DocenteCedula == docenteCedula && 
                               a.TipoRecurso == "Investigacion" && 
                               a.EstadoAscenso == "Aprobado")
                    .Select(a => a.RecursoId)
                    .ToListAsync();

                var investigaciones = await _context.Investigaciones
                    .Where(i => i.Cedula == docenteCedula && 
                               !i.IsDeleted && // Excluir documentos eliminados
                               !investigacionesYaUtilizadas.Contains(i.Id)) // Excluir ya utilizadas
                    .OrderBy(i => i.FechaPublicacion) // Más antiguas primero
                    .Take(requisitos.InvestigacionesMinimas)
                    .ToListAsync();

                // Obtener estadísticas adicionales para logging
                var investigacionesEliminadas = await _context.Investigaciones
                    .Where(i => i.Cedula == docenteCedula && i.IsDeleted)
                    .CountAsync();

                Console.WriteLine($"[ARCHIVOS] ✅ Seleccionadas {investigaciones.Count} investigaciones de {requisitos.InvestigacionesMinimas} requeridas");
                Console.WriteLine($"[ARCHIVOS] (Excluidas {investigacionesYaUtilizadas.Count} ya utilizadas + {investigacionesEliminadas} eliminadas)");
                Console.WriteLine($"[ARCHIVOS] Investigaciones que se registrarán como utilizadas:");
                foreach (var inv in investigaciones)
                {
                    Console.WriteLine($"[ARCHIVOS] - {inv.Titulo} (ID: {inv.Id})");
                }

                if (investigaciones.Count < requisitos.InvestigacionesMinimas)
                {
                    Console.WriteLine($"[ARCHIVOS] ⚠️ ADVERTENCIA: Solo se encontraron {investigaciones.Count} investigaciones disponibles de {requisitos.InvestigacionesMinimas} requeridas");
                    Console.WriteLine($"[ARCHIVOS] Esto puede indicar que no hay suficientes documentos nuevos disponibles para esta promoción");
                }

                foreach (var investigacion in investigaciones)
                {
                    archivosUtilizados.Add(new ArchivosUtilizadosEscalafon
                    {
                        SolicitudEscalafonId = solicitudEscalafonId,
                        TipoRecurso = "Investigacion",
                        RecursoId = investigacion.Id,
                        DocenteCedula = docenteCedula,
                        NivelOrigen = nivelOrigen,
                        NivelDestino = nivelDestino,
                        FechaUtilizacion = DateTime.UtcNow,
                        Descripcion = investigacion.Titulo,
                        EstadoAscenso = "Aprobado"
                    });
                }
            }

            // 2. Seleccionar evaluaciones necesarias (ordenadas por fecha más antigua primero)
            // IMPORTANTE: Excluir las ya utilizadas en promociones anteriores aprobadas
            if (requisitos.EvaluacionesMinimas > 0)
            {
                // Obtener IDs de evaluaciones ya utilizadas
                var evaluacionesYaUtilizadas = await _context.ArchivosUtilizadosEscalafon
                    .Where(a => a.DocenteCedula == docenteCedula && 
                               a.TipoRecurso == "EvaluacionDesempeno" && 
                               a.EstadoAscenso == "Aprobado")
                    .Select(a => a.RecursoId)
                    .ToListAsync();

                var evaluaciones = await _context.DAC
                    .Where(e => e.Cedula == docenteCedula && 
                               !e.IsDeleted && // Excluir documentos eliminados
                               !evaluacionesYaUtilizadas.Contains(e.Id)) // Excluir ya utilizadas
                    .OrderBy(e => e.FechaEvaluacion) // Más antiguas primero
                    .Take(requisitos.EvaluacionesMinimas)
                    .ToListAsync();

                // Obtener estadísticas adicionales para logging
                var evaluacionesEliminadas = await _context.DAC
                    .Where(e => e.Cedula == docenteCedula && e.IsDeleted)
                    .CountAsync();

                Console.WriteLine($"[ARCHIVOS] ✅ Seleccionadas {evaluaciones.Count} evaluaciones de {requisitos.EvaluacionesMinimas} requeridas");
                Console.WriteLine($"[ARCHIVOS] (Excluidas {evaluacionesYaUtilizadas.Count} ya utilizadas + {evaluacionesEliminadas} eliminadas)");
                Console.WriteLine($"[ARCHIVOS] Evaluaciones que se registrarán como utilizadas:");
                foreach (var eval in evaluaciones)
                {
                    Console.WriteLine($"[ARCHIVOS] - {eval.PeriodoAcademico} - {eval.PorcentajeObtenido:F1}% (ID: {eval.Id})");
                }

                if (evaluaciones.Count < requisitos.EvaluacionesMinimas)
                {
                    Console.WriteLine($"[ARCHIVOS] ⚠️ ADVERTENCIA: Solo se encontraron {evaluaciones.Count} evaluaciones disponibles de {requisitos.EvaluacionesMinimas} requeridas");
                    Console.WriteLine($"[ARCHIVOS] Esto puede indicar que no hay suficientes documentos nuevos disponibles para esta promoción");
                }

                foreach (var evaluacion in evaluaciones)
                {
                    archivosUtilizados.Add(new ArchivosUtilizadosEscalafon
                    {
                        SolicitudEscalafonId = solicitudEscalafonId,
                        TipoRecurso = "EvaluacionDesempeno",
                        RecursoId = evaluacion.Id,
                        DocenteCedula = docenteCedula,
                        NivelOrigen = nivelOrigen,
                        NivelDestino = nivelDestino,
                        FechaUtilizacion = DateTime.UtcNow,
                        Descripcion = $"{evaluacion.PeriodoAcademico} - {evaluacion.PorcentajeObtenido:F1}%",
                        EstadoAscenso = "Aprobado"
                    });
                }
            }

            // 3. Seleccionar capacitaciones necesarias hasta cumplir las horas mínimas (ordenadas por fecha más antigua primero)
            // IMPORTANTE: Excluir las ya utilizadas en promociones anteriores aprobadas
            if (requisitos.HorasCapacitacionMinimas > 0)
            {
                // Obtener IDs de capacitaciones ya utilizadas
                var capacitacionesYaUtilizadas = await _context.ArchivosUtilizadosEscalafon
                    .Where(a => a.DocenteCedula == docenteCedula && 
                               a.TipoRecurso == "Capacitacion" && 
                               a.EstadoAscenso == "Aprobado")
                    .Select(a => a.RecursoId)
                    .ToListAsync();

                var capacitaciones = await _context.DITIC
                    .Where(c => c.Cedula == docenteCedula && 
                               !c.IsDeleted && // Excluir documentos eliminados
                               !capacitacionesYaUtilizadas.Contains(c.Id)) // Excluir ya utilizadas
                    .OrderBy(c => c.FechaInicio) // Más antiguas primero
                    .ToListAsync();

                // Obtener estadísticas adicionales para logging
                var capacitacionesEliminadas = await _context.DITIC
                    .Where(c => c.Cedula == docenteCedula && c.IsDeleted)
                    .CountAsync();

                Console.WriteLine($"[ARCHIVOS] ✅ Evaluando capacitaciones para cumplir {requisitos.HorasCapacitacionMinimas} horas mínimas");
                Console.WriteLine($"[ARCHIVOS] (Excluidas {capacitacionesYaUtilizadas.Count} ya utilizadas + {capacitacionesEliminadas} eliminadas)");

                int horasAcumuladas = 0;
                var capacitacionesSeleccionadas = new List<string>();
                foreach (var capacitacion in capacitaciones)
                {
                    if (horasAcumuladas >= requisitos.HorasCapacitacionMinimas) break;

                    var horasCapacitacion = capacitacion.HorasAcademicas > 0 ? capacitacion.HorasAcademicas : 20; // Default 20 horas si no está especificado
                    horasAcumuladas += horasCapacitacion;
                    capacitacionesSeleccionadas.Add($"{capacitacion.NombreCapacitacion} ({horasCapacitacion}h)");

                    archivosUtilizados.Add(new ArchivosUtilizadosEscalafon
                    {
                        SolicitudEscalafonId = solicitudEscalafonId,
                        TipoRecurso = "Capacitacion",
                        RecursoId = capacitacion.Id,
                        DocenteCedula = docenteCedula,
                        NivelOrigen = nivelOrigen,
                        NivelDestino = nivelDestino,
                        FechaUtilizacion = DateTime.UtcNow,
                        Descripcion = capacitacion.NombreCapacitacion,
                        EstadoAscenso = "Aprobado"
                    });
                }

                if (horasAcumuladas < requisitos.HorasCapacitacionMinimas)
                {
                    Console.WriteLine($"[ARCHIVOS] ⚠️ ADVERTENCIA: Solo se acumularon {horasAcumuladas} horas de {requisitos.HorasCapacitacionMinimas} requeridas");
                    Console.WriteLine($"[ARCHIVOS] Esto puede indicar que no hay suficientes capacitaciones nuevas disponibles para esta promoción");
                }

                Console.WriteLine($"[ARCHIVOS] ✅ Acumuladas {horasAcumuladas} horas con {archivosUtilizados.Count(a => a.TipoRecurso == "Capacitacion")} capacitaciones");
                Console.WriteLine($"[ARCHIVOS] Capacitaciones que se registrarán como utilizadas:");
                foreach (var cap in capacitacionesSeleccionadas)
                {
                    Console.WriteLine($"[ARCHIVOS] - {cap}");
                }
            }

            // Guardar los archivos utilizados
            _context.ArchivosUtilizadosEscalafon.AddRange(archivosUtilizados);
            await _context.SaveChangesAsync();

            // Generar resumen detallado de lo que se registró
            var investigacionesRegistradas = archivosUtilizados.Count(a => a.TipoRecurso == "Investigacion");
            var evaluacionesRegistradas = archivosUtilizados.Count(a => a.TipoRecurso == "EvaluacionDesempeno");
            var capacitacionesRegistradas = archivosUtilizados.Count(a => a.TipoRecurso == "Capacitacion");

            Console.WriteLine($"[ARCHIVOS] ✅ REGISTRO COMPLETADO - Solicitud {solicitudEscalafonId}");
            Console.WriteLine($"[ARCHIVOS] 📊 RESUMEN FINAL:");
            Console.WriteLine($"[ARCHIVOS] - Investigaciones registradas: {investigacionesRegistradas}");
            Console.WriteLine($"[ARCHIVOS] - Evaluaciones registradas: {evaluacionesRegistradas}");
            Console.WriteLine($"[ARCHIVOS] - Capacitaciones registradas: {capacitacionesRegistradas}");
            Console.WriteLine($"[ARCHIVOS] - TOTAL documentos registrados: {archivosUtilizados.Count}");
            Console.WriteLine($"[ARCHIVOS] Registro completado correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ARCHIVOS] Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Determina los requisitos mínimos según el nivel de escalafón
    /// </summary>
    private RequisitosPorNivel DeterminarRequisitos(string nivelOrigen, string nivelDestino)
    {
        Console.WriteLine($"[ARCHIVOS] Determinando requisitos - Origen: '{nivelOrigen}', Destino: '{nivelDestino}'");
        
        // Definir requisitos según el nivel de destino
        // Estos valores podrían venir de una tabla de configuración en una implementación más avanzada
        var requisitos = nivelDestino.ToLower() switch
        {
            var nivel when nivel.Contains("auxiliar 2") => new RequisitosPorNivel 
            { 
                InvestigacionesMinimas = 2, 
                EvaluacionesMinimas = 3, 
                HorasCapacitacionMinimas = 80 
            },
            var nivel when nivel.Contains("auxiliar 3") => new RequisitosPorNivel 
            { 
                InvestigacionesMinimas = 3, 
                EvaluacionesMinimas = 4, 
                HorasCapacitacionMinimas = 100 
            },
            var nivel when nivel.Contains("agregado") => new RequisitosPorNivel 
            { 
                InvestigacionesMinimas = 4, 
                EvaluacionesMinimas = 4, 
                HorasCapacitacionMinimas = 120 
            },
            var nivel when nivel.Contains("principal") => new RequisitosPorNivel 
            { 
                InvestigacionesMinimas = 6, 
                EvaluacionesMinimas = 4, 
                HorasCapacitacionMinimas = 150 
            },
            _ => new RequisitosPorNivel 
            { 
                InvestigacionesMinimas = 2, 
                EvaluacionesMinimas = 3, 
                HorasCapacitacionMinimas = 80 
            }
        };

        Console.WriteLine($"[ARCHIVOS] Requisitos determinados - Investigaciones: {requisitos.InvestigacionesMinimas}, Evaluaciones: {requisitos.EvaluacionesMinimas}, Horas: {requisitos.HorasCapacitacionMinimas}");
        return requisitos;
    }

    /// <summary>
    /// Clase auxiliar para definir requisitos por nivel
    /// </summary>
    private class RequisitosPorNivel
    {
        public int InvestigacionesMinimas { get; set; }
        public int EvaluacionesMinimas { get; set; }
        public int HorasCapacitacionMinimas { get; set; }
    }

    public async Task<List<int>> ObtenerInvestigacionesUtilizadas(string docenteCedula)
    {
        var investigacionesUtilizadas = await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.DocenteCedula == docenteCedula && 
                       a.TipoRecurso == "Investigacion" && 
                       a.EstadoAscenso == "Aprobado")
            .Select(a => a.RecursoId)
            .Distinct()
            .ToListAsync();

        // Verificar que las investigaciones referenciadas no estén eliminadas
        var investigacionesValidas = await _context.Investigaciones
            .Where(i => investigacionesUtilizadas.Contains(i.Id) && !i.IsDeleted)
            .Select(i => i.Id)
            .ToListAsync();

        return investigacionesValidas;
    }

    public async Task<List<int>> ObtenerEvaluacionesUtilizadas(string docenteCedula)
    {
        var evaluacionesUtilizadas = await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.DocenteCedula == docenteCedula && 
                       a.TipoRecurso == "EvaluacionDesempeno" && 
                       a.EstadoAscenso == "Aprobado")
            .Select(a => a.RecursoId)
            .Distinct()
            .ToListAsync();

        // Verificar que las evaluaciones referenciadas no estén eliminadas
        var evaluacionesValidas = await _context.DAC
            .Where(e => evaluacionesUtilizadas.Contains(e.Id) && !e.IsDeleted)
            .Select(e => e.Id)
            .ToListAsync();

        return evaluacionesValidas;
    }

    public async Task<List<int>> ObtenerCapacitacionesUtilizadas(string docenteCedula)
    {
        var capacitacionesUtilizadas = await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.DocenteCedula == docenteCedula && 
                       a.TipoRecurso == "Capacitacion" && 
                       a.EstadoAscenso == "Aprobado")
            .Select(a => a.RecursoId)
            .Distinct()
            .ToListAsync();

        // Verificar que las capacitaciones referenciadas no estén eliminadas
        var capacitacionesValidas = await _context.DITIC
            .Where(c => capacitacionesUtilizadas.Contains(c.Id) && !c.IsDeleted)
            .Select(c => c.Id)
            .ToListAsync();

        return capacitacionesValidas;
    }

    public async Task<List<ArchivosUtilizadosDto>> ObtenerHistorialArchivos(string docenteCedula)
    {
        var archivosUtilizados = await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.DocenteCedula == docenteCedula)
            .OrderByDescending(a => a.FechaUtilizacion)
            .ToListAsync();

        var historialValido = new List<ArchivosUtilizadosDto>();

        foreach (var archivo in archivosUtilizados)
        {
            // Verificar que el documento original no esté eliminado
            var documentoValido = archivo.TipoRecurso switch
            {
                "Investigacion" => await _context.Investigaciones.AnyAsync(i => i.Id == archivo.RecursoId && !i.IsDeleted),
                "EvaluacionDesempeno" => await _context.DAC.AnyAsync(e => e.Id == archivo.RecursoId && !e.IsDeleted),
                "Capacitacion" => await _context.DITIC.AnyAsync(c => c.Id == archivo.RecursoId && !c.IsDeleted),
                _ => false
            };

            // Solo incluir en el historial si el documento original no está eliminado
            if (documentoValido)
            {
                historialValido.Add(new ArchivosUtilizadosDto
                {
                    Id = archivo.Id,
                    SolicitudEscalafonId = archivo.SolicitudEscalafonId,
                    TipoRecurso = archivo.TipoRecurso,
                    RecursoId = archivo.RecursoId,
                    DocenteCedula = archivo.DocenteCedula,
                    NivelOrigen = archivo.NivelOrigen,
                    NivelDestino = archivo.NivelDestino,
                    FechaUtilizacion = archivo.FechaUtilizacion,
                    Descripcion = archivo.Descripcion,
                    EstadoAscenso = archivo.EstadoAscenso,
                    TituloRecurso = archivo.Descripcion,
                    DetallesRecurso = $"{archivo.TipoRecurso} utilizado para ascender de {archivo.NivelOrigen} a {archivo.NivelDestino}"
                });
            }
        }

        return historialValido;
    }

    public async Task<bool> ArchivoYaUtilizado(string docenteCedula, string tipoRecurso, int recursoId)
    {
        // Verificar si el archivo fue utilizado Y si el documento original no está eliminado
        var archivoUtilizado = await _context.ArchivosUtilizadosEscalafon
            .AnyAsync(a => a.DocenteCedula == docenteCedula && 
                          a.TipoRecurso == tipoRecurso && 
                          a.RecursoId == recursoId && 
                          a.EstadoAscenso == "Aprobado");

        if (!archivoUtilizado) return false;

        // Verificar que el documento original no esté eliminado
        var documentoValido = tipoRecurso switch
        {
            "Investigacion" => await _context.Investigaciones.AnyAsync(i => i.Id == recursoId && !i.IsDeleted),
            "EvaluacionDesempeno" => await _context.DAC.AnyAsync(e => e.Id == recursoId && !e.IsDeleted),
            "Capacitacion" => await _context.DITIC.AnyAsync(c => c.Id == recursoId && !c.IsDeleted),
            _ => false
        };

        return documentoValido;
    }

    public async Task<Dictionary<string, int>> ObtenerEstadisticasArchivosUtilizados(string docenteCedula)
    {
        var archivosUtilizados = await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.DocenteCedula == docenteCedula && a.EstadoAscenso == "Aprobado")
            .ToListAsync();

        var estadisticasValidas = new Dictionary<string, int>();

        foreach (var grupo in archivosUtilizados.GroupBy(a => a.TipoRecurso))
        {
            var tipoRecurso = grupo.Key;
            var archivos = grupo.ToList();
            var conteoValidos = 0;

            foreach (var archivo in archivos)
            {
                // Verificar que el documento original no esté eliminado
                var documentoValido = archivo.TipoRecurso switch
                {
                    "Investigacion" => await _context.Investigaciones.AnyAsync(i => i.Id == archivo.RecursoId && !i.IsDeleted),
                    "EvaluacionDesempeno" => await _context.DAC.AnyAsync(e => e.Id == archivo.RecursoId && !e.IsDeleted),
                    "Capacitacion" => await _context.DITIC.AnyAsync(c => c.Id == archivo.RecursoId && !c.IsDeleted),
                    _ => false
                };

                if (documentoValido)
                {
                    conteoValidos++;
                }
            }

            if (conteoValidos > 0)
            {
                estadisticasValidas[tipoRecurso] = conteoValidos;
            }
        }

        return estadisticasValidas;
    }

    public async Task<List<ArchivosUtilizadosDto>> ObtenerArchivosPorSolicitud(int solicitudEscalafonId)
    {
        var archivos = await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.SolicitudEscalafonId == solicitudEscalafonId)
            .OrderBy(a => a.TipoRecurso)
            .ToListAsync();

        var archivosValidos = new List<ArchivosUtilizadosDto>();

        foreach (var archivo in archivos)
        {
            // Verificar que el documento original no esté eliminado
            var documentoValido = archivo.TipoRecurso switch
            {
                "Investigacion" => await _context.Investigaciones.AnyAsync(i => i.Id == archivo.RecursoId && !i.IsDeleted),
                "EvaluacionDesempeno" => await _context.DAC.AnyAsync(e => e.Id == archivo.RecursoId && !e.IsDeleted),
                "Capacitacion" => await _context.DITIC.AnyAsync(c => c.Id == archivo.RecursoId && !c.IsDeleted),
                _ => false
            };

            // Solo incluir si el documento original no está eliminado
            if (documentoValido)
            {
                archivosValidos.Add(new ArchivosUtilizadosDto
                {
                    Id = archivo.Id,
                    SolicitudEscalafonId = archivo.SolicitudEscalafonId,
                    TipoRecurso = archivo.TipoRecurso,
                    RecursoId = archivo.RecursoId,
                    DocenteCedula = archivo.DocenteCedula,
                    NivelOrigen = archivo.NivelOrigen,
                    NivelDestino = archivo.NivelDestino,
                    FechaUtilizacion = archivo.FechaUtilizacion,
                    Descripcion = archivo.Descripcion,
                    EstadoAscenso = archivo.EstadoAscenso,
                    TituloRecurso = archivo.Descripcion ?? ObtenerTituloGenerico(archivo.TipoRecurso),
                    DetallesRecurso = archivo.Descripcion ?? "Sin detalles específicos"
                });
            }
        }

        return archivosValidos;
    }

    private static string ObtenerTituloGenerico(string tipoRecurso)
    {
        return tipoRecurso switch
        {
            "Investigacion" => "Publicación científica",
            "EvaluacionDesempeno" => "Evaluación de desempeño",
            "Capacitacion" => "Capacitación profesional",
            _ => "Documento académico"
        };
    }
}
