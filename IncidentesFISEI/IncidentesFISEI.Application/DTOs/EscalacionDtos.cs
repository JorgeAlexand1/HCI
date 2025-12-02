namespace IncidentesFISEI.Application.DTOs
{
    public class EstadisticasEscalacionDto
    {
        public int TotalEscalaciones { get; set; }
        public int EscalacionesAutomaticas { get; set; }
        public int EscalacionesManuales { get; set; }
        public int IncidentesEnL1 { get; set; }
        public int IncidentesEnL2 { get; set; }
        public int IncidentesEnL3 { get; set; }
        public int IncidentesEnL4 { get; set; }
        public double TiempoPromedioHastaEscalacion { get; set; }
        public double PorcentajeResueltoEnL1 { get; set; }
        public double PorcentajeEscaladoDeL1 { get; set; }
    }

    public class HistorialEscalacionDto
    {
        public int Id { get; set; }
        public int IncidenteId { get; set; }
        public string NumeroIncidente { get; set; } = string.Empty;
        public string NivelOrigen { get; set; } = string.Empty;
        public string NivelDestino { get; set; } = string.Empty;
        public string? TecnicoOrigenNombre { get; set; }
        public string? TecnicoDestinoNombre { get; set; }
        public string Razon { get; set; } = string.Empty;
        public bool FueAutomatico { get; set; }
        public DateTime FechaEscalacion { get; set; }
    }
}
