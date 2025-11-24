using FISEI.Incidentes.Core.Entities;
using FISEI.Incidentes.Core.Interfaces.IRepositories;
using FISEI.Incidentes.Core.Interfaces.IServices;
using FISEI.Incidentes.Infrastructure.Data.Repositories;

namespace FISEI.Incidentes.Application.Services
{
    /// <summary>
    /// Servicio de Base de Conocimiento (Knowledge Base) según ITIL v3
    /// </summary>
    public class ConocimientoService : IConocimientoService
    {
        private readonly IConocimientoRepository _conocimientoRepository;
        private readonly IIncidenteRepository _incidenteRepository;

        public ConocimientoService(
            IConocimientoRepository conocimientoRepository,
            IIncidenteRepository incidenteRepository)
        {
            _conocimientoRepository = conocimientoRepository;
            _incidenteRepository = incidenteRepository;
        }

        /// <summary>
        /// Crea un artículo de conocimiento desde un incidente resuelto
        /// </summary>
        public async Task<Conocimiento> CrearArticuloAsync(int idIncidenteResuelto)
        {
            var incidente = await _incidenteRepository.GetByIdAsync(idIncidenteResuelto);
            if (incidente == null)
                throw new Exception("Incidente no encontrado");

            if (incidente.FechaCierre == null)
                throw new Exception("Solo se pueden crear artículos de incidentes cerrados");

            var articulo = new Conocimiento
            {
                Titulo = incidente.Titulo,
                Descripcion = incidente.Descripcion,
                Solucion = "Pendiente de documentación", // Debe ser completado manualmente
                IdCategoria = incidente.IdCategoria,
                IdIncidenteOrigen = incidente.IdIncidente,
                FechaCreacion = DateTime.Now,
                Aprobado = false // Requiere aprobación
            };

            return await _conocimientoRepository.AddAsync(articulo);
        }

        public async Task<IEnumerable<Conocimiento>> BuscarSolucionesAsync(string palabrasClave)
        {
            return await _conocimientoRepository.BuscarPorPalabrasClave(palabrasClave);
        }

        /// <summary>
        /// Busca soluciones similares para un incidente antes de escalarlo
        /// </summary>
        public async Task<Conocimiento?> ObtenerSolucionSimilarAsync(int idIncidente)
        {
            var incidente = await _incidenteRepository.GetByIdAsync(idIncidente);
            if (incidente == null)
                return null;

            var soluciones = await _conocimientoRepository.BuscarPorPalabrasClave(incidente.Titulo);
            return soluciones.FirstOrDefault();
        }

        public async Task ValorarArticuloAsync(int idConocimiento, int calificacion)
        {
            if (calificacion < 1 || calificacion > 5)
                throw new ArgumentException("La calificación debe estar entre 1 y 5");

            var articulo = await _conocimientoRepository.GetByIdAsync(idConocimiento);
            if (articulo == null)
                throw new Exception("Artículo no encontrado");

            articulo.Calificacion = calificacion;
            await _conocimientoRepository.UpdateAsync(articulo);

            // Incrementar visualizaciones
            await _conocimientoRepository.IncrementarVisualizacionesAsync(idConocimiento);
        }
    }
}