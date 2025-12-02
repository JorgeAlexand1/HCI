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
    public DbSet<HistorialEscalacion> HistorialEscalaciones { get; set; }
    public DbSet<ServicioDITIC> ServiciosDITIC { get; set; }
    public DbSet<EtiquetaConocimiento> EtiquetasConocimiento { get; set; }
    public DbSet<ArticuloEtiqueta> ArticulosEtiquetas { get; set; }
    public DbSet<VersionArticulo> VersionesArticulo { get; set; }
    public DbSet<ValidacionArticulo> ValidacionesArticulo { get; set; }
    public DbSet<Notificacion> Notificaciones { get; set; }
    public DbSet<PlantillaEncuesta> PlantillasEncuesta { get; set; }
    public DbSet<PreguntaEncuesta> PreguntasEncuesta { get; set; }
    public DbSet<Encuesta> Encuestas { get; set; }
    public DbSet<RespuestaEncuesta> RespuestasEncuesta { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

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

            entity.HasOne(e => e.ServicioDITIC)
                .WithMany(s => s.IncidentesRelacionados)
                .HasForeignKey(e => e.ServicioDITICId)
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

        // Configuración de HistorialEscalacion
        modelBuilder.Entity<HistorialEscalacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Razon).IsRequired().HasMaxLength(500);

            entity.HasOne(e => e.Incidente)
                .WithMany()
                .HasForeignKey(e => e.IncidenteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.TecnicoOrigen)
                .WithMany()
                .HasForeignKey(e => e.TecnicoOrigenId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.TecnicoDestino)
                .WithMany()
                .HasForeignKey(e => e.TecnicoDestinoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de ServicioDITIC
        modelBuilder.Entity<ServicioDITIC>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(500);
            entity.Property(e => e.DescripcionDetallada).HasMaxLength(2000);
            entity.Property(e => e.HorarioDisponibilidad).HasMaxLength(100);
            entity.Property(e => e.Requisitos).HasMaxLength(1000);
            entity.Property(e => e.Limitaciones).HasMaxLength(1000);
            entity.Property(e => e.DocumentacionURL).HasMaxLength(500);
            entity.Property(e => e.UnidadCosto).HasMaxLength(50);
            entity.Property(e => e.CostoEstimado).HasColumnType("decimal(18,2)");

            entity.HasIndex(e => e.Codigo).IsUnique();

            entity.HasOne(e => e.SLA)
                .WithMany()
                .HasForeignKey(e => e.SLAId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Categoria)
                .WithMany()
                .HasForeignKey(e => e.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.ResponsableTecnico)
                .WithMany()
                .HasForeignKey(e => e.ResponsableTecnicoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ResponsableNegocio)
                .WithMany()
                .HasForeignKey(e => e.ResponsableNegocioId)
                .OnDelete(DeleteBehavior.Restrict);
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

        // Catálogo de Servicios DITIC de FISEI
        modelBuilder.Entity<ServicioDITIC>().HasData(
            new ServicioDITIC
            {
                Id = 1,
                Codigo = "SRV-001",
                Nombre = "Acceso a Internet WiFi",
                Descripcion = "Conectividad WiFi en todas las instalaciones de FISEI",
                DescripcionDetallada = "Servicio de acceso inalámbrico a internet para estudiantes, docentes y personal administrativo en todo el campus de FISEI.",
                TipoServicio = TipoServicioDITIC.RedesConectividad,
                EsServicioEsencial = true,
                HorarioDisponibilidad = "24/7",
                PorcentajeDisponibilidad = 99.0,
                SLAId = 2,
                CategoriaId = 3,
                ResponsableTecnicoId = 2,
                Requisitos = "Credenciales institucionales UTA",
                EstaActivo = true,
                FechaInicioServicio = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            },
            new ServicioDITIC
            {
                Id = 2,
                Codigo = "SRV-002",
                Nombre = "Correo Institucional",
                Descripcion = "Servicio de correo electrónico @fisei.uta.edu.ec",
                DescripcionDetallada = "Cuentas de correo institucional con 50GB de almacenamiento, integración con Office 365 y servicios en la nube.",
                TipoServicio = TipoServicioDITIC.Comunicaciones,
                EsServicioEsencial = true,
                HorarioDisponibilidad = "24/7",
                PorcentajeDisponibilidad = 99.9,
                SLAId = 1,
                CategoriaId = 5,
                ResponsableTecnicoId = 2,
                Requisitos = "Ser estudiante, docente o administrativo activo",
                Limitaciones = "Cuota de 50GB, políticas anti-spam aplicables",
                EstaActivo = true,
                FechaInicioServicio = new DateTime(2019, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            },
            new ServicioDITIC
            {
                Id = 3,
                Codigo = "SRV-003",
                Nombre = "Laboratorios de Computación",
                Descripcion = "Acceso a laboratorios con equipos especializados",
                DescripcionDetallada = "5 laboratorios equipados con 150+ computadoras con software especializado para ingeniería (MATLAB, AutoCAD, IDEs, etc.)",
                TipoServicio = TipoServicioDITIC.Hardware,
                EsServicioEsencial = true,
                HorarioDisponibilidad = "Lunes a Viernes 7:00-21:00, Sábados 8:00-13:00",
                PorcentajeDisponibilidad = 98.0,
                SLAId = 3,
                CategoriaId = 1,
                ResponsableTecnicoId = 3,
                Requisitos = "Horario asignado por materia o reserva previa",
                Limitaciones = "Capacidad limitada, requiere reserva para uso fuera de horario de clase",
                EstaActivo = true,
                FechaInicioServicio = new DateTime(2018, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            },
            new ServicioDITIC
            {
                Id = 4,
                Codigo = "SRV-004",
                Nombre = "Soporte Técnico Help Desk",
                Descripcion = "Mesa de ayuda para problemas técnicos",
                DescripcionDetallada = "Atención personalizada para resolver problemas de hardware, software, acceso a sistemas y otros inconvenientes técnicos.",
                TipoServicio = TipoServicioDITIC.SoporteUsuario,
                EsServicioEsencial = true,
                HorarioDisponibilidad = "Lunes a Viernes 8:00-17:00",
                PorcentajeDisponibilidad = 100.0,
                SLAId = 2,
                ResponsableTecnicoId = 2,
                Requisitos = "Ninguno - servicio abierto a toda la comunidad FISEI",
                EstaActivo = true,
                FechaInicioServicio = new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            },
            new ServicioDITIC
            {
                Id = 5,
                Codigo = "SRV-005",
                Nombre = "Sistema de Gestión Académica",
                Descripcion = "Plataforma para registro de calificaciones y gestión académica",
                DescripcionDetallada = "Sistema integral para gestión de matrículas, calificaciones, horarios, y seguimiento académico de estudiantes.",
                TipoServicio = TipoServicioDITIC.Aplicaciones,
                EsServicioEsencial = true,
                HorarioDisponibilidad = "24/7",
                PorcentajeDisponibilidad = 99.5,
                SLAId = 1,
                CategoriaId = 2,
                ResponsableTecnicoId = 2,
                Requisitos = "Credenciales institucionales",
                Limitaciones = "Mantenimiento programado los domingos 2:00-4:00 AM",
                EstaActivo = true,
                FechaInicioServicio = new DateTime(2017, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            },
            new ServicioDITIC
            {
                Id = 6,
                Codigo = "SRV-006",
                Nombre = "Aula Virtual (Moodle)",
                Descripcion = "Plataforma de aprendizaje en línea",
                DescripcionDetallada = "Sistema LMS para gestión de cursos virtuales, tareas, foros, evaluaciones y recursos educativos digitales.",
                TipoServicio = TipoServicioDITIC.Aplicaciones,
                EsServicioEsencial = true,
                HorarioDisponibilidad = "24/7",
                PorcentajeDisponibilidad = 99.0,
                SLAId = 2,
                CategoriaId = 2,
                ResponsableTecnicoId = 2,
                Requisitos = "Estar matriculado o ser docente activo",
                DocumentacionURL = "https://aulavirtual.fisei.uta.edu.ec/ayuda",
                EstaActivo = true,
                FechaInicioServicio = new DateTime(2016, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            },
            new ServicioDITIC
            {
                Id = 7,
                Codigo = "SRV-007",
                Nombre = "Impresión y Fotocopiado",
                Descripcion = "Servicio de impresión para trabajos académicos",
                DescripcionDetallada = "Red de impresoras multifunción disponibles para estudiantes y docentes. Incluye impresión, escaneo y fotocopiado.",
                TipoServicio = TipoServicioDITIC.Hardware,
                EsServicioEsencial = false,
                HorarioDisponibilidad = "Lunes a Viernes 8:00-18:00",
                PorcentajeDisponibilidad = 95.0,
                SLAId = 3,
                CategoriaId = 1,
                ResponsableTecnicoId = 4,
                Requisitos = "Credenciales institucionales, cuota de impresión prepagada",
                Limitaciones = "Máximo 50 páginas por día por usuario",
                CostoEstimado = 0.05m,
                UnidadCosto = "por página B/N",
                EstaActivo = true,
                FechaInicioServicio = new DateTime(2019, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            },
            new ServicioDITIC
            {
                Id = 8,
                Codigo = "SRV-008",
                Nombre = "VPN Institucional",
                Descripcion = "Acceso remoto seguro a recursos internos",
                DescripcionDetallada = "Red privada virtual para acceder a recursos institucionales (bibliotecas digitales, sistemas internos) desde fuera del campus.",
                TipoServicio = TipoServicioDITIC.Seguridad,
                EsServicioEsencial = false,
                HorarioDisponibilidad = "24/7",
                PorcentajeDisponibilidad = 98.0,
                SLAId = 3,
                CategoriaId = 3,
                ResponsableTecnicoId = 2,
                Requisitos = "Solicitud previa, credenciales institucionales",
                Limitaciones = "Acceso limitado a docentes e investigadores",
                DocumentacionURL = "https://ditic.uta.edu.ec/vpn",
                EstaActivo = true,
                FechaInicioServicio = new DateTime(2020, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            },
            new ServicioDITIC
            {
                Id = 9,
                Codigo = "SRV-009",
                Nombre = "Repositorio Digital Institucional",
                Descripcion = "Almacenamiento de trabajos de titulación y publicaciones",
                DescripcionDetallada = "Plataforma para almacenar, preservar y difundir la producción académica de FISEI (tesis, papers, proyectos).",
                TipoServicio = TipoServicioDITIC.GestionDatos,
                EsServicioEsencial = false,
                HorarioDisponibilidad = "24/7",
                PorcentajeDisponibilidad = 99.5,
                SLAId = 4,
                CategoriaId = 2,
                ResponsableTecnicoId = 2,
                Requisitos = "Aprobación del tutor y defensa del trabajo",
                DocumentacionURL = "https://repositorio.uta.edu.ec",
                EstaActivo = true,
                FechaInicioServicio = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            },
            new ServicioDITIC
            {
                Id = 10,
                Codigo = "SRV-010",
                Nombre = "Licencias de Software Académico",
                Descripcion = "Acceso a software especializado con licencia institucional",
                DescripcionDetallada = "Provisión de licencias para software de ingeniería: MATLAB, Autodesk, Microsoft Office 365, JetBrains, Visual Studio, etc.",
                TipoServicio = TipoServicioDITIC.Aplicaciones,
                EsServicioEsencial = true,
                HorarioDisponibilidad = "24/7 (activación)",
                PorcentajeDisponibilidad = 99.0,
                SLAId = 3,
                CategoriaId = 2,
                ResponsableTecnicoId = 2,
                Requisitos = "Ser estudiante o docente activo, solicitud por correo institucional",
                Limitaciones = "Licencias limitadas según convenios vigentes",
                EstaActivo = true,
                FechaInicioServicio = new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            }
        );

        // Configuración de EtiquetaConocimiento
        modelBuilder.Entity<EtiquetaConocimiento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Color).IsRequired().HasMaxLength(7).HasDefaultValue("#007bff");
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        // Configuración de ArticuloEtiqueta (tabla intermedia)
        modelBuilder.Entity<ArticuloEtiqueta>(entity =>
        {
            entity.HasKey(ae => new { ae.ArticuloConocimientoId, ae.EtiquetaId });
            
            entity.HasOne(ae => ae.ArticuloConocimiento)
                .WithMany(a => a.ArticulosEtiquetas)
                .HasForeignKey(ae => ae.ArticuloConocimientoId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(ae => ae.Etiqueta)
                .WithMany(e => e.ArticulosEtiquetas)
                .HasForeignKey(ae => ae.EtiquetaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de VersionArticulo
        modelBuilder.Entity<VersionArticulo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Contenido).IsRequired();
            entity.Property(e => e.CambiosRealizados).HasMaxLength(500);
            
            entity.HasOne(e => e.ArticuloConocimiento)
                .WithMany(a => a.Versiones)
                .HasForeignKey(e => e.ArticuloConocimientoId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.ModificadoPor)
                .WithMany()
                .HasForeignKey(e => e.ModificadoPorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de ValidacionArticulo
        modelBuilder.Entity<ValidacionArticulo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ComentariosValidador).HasMaxLength(1000);
            
            entity.HasOne(e => e.ArticuloConocimiento)
                .WithMany(a => a.Validaciones)
                .HasForeignKey(e => e.ArticuloConocimientoId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.SolicitadoPor)
                .WithMany()
                .HasForeignKey(e => e.SolicitadoPorId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Validador)
                .WithMany()
                .HasForeignKey(e => e.ValidadorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de ComentarioArticulo con auto-referencia
        modelBuilder.Entity<ComentarioArticulo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Contenido).IsRequired().HasMaxLength(1000);
            
            entity.HasOne(e => e.ArticuloConocimiento)
                .WithMany(a => a.Comentarios)
                .HasForeignKey(e => e.ArticuloConocimientoId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.ComentarioPadre)
                .WithMany(c => c.Respuestas)
                .HasForeignKey(e => e.ComentarioPadreId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de VotacionArticulo
        modelBuilder.Entity<VotacionArticulo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Comentario).HasMaxLength(500);
            
            entity.HasOne(e => e.ArticuloConocimiento)
                .WithMany(a => a.Votaciones)
                .HasForeignKey(e => e.ArticuloConocimientoId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Un usuario solo puede votar una vez por artículo
            entity.HasIndex(e => new { e.ArticuloConocimientoId, e.UsuarioId }).IsUnique();
        });

        // Seeds de Etiquetas
        modelBuilder.Entity<EtiquetaConocimiento>().HasData(
            new EtiquetaConocimiento { Id = 1, Nombre = "Windows", Color = "#0078D4", CreatedAt = DateTime.UtcNow },
            new EtiquetaConocimiento { Id = 2, Nombre = "Linux", Color = "#FCC624", CreatedAt = DateTime.UtcNow },
            new EtiquetaConocimiento { Id = 3, Nombre = "Redes", Color = "#00A4EF", CreatedAt = DateTime.UtcNow },
            new EtiquetaConocimiento { Id = 4, Nombre = "Hardware", Color = "#FF5722", CreatedAt = DateTime.UtcNow },
            new EtiquetaConocimiento { Id = 5, Nombre = "Software", Color = "#4CAF50", CreatedAt = DateTime.UtcNow },
            new EtiquetaConocimiento { Id = 6, Nombre = "Seguridad", Color = "#F44336", CreatedAt = DateTime.UtcNow },
            new EtiquetaConocimiento { Id = 7, Nombre = "Base de Datos", Color = "#9C27B0", CreatedAt = DateTime.UtcNow },
            new EtiquetaConocimiento { Id = 8, Nombre = "Impresión", Color = "#607D8B", CreatedAt = DateTime.UtcNow },
            new EtiquetaConocimiento { Id = 9, Nombre = "Email", Color = "#FF9800", CreatedAt = DateTime.UtcNow },
            new EtiquetaConocimiento { Id = 10, Nombre = "VPN", Color = "#3F51B5", CreatedAt = DateTime.UtcNow }
        );

        // Configuración de Notificacion
        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Mensaje).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.GrupoNotificacion).HasMaxLength(100);

            // Relación con Usuario
            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación con Incidente (opcional)
            entity.HasOne(e => e.Incidente)
                .WithMany()
                .HasForeignKey(e => e.IncidenteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices para optimizar consultas
            entity.HasIndex(e => e.UsuarioId);
            entity.HasIndex(e => e.Leida);
            entity.HasIndex(e => new { e.UsuarioId, e.Leida });
            entity.HasIndex(e => e.CreatedAt);
        });

        // Configuración de PlantillaEncuesta
        modelBuilder.Entity<PlantillaEncuesta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descripcion).HasMaxLength(500);

            entity.HasIndex(e => e.Tipo);
            entity.HasIndex(e => e.EsActiva);
        });

        // Configuración de PreguntaEncuesta
        modelBuilder.Entity<PreguntaEncuesta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TextoPregunta).IsRequired().HasMaxLength(500);
            entity.Property(e => e.OpcionesJson).HasMaxLength(2000);
            entity.Property(e => e.EtiquetaMinimo).HasMaxLength(100);
            entity.Property(e => e.EtiquetaMaximo).HasMaxLength(100);

            entity.HasOne(e => e.PlantillaEncuesta)
                .WithMany(p => p.Preguntas)
                .HasForeignKey(e => e.PlantillaEncuestaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.PlantillaEncuestaId);
            entity.HasIndex(e => e.Orden);
        });

        // Configuración de Encuesta
        modelBuilder.Entity<Encuesta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ComentariosGenerales).HasMaxLength(1000);

            entity.HasOne(e => e.Incidente)
                .WithMany()
                .HasForeignKey(e => e.IncidenteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PlantillaEncuesta)
                .WithMany(p => p.Encuestas)
                .HasForeignKey(e => e.PlantillaEncuestaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.IncidenteId);
            entity.HasIndex(e => e.UsuarioId);
            entity.HasIndex(e => e.EsRespondida);
            entity.HasIndex(e => e.EsVencida);
            entity.HasIndex(e => e.FechaEnvio);
            entity.HasIndex(e => e.FechaVencimiento);
        });

        // Configuración de RespuestaEncuesta
        modelBuilder.Entity<RespuestaEncuesta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RespuestaTexto).HasMaxLength(2000);
            entity.Property(e => e.RespuestasSeleccionJson).HasMaxLength(2000);

            entity.HasOne(e => e.Encuesta)
                .WithMany(enc => enc.Respuestas)
                .HasForeignKey(e => e.EncuestaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.PreguntaEncuesta)
                .WithMany(p => p.Respuestas)
                .HasForeignKey(e => e.PreguntaEncuestaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.EncuestaId);
            entity.HasIndex(e => e.PreguntaEncuestaId);
        });

        // Seed de Plantilla de Encuesta por Defecto
        var plantillaDefecto = new PlantillaEncuesta
        {
            Id = 1,
            Nombre = "Encuesta de Satisfacción Post-Cierre",
            Descripcion = "Encuesta estándar enviada automáticamente al cerrar un incidente",
            Tipo = TipoEncuesta.SatisfaccionGeneral,
            EsActiva = true,
            MostrarAutomaticamente = true,
            DiasVigencia = 7,
            CreatedAt = DateTime.UtcNow
        };
        modelBuilder.Entity<PlantillaEncuesta>().HasData(plantillaDefecto);

        // Seed de Preguntas por Defecto
        modelBuilder.Entity<PreguntaEncuesta>().HasData(
            new PreguntaEncuesta
            {
                Id = 1,
                PlantillaEncuestaId = 1,
                TextoPregunta = "¿Qué tan satisfecho está con la resolución del incidente?",
                Tipo = TipoPregunta.Calificacion,
                Orden = 1,
                EsObligatoria = true,
                ValorMinimo = 1,
                ValorMaximo = 5,
                EtiquetaMinimo = "Muy insatisfecho",
                EtiquetaMaximo = "Muy satisfecho",
                CreatedAt = DateTime.UtcNow
            },
            new PreguntaEncuesta
            {
                Id = 2,
                PlantillaEncuestaId = 1,
                TextoPregunta = "¿El tiempo de respuesta fue adecuado?",
                Tipo = TipoPregunta.SiNo,
                Orden = 2,
                EsObligatoria = true,
                CreatedAt = DateTime.UtcNow
            },
            new PreguntaEncuesta
            {
                Id = 3,
                PlantillaEncuestaId = 1,
                TextoPregunta = "¿Qué tan probable es que recomiende nuestro servicio de soporte?",
                Tipo = TipoPregunta.EscalaLineal,
                Orden = 3,
                EsObligatoria = true,
                ValorMinimo = 0,
                ValorMaximo = 10,
                EtiquetaMinimo = "Nada probable",
                EtiquetaMaximo = "Muy probable",
                CreatedAt = DateTime.UtcNow
            },
            new PreguntaEncuesta
            {
                Id = 4,
                PlantillaEncuestaId = 1,
                TextoPregunta = "¿El técnico fue profesional y cortés?",
                Tipo = TipoPregunta.Calificacion,
                Orden = 4,
                EsObligatoria = true,
                ValorMinimo = 1,
                ValorMaximo = 5,
                EtiquetaMinimo = "Muy malo",
                EtiquetaMaximo = "Excelente",
                CreatedAt = DateTime.UtcNow
            },
            new PreguntaEncuesta
            {
                Id = 5,
                PlantillaEncuestaId = 1,
                TextoPregunta = "Comentarios adicionales o sugerencias",
                Tipo = TipoPregunta.TextoLargo,
                Orden = 5,
                EsObligatoria = false,
                CreatedAt = DateTime.UtcNow
            }
        );

        // Configuración de AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // String lengths
            entity.Property(e => e.UsuarioNombre).HasMaxLength(150);
            entity.Property(e => e.DireccionIP).HasMaxLength(45); // IPv6
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.EntidadDescripcion).HasMaxLength(200);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(500);
            entity.Property(e => e.MensajeError).HasMaxLength(1000);
            entity.Property(e => e.FiltrosAplicados).HasMaxLength(500);
            entity.Property(e => e.Modulo).HasMaxLength(50);
            entity.Property(e => e.Endpoint).HasMaxLength(200);
            
            // JSON columns (EF Core 8 stores as text)
            entity.Property(e => e.ValoresAnteriores).HasMaxLength(4000);
            entity.Property(e => e.ValoresNuevos).HasMaxLength(4000);
            entity.Property(e => e.MetadataJson).HasMaxLength(2000);
            
            // Relationship with Usuario (optional - user might be deleted)
            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Indexes for performance
            entity.HasIndex(e => e.UsuarioId);
            entity.HasIndex(e => e.TipoAccion);
            entity.HasIndex(e => e.TipoEntidad);
            entity.HasIndex(e => e.FechaHora);
            entity.HasIndex(e => new { e.TipoEntidad, e.EntidadId });
            entity.HasIndex(e => e.NivelSeveridad);
            entity.HasIndex(e => e.EsExitoso);
            entity.HasIndex(e => e.Modulo);
        });
    }
}
