namespace IncidentesFISEI.Domain.Enums;

public enum TipoAccionAuditoria
{
    Creacion = 1,
    Actualizacion = 2,
    Eliminacion = 3,
    Consulta = 4,
    Login = 5,
    Logout = 6,
    CambioEstado = 7,
    Asignacion = 8,
    Escalacion = 9,
    Aprobacion = 10,
    Rechazo = 11,
    Exportacion = 12,
    Importacion = 13
}

public enum TipoEntidadAuditoria
{
    Usuario = 1,
    Incidente = 2,
    Comentario = 3,
    ArticuloConocimiento = 4,
    Categoria = 5,
    SLA = 6,
    Escalacion = 7,
    Notificacion = 8,
    Encuesta = 9,
    ServicioDITIC = 10,
    ArchivoAdjunto = 11,
    Configuracion = 12
}

public enum NivelSeveridadAuditoria
{
    Informativo = 1,
    Bajo = 2,
    Medio = 3,
    Alto = 4,
    Critico = 5
}
