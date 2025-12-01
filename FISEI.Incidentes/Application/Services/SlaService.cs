using FISEI.Incidentes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FISEI.Incidentes.Application.Services
{
    public class SlaService
    {
        private readonly ApplicationDbContext _context;
        public SlaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SlaMetrics> GetGlobalMetricsAsync(DateTime? reference = null)
        {
            var now = reference ?? DateTime.UtcNow;
            var incidentes = await _context.Incidentes.AsNoTracking().ToListAsync();
            var objetivos = await _context.SLAObjetivos.AsNoTracking().ToListAsync();
            var slaMap = await _context.SLAs.AsNoTracking().ToListAsync();

            int vencidos = 0;
            int vencenHoy = 0;
            int cumplidos = 0;

            foreach (var inc in incidentes)
            {
                var objetivo = ResolveObjetivo(inc, objetivos, slaMap);
                if (objetivo == null) continue;
                var limiteResolucion = inc.FechaCreacion.AddMinutes(objetivo.MinutosResolucion);
                if (inc.FechaCierre.HasValue)
                {
                    if (inc.FechaCierre.Value <= limiteResolucion) cumplidos++; else vencidos++;
                }
                else
                {
                    if (limiteResolucion < now) vencidos++; else if (limiteResolucion.Date == now.Date) vencenHoy++;
                }
            }

            int totalRelevantes = vencidos + vencenHoy + cumplidos;
            double porcentaje = totalRelevantes == 0 ? 0 : (double)cumplidos / totalRelevantes * 100.0;

            return new SlaMetrics
            {
                IncidentesVencidos = vencidos,
                IncidentesVencenHoy = vencenHoy,
                IncidentesCumplidos = cumplidos,
                PorcentajeCumplimiento = Math.Round(porcentaje, 2)
            };
        }

        public async Task<IncidentSlaDetail?> GetIncidentSlaAsync(int idIncidente, DateTime? reference = null)
        {
            var now = reference ?? DateTime.UtcNow;
            var inc = await _context.Incidentes.FirstOrDefaultAsync(i => i.IdIncidente == idIncidente);
            if (inc == null) return null;
            var objetivos = await _context.SLAObjetivos.AsNoTracking().ToListAsync();
            var slaMap = await _context.SLAs.AsNoTracking().ToListAsync();
            var objetivo = ResolveObjetivo(inc, objetivos, slaMap);
            if (objetivo == null) return null;
            var limitePR = inc.FechaCreacion.AddMinutes(objetivo.MinutosPrimeraRespuesta);
            var limiteResolucion = inc.FechaCreacion.AddMinutes(objetivo.MinutosResolucion);
            return new IncidentSlaDetail
            {
                IdIncidente = inc.IdIncidente,
                Prioridad = inc.Prioridad,
                LimitePrimeraRespuesta = limitePR,
                LimiteResolucion = limiteResolucion,
                PrimeraRespuestaCumplida = inc.FechaPrimeraRespuesta.HasValue && inc.FechaPrimeraRespuesta <= limitePR,
                ResolucionCumplida = inc.FechaCierre.HasValue && inc.FechaCierre <= limiteResolucion,
                MinutosRestantesResolucion = inc.FechaCierre.HasValue ? 0 : Math.Max(0, (int)(limiteResolucion - now).TotalMinutes)
            };
        }

        private Core.Entities.SLAObjetivo? ResolveObjetivo(Core.Entities.Incidente inc, List<Core.Entities.SLAObjetivo> objetivos, List<Core.Entities.SLA> slas)
        {
            var sla = slas.FirstOrDefault(s => s.IdServicio == inc.IdServicio);
            if (sla == null) return null;
            var prioridad = string.IsNullOrWhiteSpace(inc.Prioridad) ? "P3" : inc.Prioridad!; // default
            return objetivos.FirstOrDefault(o => o.IdSLA == sla.IdSLA && o.Prioridad == prioridad);
        }
    }

    public class SlaMetrics
    {
        public int IncidentesVencidos { get; set; }
        public int IncidentesVencenHoy { get; set; }
        public int IncidentesCumplidos { get; set; }
        public double PorcentajeCumplimiento { get; set; }
    }

    public class IncidentSlaDetail
    {
        public int IdIncidente { get; set; }
        public string? Prioridad { get; set; }
        public DateTime LimitePrimeraRespuesta { get; set; }
        public DateTime LimiteResolucion { get; set; }
        public bool PrimeraRespuestaCumplida { get; set; }
        public bool ResolucionCumplida { get; set; }
        public int MinutosRestantesResolucion { get; set; }
    }
}