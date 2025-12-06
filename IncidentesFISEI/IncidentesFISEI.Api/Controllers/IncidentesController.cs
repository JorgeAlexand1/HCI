using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IncidentesFISEI.Infrastructure.Data;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Application.DTOs;

namespace IncidentesFISEI.Api.Controllers;

/// <summary>
/// Controlador para la gestión de incidentes según ITIL v3
/// </summary>
[ApiController]
[Route("api/[controller]")]
//[Authorize] // Temporalmente deshabilitado
public class IncidentesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<IncidentesController> _logger;

    public IncidentesController(ApplicationDbContext context, ILogger<IncidentesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todos los incidentes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<IncidenteListDto>), 200)]
    public async Task<IActionResult> GetIncidentes([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            var incidentes = await _context.Incidentes
                .Include(i => i.ReportadoPor)
                .Include(i => i.AsignadoA)
                .Include(i => i.Categoria)
                .Include(i => i.Servicio)
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new IncidenteListDto
                {
                    Id = i.Id,
                    NumeroIncidente = i.NumeroIncidente,
                    Titulo = i.Titulo,
                    Descripcion = i.Descripcion,
                    Prioridad = i.Prioridad,
                    Estado = i.Estado,
                    ReportadoPor = i.ReportadoPor != null ? 
                        $"{i.ReportadoPor.FirstName} {i.ReportadoPor.LastName}" : "Usuario desconocido",
                    ReportadoPorId = i.ReportadoPorId,
                    AsignadoA = i.AsignadoA != null ? 
                        $"{i.AsignadoA.FirstName} {i.AsignadoA.LastName}" : null,
                    AsignadoAId = i.AsignadoAId,
                    CategoriaNombre = i.Categoria != null ? i.Categoria.Nombre : "Sin categoría",
                    CategoriaId = i.CategoriaId,
                    ServicioNombre = i.Servicio != null ? i.Servicio.Nombre : null,
                    ServicioId = i.ServicioId,
                    FechaReporte = i.CreatedAt,
                    FechaResolucion = i.FechaResolucion
                })
                .ToListAsync();

            return Ok(incidentes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener lista de incidentes");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtener un incidente por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IncidenteListDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetIncidente(int id)
    {
        try
        {
            var incidente = await _context.Incidentes
                .Include(i => i.ReportadoPor)
                .Include(i => i.AsignadoA)
                .Include(i => i.Categoria)
                .Include(i => i.Servicio)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (incidente == null)
            {
                return NotFound($"Incidente con ID {id} no encontrado");
            }

            var incidenteDto = new IncidenteListDto
            {
                Id = incidente.Id,
                NumeroIncidente = incidente.NumeroIncidente,
                Titulo = incidente.Titulo,
                Descripcion = incidente.Descripcion,
                Prioridad = incidente.Prioridad,
                Estado = incidente.Estado,
                ReportadoPor = incidente.ReportadoPor != null ? 
                    $"{incidente.ReportadoPor.FirstName} {incidente.ReportadoPor.LastName}" : "Usuario desconocido",
                ReportadoPorId = incidente.ReportadoPorId,
                AsignadoA = incidente.AsignadoA != null ? 
                    $"{incidente.AsignadoA.FirstName} {incidente.AsignadoA.LastName}" : null,
                AsignadoAId = incidente.AsignadoAId,
                CategoriaNombre = incidente.Categoria != null ? incidente.Categoria.Nombre : "Sin categoría",
                CategoriaId = incidente.CategoriaId,
                ServicioNombre = incidente.Servicio != null ? incidente.Servicio.Nombre : null,
                ServicioId = incidente.ServicioId,
                FechaReporte = incidente.CreatedAt,
                FechaResolucion = incidente.FechaResolucion
            };

            return Ok(incidenteDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener incidente {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crear un nuevo incidente
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(IncidenteListDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateIncidente([FromBody] CreateIncidenteDto createDto)
    {
        try
        {
            // Log de los datos recibidos para debugging
            _logger.LogInformation("Datos recibidos para crear incidente: ReportadoPor={ReportadoPor}, CategoriaId={CategoriaId}, ServicioId={ServicioId}, Titulo={Titulo}", 
                createDto.ReportadoPor, createDto.CategoriaId, createDto.ServicioId, createDto.Titulo);

            // Verificar que el usuario existe antes de crear el incidente
            var reportadoPorId = createDto.ReportadoPor ?? 2; // Usar ID 2 como fallback (Carlos Mendoza)
            
            // Verificar si el usuario existe y está activo
            var userExists = await _context.Usuarios.AnyAsync(u => u.Id == reportadoPorId && u.IsActive);
            if (!userExists)
            {
                _logger.LogWarning("Usuario con ID {UserId} no existe o no está activo. Buscando usuario por defecto.", reportadoPorId);
                
                // Buscar el primer usuario activo disponible
                var defaultUser = await _context.Usuarios
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.Id)
                    .FirstOrDefaultAsync();
                
                if (defaultUser != null)
                {
                    reportadoPorId = defaultUser.Id;
                    _logger.LogInformation("Usando usuario por defecto: {UserId} - {UserName}", defaultUser.Id, $"{defaultUser.FirstName} {defaultUser.LastName}");
                }
                else
                {
                    _logger.LogError("No hay usuarios activos en el sistema");
                    return BadRequest("No hay usuarios activos disponibles en el sistema");
                }
            }
            else
            {
                _logger.LogInformation("Usuario válido encontrado: {UserId}", reportadoPorId);
            }

            // Generar número de incidente único
            var numeroIncidente = await GenerateIncidentNumber();

            // Asignación automática basada en la categoría
            var categoriaId = createDto.CategoriaId ?? 1;
            var asignadoAId = await GetAutoAssigneeByCategory(categoriaId);

            var incidente = new Incidente
            {
                NumeroIncidente = numeroIncidente,
                Titulo = createDto.Titulo ?? string.Empty,
                Descripcion = createDto.Descripcion ?? string.Empty,
                Prioridad = createDto.Prioridad,
                Estado = EstadoIncidente.Abierto,
                ReportadoPorId = reportadoPorId, // Usuario verificado
                CategoriaId = categoriaId, // Categoría por defecto si no se especifica
                ServicioId = createDto.ServicioId, // Agregar el servicio si se especifica
                AsignadoAId = asignadoAId, // Asignación automática
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Incidentes.Add(incidente);
            await _context.SaveChangesAsync();

            // Cargar el incidente con las relaciones para el retorno
            var incidenteCreado = await _context.Incidentes
                .Include(i => i.ReportadoPor)
                .Include(i => i.AsignadoA)
                .Include(i => i.Categoria)
                .Include(i => i.Servicio)
                .FirstAsync(i => i.Id == incidente.Id);

            var resultado = new IncidenteListDto
            {
                Id = incidenteCreado.Id,
                NumeroIncidente = incidenteCreado.NumeroIncidente,
                Titulo = incidenteCreado.Titulo,
                Descripcion = incidenteCreado.Descripcion,
                Prioridad = incidenteCreado.Prioridad,
                Estado = incidenteCreado.Estado,
                ReportadoPor = incidenteCreado.ReportadoPor != null ? 
                    $"{incidenteCreado.ReportadoPor.FirstName} {incidenteCreado.ReportadoPor.LastName}" : "Usuario desconocido",
                ReportadoPorId = incidenteCreado.ReportadoPorId,
                AsignadoA = null,
                AsignadoAId = null,
                CategoriaNombre = incidenteCreado.Categoria?.Nombre ?? "Sin categoría",
                CategoriaId = incidenteCreado.CategoriaId,
                ServicioNombre = incidenteCreado.Servicio?.Nombre,
                ServicioId = incidenteCreado.ServicioId,
                FechaReporte = incidenteCreado.CreatedAt,
                FechaResolucion = null
            };

            _logger.LogInformation("Incidente creado: {NumeroIncidente}", numeroIncidente);
            return CreatedAtAction(nameof(GetIncidente), new { id = incidente.Id }, resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear incidente");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualizar un incidente existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(IncidenteListDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateIncident(int id, [FromBody] UpdateIncidenteDto updateData)
    {
        try
        {
            var incidente = await _context.Incidentes
                .Include(i => i.ReportadoPor)
                .Include(i => i.AsignadoA)
                .Include(i => i.Categoria)
                .Include(i => i.Servicio)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (incidente == null)
            {
                return NotFound($"Incidente con ID {id} no encontrado");
            }

            // Actualizar propiedades
            if (!string.IsNullOrEmpty(updateData.Titulo))
                incidente.Titulo = updateData.Titulo;
            
            if (!string.IsNullOrEmpty(updateData.Descripcion))
                incidente.Descripcion = updateData.Descripcion;
            
            incidente.Estado = updateData.Estado;
            incidente.Prioridad = updateData.Prioridad;
            incidente.CategoriaId = updateData.CategoriaId;
            incidente.ServicioId = updateData.ServicioId;
            incidente.AsignadoAId = updateData.AsignadoAId;
            incidente.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Recargar el incidente con las relaciones actualizadas
            var incidenteActualizado = await _context.Incidentes
                .Include(i => i.ReportadoPor)
                .Include(i => i.AsignadoA)
                .Include(i => i.Categoria)
                .Include(i => i.Servicio)
                .FirstAsync(i => i.Id == id);

            var resultado = new IncidenteListDto
            {
                Id = incidenteActualizado.Id,
                NumeroIncidente = incidenteActualizado.NumeroIncidente,
                Titulo = incidenteActualizado.Titulo,
                Descripcion = incidenteActualizado.Descripcion,
                Prioridad = incidenteActualizado.Prioridad,
                Estado = incidenteActualizado.Estado,
                ReportadoPor = incidenteActualizado.ReportadoPor != null ? 
                    $"{incidenteActualizado.ReportadoPor.FirstName} {incidenteActualizado.ReportadoPor.LastName}" : "Usuario desconocido",
                ReportadoPorId = incidenteActualizado.ReportadoPorId,
                AsignadoA = incidenteActualizado.AsignadoA != null ? 
                    $"{incidenteActualizado.AsignadoA.FirstName} {incidenteActualizado.AsignadoA.LastName}" : null,
                AsignadoAId = incidenteActualizado.AsignadoAId,
                CategoriaNombre = incidenteActualizado.Categoria != null ? incidenteActualizado.Categoria.Nombre : "Sin categoría",
                CategoriaId = incidenteActualizado.CategoriaId,
                ServicioNombre = incidenteActualizado.Servicio != null ? incidenteActualizado.Servicio.Nombre : null,
                ServicioId = incidenteActualizado.ServicioId,
                FechaReporte = incidenteActualizado.CreatedAt,
                FechaResolucion = incidenteActualizado.FechaResolucion
            };

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar incidente {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    private async Task<string> GenerateIncidentNumber()
    {
        var año = DateTime.Now.Year;
        var ultimoNumero = await _context.Incidentes
            .Where(i => i.NumeroIncidente.StartsWith($"INC-{año}-"))
            .OrderByDescending(i => i.NumeroIncidente)
            .Select(i => i.NumeroIncidente)
            .FirstOrDefaultAsync();

        int siguienteNumero = 1;
        if (!string.IsNullOrEmpty(ultimoNumero))
        {
            var partes = ultimoNumero.Split('-');
            if (partes.Length == 3 && int.TryParse(partes[2], out int numero))
            {
                siguienteNumero = numero + 1;
            }
        }

        return $"INC-{año}-{siguienteNumero:D6}";
    }

    /// <summary>
    /// Cerrar un incidente
    /// </summary>
    [HttpPut("{id}/close")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CloseIncident(int id)
    {
        try
        {
            var incidente = await _context.Incidentes.FindAsync(id);
            if (incidente == null)
            {
                return NotFound($"Incidente con ID {id} no encontrado");
            }

            incidente.Estado = EstadoIncidente.Cerrado;
            incidente.FechaCierre = DateTime.UtcNow;
            incidente.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Incidente cerrado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cerrar incidente {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Reabrir un incidente cerrado
    /// </summary>
    [HttpPut("{id}/reopen")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ReopenIncident(int id)
    {
        try
        {
            var incidente = await _context.Incidentes.FindAsync(id);
            if (incidente == null)
            {
                return NotFound($"Incidente con ID {id} no encontrado");
            }

            if (incidente.Estado != EstadoIncidente.Cerrado)
            {
                return BadRequest("Solo se pueden reabrir incidentes cerrados");
            }

            incidente.Estado = EstadoIncidente.Abierto;
            incidente.FechaCierre = null;
            incidente.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Incidente reabierto exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reabrir incidente {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Eliminar un incidente
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteIncidente(int id)
    {
        try
        {
            var incidente = await _context.Incidentes.FindAsync(id);
            if (incidente == null)
            {
                return NotFound($"Incidente con ID {id} no encontrado");
            }

            // Opcional: verificar que solo se puedan eliminar incidentes en ciertos estados
            if (incidente.Estado == EstadoIncidente.EnProgreso)
            {
                return BadRequest("No se puede eliminar un incidente en progreso. Por favor, ciérrelo primero.");
            }

            _context.Incidentes.Remove(incidente);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Incidente {Id} eliminado exitosamente", id);
            return Ok(new { message = "Incidente eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar incidente {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Asignar un incidente a un técnico (solo admin y supervisores)
    /// </summary>
    [HttpPut("{id}/asignar")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AsignarIncidente(int id, [FromBody] AsignarIncidenteRequest request)
    {
        try
        {
            _logger.LogInformation("=== ASIGNAR INCIDENTE ===");
            _logger.LogInformation("ID del incidente: {Id}", id);
            _logger.LogInformation("AsignadoAId: {AsignadoAId}", request.AsignadoAId);
            
            var usuarioActual = await GetUsuarioActual();
            _logger.LogInformation("Usuario actual: {Email}, Tipo: {Tipo}", usuarioActual?.Email, usuarioActual?.TipoUsuario);
            
            // Solo admin y supervisores pueden asignar incidentes
            if (usuarioActual?.TipoUsuario != TipoUsuario.Administrador && usuarioActual?.TipoUsuario != TipoUsuario.Supervisor)
                return BadRequest(new { mensaje = "Solo administradores y supervisores pueden asignar incidentes" });
            
            var incidente = await _context.Incidentes.FindAsync(id);
            if (incidente == null)
                return NotFound(new { mensaje = "Incidente no encontrado" });
                
            // Verificar que el usuario a asignar existe y es técnico
            var tecnicoAsignado = await _context.Usuarios.FindAsync(request.AsignadoAId);
            if (tecnicoAsignado == null)
                return BadRequest(new { mensaje = "Usuario no encontrado" });
                
            if (tecnicoAsignado.TipoUsuario != TipoUsuario.Tecnico && tecnicoAsignado.TipoUsuario != TipoUsuario.Administrador)
                return BadRequest(new { mensaje = "Solo se puede asignar a técnicos o administradores" });
            
            // Asignar el incidente
            incidente.AsignadoAId = request.AsignadoAId;
            incidente.FechaAsignacion = DateTime.UtcNow;
            incidente.UpdatedAt = DateTime.UtcNow;
            
            // Si estaba sin asignar, cambiar a En Progreso
            if (incidente.Estado == EstadoIncidente.Abierto)
                incidente.Estado = EstadoIncidente.EnProgreso;
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Incidente {Id} asignado a {TecnicoId} por {AdminId}", id, request.AsignadoAId, usuarioActual.Id);
            
            return Ok(new { mensaje = "Incidente asignado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar incidente {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene automáticamente el usuario asignado basándose en la categoría del incidente
    /// </summary>
    private async Task<int?> GetAutoAssigneeByCategory(int categoriaId)
    {
        try
        {
            TipoUsuario tipoUsuarioRequerido;
            
            // Determinar el tipo de usuario requerido según la categoría
            switch (categoriaId)
            {
                case 1: // Hardware
                case 2: // Software 
                case 3: // Red
                    tipoUsuarioRequerido = TipoUsuario.Tecnico;
                    break;
                case 4: // Acceso
                case 5: // Correo
                    tipoUsuarioRequerido = TipoUsuario.Supervisor;
                    break;
                default:
                    tipoUsuarioRequerido = TipoUsuario.Tecnico; // Por defecto a técnico
                    break;
            }

            // Buscar usuarios disponibles del tipo requerido
            var usuariosDisponibles = await _context.Usuarios
                .Where(u => u.TipoUsuario == tipoUsuarioRequerido && u.IsActive)
                .ToListAsync();

            if (!usuariosDisponibles.Any())
            {
                _logger.LogWarning("No hay usuarios del tipo {TipoUsuario} disponibles para asignación automática", tipoUsuarioRequerido);
                return null;
            }

            // Contar incidentes activos por usuario para balancear la carga
            var usuarioConMenosIncidentes = await _context.Usuarios
                .Where(u => u.TipoUsuario == tipoUsuarioRequerido && u.IsActive)
                .Select(u => new { 
                    Usuario = u, 
                    IncidentesActivos = _context.Incidentes.Count(i => 
                        i.AsignadoAId == u.Id && 
                        i.Estado != EstadoIncidente.Cerrado && 
                        i.Estado != EstadoIncidente.Cancelado)
                })
                .OrderBy(x => x.IncidentesActivos)
                .ThenBy(x => x.Usuario.Id) // Desempate por ID para consistencia
                .FirstOrDefaultAsync();

            if (usuarioConMenosIncidentes != null)
            {
                _logger.LogInformation("Incidente asignado automáticamente a {Usuario} ({TipoUsuario}) - Incidentes activos: {Count}", 
                    $"{usuarioConMenosIncidentes.Usuario.FirstName} {usuarioConMenosIncidentes.Usuario.LastName}",
                    tipoUsuarioRequerido, 
                    usuarioConMenosIncidentes.IncidentesActivos);
                return usuarioConMenosIncidentes.Usuario.Id;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en asignación automática para categoría {CategoriaId}", categoriaId);
            return null;
        }
    }
    /// <summary>
    /// Marcar incidente como resuelto (solo técnicos asignados)
    /// </summary>
    [HttpPatch("{id}/resolver")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ResolverIncidente(int id, [FromBody] ResolverIncidenteRequest request)
    {
        try
        {
            _logger.LogInformation("=== RESOLVER INCIDENTE ===");
            _logger.LogInformation("ID del incidente: {Id}", id);
            _logger.LogInformation("Solución recibida: {Solucion}", request.Solucion);
            
            // Obtener usuario actual (simulado por ahora)
            var usuarioActual = await GetUsuarioActual();
            _logger.LogInformation("Usuario actual obtenido: {Email}, Tipo: {Tipo}", usuarioActual?.Email, usuarioActual?.TipoUsuario);
            
            var incidente = await _context.Incidentes
                .Include(i => i.AsignadoA)
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
                
            if (incidente == null)
                return NotFound(new { mensaje = "Incidente no encontrado" });

            // Validar permisos - solo técnicos asignados pueden resolver
            var puedeResolver = PuedeResolverIncidente(usuarioActual, incidente);
            _logger.LogInformation("¿Puede resolver el incidente? {PuedeResolver}", puedeResolver);
            
            if (!puedeResolver)
            {
                _logger.LogWarning("Acceso denegado para resolver incidente {Id} - Usuario {Email} tipo {Tipo}", 
                    id, usuarioActual?.Email, usuarioActual?.TipoUsuario);
                return BadRequest(new { mensaje = "Solo el técnico asignado puede resolver este incidente" });
            }

            // Validar estado
            if (incidente.Estado == EstadoIncidente.Cerrado)
                return BadRequest(new { mensaje = "No se puede resolver un incidente ya cerrado" });

            // Resolver incidente
            incidente.Estado = EstadoIncidente.Resuelto;
            incidente.Solucion = request.Solucion;
            incidente.FechaResolucion = DateTime.UtcNow;
            incidente.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Incidente {Id} resuelto por técnico {TecnicoId}", id, usuarioActual?.Id);
            
            return Ok(new { mensaje = "Incidente resuelto exitosamente", estado = "Resuelto" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al resolver incidente {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Cerrar incidente definitivamente (supervisores y administradores)
    /// </summary>
    [HttpPatch("{id}/cerrar")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CerrarIncidente(int id, [FromBody] CerrarIncidenteRequest request)
    {
        try
        {
            // Obtener usuario actual (simulado por ahora)
            var usuarioActual = await GetUsuarioActual();
            
            var incidente = await _context.Incidentes
                .Include(i => i.AsignadoA)
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
                
            if (incidente == null)
                return NotFound(new { mensaje = "Incidente no encontrado" });

            // Validar permisos - solo supervisores y administradores pueden cerrar
            if (!PuedeCerrarIncidente(usuarioActual, incidente))
                return BadRequest(new { mensaje = "Solo supervisores y administradores pueden cerrar incidentes" });

            // Validar estado - debe estar resuelto primero
            if (incidente.Estado != EstadoIncidente.Resuelto)
                return BadRequest(new { mensaje = "El incidente debe estar resuelto antes de ser cerrado" });

            if (incidente.Estado == EstadoIncidente.Cerrado)
                return BadRequest(new { mensaje = "El incidente ya está cerrado" });

            // Cerrar incidente
            incidente.Estado = EstadoIncidente.Cerrado;
            incidente.ComentarioCierre = request.ComentarioCierre;
            incidente.FechaCierre = DateTime.UtcNow;
            incidente.CerradoPorId = usuarioActual?.Id;
            incidente.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Incidente {Id} cerrado por {UsuarioId}", id, usuarioActual?.Id);
            
            return Ok(new { mensaje = "Incidente cerrado exitosamente", estado = "Cerrado" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cerrar incidente {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Reabrir incidente cerrado (solo administradores)
    /// </summary>
    [HttpPatch("{id}/reabrir")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ReabrirIncidente(int id, [FromBody] ReabrirIncidenteRequest request)
    {
        try
        {
            var usuarioActual = await GetUsuarioActual();
            
            var incidente = await _context.Incidentes.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
                
            if (incidente == null)
                return NotFound(new { mensaje = "Incidente no encontrado" });

            // Solo administradores pueden reabrir
            if (usuarioActual?.TipoUsuario != TipoUsuario.Administrador)
                return BadRequest(new { mensaje = "Solo administradores pueden reabrir incidentes" });

            if (incidente.Estado != EstadoIncidente.Cerrado)
                return BadRequest(new { mensaje = "Solo se pueden reabrir incidentes cerrados" });

            // Reabrir incidente
            incidente.Estado = EstadoIncidente.EnProgreso;
            incidente.MotivoReapertura = request.Motivo;
            incidente.FechaReapertura = DateTime.UtcNow;
            incidente.ReabiertoPorId = usuarioActual.Id;
            incidente.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Incidente {Id} reabierto por administrador {AdminId}", id, usuarioActual.Id);
            
            return Ok(new { mensaje = "Incidente reabierto exitosamente", estado = "En Proceso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reabrir incidente {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    // Métodos auxiliares para validación de permisos
    private bool PuedeResolverIncidente(Usuario? usuario, Incidente incidente)
    {
        if (usuario == null) return false;
        
        // No se pueden resolver incidentes ya cerrados
        if (incidente.Estado == EstadoIncidente.Cerrado) return false;
        
        return usuario.TipoUsuario switch
        {
            TipoUsuario.Administrador => true, // Admin puede resolver cualquier incidente
            TipoUsuario.Tecnico => incidente.AsignadoAId == usuario.Id, // Técnico solo los asignados
            TipoUsuario.Supervisor => true, // Supervisor puede resolver incidentes de su área
            _ => false
        };
    }

    private bool PuedeCerrarIncidente(Usuario? usuario, Incidente incidente)
    {
        if (usuario == null) return false;
        
        return usuario.TipoUsuario switch
        {
            TipoUsuario.Administrador => true, // Admin puede cerrar cualquier incidente
            TipoUsuario.Supervisor => true, // Supervisor puede cerrar incidentes
            _ => false // Solo admin y supervisores pueden cerrar
        };
    }

    private async Task<Usuario?> GetUsuarioActual()
    {
        // Por ahora simulamos un administrador
        // En producción, esto vendría del token JWT
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == "admin@uta.edu.ec");
            
        if (usuario == null)
        {
            _logger.LogWarning("Usuario admin@uta.edu.ec no encontrado. Buscando cualquier administrador...");
            
            // Buscar cualquier administrador disponible
            usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.TipoUsuario == TipoUsuario.Administrador && u.IsActive);
                
            if (usuario != null)
            {
                _logger.LogInformation("Usando administrador alternativo: {Email}", usuario.Email);
            }
            else
            {
                _logger.LogError("No se encontró ningún administrador en la base de datos");
            }
        }
        
        return usuario;
    }
}

/// <summary>
/// DTO para resolver incidentes
/// </summary>
public class ResolverIncidenteRequest
{
    public string Solucion { get; set; } = string.Empty;
}

/// <summary>
/// DTO para cerrar incidentes
/// </summary>
public class CerrarIncidenteRequest
{
    public string? ComentarioCierre { get; set; }
}

/// <summary>
/// DTO para reabrir incidentes
/// </summary>
public class ReabrirIncidenteRequest
{
    public string Motivo { get; set; } = string.Empty;
}

/// <summary>
/// DTO para crear incidentes
/// </summary>
public class CreateIncidenteDto
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public PrioridadIncidente Prioridad { get; set; }
    public int? CategoriaId { get; set; }
    public int? ServicioId { get; set; }
    public int? ReportadoPor { get; set; }
}

/// <summary>
/// DTO para actualizar incidentes
/// </summary>
public class UpdateIncidenteDto
{
    public string? Titulo { get; set; }
    public string? Descripcion { get; set; }
    public EstadoIncidente Estado { get; set; }
    public PrioridadIncidente Prioridad { get; set; }
    public int CategoriaId { get; set; }
    public int? ServicioId { get; set; }
    public int? AsignadoAId { get; set; }
}

/// <summary>
/// DTO para asignar incidentes
/// </summary>
public class AsignarIncidenteRequest
{
    public int AsignadoAId { get; set; }
}