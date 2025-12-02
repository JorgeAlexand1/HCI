# 🎓 IncidentesFISEI - Sistema de Gestión de Incidentes

## 📋 Descripción General

**IncidentesFISEI** es un sistema completo de gestión de incidentes tecnológicos desarrollado específicamente para la **Facultad de Ingeniería en Sistemas** de la **Universidad Técnica de Ambato**. El sistema implementa las mejores prácticas de **ITIL v3** para garantizar una gestión eficiente y profesional de los incidentes.

---

## 🏗️ Arquitectura del Sistema

El proyecto sigue el patrón **Clean Architecture** dividido en las siguientes capas:

```
📁 IncidentesFISEI/
├── 🎨 IncidentesFISEI.Blazor/          # Capa de Presentación (Blazor WebAssembly)
├── 🚀 IncidentesFISEI.Api/             # Capa de API (ASP.NET Core Web API)
├── 🔧 IncidentesFISEI.Application/     # Capa de Aplicación (Casos de Uso)
├── 🏛️ IncidentesFISEI.Domain/          # Capa de Dominio (Entidades y Reglas)
└── 🗃️ IncidentesFISEI.Infrastructure/  # Capa de Infraestructura (Datos)
```

---

## ⚡ Características Principales

### 📊 Gestión de Incidentes según ITIL v3
- ✅ Creación, seguimiento y resolución de incidentes
- ✅ Clasificación automática por prioridad, impacto y urgencia
- ✅ Asignación inteligente a técnicos especializados
- ✅ Seguimiento de SLA (Service Level Agreement)
- ✅ Escalación automática de incidentes críticos
- ✅ Historial completo de actividades

### 📚 Base de Conocimiento
- ✅ Artículos de solución documentados
- ✅ Sistema de votación y retroalimentación
- ✅ Búsqueda avanzada por categorías y tags
- ✅ Versionado de artículos
- ✅ Integración con resolución de incidentes

### 👥 Gestión de Usuarios y Roles
- ✅ **Usuario**: Reporta incidentes y consulta soluciones
- ✅ **Técnico**: Resuelve incidentes asignados
- ✅ **Supervisor**: Gestiona equipos y supervisa SLA
- ✅ **Administrador**: Configuración completa del sistema

### 📈 Analítica y Reportes
- ✅ Dashboard en tiempo real
- ✅ Métricas de rendimiento (MTTR, MTBF)
- ✅ Reportes de cumplimiento de SLA
- ✅ Análisis de tendencias
- ✅ Exportación de datos

---

## 🛠️ Tecnologías Utilizadas

### Frontend (Blazor WebAssembly)
- **Framework**: ASP.NET Core 9 / Blazor WebAssembly
- **UI Framework**: Bootstrap 5
- **Iconos**: Font Awesome 6
- **Estado**: Blazored.LocalStorage
- **HTTP**: System.Net.Http.Json

### Backend (ASP.NET Core Web API)
- **Framework**: ASP.NET Core 9 Web API
- **ORM**: Entity Framework Core 9
- **Base de Datos**: SQL Server
- **Autenticación**: JWT Bearer Tokens
- **Documentación**: Swagger/OpenAPI
- **Mapeo**: AutoMapper
- **Logging**: Serilog (recomendado)

### Arquitectura y Patrones
- **Clean Architecture**: Separación clara de responsabilidades
- **Repository Pattern**: Abstracción de acceso a datos
- **CQRS**: Command Query Responsibility Segregation
- **Dependency Injection**: Inyección de dependencias nativa
- **Domain-Driven Design**: Modelado centrado en el dominio

---

## 🚀 Instalación y Configuración

### Prerrequisitos
- ✅ .NET 9 SDK
- ✅ SQL Server (LocalDB o completo)
- ✅ Visual Studio 2022 o VS Code
- ✅ Git

### Pasos de Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/tu-usuario/IncidentesFISEI.git
   cd IncidentesFISEI
   ```

2. **Restaurar paquetes NuGet**
   ```bash
   dotnet restore
   ```

3. **Configurar cadena de conexión**
   
   Editar `IncidentesFISEI.Api/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=IncidentesFISEI;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Aplicar migraciones de base de datos**
   ```bash
   cd IncidentesFISEI.Api
   dotnet ef database update
   ```

5. **Ejecutar la aplicación**
   
   **API (Terminal 1):**
   ```bash
   cd IncidentesFISEI.Api
   dotnet run
   ```
   
   **Cliente Blazor (Terminal 2):**
   ```bash
   cd IncidentesFISEI.Blazor
   dotnet run
   ```

### URLs por Defecto
- **API**: `https://localhost:5200`
- **Swagger**: `https://localhost:5200`
- **Cliente Blazor**: `https://localhost:5001`

---

## 🎯 Funcionalidades por Rol

### 👤 Usuario Final
- Reportar nuevos incidentes
- Consultar estado de mis incidentes
- Buscar en la base de conocimiento
- Recibir notificaciones de actualización
- Evaluar calidad del servicio

### 🔧 Técnico de Soporte
- Ver incidentes asignados
- Actualizar progreso de resolución
- Registrar tiempo trabajado
- Crear artículos de conocimiento
- Escalar incidentes complejos

### 👔 Supervisor
- Gestionar asignaciones de equipo
- Monitorear cumplimiento de SLA
- Generar reportes de rendimiento
- Aprobar escalaciones
- Revisar artículos de conocimiento

### 🛡️ Administrador del Sistema
- Gestionar usuarios y permisos
- Configurar categorías y SLA
- Definir flujos de escalación
- Administrar configuraciones globales
- Acceso completo a reportes y analítica

---

## 📊 Modelo de Datos Principal

### Entidades Core
- **Usuario**: Información de usuarios del sistema
- **Incidente**: Registro completo de incidentes
- **CategoriaIncidente**: Clasificación de tipos de problema
- **ArticuloConocimiento**: Base de conocimiento
- **ComentarioIncidente**: Seguimiento de conversaciones
- **RegistroTiempo**: Control de tiempo trabajado
- **SLA**: Definición de acuerdos de nivel de servicio

### Estados del Incidente (ITIL v3)
1. **Abierto**: Incidente reportado y registrado
2. **En Progreso**: Técnico trabajando en la solución
3. **En Espera**: Esperando información/recursos
4. **Resuelto**: Solución implementada, pendiente validación
5. **Cerrado**: Incidente completamente resuelto
6. **Cancelado**: Incidente cancelado o duplicado

### Niveles de Prioridad
- **Crítica**: Afecta producción, requiere atención inmediata
- **Alta**: Impacto significativo, resolución urgente
- **Media**: Impacto moderado, resolución normal
- **Baja**: Impacto mínimo, puede programarse

---

## 🔐 Seguridad y Autenticación

### Características de Seguridad
- ✅ Autenticación JWT con expiración configurable
- ✅ Autorización basada en roles
- ✅ Cifrado de contraseñas con BCrypt
- ✅ Validación de entrada en todas las capas
- ✅ Protección contra ataques comunes (XSS, CSRF)
- ✅ Rate limiting para APIs públicas

### Configuración JWT
```json
{
  "JwtSettings": {
    "SecretKey": "tu-clave-secreta-segura-de-32-caracteres",
    "Issuer": "IncidentesFISEI.Api",
    "Audience": "IncidentesFISEI.Blazor",
    "ExpiryInMinutes": 480
  }
}
```

---

## 📈 Métricas y KPIs

### Métricas de Servicio
- **MTTR** (Mean Time To Resolution): Tiempo promedio de resolución
- **MTBF** (Mean Time Between Failures): Tiempo entre fallos
- **First Call Resolution**: Resolución en primer contacto
- **SLA Compliance**: Cumplimiento de acuerdos de servicio
- **Customer Satisfaction**: Satisfacción del usuario

### Reportes Disponibles
- Dashboard ejecutivo en tiempo real
- Informe de incidentes por categoría
- Análisis de tendencias temporales
- Reporte de rendimiento por técnico
- Cumplimiento de SLA por período

---

## 🧪 Testing y Calidad

### Estrategia de Testing
- **Unit Tests**: Pruebas unitarias de lógica de negocio
- **Integration Tests**: Pruebas de integración de API
- **E2E Tests**: Pruebas end-to-end con Playwright
- **Performance Tests**: Pruebas de carga con NBomber

### Herramientas de Calidad
- **SonarQube**: Análisis estático de código
- **CodeCoverage**: Cobertura de pruebas
- **StyleCop**: Estándares de codificación
- **Security Scanning**: Análisis de vulnerabilidades

---

## 🚀 Deployment y DevOps

### Opciones de Deployment
- **IIS**: Deployment tradicional en Windows Server
- **Docker**: Contenedorización para cualquier plataforma
- **Azure App Service**: Cloud deployment en Azure
- **Kubernetes**: Orquestación para alta disponibilidad

### CI/CD Pipeline
```yaml
# Ejemplo de Azure DevOps Pipeline
- Build y Test automatizados
- Code quality gates con SonarQube
- Security scanning
- Automated deployment a staging
- Manual approval para production
- Rollback automático en caso de fallas
```

---

## 📞 Soporte y Contribuciones

### Contacto
- **Email**: soporte@fisei.uta.edu.ec
- **Documentación**: [Wiki del Proyecto]
- **Issues**: [GitHub Issues]
- **Slack**: #incidentesfisei

### Contribuir al Proyecto
1. Fork del repositorio
2. Crear rama feature (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Abrir Pull Request

---

## 📝 Licencia

Este proyecto está licenciado bajo la Licencia MIT - ver el archivo [LICENSE.md](LICENSE.md) para más detalles.

---

## 🙏 Agradecimientos

- **Universidad Técnica de Ambato** por el apoyo institucional
- **Facultad de Ingeniería en Sistemas** por los recursos técnicos
- **Comunidad ITIL** por las mejores prácticas implementadas
- **Microsoft** por las tecnologías .NET utilizadas

---

**Desarrollado con ❤️ para la comunidad FISEI-UTA**

*Última actualización: Diciembre 2024*