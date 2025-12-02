using System.ComponentModel.DataAnnotations;

namespace IncidentesFISEI.Application.DTOs;

public class ComentarioDto
{
    public int Id { get; set; }
    public int IncidenteId { get; set; }
    public int AutorId { get; set; }
    public string AutorNombre { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public bool EsInterno { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class ComentarioIncidenteDto
{
    public int Id { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public bool EsInterno { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string AutorNombre { get; set; } = string.Empty;
    public int AutorId { get; set; }
}

public class CreateComentarioDto
{
    public int IncidenteId { get; set; }
    public int UsuarioId { get; set; }
    
    [Required(ErrorMessage = "El contenido del comentario es requerido")]
    [MinLength(10, ErrorMessage = "El comentario debe tener al menos 10 caracteres")]
    public string Contenido { get; set; } = string.Empty;
    
    public bool EsInterno { get; set; } = false;
}

public class UpdateComentarioDto
{
    [Required(ErrorMessage = "El contenido del comentario es requerido")]
    [MinLength(10, ErrorMessage = "El comentario debe tener al menos 10 caracteres")]
    public string Contenido { get; set; } = string.Empty;
}

public class ArchivoAdjuntoDto
{
    public int Id { get; set; }
    public string NombreOriginal { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public string TipoMime { get; set; } = string.Empty;
    public long TamañoBytes { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class RegistroTiempoDto
{
    public int Id { get; set; }
    public int IncidenteId { get; set; }
    public int UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public TimeSpan TiempoTrabajado { get; set; }
    public DateTime FechaRegistro { get; set; }
}