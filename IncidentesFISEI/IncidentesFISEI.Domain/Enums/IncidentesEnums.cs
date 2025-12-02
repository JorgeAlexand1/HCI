namespace IncidentesFISEI.Domain.Enums;

public enum TipoUsuario
{
    Usuario = 1,
    Tecnico = 2,
    Supervisor = 3,
    Administrador = 4
}

public enum RolUsuario
{
    Estudiante = 1,
    Docente = 2,
    Administrativo = 3,
    Tecnico = 4,
    SupervisorTecnico = 5,
    Administrador = 6
}

public enum EstadoIncidente
{
    Abierto = 1,
    EnProgreso = 2,
    EnEspera = 3,
    Resuelto = 4,
    Cerrado = 5,
    Cancelado = 6,
    Escalado = 7
}

public enum PrioridadIncidente
{
    Baja = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}

public enum ImpactoIncidente
{
    Bajo = 1,
    Medio = 2,
    Alto = 3,
    Critico = 4
}

public enum UrgenciaIncidente
{
    Baja = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}

public enum EstadoArticulo
{
    Borrador = 1,
    Revision = 2,
    Publicado = 3,
    Archivado = 4,
    Rechazado = 5
}

public enum TipoComentario
{
    Comentario = 1,
    ActualizacionEstado = 2,
    Solucion = 3,
    Escalacion = 4,
    CierreIncidente = 5
}

public enum TipoRelacion
{
    Duplicado = 1,
    Relacionado = 2,
    CausadoPor = 3,
    Padre = 4,
    Hijo = 5,
    Dependiente = 6
}

public enum TipoActividad
{
    Investigacion = 1,
    Diagnostico = 2,
    Implementacion = 3,
    Pruebas = 4,
    Documentacion = 5,
    Comunicacion = 6,
    Escalacion = 7,
    Cierre = 8
}

public enum TipoArticulo
{
    ProcedimientoGeneral = 1,
    SolucionProblema = 2,
    FAQ = 3,
    Manual = 4,
    Tutorial = 5,
    KnownError = 6,
    Workaround = 7
}

public enum TipoVoto
{
    Negativo = 0,
    Positivo = 1
}

public enum EstadoValidacion
{
    Pendiente = 1,
    EnRevision = 2,
    Aprobado = 3,
    Rechazado = 4
}