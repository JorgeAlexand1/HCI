namespace IncidentesFISEI.Domain.Enums;

public enum TipoEncuesta
{
    SatisfaccionGeneral = 1,
    CalidadSoporte = 2,
    TiempoRespuesta = 3,
    SolucionProblema = 4,
    Personalizada = 5
}

public enum TipoPregunta
{
    TextoCorto = 1,        // Input de texto simple
    TextoLargo = 2,        // Textarea
    EscalaLineal = 3,      // 1-5, 1-10, etc.
    OpcionUnica = 4,       // Radio buttons
    OpcionMultiple = 5,    // Checkboxes
    SiNo = 6,             // Boolean
    Fecha = 7,            // Date picker
    Calificacion = 8      // Stars rating (1-5)
}
