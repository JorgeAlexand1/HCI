using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IncidentesFISEI.Infrastructure.Data;

namespace IncidentesFISEI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UtilityController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("fix-servicios-utf8")]
        public async Task<IActionResult> FixServiciosUtf8()
        {
            try
            {
                // Servicios con caracteres UTF-8 correctos
                var serviciosCorrect = new[]
                {
                    new { Codigo = "SRV-HW-001", Nombre = "Soporte de Computadoras", Descripcion = "Mantenimiento y reparación de equipos de cómputo", ResponsableArea = "Soporte Técnico", CategoriaId = 1 },
                    new { Codigo = "SRV-HW-002", Nombre = "Soporte de Impresoras", Descripcion = "Configuración, mantenimiento y reparación de impresoras", ResponsableArea = "Soporte Técnico", CategoriaId = 1 },
                    new { Codigo = "SRV-HW-003", Nombre = "Soporte de Proyectores", Descripcion = "Instalación y mantenimiento de equipos de proyección", ResponsableArea = "Audiovisuales", CategoriaId = 1 },
                    
                    new { Codigo = "SRV-SW-001", Nombre = "Instalación de Software Académico", Descripcion = "Instalación de programas especializados para educación", ResponsableArea = "Sistemas", CategoriaId = 2 },
                    new { Codigo = "SRV-SW-002", Nombre = "Soporte de Sistema Operativo", Descripcion = "Mantenimiento y configuración del SO", ResponsableArea = "Sistemas", CategoriaId = 2 },
                    new { Codigo = "SRV-SW-003", Nombre = "Soporte de Antivirus", Descripcion = "Gestión y actualización de software antivirus", ResponsableArea = "Seguridad TI", CategoriaId = 2 },
                    
                    new { Codigo = "SRV-NET-001", Nombre = "Conectividad WiFi", Descripcion = "Problemas de conexión a la red inalámbrica institucional", ResponsableArea = "Redes y Comunicaciones", CategoriaId = 3 },
                    new { Codigo = "SRV-NET-002", Nombre = "Acceso a Internet", Descripcion = "Problemas de navegación web y acceso a recursos online", ResponsableArea = "Redes y Comunicaciones", CategoriaId = 3 },
                    new { Codigo = "SRV-NET-003", Nombre = "Red Cableada", Descripcion = "Problemas con conexiones Ethernet en aulas y oficinas", ResponsableArea = "Infraestructura TI", CategoriaId = 3 },
                    
                    new { Codigo = "SRV-SEC-001", Nombre = "Gestión de Cuentas de Usuario", Descripcion = "Creación, modificación y desactivación de cuentas", ResponsableArea = "Administración de Sistemas", CategoriaId = 4 },
                    new { Codigo = "SRV-SEC-002", Nombre = "Recuperación de Contraseñas", Descripcion = "Reset y recuperación de contraseñas institucionales", ResponsableArea = "Soporte al Usuario", CategoriaId = 4 },
                    new { Codigo = "SRV-SEC-003", Nombre = "Permisos y Accesos", Descripcion = "Gestión de permisos a sistemas y recursos", ResponsableArea = "Administración de Sistemas", CategoriaId = 4 },
                    
                    new { Codigo = "SRV-WEB-001", Nombre = "Portal Académico", Descripcion = "Soporte para el sistema de gestión académica", ResponsableArea = "Desarrollo Web", CategoriaId = 5 },
                    new { Codigo = "SRV-WEB-002", Nombre = "Sistema de Biblioteca", Descripcion = "Soporte para el catálogo y servicios digitales de biblioteca", ResponsableArea = "Servicios Digitales", CategoriaId = 5 },
                    new { Codigo = "SRV-WEB-003", Nombre = "Plataforma E-Learning", Descripcion = "Soporte para aulas virtuales y contenido educativo online", ResponsableArea = "Educación Virtual", CategoriaId = 5 }
                };

                int updatedCount = 0;
                int createdCount = 0;

                foreach (var servicioData in serviciosCorrect)
                {
                    var servicio = await _context.Servicios.FirstOrDefaultAsync(s => s.Codigo == servicioData.Codigo);
                    if (servicio != null)
                    {
                        // Actualizar servicio existente
                        servicio.Nombre = servicioData.Nombre;
                        servicio.Descripcion = servicioData.Descripcion;
                        servicio.ResponsableArea = servicioData.ResponsableArea;
                        servicio.UpdatedAt = DateTime.UtcNow;
                        updatedCount++;
                    }
                    else
                    {
                        // Crear nuevo servicio
                        var nuevoServicio = new IncidentesFISEI.Domain.Entities.Servicio
                        {
                            Codigo = servicioData.Codigo,
                            Nombre = servicioData.Nombre,
                            Descripcion = servicioData.Descripcion,
                            ResponsableArea = servicioData.ResponsableArea,
                            CategoriaId = servicioData.CategoriaId,
                            ContactoTecnico = GetContactoTecnico(servicioData.Codigo),
                            TiempoRespuestaMinutos = GetTiempoRespuesta(servicioData.Codigo),
                            TiempoResolucionMinutos = GetTiempoResolucion(servicioData.Codigo),
                            RequiereAprobacion = RequiereAprobacion(servicioData.Codigo),
                            IsActive = true,
                            IsDeleted = false,
                            CreatedAt = DateTime.UtcNow
                        };
                        
                        _context.Servicios.Add(nuevoServicio);
                        createdCount++;
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Servicios procesados correctamente con caracteres UTF-8", 
                    updatedCount = updatedCount,
                    createdCount = createdCount,
                    totalProcessed = updatedCount + createdCount,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = "Error al actualizar servicios", 
                    error = ex.Message 
                });
            }
        }

        [HttpGet("test-servicios-utf8")]
        public async Task<IActionResult> TestServiciosUtf8()
        {
            try
            {
                var servicios = await _context.Servicios
                    .Select(s => new { 
                        s.Id, 
                        s.Codigo, 
                        s.Nombre, 
                        s.Descripcion, 
                        s.ResponsableArea 
                    })
                    .OrderBy(s => s.Codigo)
                    .ToListAsync();

                return Ok(servicios);
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = "Error al obtener servicios", 
                    error = ex.Message 
                });
            }
        }

        private string GetContactoTecnico(string codigo)
        {
            return codigo switch
            {
                "SRV-HW-001" or "SRV-HW-002" => "soporte.hw@fisei.uta.edu.ec",
                "SRV-HW-003" => "audiovisuales@fisei.uta.edu.ec",
                "SRV-SW-001" or "SRV-SW-002" => "sistemas@fisei.uta.edu.ec",
                "SRV-SW-003" => "seguridad@fisei.uta.edu.ec",
                "SRV-NET-001" or "SRV-NET-002" => "redes@fisei.uta.edu.ec",
                "SRV-NET-003" => "infraestructura@fisei.uta.edu.ec",
                "SRV-SEC-001" or "SRV-SEC-003" => "admin@fisei.uta.edu.ec",
                "SRV-SEC-002" => "soporte@fisei.uta.edu.ec",
                "SRV-WEB-001" => "desarrollo@fisei.uta.edu.ec",
                "SRV-WEB-002" => "biblioteca@fisei.uta.edu.ec",
                "SRV-WEB-003" => "elearning@fisei.uta.edu.ec",
                _ => "soporte@fisei.uta.edu.ec"
            };
        }

        private int GetTiempoRespuesta(string codigo)
        {
            return codigo switch
            {
                "SRV-HW-002" or "SRV-NET-001" or "SRV-SW-003" => 15,
                "SRV-HW-001" or "SRV-SW-002" or "SRV-NET-002" or "SRV-SEC-002" or "SRV-WEB-002" => 30,
                "SRV-HW-003" or "SRV-NET-003" or "SRV-WEB-001" => 45,
                "SRV-SW-001" or "SRV-SEC-003" or "SRV-WEB-003" => 60,
                "SRV-SEC-001" => 120,
                _ => 30
            };
        }

        private int GetTiempoResolucion(string codigo)
        {
            return codigo switch
            {
                "SRV-SEC-002" => 60,
                "SRV-SW-003" => 90,
                "SRV-HW-002" or "SRV-NET-001" => 120,
                "SRV-HW-003" or "SRV-WEB-002" => 180,
                "SRV-HW-001" or "SRV-NET-002" or "SRV-SEC-003" => 240,
                "SRV-NET-003" or "SRV-SW-002" or "SRV-WEB-001" => 360,
                "SRV-SW-001" or "SRV-SEC-001" or "SRV-WEB-003" => 480,
                _ => 240
            };
        }

        private bool RequiereAprobacion(string codigo)
        {
            return codigo switch
            {
                "SRV-SW-001" or "SRV-SEC-001" or "SRV-SEC-003" => true,
                _ => false
            };
        }
    }
}