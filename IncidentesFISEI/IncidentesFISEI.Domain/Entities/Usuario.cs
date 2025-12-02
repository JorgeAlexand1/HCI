using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public TipoUsuario TipoUsuario { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsEmailConfirmed { get; set; } = false;
    public DateTime? LastLoginAt { get; set; }
    
    // Propiedades específicas para técnicos
    public string? Especialidad { get; set; }
    public int? AñosExperiencia { get; set; }
    
    // Navegación
    public ICollection<Incidente> IncidentesReportados { get; set; } = new List<Incidente>();
    public ICollection<Incidente> IncidentesAsignados { get; set; } = new List<Incidente>();
    public ICollection<ComentarioIncidente> Comentarios { get; set; } = new List<ComentarioIncidente>();
    public ICollection<ArticuloConocimiento> ArticulosCreados { get; set; } = new List<ArticuloConocimiento>();
}