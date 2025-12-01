using FISEI.Incidentes.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FISEI.Incidentes.Infrastructure.Data
{
    // DbContext simple basado en el dominio (sin Identity)
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tablas
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Estado> EstadosIncidentes { get; set; }
        public DbSet<Incidente> Incidentes { get; set; }
        public DbSet<Historial> HistorialesIncidentes { get; set; }
        public DbSet<NivelSoporte> NivelesSoporte { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Asignacion> Asignaciones { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Conocimiento> BaseConocimiento { get; set; }
        public DbSet<SLA> SLAs { get; set; }
        public DbSet<SLAObjetivo> SLAObjetivos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // No llamar a base.OnModelCreating de Identity
            // Relación USUARIO ↔ ROL (opcional)
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany()
                .HasForeignKey(u => u.IdRol)
                .OnDelete(DeleteBehavior.SetNull);

            // =========================================
            // Relaciones de USUARIO
            // =========================================

            // =========================================
            // Relaciones de INCIDENTE
            // =========================================
            modelBuilder.Entity<Incidente>()
                .HasOne(i => i.Categoria)
                .WithMany()
                .HasForeignKey(i => i.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Incidente>()
                .HasOne(i => i.Estado)
                .WithMany()
                .HasForeignKey(i => i.IdEstado)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Incidente>()
                .HasOne(i => i.Usuario)
                .WithMany()
                .HasForeignKey(i => i.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Incidente>()
                .HasOne(i => i.NivelSoporte)
                .WithMany()
                .HasForeignKey(i => i.IdNivelSoporte)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Incidente>()
                .HasOne(i => i.Servicio)
                .WithMany()
                .HasForeignKey(i => i.IdServicio)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================
            // Relaciones de HISTORIAL
            // =========================================
            modelBuilder.Entity<Historial>()
                .HasOne(h => h.Incidente)
                .WithMany()
                .HasForeignKey(h => h.IdIncidente)              
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Historial>()
                .HasOne(h => h.Usuario)
                .WithMany()
                .HasForeignKey(h => h.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================
            // Relaciones de ASIGNACION
            // =========================================
            modelBuilder.Entity<Asignacion>()
                .HasOne(a => a.Incidente)
                .WithMany()
                .HasForeignKey(a => a.IdIncidente)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Asignacion>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.IdUsuarioAsignado)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================
            // Relaciones de NOTIFICACION
            // =========================================
            modelBuilder.Entity<Notificacion>()
                .HasOne(n => n.Usuario)
                .WithMany()
                .HasForeignKey(n => n.IdUsuarioDestino)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Conocimiento>()
    .HasOne(c => c.Categoria)
    .WithMany()
    .HasForeignKey(c => c.IdCategoria)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conocimiento>()
                .HasOne(c => c.IncidenteOrigen)
                .WithMany()
                .HasForeignKey(c => c.IdIncidenteOrigen)
                .OnDelete(DeleteBehavior.SetNull);

            // =========================================
            // Relaciones de SLA
            // =========================================
            modelBuilder.Entity<SLA>()
                .HasOne<Servicio>()
                .WithMany()
                .HasForeignKey(s => s.IdServicio)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SLAObjetivo>()
                .HasOne<SLA>()
                .WithMany()
                .HasForeignKey(o => o.IdSLA)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
