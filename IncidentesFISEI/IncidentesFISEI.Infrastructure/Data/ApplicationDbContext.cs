using Microsoft.EntityFrameworkCore;
using IncidentesFISEI.Domain.Entities;
using IncidentesFISEI.Domain.Enums;

namespace IncidentesFISEI.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Incidente> Incidentes { get; set; }
    public DbSet<CategoriaIncidente> Categorias { get; set; }
    public DbSet<ArticuloConocimiento> ArticulosConocimiento { get; set; }
    public DbSet<ComentarioIncidente> ComentariosIncidente { get; set; }
    public DbSet<ComentarioArticulo> ComentariosArticulo { get; set; }
    public DbSet<VotacionArticulo> VotacionesArticulo { get; set; }
    public DbSet<ArchivoAdjunto> ArchivosAdjuntos { get; set; }
    public DbSet<IncidenteRelacionado> IncidentesRelacionados { get; set; }
    public DbSet<RegistroTiempo> RegistrosTiempo { get; set; }
    public DbSet<SLA> SLAs { get; set; }
    public DbSet<EscalacionSLA> EscalacionesSLA { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        // Suprimir la advertencia de cambios pendientes en el modelo
        optionsBuilder.ConfigureWarnings(warnings => 
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Especialidad).HasMaxLength(100);

            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configuración de Incidente
        modelBuilder.Entity<Incidente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroIncidente).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Solucion).HasMaxLength(2000);
            entity.Property(e => e.CausaRaiz).HasMaxLength(1000);
            entity.Property(e => e.PasosReproducir).HasMaxLength(2000);
            entity.Property(e => e.ActivosAfectados).HasMaxLength(1000);
            entity.Property(e => e.ImpactoDetallado).HasMaxLength(1000);

            entity.HasIndex(e => e.NumeroIncidente).IsUnique();

            // Relaciones
            entity.HasOne(e => e.ReportadoPor)
                .WithMany(u => u.IncidentesReportados)
                .HasForeignKey(e => e.ReportadoPorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AsignadoA)
                .WithMany(u => u.IncidentesAsignados)
                .HasForeignKey(e => e.AsignadoAId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Categoria)
                .WithMany(c => c.Incidentes)
                .HasForeignKey(e => e.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ArticuloConocimiento)
                .WithMany(a => a.IncidentesRelacionados)
                .HasForeignKey(e => e.ArticuloConocimientoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de CategoriaIncidente
        modelBuilder.Entity<CategoriaIncidente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Color).HasMaxLength(7);
            entity.Property(e => e.Icono).HasMaxLength(50);

            // Auto-referencia para subcategorías
            entity.HasOne(e => e.ParentCategory)
                .WithMany(e => e.SubCategorias)
                .HasForeignKey(e => e.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de ArticuloConocimiento
        modelBuilder.Entity<ArticuloConocimiento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Contenido).IsRequired();
            entity.Property(e => e.Resumen).HasMaxLength(500);
            entity.Property(e => e.PasosDetallados).HasMaxLength(3000);
            entity.Property(e => e.Prerequisites).HasMaxLength(1000);
            entity.Property(e => e.Limitaciones).HasMaxLength(1000);

            // Configurar Tags como JSON
            entity.Property(e => e.Tags)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

            // Relaciones
            entity.HasOne(e => e.Autor)
                .WithMany(u => u.ArticulosCreados)
                .HasForeignKey(e => e.AutorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.RevisadoPor)
                .WithMany()
                .HasForeignKey(e => e.RevisadoPorId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Categoria)
                .WithMany(c => c.ArticulosConocimiento)
                .HasForeignKey(e => e.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de ComentarioIncidente
        modelBuilder.Entity<ComentarioIncidente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Contenido).IsRequired().HasMaxLength(2000);

            entity.HasOne(e => e.Incidente)
                .WithMany(i => i.Comentarios)
                .HasForeignKey(e => e.IncidenteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Autor)
                .WithMany(u => u.Comentarios)
                .HasForeignKey(e => e.AutorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de ComentarioArticulo
        modelBuilder.Entity<ComentarioArticulo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Contenido).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Puntuacion).HasDefaultValue(0);

            entity.HasOne(e => e.Articulo)
                .WithMany(a => a.Comentarios)
                .HasForeignKey(e => e.ArticuloId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Autor)
                .WithMany()
                .HasForeignKey(e => e.AutorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de VotacionArticulo
        modelBuilder.Entity<VotacionArticulo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Articulo)
                .WithMany(a => a.Votaciones)
                .HasForeignKey(e => e.ArticuloId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Un usuario solo puede votar una vez por artículo
            entity.HasIndex(e => new { e.ArticuloId, e.UsuarioId }).IsUnique();
        });

        // Configuración de ArchivoAdjunto
        modelBuilder.Entity<ArchivoAdjunto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombreOriginal).IsRequired().HasMaxLength(255);
            entity.Property(e => e.NombreArchivo).IsRequired().HasMaxLength(255);
            entity.Property(e => e.RutaArchivo).IsRequired().HasMaxLength(500);
            entity.Property(e => e.TipoMime).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Incidente)
                .WithMany(i => i.ArchivosAdjuntos)
                .HasForeignKey(e => e.IncidenteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Articulo)
                .WithMany(a => a.ArchivosAdjuntos)
                .HasForeignKey(e => e.ArticuloId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.SubidoPor)
                .WithMany()
                .HasForeignKey(e => e.SubidoPorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de IncidenteRelacionado
        modelBuilder.Entity<IncidenteRelacionado>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descripcion).HasMaxLength(500);

            entity.HasOne(e => e.IncidentePrincipal)
                .WithMany(i => i.IncidentesRelacionados)
                .HasForeignKey(e => e.IncidentePrincipalId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.IncidenteRef)
                .WithMany()
                .HasForeignKey(e => e.IncidenteRelacionadoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de RegistroTiempo
        modelBuilder.Entity<RegistroTiempo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descripcion).HasMaxLength(500);

            entity.HasOne(e => e.Incidente)
                .WithMany(i => i.RegistrosTiempo)
                .HasForeignKey(e => e.IncidenteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tecnico)
                .WithMany()
                .HasForeignKey(e => e.TecnicoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de SLA
        modelBuilder.Entity<SLA>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(500);
        });

        // Configuración de EscalacionSLA
        modelBuilder.Entity<EscalacionSLA>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Motivo).IsRequired().HasMaxLength(500);

            entity.HasOne(e => e.SLA)
                .WithMany()
                .HasForeignKey(e => e.SLAId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Incidente)
                .WithMany()
                .HasForeignKey(e => e.IncidenteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Datos iniciales (Seeds)
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Categorías iniciales
        modelBuilder.Entity<CategoriaIncidente>().HasData(
            new CategoriaIncidente { Id = 1, Nombre = "Hardware", Descripcion = "Problemas relacionados con hardware", Color = "#dc3545", Icono = "fas fa-desktop", TiempoRespuestaMinutos = 30, TiempoResolucionMinutos = 240, CreatedAt = DateTime.UtcNow },
            new CategoriaIncidente { Id = 2, Nombre = "Software", Descripcion = "Problemas relacionados con software", Color = "#007bff", Icono = "fas fa-code", TiempoRespuestaMinutos = 60, TiempoResolucionMinutos = 480, CreatedAt = DateTime.UtcNow },
            new CategoriaIncidente { Id = 3, Nombre = "Red", Descripcion = "Problemas de conectividad y red", Color = "#28a745", Icono = "fas fa-network-wired", TiempoRespuestaMinutos = 15, TiempoResolucionMinutos = 120, CreatedAt = DateTime.UtcNow },
            new CategoriaIncidente { Id = 4, Nombre = "Acceso", Descripcion = "Problemas de autenticación y permisos", Color = "#ffc107", Icono = "fas fa-lock", TiempoRespuestaMinutos = 45, TiempoResolucionMinutos = 180, CreatedAt = DateTime.UtcNow },
            new CategoriaIncidente { Id = 5, Nombre = "Correo", Descripcion = "Problemas con correo electrónico", Color = "#17a2b8", Icono = "fas fa-envelope", TiempoRespuestaMinutos = 60, TiempoResolucionMinutos = 360, CreatedAt = DateTime.UtcNow }
        );

        // Usuarios predefinidos para el sistema
        var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        modelBuilder.Entity<Usuario>().HasData(
            // ADMINISTRADOR
            new Usuario 
            { 
                Id = 1, 
                Username = "admin", 
                Email = "admin@fisei.uta.edu.ec", 
                PasswordHash = "$2a$11$KjLnhQkoD9HBy0sZQaxvKe5sBoELZbldjM1ywnR.wn9aAgOOXQr1K", // password: Admin123!
                FirstName = "Administrador", 
                LastName = "Sistema", 
                Department = "FISEI", 
                TipoUsuario = TipoUsuario.Administrador,
                IsActive = true,
                IsEmailConfirmed = true,
                CreatedAt = baseDate 
            },
            
            // SUPERVISOR TÉCNICO
            new Usuario
            {
                Id = 2,
                Username = "supervisor1",
                Email = "supervisor@fisei.uta.edu.ec",
                PasswordHash = "$2a$11$r9PuDRz5A1J/7FHn3AXd5.Zg/AOUy0Ok/8sDSxJEKVGJYtFRlUdOK", // password: Supervisor123!
                FirstName = "Carlos",
                LastName = "Mendoza",
                Department = "Soporte Técnico",
                TipoUsuario = TipoUsuario.Supervisor,
                Especialidad = "Redes y Sistemas",
                AñosExperiencia = 8,
                IsActive = true,
                IsEmailConfirmed = true,
                CreatedAt = baseDate
            },
            
            // TÉCNICO DE SOPORTE
            new Usuario
            {
                Id = 3,
                Username = "tecnico1",
                Email = "tecnico1@fisei.uta.edu.ec",
                PasswordHash = "$2a$11$wxqje4Ox3VO3RGljJA9Vp.9WByK3DpGbgDt2Bfde5awiLFBlY7.0e", // password: Tecnico123!
                FirstName = "María",
                LastName = "González",
                Department = "Soporte Técnico",
                TipoUsuario = TipoUsuario.Tecnico,
                Especialidad = "Hardware y Software",
                AñosExperiencia = 3,
                IsActive = true,
                IsEmailConfirmed = true,
                CreatedAt = baseDate
            },
            
            // SEGUNDO TÉCNICO
            new Usuario
            {
                Id = 4,
                Username = "tecnico2",
                Email = "tecnico2@fisei.uta.edu.ec",
                PasswordHash = "$2a$11$9f5rP2P5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5q", // password: Tecnico123!
                FirstName = "Luis",
                LastName = "Ramírez",
                Department = "Soporte Técnico",
                TipoUsuario = TipoUsuario.Tecnico,
                Especialidad = "Redes y Conectividad",
                AñosExperiencia = 5,
                IsActive = true,
                IsEmailConfirmed = true,
                CreatedAt = baseDate
            },
            
            // DOCENTE
            new Usuario
            {
                Id = 5,
                Username = "docente1",
                Email = "docente1@fisei.uta.edu.ec",
                PasswordHash = "$2a$11$fS7059bDQD23mvMoKYCjoOvs7umhzDvu5tOW.lGwA7y6gzUWaNGo.", // password: Docente123!
                FirstName = "Ana",
                LastName = "Pérez",
                Department = "Ingeniería en Sistemas",
                TipoUsuario = TipoUsuario.Usuario,
                Phone = "0998123456",
                IsActive = true,
                IsEmailConfirmed = true,
                CreatedAt = baseDate
            },
            
            // ESTUDIANTE 1
            new Usuario
            {
                Id = 6,
                Username = "estudiante1",
                Email = "estudiante1@fisei.uta.edu.ec",
                PasswordHash = "$2a$11$9f5rP2P5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5q", // password: Estudiante123!
                FirstName = "José",
                LastName = "Morales",
                Department = "Ingeniería en Sistemas",
                TipoUsuario = TipoUsuario.Usuario,
                IsActive = false, // Inicialmente inactivo hasta que admin asigne rol
                IsEmailConfirmed = false,
                CreatedAt = baseDate
            },
            
            // ESTUDIANTE 2
            new Usuario
            {
                Id = 7,
                Username = "estudiante2",
                Email = "estudiante2@fisei.uta.edu.ec",
                PasswordHash = "$2a$11$9f5rP2P5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5qP5q", // password: Estudiante123!
                FirstName = "Carmen",
                LastName = "Torres",
                Department = "Ingeniería Industrial",
                TipoUsuario = TipoUsuario.Usuario,
                IsActive = false, // Inicialmente inactivo hasta que admin asigne rol
                IsEmailConfirmed = false,
                CreatedAt = baseDate
            }
        );

        // SLA por defecto
        modelBuilder.Entity<SLA>().HasData(
            new SLA { Id = 1, Nombre = "Crítico", Descripcion = "SLA para incidentes críticos", Prioridad = PrioridadIncidente.Critica, Impacto = ImpactoIncidente.Critico, Urgencia = UrgenciaIncidente.Critica, TiempoRespuesta = 15, TiempoResolucion = 60, CreatedAt = DateTime.UtcNow },
            new SLA { Id = 2, Nombre = "Alto", Descripcion = "SLA para incidentes de alta prioridad", Prioridad = PrioridadIncidente.Alta, Impacto = ImpactoIncidente.Alto, Urgencia = UrgenciaIncidente.Alta, TiempoRespuesta = 30, TiempoResolucion = 120, CreatedAt = DateTime.UtcNow },
            new SLA { Id = 3, Nombre = "Medio", Descripcion = "SLA para incidentes de prioridad media", Prioridad = PrioridadIncidente.Media, Impacto = ImpactoIncidente.Medio, Urgencia = UrgenciaIncidente.Media, TiempoRespuesta = 60, TiempoResolucion = 480, CreatedAt = DateTime.UtcNow },
            new SLA { Id = 4, Nombre = "Bajo", Descripcion = "SLA para incidentes de baja prioridad", Prioridad = PrioridadIncidente.Baja, Impacto = ImpactoIncidente.Bajo, Urgencia = UrgenciaIncidente.Baja, TiempoRespuesta = 240, TiempoResolucion = 2880, CreatedAt = DateTime.UtcNow }
        );
    }
}