namespace IncidentesFISEI.Domain.Enums
{
    /// <summary>
    /// Tipos de servicios ofrecidos por DITIC según ITIL v4
    /// </summary>
    public enum TipoServicioDITIC
    {
        /// <summary>
        /// Servicios de infraestructura (servidores, red, almacenamiento)
        /// </summary>
        Infraestructura = 1,
        
        /// <summary>
        /// Aplicaciones y sistemas (ERP, CRM, aplicaciones custom)
        /// </summary>
        Aplicaciones = 2,
        
        /// <summary>
        /// Soporte al usuario final (help desk, asistencia técnica)
        /// </summary>
        SoporteUsuario = 3,
        
        /// <summary>
        /// Seguridad de la información (antivirus, firewall, auditoría)
        /// </summary>
        Seguridad = 4,
        
        /// <summary>
        /// Comunicaciones (email, telefonía, videoconferencia)
        /// </summary>
        Comunicaciones = 5,
        
        /// <summary>
        /// Gestión de datos (backup, recuperación, bases de datos)
        /// </summary>
        GestionDatos = 6,
        
        /// <summary>
        /// Desarrollo y mantenimiento de software
        /// </summary>
        Desarrollo = 7,
        
        /// <summary>
        /// Capacitación y entrenamiento en TI
        /// </summary>
        Capacitacion = 8,
        
        /// <summary>
        /// Servicios de hardware (equipos, impresoras, periféricos)
        /// </summary>
        Hardware = 9,
        
        /// <summary>
        /// Servicios de conectividad y redes
        /// </summary>
        RedesConectividad = 10
    }
}
