using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IncidentesFISEI.Application.Interfaces;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;
using IncidentesFISEI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentesFISEI.Infrastructure.Services;

/// <summary>
/// Servicio para gestión de Base de Conocimiento
/// Supervisores crean artículos, admins aprueban/rechazan
/// </summary>
public interface IKnowledgeBaseService
{
    // Supervisor operations
    Task<int> CrearArticuloAsync(ArticuloConocimiento articulo);
    Task<bool> EnviarARevisionAsync(int articuloId);
    Task<List<ArticuloConocimiento>> ObtenerMisArticulosAsync(int autorId);
    
    // Admin operations
    Task<List<ArticuloConocimiento>> ObtenerArticulosEnRevisionAsync();
    Task<bool> AprobarArticuloAsync(int articuloId, int administradorId);
    Task<bool> RechazarArticuloAsync(int articuloId, int administradorId, string motivo);
    
    // Public operations
    Task<List<ArticuloConocimiento>> ObtenerArticulosPublicadosAsync();
    Task<ArticuloConocimiento?> ObtenerArticuloAsync(int articuloId);
    Task IncrementarVisualizacionesAsync(int articuloId);
}

public class KnowledgeBaseService : IKnowledgeBaseService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;

    public KnowledgeBaseService(
        ApplicationDbContext context,
        INotificationService notificationService,
        IEmailService emailService)
    {
        _context = context;
        _notificationService = notificationService;
        _emailService = emailService;
    }

    /// <summary>
    /// Crear un nuevo artículo (como borrador)
    /// </summary>
    public async Task<int> CrearArticuloAsync(ArticuloConocimiento articulo)
    {
        articulo.Estado = EstadoArticulo.Borrador;
        articulo.CreatedAt = DateTime.UtcNow;
        
        _context.ArticulosConocimiento.Add(articulo);
        await _context.SaveChangesAsync();
        
        return articulo.Id;
    }

    /// <summary>
    /// Enviar artículo a revisión
    /// Notifica a los administradores
    /// </summary>
    public async Task<bool> EnviarARevisionAsync(int articuloId)
    {
        var articulo = await _context.ArticulosConocimiento
            .Include(a => a.Autor)
            .FirstOrDefaultAsync(a => a.Id == articuloId);
        
        if (articulo == null)
            return false;

        articulo.Estado = EstadoArticulo.Revision;
        articulo.UpdatedAt = DateTime.UtcNow;
        
        _context.ArticulosConocimiento.Update(articulo);
        await _context.SaveChangesAsync();

        // Notificar a administradores
        await NotificarAdminsNuevoArticuloAsync(articulo);

        return true;
    }

    /// <summary>
    /// Obtener artículos del supervisor actual
    /// </summary>
    public async Task<List<ArticuloConocimiento>> ObtenerMisArticulosAsync(int autorId)
    {
        return await _context.ArticulosConocimiento
            .Where(a => a.AutorId == autorId && !a.IsDeleted)
            .Include(a => a.Categoria)
            .Include(a => a.RevisadoPor)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Obtener artículos en revisión pendiente
    /// </summary>
    public async Task<List<ArticuloConocimiento>> ObtenerArticulosEnRevisionAsync()
    {
        return await _context.ArticulosConocimiento
            .Where(a => a.Estado == EstadoArticulo.Revision && !a.IsDeleted)
            .Include(a => a.Autor)
            .Include(a => a.Categoria)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Aprobar un artículo
    /// Cambia estado a Aprobado, lo publica y notifica al supervisor
    /// </summary>
    public async Task<bool> AprobarArticuloAsync(int articuloId, int administradorId)
    {
        var articulo = await _context.ArticulosConocimiento
            .Include(a => a.Autor)
            .FirstOrDefaultAsync(a => a.Id == articuloId);
        
        if (articulo == null)
            return false;

        articulo.Estado = EstadoArticulo.Publicado;
        articulo.FechaPublicacion = DateTime.UtcNow;
        articulo.FechaRevision = DateTime.UtcNow;
        articulo.RevisadoPorId = administradorId;
        articulo.UpdatedAt = DateTime.UtcNow;

        _context.ArticulosConocimiento.Update(articulo);
        await _context.SaveChangesAsync();

        // Notificar al autor (supervisor)
        await NotificarSupervisorArticuloAprobadoAsync(articulo);

        return true;
    }

    /// <summary>
    /// Rechazar un artículo
    /// Notifica al supervisor con el motivo del rechazo
    /// </summary>
    public async Task<bool> RechazarArticuloAsync(int articuloId, int administradorId, string motivo)
    {
        var articulo = await _context.ArticulosConocimiento
            .Include(a => a.Autor)
            .FirstOrDefaultAsync(a => a.Id == articuloId);
        
        if (articulo == null)
            return false;

        articulo.Estado = EstadoArticulo.Rechazado;
        articulo.FechaRevision = DateTime.UtcNow;
        articulo.RevisadoPorId = administradorId;
        articulo.UpdatedAt = DateTime.UtcNow;

        _context.ArticulosConocimiento.Update(articulo);
        await _context.SaveChangesAsync();

        // Notificar al autor
        await NotificarSupervisorArticuloRechazadoAsync(articulo, motivo);

        return true;
    }

    /// <summary>
    /// Obtener artículos publicados
    /// </summary>
    public async Task<List<ArticuloConocimiento>> ObtenerArticulosPublicadosAsync()
    {
        return await _context.ArticulosConocimiento
            .Where(a => a.Estado == EstadoArticulo.Publicado && !a.IsDeleted)
            .Include(a => a.Autor)
            .Include(a => a.Categoria)
            .OrderByDescending(a => a.FechaPublicacion)
            .ToListAsync();
    }

    /// <summary>
    /// Obtener un artículo específico
    /// </summary>
    public async Task<ArticuloConocimiento?> ObtenerArticuloAsync(int articuloId)
    {
        return await _context.ArticulosConocimiento
            .Include(a => a.Autor)
            .Include(a => a.Categoria)
            .Include(a => a.Comentarios)
            .FirstOrDefaultAsync(a => a.Id == articuloId && !a.IsDeleted);
    }

    /// <summary>
    /// Incrementar visualizaciones
    /// </summary>
    public async Task IncrementarVisualizacionesAsync(int articuloId)
    {
        var articulo = await _context.ArticulosConocimiento.FindAsync(articuloId);
        if (articulo != null)
        {
            articulo.Visualizaciones++;
            _context.ArticulosConocimiento.Update(articulo);
            await _context.SaveChangesAsync();
        }
    }

    // Métodos privados de notificación

    private async Task NotificarAdminsNuevoArticuloAsync(ArticuloConocimiento articulo)
    {
        try
        {
            var admins = await _context.Usuarios
                .Where(u => u.TipoUsuario == TipoUsuario.Administrador && u.IsActive && !u.IsDeleted)
                .ToListAsync();

            foreach (var admin in admins)
            {
                // Notificación en sistema
                await _notificationService.CrearNotificacionAsync(
                    admin.Id,
                    TipoNotificacion.NuevoArticuloConocimiento,
                    "Nuevo artículo en revisión",
                    $"El supervisor {articulo.Autor.FirstName} {articulo.Autor.LastName} ha enviado el artículo \"{articulo.Titulo}\" para revisión",
                    articulo.Id
                );

                // Email
                if (!string.IsNullOrEmpty(admin.Email))
                {
                    await _emailService.EnviarEmailAsync(
                        admin.Email,
                        "Nuevo Artículo de Conocimiento para Revisar",
                        GenerarEmailNuevoArticulo(admin, articulo)
                    );
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error notificando a admins: {ex.Message}");
        }
    }

    private async Task NotificarSupervisorArticuloAprobadoAsync(ArticuloConocimiento articulo)
    {
        try
        {
            var supervisor = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == articulo.AutorId);

            if (supervisor != null)
            {
                // Notificación en sistema
                await _notificationService.CrearNotificacionAsync(
                    supervisor.Id,
                    TipoNotificacion.ArticuloAprobado,
                    "Artículo Aprobado",
                    $"Tu artículo \"{articulo.Titulo}\" ha sido aprobado y publicado en la base de conocimiento",
                    articulo.Id
                );

                // Email
                if (!string.IsNullOrEmpty(supervisor.Email))
                {
                    await _emailService.EnviarEmailAsync(
                        supervisor.Email,
                        "Tu Artículo ha sido Aprobado",
                        GenerarEmailArticuloAprobado(supervisor, articulo)
                    );
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error notificando aprobación: {ex.Message}");
        }
    }

    private async Task NotificarSupervisorArticuloRechazadoAsync(ArticuloConocimiento articulo, string motivo)
    {
        try
        {
            var supervisor = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == articulo.AutorId);

            if (supervisor != null)
            {
                // Notificación en sistema
                await _notificationService.CrearNotificacionAsync(
                    supervisor.Id,
                    TipoNotificacion.ArticuloRechazado,
                    "Artículo Rechazado",
                    $"Tu artículo \"{articulo.Titulo}\" ha sido rechazado. Motivo: {motivo}",
                    articulo.Id
                );

                // Email
                if (!string.IsNullOrEmpty(supervisor.Email))
                {
                    await _emailService.EnviarEmailAsync(
                        supervisor.Email,
                        "Tu Artículo ha sido Rechazado",
                        GenerarEmailArticuloRechazado(supervisor, articulo, motivo)
                    );
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error notificando rechazo: {ex.Message}");
        }
    }

    // Métodos para generar contenido de emails

    private string GenerarEmailNuevoArticulo(Usuario admin, ArticuloConocimiento articulo)
    {
        return $@"
            <h2>Nuevo Artículo de Conocimiento</h2>
            <p>Hola {admin.FirstName},</p>
            <p>El supervisor <strong>{articulo.Autor.FirstName} {articulo.Autor.LastName}</strong> ha enviado un nuevo artículo para revisión:</p>
            <div style='background: #f8f9fa; padding: 15px; border-radius: 8px; margin: 15px 0;'>
                <h3>{articulo.Titulo}</h3>
                <p><strong>Categoría:</strong> {articulo.Categoria.Nombre}</p>
                <p><strong>Resumen:</strong> {articulo.Resumen}</p>
            </div>
            <p>Por favor, inicia sesión en el sistema para revisar y aprobar o rechazar este artículo.</p>
            <p>Gracias,<br/>Sistema de Gestión de Incidentes</p>
        ";
    }

    private string GenerarEmailArticuloAprobado(Usuario supervisor, ArticuloConocimiento articulo)
    {
        return $@"
            <h2>¡Tu Artículo ha sido Aprobado!</h2>
            <p>Hola {supervisor.FirstName},</p>
            <p>Te complacemos informarte que tu artículo <strong>{articulo.Titulo}</strong> ha sido aprobado y publicado en la base de conocimiento.</p>
            <p>Tu contribución ayudará a otros miembros del equipo a resolver problemas similares.</p>
            <p>Puedes verlo en el portal de conocimiento o acceder a él desde tu dashboard.</p>
            <p>Gracias por tu contribución,<br/>Sistema de Gestión de Incidentes</p>
        ";
    }

    private string GenerarEmailArticuloRechazado(Usuario supervisor, ArticuloConocimiento articulo, string motivo)
    {
        return $@"
            <h2>Revisión de tu Artículo</h2>
            <p>Hola {supervisor.FirstName},</p>
            <p>Tu artículo <strong>{articulo.Titulo}</strong> ha sido revisado y requiere ajustes antes de ser publicado.</p>
            <div style='background: #f8d7da; padding: 15px; border-radius: 8px; margin: 15px 0; border-left: 4px solid #721c24;'>
                <strong>Motivo del rechazo:</strong><br/>
                {motivo}
            </div>
            <p>Por favor, revisa tu artículo, realiza los cambios necesarios y vuelve a enviarlo para revisión.</p>
            <p>Si tienes preguntas, no dudes en contactar al equipo administrativo.</p>
            <p>Gracias,<br/>Sistema de Gestión de Incidentes</p>
        ";
    }
}

