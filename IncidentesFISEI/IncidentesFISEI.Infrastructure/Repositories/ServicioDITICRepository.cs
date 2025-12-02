using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Interfaces;
using IncidentesFISEI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentesFISEI.Infrastructure.Repositories
{
    public class ServicioDITICRepository : Repository<ServicioDITIC>, IServicioDITICRepository
    {
        public ServicioDITICRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ServicioDITIC>> GetServiciosActivosAsync()
        {
            return await _context.ServiciosDITIC
                .Include(s => s.SLA)
                .Include(s => s.Categoria)
                .Include(s => s.ResponsableTecnico)
                .Include(s => s.ResponsableNegocio)
                .Where(s => s.EstaActivo && !s.IsDeleted)
                .OrderBy(s => s.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServicioDITIC>> GetServiciosPorTipoAsync(int tipoServicio)
        {
            return await _context.ServiciosDITIC
                .Include(s => s.SLA)
                .Include(s => s.Categoria)
                .Where(s => (int)s.TipoServicio == tipoServicio && s.EstaActivo && !s.IsDeleted)
                .OrderBy(s => s.Nombre)
                .ToListAsync();
        }

        public async Task<ServicioDITIC?> GetServicioPorCodigoAsync(string codigo)
        {
            return await _context.ServiciosDITIC
                .Include(s => s.SLA)
                .Include(s => s.Categoria)
                .Include(s => s.ResponsableTecnico)
                .Include(s => s.ResponsableNegocio)
                .FirstOrDefaultAsync(s => s.Codigo == codigo && !s.IsDeleted);
        }

        public async Task<IEnumerable<ServicioDITIC>> GetServiciosEsencialesAsync()
        {
            return await _context.ServiciosDITIC
                .Include(s => s.SLA)
                .Include(s => s.Categoria)
                .Where(s => s.EsServicioEsencial && s.EstaActivo && !s.IsDeleted)
                .OrderBy(s => s.Nombre)
                .ToListAsync();
        }
    }
}
