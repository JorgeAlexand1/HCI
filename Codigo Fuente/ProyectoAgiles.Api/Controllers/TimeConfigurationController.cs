using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProyectoAgiles.Infrastructure.Data;
using ProyectoAgiles.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProyectoAgiles.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeConfigurationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TimeConfigurationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("current")]
        public async Task<ActionResult<TimeConfigurationDto>> GetCurrentConfiguration()
        {
            try
            {
                Console.WriteLine("TimeConfigurationController: GetCurrentConfiguration called");
                var currentConfig = await _context.TimeConfigurations
                    .Where(tc => tc.IsActive)
                    .FirstOrDefaultAsync();

                Console.WriteLine($"TimeConfigurationController: Found {(currentConfig != null ? "1" : "0")} active configurations");

                if (currentConfig == null)
                {
                    Console.WriteLine("TimeConfigurationController: No active configuration found, returning 404");
                    return NotFound(new { message = "No hay configuración activa" });
                }

                var dto = new TimeConfigurationDto
                {
                    Id = currentConfig.Id,
                    StartDate = currentConfig.StartDate,
                    EndDate = currentConfig.EndDate,
                    Description = currentConfig.Description,
                    IsActive = currentConfig.IsActive,
                    CreatedDate = currentConfig.CreatedDate,
                    CreatedBy = currentConfig.CreatedBy
                };

                Console.WriteLine("TimeConfigurationController: Returning active configuration");
                return Ok(dto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TimeConfigurationController: Error - {ex.Message}");
                return StatusCode(500, new { message = $"Error al obtener la configuración actual: {ex.Message}" });
            }
        }

        [HttpGet("history")]
        public async Task<ActionResult<List<TimeConfigurationDto>>> GetConfigurationHistory()
        {
            try
            {
                var configurations = await _context.TimeConfigurations
                    .OrderByDescending(tc => tc.CreatedDate)
                    .ToListAsync();

                var dtos = configurations.Select(tc => new TimeConfigurationDto
                {
                    Id = tc.Id,
                    StartDate = tc.StartDate,
                    EndDate = tc.EndDate,
                    Description = tc.Description,
                    IsActive = tc.IsActive,
                    CreatedDate = tc.CreatedDate,
                    CreatedBy = tc.CreatedBy
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al obtener el historial: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<TimeConfigurationDto>> CreateConfiguration([FromBody] CreateTimeConfigurationDto createDto)
        {
            try
            {
                if (createDto.StartDate >= createDto.EndDate)
                {
                    return BadRequest(new { message = "La fecha de inicio debe ser anterior a la fecha de fin" });
                }

                if (createDto.StartDate < DateTime.Now.Date)
                {
                    return BadRequest(new { message = "La fecha de inicio no puede ser anterior a la fecha actual" });
                }

                // Si se va a activar, desactivar cualquier configuración activa existente
                if (createDto.IsActive)
                {
                    var existingActive = await _context.TimeConfigurations
                        .Where(tc => tc.IsActive)
                        .ToListAsync();

                    foreach (var config in existingActive)
                    {
                        config.IsActive = false;
                    }
                }

                var newConfig = new TimeConfiguration
                {
                    StartDate = createDto.StartDate,
                    EndDate = createDto.EndDate,
                    Description = createDto.Description ?? "",
                    IsActive = createDto.IsActive,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity?.Name ?? "Admin"
                };

                _context.TimeConfigurations.Add(newConfig);
                await _context.SaveChangesAsync();

                var dto = new TimeConfigurationDto
                {
                    Id = newConfig.Id,
                    StartDate = newConfig.StartDate,
                    EndDate = newConfig.EndDate,
                    Description = newConfig.Description,
                    IsActive = newConfig.IsActive,
                    CreatedDate = newConfig.CreatedDate,
                    CreatedBy = newConfig.CreatedBy
                };

                return CreatedAtAction(nameof(GetCurrentConfiguration), new { id = newConfig.Id }, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al crear la configuración: {ex.Message}" });
            }
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateConfiguration(int id)
        {
            try
            {
                var config = await _context.TimeConfigurations.FindAsync(id);
                if (config == null)
                {
                    return NotFound(new { message = "Configuración no encontrada" });
                }

                // Desactivar todas las configuraciones activas
                var activeConfigs = await _context.TimeConfigurations
                    .Where(tc => tc.IsActive)
                    .ToListAsync();

                foreach (var activeConfig in activeConfigs)
                {
                    activeConfig.IsActive = false;
                }

                // Activar la configuración seleccionada
                config.IsActive = true;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Configuración activada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al activar la configuración: {ex.Message}" });
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateConfiguration(int id)
        {
            try
            {
                var config = await _context.TimeConfigurations.FindAsync(id);
                if (config == null)
                {
                    return NotFound(new { message = "Configuración no encontrada" });
                }

                config.IsActive = false;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Configuración desactivada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al desactivar la configuración: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfiguration(int id)
        {
            try
            {
                var config = await _context.TimeConfigurations.FindAsync(id);
                if (config == null)
                {
                    return NotFound(new { message = "Configuración no encontrada" });
                }

                _context.TimeConfigurations.Remove(config);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Configuración eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al eliminar la configuración: {ex.Message}" });
            }
        }

        [HttpGet("is-active")]
        public async Task<ActionResult<bool>> IsTimeConfigurationActive()
        {
            try
            {
                var activeConfig = await _context.TimeConfigurations
                    .Where(tc => tc.IsActive && 
                                tc.StartDate <= DateTime.Now && 
                                tc.EndDate >= DateTime.Now)
                    .FirstOrDefaultAsync();

                return Ok(activeConfig != null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al verificar la configuración: {ex.Message}" });
            }
        }
    }

    // DTOs
    public class TimeConfigurationDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; } = "";
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = "";
    }

    public class CreateTimeConfigurationDto
    {
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
