namespace IncidentesFISEI.Domain.Enums
{
    /// <summary>
    /// Niveles de soporte según modelo ITIL v4
    /// </summary>
    public enum NivelSoporte
    {
        /// <summary>
        /// Nivel 1 - Técnico de soporte básico (Primera línea)
        /// Resolución de incidentes básicos, tickets de usuario final
        /// </summary>
        L1_Tecnico = 1,

        /// <summary>
        /// Nivel 2 - Experto técnico (Segunda línea)
        /// Incidentes escalados que requieren conocimiento especializado
        /// </summary>
        L2_Experto = 2,

        /// <summary>
        /// Nivel 3 - Especialista (Tercera línea)
        /// Problemas complejos, desarrollo, configuración avanzada
        /// </summary>
        L3_Especialista = 3,

        /// <summary>
        /// Nivel 4 - Proveedor externo (Cuarta línea)
        /// Soporte del fabricante o proveedor de servicio
        /// </summary>
        L4_Proveedor = 4
    }
}
