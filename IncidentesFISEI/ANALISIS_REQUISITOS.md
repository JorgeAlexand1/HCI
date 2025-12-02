# 📋 Análisis de Cumplimiento de Requisitos - IncidentesFISEI

**Fecha de Análisis:** 2 de Diciembre, 2025  
**Proyecto:** Sistema de Gestión de Incidentes FISEI  
**Arquitectura:** Clean Architecture (Onion)  
**Stack Tecnológico:** .NET 9, Blazor WebAssembly, SQL Server

---

## ✅ RESUMEN EJECUTIVO

### Estado General del Proyecto: **PARCIALMENTE IMPLEMENTADO** ⚠️

**Puntuación:** 6.5/10

El proyecto tiene una **excelente base arquitectónica** y estructura de dominio, pero le faltan **implementaciones críticas** de requisitos funcionales específicos. La arquitectura sigue patrones sólidos de Clean Architecture y las entidades del dominio están bien diseñadas según ITIL v3.

---

## 📊 ANÁLISIS DETALLADO POR REQUISITO

### ✅ **1. ARQUITECTURA Y TECNOLOGÍAS**

**Estado: CUMPLE COMPLETAMENTE** ✔️

#### ✅ Arquitectura Onion/Clean Architecture
- **Implementado:** Sí
- **Calidad:** Excelente
- **Evidencia:**
  ```
  ├── Domain (Núcleo - Entidades, Interfaces)
  ├── Application (Casos de Uso, DTOs, Interfaces de Servicios)
  ├── Infrastructure (Repositorios, DbContext, Servicios externos)
  └── Api (Controllers, Configuración)
  ```
- **Observaciones:** La separación de responsabilidades es clara y correcta.

#### ✅ Tecnologías Requeridas
- **.NET:** ✔️ .NET 9 (última versión)
- **Blazor:** ✔️ Blazor WebAssembly configurado
- **WebAssembly:** ✔️ Implementado
- **MSSQL:** ✔️ SQL Server con EF Core 9

---

### ⚠️ **2. PUNTO ÚNICO DE CONTACTO (SPOC)**

**Estado: NO IMPLEMENTADO** ❌

#### Requisitos No Cumplidos:
1. **Encargado de asignación (SPOC):** 
   - ❌ No existe rol específico de "Coordinador" o "Supervisor de Asignaciones"
   - ❌ No hay lógica para designar un SPOC
   - ✅ Existe rol `Supervisor` en el enum `TipoUsuario`

2. **Distribución Equitativa de Tickets:**
   - ❌ No existe algoritmo de balanceo de carga
   - ❌ No hay métricas de tickets por técnico
   - ❌ No hay lógica automática de asignación equitativa

3. **Toma de Tickets por Otros Técnicos:**
   - ⚠️ Existe `AsignarIncidenteAsync()` pero sin validación de disponibilidad del SPOC
   - ❌ No hay estado de "disponible" para SPOC
   - ❌ No hay lógica de permisos condicionales

#### 🔧 Recomendaciones de Implementación:

**Backend (Servicios a Crear):**

```csharp
// 1. Agregar al dominio
public class Usuario 
{
    // ... campos existentes
    public bool IsSPOC { get; set; } = false;
    public bool IsAvailable { get; set; } = true; // Para saber si SPOC está disponible
    public int CargaTrabajoActual { get; set; } = 0; // Tickets asignados actualmente
}

// 2. Crear servicio de asignación inteligente
public interface IAsignacionService
{
    Task<Usuario?> GetSPOCDisponibleAsync();
    Task<Usuario?> GetTecnicoConMenorCargaAsync(string? especialidad = null);
    Task<bool> AsignarIncidenteEquitativamenteAsync(int incidenteId);
    Task<Dictionary<int, int>> GetCargaTrabajoTecnicosAsync();
}

// 3. Implementar lógica de distribución equitativa
public class AsignacionService : IAsignacionService
{
    public async Task<Usuario?> GetTecnicoConMenorCargaAsync(string? especialidad = null)
    {
        var tecnicos = await _usuarioRepository.GetTecnicosAsync();
        
        if (!string.IsNullOrEmpty(especialidad))
            tecnicos = tecnicos.Where(t => t.Especialidad == especialidad);
            
        // Obtener carga actual de cada técnico
        var tecnicoConMenorCarga = tecnicos
            .OrderBy(t => t.CargaTrabajoActual)
            .FirstOrDefault();
            
        return tecnicoConMenorCarga;
    }
}
```

**Estado Actual:** Tienes la estructura pero falta la lógica.

---

### ❌ **3. NIVELES DEL PERSONAL DE TI (Técnicos → Expertos → Proveedores)**

**Estado: NO IMPLEMENTADO** ❌

#### Análisis:

**Lo que EXISTE:**
- ✅ Enum `TipoUsuario` con niveles: Usuario, Tecnico, Supervisor, Administrador
- ✅ Campo `Especialidad` en Usuario
- ✅ Campo `AñosExperiencia` en Usuario
- ✅ Estado `Escalado` en `EstadoIncidente`
- ✅ Entidad `EscalacionSLA` para registrar escalaciones

**Lo que FALTA:**
- ❌ **Niveles jerárquicos de soporte** (L1, L2, L3)
- ❌ **Lógica de escalación automática** por nivel
- ❌ **Reglas de negocio** para pasar al siguiente nivel
- ❌ **Tiempo límite** antes de escalar
- ❌ **Integración con proveedores externos**

#### 🔧 Recomendaciones de Implementación:

```csharp
// 1. Agregar al dominio
public enum NivelSoporte
{
    L1_Tecnico = 1,      // Soporte básico
    L2_Experto = 2,      // Soporte avanzado
    L3_Especialista = 3, // Problemas complejos
    L4_Proveedor = 4     // Escalación a proveedores externos
}

public class Usuario 
{
    // ... campos existentes
    public NivelSoporte NivelSoporte { get; set; } = NivelSoporte.L1_Tecnico;
}

public class Incidente
{
    // ... campos existentes
    public NivelSoporte NivelActual { get; set; } = NivelSoporte.L1_Tecnico;
    public int NumeroEscalaciones { get; set; } = 0;
}

// 2. Crear servicio de escalación
public interface IEscalacionService
{
    Task<bool> EscalarIncidenteAsync(int incidenteId, string motivo);
    Task<bool> VerificarNecesidadEscalacionAsync(int incidenteId);
    Task<Usuario?> ObtenerTecnicoNivelSuperiorAsync(NivelSoporte nivelActual, string? especialidad);
}

// 3. Implementar lógica automática
public class EscalacionService : IEscalacionService
{
    public async Task<bool> VerificarNecesidadEscalacionAsync(int incidenteId)
    {
        var incidente = await _incidenteRepository.GetByIdAsync(incidenteId);
        
        // Escalar si lleva más de X tiempo sin resolver en el nivel actual
        if (incidente.FechaAsignacion.HasValue)
        {
            var tiempoEnNivel = DateTime.UtcNow - incidente.FechaAsignacion.Value;
            var limiteNivel = ObtenerLimiteTiempoPorNivel(incidente.NivelActual);
            
            if (tiempoEnNivel > limiteNivel)
            {
                await EscalarIncidenteAsync(incidenteId, "Tiempo límite excedido en nivel actual");
                return true;
            }
        }
        
        return false;
    }
}

// 4. Job en segundo plano para verificar escalaciones
public class EscalacionBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var incidentesActivos = await _incidenteRepository
                .GetIncidentesByEstadoAsync(EstadoIncidente.EnProgreso);
            
            foreach (var incidente in incidentesActivos)
            {
                await _escalacionService.VerificarNecesidadEscalacionAsync(incidente.Id);
            }
            
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

---

### ✅ **4. BASE DE DATOS DE CONOCIMIENTO (BDC)**

**Estado: IMPLEMENTADO PARCIALMENTE** ⚠️

#### ✅ Lo que EXISTE:
- ✅ Entidad `ArticuloConocimiento` completa
- ✅ Sistema de estados (Borrador, Revisión, Publicado, Archivado)
- ✅ Relación con Incidentes
- ✅ Sistema de votación (`VotacionArticulo`)
- ✅ Comentarios en artículos
- ✅ Tags y categorización
- ✅ Visualizaciones

#### ⚠️ Lo que FALTA:
- ❌ **Creación automática de artículos** desde incidentes resueltos
- ❌ **Sugerencias de artículos** basadas en descripción del incidente
- ❌ **Motor de búsqueda avanzado** (full-text search)
- ❌ **Versionado de artículos** (aunque está en el README, no está implementado)

#### 🔧 Recomendaciones:

```csharp
// Servicio para vincular incidentes con conocimiento
public interface IBaseConocimientoService
{
    Task<List<ArticuloConocimiento>> SugerirArticulosAsync(string descripcion);
    Task<ArticuloConocimiento> CrearArticuloDesdeIncidenteAsync(int incidenteId);
    Task<bool> VincularArticuloConIncidenteAsync(int incidenteId, int articuloId);
}

// Implementación básica de búsqueda
public async Task<List<ArticuloConocimiento>> SugerirArticulosAsync(string descripcion)
{
    // Búsqueda por palabras clave en título y contenido
    return await _articuloRepository
        .BuscarArticulosAsync(descripcion)
        .OrderByDescending(a => a.VotosPositivos)
        .Take(5)
        .ToListAsync();
}
```

---

### ❌ **5. PROBLEMAS RECURSIVOS/REPETITIVOS ESCALAN**

**Estado: NO IMPLEMENTADO** ❌

#### Análisis:
- ✅ Existe `IncidenteRelacionado` para vincular incidentes
- ✅ Existe enum `TipoRelacion.Duplicado`
- ❌ **No hay lógica para detectar patrones repetitivos**
- ❌ **No hay escalación automática por recurrencia**
- ❌ **No hay análisis de causa raíz común**

#### 🔧 Recomendaciones:

```csharp
// 1. Servicio de detección de patrones
public interface IAnalisisPatronesService
{
    Task<List<Incidente>> DetectarIncidentesRecurrentesAsync(int dias = 30);
    Task<bool> EsIncidenteRecurrenteAsync(int incidenteId);
    Task EscalarPorRecurrenciaAsync(int incidenteId);
}

// 2. Implementación
public async Task<bool> EsIncidenteRecurrenteAsync(int incidenteId)
{
    var incidente = await _incidenteRepository.GetByIdAsync(incidenteId);
    
    // Buscar incidentes similares en los últimos 30 días
    var similares = await _incidenteRepository.FindAsync(i => 
        i.CategoriaId == incidente.CategoriaId &&
        i.Titulo.Contains(incidente.Titulo) &&
        i.FechaReporte > DateTime.UtcNow.AddDays(-30) &&
        i.Id != incidenteId
    );
    
    if (similares.Count() >= 3) // Si hay 3+ incidentes similares
    {
        await EscalarPorRecurrenciaAsync(incidenteId);
        return true;
    }
    
    return false;
}

// 3. Agregar al modelo
public class Incidente
{
    // ... campos existentes
    public bool EsRecurrente { get; set; } = false;
    public int? IncidentePadreId { get; set; } // Para agrupar recurrencias
}
```

---

### ❌ **6. FUNCIONALIDAD EN SEGUNDO PLANO**

**Estado: NO IMPLEMENTADO** ❌

#### Lo que FALTA:
- ❌ **Background Services** para tareas automáticas
- ❌ Verificación de SLA
- ❌ Escalación automática
- ❌ Notificaciones programadas
- ❌ Limpieza de datos
- ❌ Generación de reportes

#### 🔧 Recomendaciones:

```csharp
// 1. En Program.cs agregar
builder.Services.AddHostedService<SLAMonitoringService>();
builder.Services.AddHostedService<EscalacionAutomaticaService>();
builder.Services.AddHostedService<NotificacionesService>();

// 2. Implementar servicio de monitoreo SLA
public class SLAMonitoringService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await VerificarIncidentesVencidosAsync();
            await VerificarIncidentesPorVencerAsync();
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
    
    private async Task VerificarIncidentesVencidosAsync()
    {
        var vencidos = await _incidenteRepository.GetIncidentesVencidosAsync();
        
        foreach (var incidente in vencidos)
        {
            // Crear escalación SLA
            var escalacion = new EscalacionSLA
            {
                IncidenteId = incidente.Id,
                FechaEscalacion = DateTime.UtcNow,
                Motivo = "SLA vencido",
                FueNotificado = false
            };
            
            await _escalacionRepository.AddAsync(escalacion);
            
            // Enviar notificación
            await _notificacionService.NotificarSLAVencidoAsync(incidente);
        }
    }
}
```

---

### ❌ **7. NOTIFICACIONES PUSH (como WhatsApp)**

**Estado: NO IMPLEMENTADO** ❌

#### Lo que EXISTE:
- ⚠️ Solo se mencionan `EmailSettings` en `appsettings.json`
- ❌ No hay servicio de notificaciones implementado
- ❌ No hay integración con WhatsApp/Telegram
- ❌ No hay notificaciones en tiempo real (SignalR)

#### 🔧 Recomendaciones:

```csharp
// 1. Instalar paquetes NuGet necesarios
// - Microsoft.AspNetCore.SignalR (para tiempo real)
// - Twilio (para WhatsApp/SMS)

// 2. Crear servicio de notificaciones
public interface INotificacionService
{
    Task NotificarNuevoIncidenteAsync(int incidenteId);
    Task NotificarAsignacionAsync(int incidenteId, int tecnicoId);
    Task NotificarActualizacionAsync(int incidenteId, string mensaje);
    Task NotificarSLAVencidoAsync(Incidente incidente);
}

// 3. Implementar con múltiples canales
public class NotificacionService : INotificacionService
{
    private readonly IEmailService _emailService;
    private readonly IWhatsAppService _whatsAppService;
    private readonly IHubContext<NotificacionesHub> _hubContext;
    
    public async Task NotificarAsignacionAsync(int incidenteId, int tecnicoId)
    {
        var tecnico = await _usuarioRepository.GetByIdAsync(tecnicoId);
        
        // Email
        await _emailService.EnviarEmailAsync(tecnico.Email, 
            "Nuevo incidente asignado", 
            $"Se te ha asignado el incidente #{incidenteId}");
        
        // WhatsApp (si el técnico tiene phone)
        if (!string.IsNullOrEmpty(tecnico.Phone))
        {
            await _whatsAppService.EnviarMensajeAsync(tecnico.Phone,
                $"Nuevo incidente asignado: #{incidenteId}");
        }
        
        // Notificación en tiempo real (SignalR)
        await _hubContext.Clients.User(tecnicoId.ToString())
            .SendAsync("NuevoIncidenteAsignado", incidenteId);
    }
}

// 4. Configurar SignalR Hub
public class NotificacionesHub : Hub
{
    public async Task UnirseASalaUsuario(int usuarioId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Usuario_{usuarioId}");
    }
}

// 5. En Program.cs
builder.Services.AddSignalR();
app.MapHub<NotificacionesHub>("/notificacionesHub");
```

---

### ✅ **8. IMPLEMENTACIÓN DE ROLES**

**Estado: IMPLEMENTADO** ✔️

#### ✅ Lo que EXISTE:
- ✅ Enum `TipoUsuario`: Usuario, Tecnico, Supervisor, Administrador
- ✅ Enum `RolUsuario` adicional con más granularidad
- ✅ Autenticación JWT implementada
- ✅ Campo `TipoUsuario` en entidad `Usuario`
- ✅ Roles en JWT Claims

#### ⚠️ Mejoras Sugeridas:
```csharp
// Agregar autorización basada en políticas
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => 
        policy.RequireRole("Administrador"));
        
    options.AddPolicy("RequireTecnico", policy => 
        policy.RequireRole("Tecnico", "Supervisor", "Administrador"));
        
    options.AddPolicy("CanAsignarIncidentes", policy =>
        policy.RequireRole("Supervisor", "Administrador"));
});

// En controllers
[Authorize(Policy = "CanAsignarIncidentes")]
[HttpPost("asignar")]
public async Task<IActionResult> AsignarIncidente(...)
```

---

### ❌ **9. CATÁLOGO DE SERVICIOS (DITIC)**

**Estado: NO IMPLEMENTADO** ❌

#### Lo que FALTA:
- ❌ Entidad `ServicioDITIC`
- ❌ Responsables por servicio
- ❌ SLA específico por servicio
- ❌ Áreas/Tipos de usuario
- ❌ Relación Servicio-Categoría

#### 🔧 Recomendaciones:

```csharp
// 1. Crear nuevas entidades en Domain
public class ServicioDITIC : BaseEntity
{
    public string Codigo { get; set; } = string.Empty; // DITIC-001
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Icono { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Responsables
    public int ResponsablePrincipalId { get; set; }
    public Usuario ResponsablePrincipal { get; set; } = null!;
    public int? ResponsableBackupId { get; set; }
    public Usuario? ResponsableBackup { get; set; }
    
    // SLA específico
    public int SLAId { get; set; }
    public SLA SLA { get; set; } = null!;
    
    // Áreas/Audiencia
    public List<string> AreasDestino { get; set; } = new(); // JSON: ["Estudiantes", "Docentes"]
    public List<TipoUsuario> TiposUsuarioPermitidos { get; set; } = new();
    
    // Navegación
    public ICollection<CategoriaIncidente> Categorias { get; set; } = new List<CategoriaIncidente>();
}

// 2. Actualizar CategoriaIncidente
public class CategoriaIncidente
{
    // ... campos existentes
    public int? ServicioDITICId { get; set; }
    public ServicioDITIC? ServicioDITIC { get; set; }
}

// 3. Seeds de datos ejemplo
public static class ServicioDITICSeed
{
    public static List<ServicioDITIC> GetServicios()
    {
        return new List<ServicioDITIC>
        {
            new ServicioDITIC
            {
                Id = 1,
                Codigo = "DITIC-001",
                Nombre = "Soporte Técnico Laboratorios",
                Descripcion = "Atención de incidentes en laboratorios de computación",
                AreasDestino = new List<string> { "Estudiantes", "Docentes" },
                // SLA: Respuesta 30min, Resolución 4h
            },
            new ServicioDITIC
            {
                Id = 2,
                Codigo = "DITIC-002",
                Nombre = "Gestión de Cuentas y Accesos",
                Descripcion = "Creación, modificación de cuentas institucionales",
                AreasDestino = new List<string> { "Todos" },
                // SLA: Respuesta 1h, Resolución 24h
            }
        };
    }
}
```

---

### ⚠️ **10. CUMPLIMIENTO ITIL v4**

**Estado: IMPLEMENTADO PARCIALMENTE** ⚠️

#### ✅ Aspectos Implementados:
- ✅ **Gestión de Incidentes**: Estructura básica correcta
- ✅ **Estados ITIL**: Abierto, En Progreso, En Espera, Resuelto, Cerrado
- ✅ **Prioridad/Impacto/Urgencia**: Matriz correcta
- ✅ **SLA**: Entidad y escalación
- ✅ **Base de Conocimiento**: Implementada
- ✅ **Registro de Tiempo**: Para métricas

#### ❌ Aspectos Faltantes ITIL:
- ❌ **Gestión de Problemas**: No implementada (cause raíz de incidentes recurrentes)
- ❌ **Gestión de Cambios**: No implementada
- ❌ **CMDB**: No hay gestión de activos/configuración
- ❌ **Métricas ITIL**:
  - ❌ MTTR (Mean Time To Resolve)
  - ❌ MTBF (Mean Time Between Failures)
  - ❌ First Call Resolution Rate
  - ❌ SLA Compliance %

#### 🔧 Recomendaciones:

```csharp
// 1. Agregar Gestión de Problemas
public class Problema : BaseEntity
{
    public string NumeroproBlema { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string CausaRaiz { get; set; } = string.Empty;
    public string Solucion { get; set; } = string.Empty;
    public EstadoProblema Estado { get; set; }
    
    // Relación con incidentes que causaron el problema
    public ICollection<Incidente> IncidentesRelacionados { get; set; } = new List<Incidente>();
}

// 2. Servicio de métricas ITIL
public interface IMetricasITILService
{
    Task<TimeSpan> CalcularMTTRAsync(DateTime desde, DateTime hasta);
    Task<double> CalcularFirstCallResolutionAsync(DateTime desde, DateTime hasta);
    Task<double> CalcularSLAComplianceAsync(DateTime desde, DateTime hasta);
}

// 3. Implementación
public async Task<TimeSpan> CalcularMTTRAsync(DateTime desde, DateTime hasta)
{
    var incidentesCerrados = await _incidenteRepository.FindAsync(i =>
        i.Estado == EstadoIncidente.Cerrado &&
        i.FechaCierre >= desde &&
        i.FechaCierre <= hasta
    );
    
    if (!incidentesCerrados.Any()) return TimeSpan.Zero;
    
    var tiemposResolucion = incidentesCerrados
        .Where(i => i.FechaCierre.HasValue)
        .Select(i => i.FechaCierre!.Value - i.FechaReporte)
        .ToList();
    
    var promedioTicks = tiemposResolucion.Average(t => t.Ticks);
    return TimeSpan.FromTicks((long)promedioTicks);
}
```

---

### ⚠️ **11. HCI Y USABILIDAD**

**Estado: NO EVALUABLE (BACKEND)** ⚠️

Este requisito corresponde al **frontend (Blazor)**, que no fue objeto de análisis profundo. Sin embargo, se pueden dar recomendaciones desde el backend:

#### 🔧 Recomendaciones Backend para UI:

```csharp
// 1. DTOs optimizados para UI
public class IncidenteDashboardDto
{
    public int TotalIncidentes { get; set; }
    public int IncidentesAbiertos { get; set; }
    public int IncidentesCriticos { get; set; }
    public int IncidentesPorVencer { get; set; }
    public double TiempoPromedioResolucionHoras { get; set; }
    public List<IncidenteDto> UltimosIncidentes { get; set; } = new();
    public Dictionary<string, int> IncidentesPorCategoria { get; set; } = new();
}

// 2. Endpoints específicos para componentes UI
[HttpGet("dashboard")]
public async Task<IActionResult> GetDashboard()
{
    var dashboard = await _dashboardService.GetDashboardDataAsync();
    return Ok(dashboard);
}

// 3. Paginación para listas grandes
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
```

---

## 📊 TABLA DE CUMPLIMIENTO GENERAL

| Requisito | Estado | Prioridad | Esfuerzo |
|-----------|--------|-----------|----------|
| 1. Arquitectura Onion | ✅ Completo | Alta | - |
| 2. Tecnologías (.NET, Blazor, SQL) | ✅ Completo | Alta | - |
| 3. SPOC y Distribución Equitativa | ❌ No Implementado | **CRÍTICA** | Alto |
| 4. Niveles de TI (L1→L2→L3) | ❌ No Implementado | **CRÍTICA** | Alto |
| 5. Base de Conocimiento | ⚠️ Parcial | Media | Medio |
| 6. Detección Recurrencia | ❌ No Implementado | Alta | Medio |
| 7. Funcionalidad Segundo Plano | ❌ No Implementado | **CRÍTICA** | Alto |
| 8. Notificaciones Push | ❌ No Implementado | **CRÍTICA** | Alto |
| 9. Roles y Autenticación | ✅ Completo | Alta | - |
| 10. Catálogo Servicios DITIC | ❌ No Implementado | Media | Medio |
| 11. ITIL v4 | ⚠️ Parcial | Alta | Medio |
| 12. HCI y Usabilidad | ⚠️ Frontend | Media | - |

---

## 🎯 PLAN DE ACCIÓN RECOMENDADO

### 🔴 **PRIORIDAD CRÍTICA** (Implementar Inmediatamente)

#### 1. Sistema SPOC y Distribución Equitativa (2-3 días)
- Agregar campo `IsSPOC` y `CargaTrabajoActual` a Usuario
- Crear `AsignacionService` con algoritmo de balanceo
- Implementar lógica de permisos condicionales
- Crear endpoint `/api/asignacion/auto-asignar`

#### 2. Niveles de Soporte y Escalación (3-4 días)
- Agregar enum `NivelSoporte` (L1, L2, L3, L4)
- Crear `EscalacionService`
- Implementar lógica de escalación automática por tiempo
- Agregar endpoints de escalación manual

#### 3. Background Services (2 días)
- Implementar `SLAMonitoringService`
- Implementar `EscalacionAutomaticaService`
- Configurar en `Program.cs`

#### 4. Sistema de Notificaciones (3-4 días)
- Integrar SignalR para tiempo real
- Implementar `EmailService`
- Opcional: Integrar Twilio para WhatsApp
- Crear hub de notificaciones

### 🟡 **PRIORIDAD ALTA** (Siguiente Sprint)

#### 5. Catálogo de Servicios DITIC (2-3 días)
- Crear entidad `ServicioDITIC`
- Migración y seeds de datos
- Endpoints CRUD
- Vincular con categorías

#### 6. Detección de Recurrencia (2 días)
- Implementar `AnalisisPatronesService`
- Lógica de detección de patrones
- Escalación automática por recurrencia

#### 7. Métricas ITIL (2 días)
- Servicio de cálculo de MTTR, MTBF
- Dashboard de métricas
- Endpoints de reportes

### 🟢 **PRIORIDAD MEDIA** (Backlog)

#### 8. Gestión de Problemas
- Entidad `Problema`
- Análisis de causa raíz
- Vinculación con incidentes

#### 9. Mejoras Base de Conocimiento
- Full-text search
- Sugerencias automáticas
- Versionado de artículos

---

## 💻 EJEMPLOS DE CÓDIGO PARA IMPLEMENTAR

### Ejemplo 1: Service de Asignación Equitativa

```csharp
public class AsignacionService : IAsignacionService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IIncidenteRepository _incidenteRepository;
    
    public async Task<bool> AsignarIncidenteAutomaticamenteAsync(int incidenteId)
    {
        var incidente = await _incidenteRepository.GetByIdAsync(incidenteId);
        if (incidente == null) return false;
        
        // 1. Verificar si hay SPOC disponible
        var spoc = await GetSPOCDisponibleAsync();
        
        if (spoc != null && spoc.IsAvailable)
        {
            // SPOC asigna manualmente - no hacer nada
            return false;
        }
        
        // 2. Si no hay SPOC disponible, asignar al técnico con menor carga
        var tecnico = await GetTecnicoConMenorCargaAsync(
            incidente.Categoria?.Nombre // Buscar por especialidad
        );
        
        if (tecnico == null) return false;
        
        // 3. Asignar
        incidente.AsignadoAId = tecnico.Id;
        incidente.FechaAsignacion = DateTime.UtcNow;
        incidente.Estado = EstadoIncidente.EnProgreso;
        
        // 4. Incrementar carga de trabajo
        tecnico.CargaTrabajoActual++;
        
        await _incidenteRepository.UpdateAsync(incidente);
        await _usuarioRepository.UpdateAsync(tecnico);
        await _usuarioRepository.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<Usuario?> GetTecnicoConMenorCargaAsync(string? especialidad)
    {
        var tecnicos = await _usuarioRepository.GetTecnicosAsync();
        
        if (!string.IsNullOrEmpty(especialidad))
        {
            tecnicos = tecnicos.Where(t => 
                t.Especialidad?.Contains(especialidad, StringComparison.OrdinalIgnoreCase) ?? false
            );
        }
        
        return tecnicos
            .Where(t => t.IsActive)
            .OrderBy(t => t.CargaTrabajoActual)
            .ThenByDescending(t => t.AñosExperiencia)
            .FirstOrDefault();
    }
    
    public async Task<Dictionary<int, int>> GetCargaTrabajoTecnicosAsync()
    {
        var tecnicos = await _usuarioRepository.GetTecnicosAsync();
        
        return tecnicos.ToDictionary(
            t => t.Id,
            t => t.CargaTrabajoActual
        );
    }
}
```

### Ejemplo 2: Background Service para SLA

```csharp
public class SLAMonitoringService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SLAMonitoringService> _logger;
    
    public SLAMonitoringService(
        IServiceProvider serviceProvider,
        ILogger<SLAMonitoringService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SLA Monitoring Service iniciado");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var incidenteRepo = scope.ServiceProvider
                    .GetRequiredService<IIncidenteRepository>();
                var notificacionService = scope.ServiceProvider
                    .GetRequiredService<INotificacionService>();
                
                // Verificar incidentes vencidos
                var vencidos = await incidenteRepo.GetIncidentesVencidosAsync();
                
                foreach (var incidente in vencidos)
                {
                    _logger.LogWarning(
                        "Incidente {NumeroIncidente} ha excedido su SLA",
                        incidente.NumeroIncidente
                    );
                    
                    await notificacionService.NotificarSLAVencidoAsync(incidente);
                }
                
                // Verificar incidentes próximos a vencer (1 hora antes)
                var porVencer = await incidenteRepo.GetIncidentesPorVencerAsync(horasAntes: 1);
                
                foreach (var incidente in porVencer)
                {
                    await notificacionService.NotificarSLAPorVencerAsync(incidente);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en SLA Monitoring Service");
            }
            
            // Ejecutar cada minuto
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}

// Registrar en Program.cs
builder.Services.AddHostedService<SLAMonitoringService>();
```

### Ejemplo 3: Escalación por Niveles

```csharp
public class EscalacionService : IEscalacionService
{
    public async Task<bool> EscalarIncidenteAsync(int incidenteId, string motivo)
    {
        var incidente = await _incidenteRepository.GetIncidenteCompletoAsync(incidenteId);
        if (incidente == null) return false;
        
        // Determinar siguiente nivel
        var nivelActual = incidente.NivelActual;
        var siguienteNivel = ObtenerSiguienteNivel(nivelActual);
        
        if (siguienteNivel == null)
        {
            _logger.LogWarning(
                "Incidente {NumeroIncidente} ya está en el nivel máximo",
                incidente.NumeroIncidente
            );
            return false;
        }
        
        // Buscar técnico del siguiente nivel
        var tecnicoNivelSuperior = await ObtenerTecnicoNivelSuperiorAsync(
            siguienteNivel.Value,
            incidente.Categoria?.Nombre
        );
        
        if (tecnicoNivelSuperior == null)
        {
            _logger.LogError(
                "No hay técnicos disponibles en nivel {Nivel}",
                siguienteNivel.Value
            );
            return false;
        }
        
        // Registrar escalación
        var escalacion = new EscalacionSLA
        {
            IncidenteId = incidenteId,
            FechaEscalacion = DateTime.UtcNow,
            Motivo = motivo,
            FueNotificado = false
        };
        
        await _escalacionRepository.AddAsync(escalacion);
        
        // Actualizar incidente
        incidente.NivelActual = siguienteNivel.Value;
        incidente.AsignadoAId = tecnicoNivelSuperior.Id;
        incidente.NumeroEscalaciones++;
        incidente.Estado = EstadoIncidente.Escalado;
        
        await _incidenteRepository.UpdateAsync(incidente);
        await _incidenteRepository.SaveChangesAsync();
        
        // Notificar
        await _notificacionService.NotificarEscalacionAsync(
            incidente,
            tecnicoNivelSuperior,
            motivo
        );
        
        _logger.LogInformation(
            "Incidente {NumeroIncidente} escalado de {NivelAnterior} a {NivelNuevo}",
            incidente.NumeroIncidente,
            nivelActual,
            siguienteNivel.Value
        );
        
        return true;
    }
    
    private NivelSoporte? ObtenerSiguienteNivel(NivelSoporte nivelActual)
    {
        return nivelActual switch
        {
            NivelSoporte.L1_Tecnico => NivelSoporte.L2_Experto,
            NivelSoporte.L2_Experto => NivelSoporte.L3_Especialista,
            NivelSoporte.L3_Especialista => NivelSoporte.L4_Proveedor,
            NivelSoporte.L4_Proveedor => null,
            _ => null
        };
    }
    
    public async Task<Usuario?> ObtenerTecnicoNivelSuperiorAsync(
        NivelSoporte nivel,
        string? especialidad)
    {
        var tecnicos = await _usuarioRepository
            .FindAsync(u => 
                u.NivelSoporte == nivel &&
                u.IsActive &&
                u.TipoUsuario == TipoUsuario.Tecnico
            );
        
        if (!string.IsNullOrEmpty(especialidad))
        {
            tecnicos = tecnicos.Where(t => 
                t.Especialidad?.Contains(especialidad) ?? false
            );
        }
        
        return tecnicos
            .OrderBy(t => t.CargaTrabajoActual)
            .ThenByDescending(t => t.AñosExperiencia)
            .FirstOrDefault();
    }
}
```

---

## 🏆 CONCLUSIONES Y RECOMENDACIONES FINALES

### ✅ **FORTALEZAS DEL PROYECTO**

1. **Arquitectura Sólida**: Clean Architecture bien implementada
2. **Modelo de Dominio Rico**: Entidades bien diseñadas según ITIL
3. **Base Tecnológica Moderna**: .NET 9, EF Core 9
4. **Escalabilidad**: Estructura preparada para crecer
5. **Seguridad**: JWT implementado correctamente

### ❌ **DEBILIDADES CRÍTICAS**

1. **Servicios .bak**: La lógica de negocio está en archivos .bak, no en producción
2. **Falta Lógica de Negocio**: Requisitos clave no implementados
3. **Sin Automatización**: No hay background services
4. **Sin Notificaciones**: Requisito crítico faltante
5. **SPOC No Implementado**: Requisito principal sin desarrollar

### 🎯 **RECOMENDACIÓN FINAL**

**PRIORIDAD: ALTA**

El proyecto tiene una **excelente base arquitectónica** pero requiere:

1. ✅ **Mover archivos .bak a producción** (1 día)
2. 🔴 **Implementar SPOC y distribución** (3 días)
3. 🔴 **Implementar niveles y escalación** (4 días)
4. 🔴 **Background services y notificaciones** (5 días)
5. 🟡 **Catálogo DITIC y recurrencia** (4 días)

**Tiempo estimado total:** 17 días hábiles (~3.5 semanas)

### 📈 **ROADMAP SUGERIDO**

**Sprint 1 (Semana 1-2):**
- Activar servicios .bak
- Implementar SPOC
- Implementar escalación por niveles
- Background services básicos

**Sprint 2 (Semana 3):**
- Sistema de notificaciones completo
- Catálogo DITIC
- Detección de recurrencia

**Sprint 3 (Semana 4):**
- Métricas ITIL
- Refinamiento y testing
- Documentación API

---

## 📞 SOPORTE Y CONTACTO

Para implementar estas recomendaciones, se sugiere:

1. **Crear issues en GitHub** por cada funcionalidad faltante
2. **Priorizar** según tabla de impacto
3. **Asignar** a desarrolladores backend
4. **Revisar** con arquitecto de software

---

**Documento generado:** 2 de Diciembre, 2025  
**Analista:** GitHub Copilot (Claude Sonnet 4.5)  
**Versión:** 1.0
