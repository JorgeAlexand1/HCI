using FISEI.Incidentes.Core.Entities;
using FISEI.Incidentes.Core.Interfaces.IRepositories;
using FISEI.Incidentes.Core.Interfaces.IServices;
using FISEI.Incidentes.Infrastructure.Data.Repositories;

namespace FISEI.Incidentes.Application.Services
{
    /// <summary>
    /// Servicio de asignación de incidentes según ITIL v3
    /// Implementa SPOC (Single Point of Contact) y distribución equitativa
    /// </summary>
    public class AsignacionService : IAsignacionService
    {
        private readonly IAsignacionRepository _asignacionRepository;
        private readonly IIncidenteRepository _incidenteRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly INotificacionService _notificacionService;

        public AsignacionService(
            IAsignacionRepository asignacionRepository,
            IIncidenteRepository incidenteRepository,
            IUsuarioRepository usuarioRepository,
            INotificacionService notificacionService)
        {
            _asignacionRepository = asignacionRepository;
            _incidenteRepository = incidenteRepository;
            _usuarioRepository = usuarioRepository;
            _notificacionService = notificacionService;
        }

        /// <summary>
        /// Asigna automáticamente al técnico con menor carga de trabajo
        /// </summary>
        public async Task<Asignacion> AsignarAutomaticamenteAsync(int idIncidente)
        {
            var incidente = await _incidenteRepository.GetByIdAsync(idIncidente);
            if (incidente == null)
                throw new Exception("Incidente no encontrado");

            // Obtener técnico con menor carga del nivel correspondiente
            var tecnico = await ObtenerTecnicoConMenorCargaAsync(incidente.IdNivelSoporte);

            // Desactivar asignaciones anteriores
            await _asignacionRepository.DesactivarAsignacionesAnterioresAsync(idIncidente);

            // Crear nueva asignación
            var asignacion = new Asignacion
            {
                IdIncidente = idIncidente,
                IdUsuarioAsignado = tecnico.IdUsuario,
                FechaAsignacion = DateTime.Now,
                Activo = true
            };

            await _asignacionRepository.AddAsync(asignacion);

            // Notificar al técnico
            await _notificacionService.NotificarAsignacionAsync(tecnico.IdUsuario, idIncidente);

            return asignacion;
        }

        /// <summary>
        /// Permite al SPOC asignar manualmente un incidente
        /// </summary>
        public async Task<Asignacion> AsignarManualmenteAsync(int idIncidente, int idTecnico, int idUsuarioSPOC)
        {
            // Verificar que quien asigna es SPOC
            var esSPOC = await _usuarioRepository.EsSPOCAsync(idUsuarioSPOC);
            if (!esSPOC)
                throw new UnauthorizedAccessException("Solo el SPOC puede asignar manualmente incidentes");

            var incidente = await _incidenteRepository.GetByIdAsync(idIncidente);
            if (incidente == null)
                throw new Exception("Incidente no encontrado");

            var tecnico = await _usuarioRepository.GetByIdAsync(idTecnico);
            if (tecnico == null)
                throw new Exception("Técnico no encontrado");

            // Desactivar asignaciones anteriores
            await _asignacionRepository.DesactivarAsignacionesAnterioresAsync(idIncidente);

            // Crear nueva asignación
            var asignacion = new Asignacion
            {
                IdIncidente = idIncidente,
                IdUsuarioAsignado = idTecnico,
                FechaAsignacion = DateTime.Now,
                Activo = true
            };

            await _asignacionRepository.AddAsync(asignacion);

            // Notificar al técnico
            await _notificacionService.NotificarAsignacionAsync(idTecnico, idIncidente);

            return asignacion;
        }

        /// <summary>
        /// Permite a un técnico tomar un incidente cuando el SPOC no está disponible
        /// </summary>
        public async Task<Asignacion> TomarIncidenteAsync(int idIncidente, int idTecnico)
        {
            var incidente = await _incidenteRepository.GetByIdAsync(idIncidente);
            if (incidente == null)
                throw new Exception("Incidente no encontrado");

            // Verificar que el incidente no tenga asignación activa
            var asignacionActual = await _asignacionRepository.GetAsignacionActivaPorIncidenteAsync(idIncidente);
            if (asignacionActual != null)
                throw new Exception("El incidente ya tiene una asignación activa");

            var tecnico = await _usuarioRepository.GetByIdAsync(idTecnico);
            if (tecnico == null)
                throw new Exception("Técnico no encontrado");

            // Crear asignación
            var asignacion = new Asignacion
            {
                IdIncidente = idIncidente,
                IdUsuarioAsignado = idTecnico,
                FechaAsignacion = DateTime.Now,
                Activo = true
            };

            await _asignacionRepository.AddAsync(asignacion);

            return asignacion;
        }

        /// <summary>
        /// Reasigna un incidente a otro técnico
        /// </summary>
        public async Task ReasignarAsync(int idIncidente, int idNuevoTecnico)
        {
            var incidente = await _incidenteRepository.GetByIdAsync(idIncidente);
            if (incidente == null)
                throw new Exception("Incidente no encontrado");

            var nuevoTecnico = await _usuarioRepository.GetByIdAsync(idNuevoTecnico);
            if (nuevoTecnico == null)
                throw new Exception("Técnico no encontrado");

            // Desactivar asignaciones anteriores
            await _asignacionRepository.DesactivarAsignacionesAnterioresAsync(idIncidente);

            // Crear nueva asignación
            var asignacion = new Asignacion
            {
                IdIncidente = idIncidente,
                IdUsuarioAsignado = idNuevoTecnico,
                FechaAsignacion = DateTime.Now,
                Activo = true
            };

            await _asignacionRepository.AddAsync(asignacion);

            // Notificar al nuevo técnico
            await _notificacionService.NotificarAsignacionAsync(idNuevoTecnico, idIncidente);
        }

        /// <summary>
        /// Obtiene el técnico con menor carga de trabajo (distribución equitativa)
        /// </summary>
        public async Task<Usuario> ObtenerTecnicoConMenorCargaAsync(int idNivelSoporte)
        {
            var tecnicos = await _usuarioRepository.GetTecnicosPorNivelAsync(idNivelSoporte);
            
            if (!tecnicos.Any())
                throw new Exception($"No hay técnicos disponibles para el nivel {idNivelSoporte}");

            // Contar incidentes asignados a cada técnico
            var tecnicoConMenorCarga = tecnicos.First();
            int menorCarga = int.MaxValue;

            foreach (var tecnico in tecnicos)
            {
                var carga = await _incidenteRepository.ContarIncidentesPorTecnicoAsync(tecnico.IdUsuario);
                if (carga < menorCarga)
                {
                    menorCarga = carga;
                    tecnicoConMenorCarga = tecnico;
                }
            }

            return tecnicoConMenorCarga;
        }
    }
}