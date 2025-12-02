# 📋 Documentación de Estructura del Proyecto

## 🎯 Descripción General
**ProyectoAgiles** es un sistema de gestión de escalafón docente universitario desarrollado con **ASP.NET Core** y **Blazor WebAssembly**. Implementa la arquitectura **Clean Architecture** con separación de capas para mantenibilidad y escalabilidad.

---

## 🏗️ Arquitectura del Proyecto

El proyecto sigue el patrón **Clean Architecture** dividido en las siguientes capas:

```
📁 ProyectoAgiles/
├── 🎨 proyectoAgiles/                     # Capa de Presentación (Blazor WebAssembly)
├── 🚀 ProyectoAgiles.Api/                 # Capa de API (ASP.NET Core Web API)
├── 🔧 ProyectoAgiles.Application/         # Capa de Aplicación (Casos de Uso)
├── 🏛️ ProyectoAgiles.Domain/              # Capa de Dominio (Entidades y Reglas de Negocio)
├── 🗃️ ProyectoAgiles.Infrastructure/      # Capa de Infraestructura (Datos y Servicios Externos)
└── 📜 Documentación/                      # Documentación del proyecto
```

---

## 📁 Estructura Detallada por Carpeta

### 🎨 `proyectoAgiles/` - Capa de Presentación (Blazor WebAssembly)
**Propósito**: Interfaz de usuario interactiva del sistema de escalafón docente.

#### 📄 Archivos Principales
- **`App.razor`** - Componente raíz de la aplicación Blazor
- **`Program.cs`** - Punto de entrada, configuración de servicios DI
- **`proyectoAgiles.csproj`** - Archivo de proyecto con referencias y configuraciones
- **`_Imports.razor`** - Importaciones globales de namespaces

#### 📁 `Layout/` - Diseño y Estructura Visual
- **`MainLayout.razor`** - Layout principal de la aplicación
- **`MainLayout.razor.css`** - Estilos específicos del layout principal
- **`AuthLayout.razor`** - Layout para páginas de autenticación
- **`AuthLayout.razor.css`** - Estilos del layout de autenticación
- **`NavMenu.razor`** - Menú de navegación principal
- **`NavMenu.razor.css`** - Estilos del menú de navegación

**Nota**: Los layouts de autenticación están en la carpeta `Shared/`

#### 📁 `Pages/` - Páginas y Componentes de la Aplicación
- **`Home.razor`** - Página de inicio con información general
- **`Login.razor`** - Página de inicio de sesión
- **`Login_backup.razor`** - Respaldo de la página de login anterior
- **`Register.razor`** - Página de registro de usuarios
- **`ForgotPassword.razor`** - Recuperación de contraseña
- **`ResetPassword.razor`** - Restablecimiento de contraseña

##### 👨‍🏫 Páginas de Docentes
- **`TeacherDashboard.razor`** - Dashboard principal del docente
  - Gestión de investigaciones, evaluaciones y capacitaciones
  - Verificación de requisitos para ascenso
  - Creación y seguimiento de solicitudes de escalafón
  - Historial de escalafones y apelaciones

##### 👤 Páginas de Administración
- **`AdminDashboard.razor`** - Dashboard administrativo
- **`ManageTeachers.razor`** - Gestión de docentes
- **`TimeManagement.razor`** - Gestión de períodos de solicitudes
- **`Reports.razor`** - Generación de reportes

##### 🏛️ Páginas de Comisiones y Direcciones
- **`ComisionAcademicaEscalafon.razor`** - Interfaz para la Comisión Académica
- **`PresidenteComisionAcademica.razor`** - Interfaz para el Presidente de la Comisión
- **`DireccionTalentoHumano.razor`** - Interfaz para Dirección de Talento Humano
- **`TalentoHumano.razor`** - Gestión de recursos humanos

##### 📊 Páginas de Gestión
- **`SolicitudDetails.razor`** - Detalles de solicitudes de escalafón
- **`SolicitudDetailsNew.razor`** - Nueva interfaz de detalles de solicitudes

#### 📁 `Layout/` - Diseño y Estructura Visual
- **`MainLayout.razor`** - Layout principal de la aplicación
- **`MainLayout.razor.css`** - Estilos específicos del layout principal
- **`NavMenu.razor`** - Menú de navegación principal
- **`NavMenu.razor.css`** - Estilos del menú de navegación

**Nota**: Los layouts de autenticación están en la carpeta `Shared/`

#### 📁 `Services/` - Servicios del Cliente
- **`AuthService.cs`** - Servicio de autenticación y autorización
- **`UserSessionService.cs`** - Gestión de sesiones de usuario
- **`VerificacionRequisitosEscalafonDto.cs`** - DTOs para verificación de requisitos

#### 📁 `Shared/` - Componentes Compartidos
- **`AuthLayout.razor`** - Layout alternativo para autenticación
- **`AuthLayout.razor.css`** - Estilos del layout de autenticación

#### 📁 `Properties/` - Configuración del Proyecto
- **`launchSettings.json`** - Configuración de lanzamiento y depuración

#### 📁 `wwwroot/` - Recursos Estáticos
- **`index.html`** - Página HTML principal
- **`appsettings.json`** - Configuración del cliente
- **`favicon.png`** - Icono de la aplicación
- **`icon-192.png`** - Icono para PWA

##### 📁 `wwwroot/css/` - Estilos CSS
- Archivos CSS globales y de Bootstrap

##### 📁 `wwwroot/js/` - Archivos JavaScript
- **`file-download.js`** - Funcionalidades de descarga de archivos
- **`file-drag-drop.js`** - Funcionalidades de drag and drop
- **`notifications.js`** - Sistema de notificaciones
- **`pdf-generator.js`** - Generación de PDFs
- **`reports.js`** - Funcionalidades de reportes

##### 📁 `wwwroot/images/` - Imágenes
- Recursos gráficos de la aplicación

##### 📁 `wwwroot/lib/` - Librerías del Cliente
- Librerías JavaScript y CSS de terceros

---

### 🚀 `ProyectoAgiles.Api/` - Capa de API (ASP.NET Core Web API)
**Propósito**: API REST que expone los servicios del sistema a través de HTTP.

#### 📄 Archivos Principales
- **`Program.cs`** - Configuración de la API, middleware, servicios
- **`appsettings.json`** - Configuración general
- **`appsettings.Development.json`** - Configuración de desarrollo
- **`ProyectoAgiles.Api.csproj`** - Archivo de proyecto de la API
- **`ProyectoAgiles.Api.http`** - Archivo de pruebas HTTP
- **`test-api.http`** - Pruebas adicionales de API

#### 📁 `Controllers/` - Controladores de API
- **`AuthController.cs`** - Autenticación y autorización
- **`DashboardController.cs`** - Datos del dashboard
- **`InvestigacionesController.cs`** - Gestión de investigaciones
- **`EvaluacionesDesempenoController.cs`** - Gestión de evaluaciones
- **`DiticController.cs`** - Gestión de capacitaciones DITIC
- **`SolicitudesEscalafonController.cs`** - Gestión de solicitudes de escalafón
- **`UsersController.cs`** - Gestión de usuarios
- **`TeacherManagementController.cs`** - Gestión de docentes
- **`TimeConfigurationController.cs`** - Gestión de períodos de tiempo
- **`ReportsController.cs`** - Generación de reportes
- **`TTHHController.cs`** - Talento Humano
- **`ArchivosUtilizadosController.cs`** - Archivos utilizados en escalafones
- **`DebugController.cs`** - Herramientas de debugging

#### 📁 `Properties/` - Configuración del Proyecto
- **`launchSettings.json`** - Configuración de lanzamiento y depuración

#### 📁 `wwwroot/` - Recursos Estáticos Web
- Archivos estáticos servidos por la API

---

### 🔧 `ProyectoAgiles.Application/` - Capa de Aplicación
**Propósito**: Contiene la lógica de negocio, casos de uso y servicios de aplicación.

#### 📄 Archivo Principal
- **`ProyectoAgiles.Application.csproj`** - Configuración del proyecto de aplicación

#### 📁 `Services/` - Servicios de Aplicación
- **`AuthService.cs`** - Lógica de autenticación
- **`UserService.cs`** - Gestión de usuarios
- **`InvestigacionService.cs`** - Lógica de investigaciones
- **`EvaluacionDesempenoService.cs`** - Lógica de evaluaciones
- **`DiticService.cs`** - Lógica de capacitaciones
- **`SolicitudEscalafonService.cs`** - Lógica de solicitudes de escalafón
- **`TeacherManagementService.cs`** - Gestión de docentes
- **`RequisitosEscalafonService.cs`** - Verificación de requisitos
- **`EmailService.cs`** - Servicio de correo electrónico
- **`MockEmailService.cs`** - Servicio de correo para pruebas
- **`FileService.cs`** - Gestión de archivos
- **`ArchivosUtilizadosService.cs`** - Gestión de archivos utilizados

#### 📁 `DTOs/` - Objetos de Transferencia de Datos
- **`ApiResponse.cs`** - Respuesta estándar de la API
- **`ArchivosUtilizadosDto.cs`** - DTOs para archivos utilizados
- **`DashboardDtos.cs`** - DTOs para dashboard
- **`DiticDto.cs`** - DTOs para capacitaciones DITIC
- **`EvaluacionDesempenoDto.cs`** - DTOs para evaluaciones de desempeño
- **`InvestigacionDto.cs`** - DTOs para investigaciones
- **`RequisitoEscalafonConfigDto.cs`** - DTOs para configuración de requisitos
- **`SolicitudEscalafonDto.cs`** - DTOs para solicitudes de escalafón
- **`TeacherManagementDtos.cs`** - DTOs para gestión de docentes
- **`UserDtos.cs`** - DTOs para usuarios

#### 📁 `Interfaces/` - Interfaces de Servicios
- **`IAuthService.cs`** - Interfaz de autenticación
- **`IUserService.cs`** - Interfaz de gestión de usuarios
- **`IInvestigacionService.cs`** - Interfaz de investigaciones
- **`IEvaluacionDesempenoService.cs`** - Interfaz de evaluaciones
- **`IDiticService.cs`** - Interfaz de capacitaciones
- **`ISolicitudEscalafonService.cs`** - Interfaz de solicitudes de escalafón
- **`ITeacherManagementService.cs`** - Interfaz de gestión de docentes
- **`IRequisitosEscalafonService.cs`** - Interfaz de verificación de requisitos
- **`IEmailService.cs`** - Interfaz de servicio de correo
- **`IFileService.cs`** - Interfaz de gestión de archivos
- **`IArchivosUtilizadosService.cs`** - Interfaz de archivos utilizados

#### 📁 `Mappings/` - Mapeos AutoMapper
- **`DiticMappingProfile.cs`** - Perfil de mapeo para capacitaciones DITIC
- **`EvaluacionDesempenoMappingProfile.cs`** - Perfil de mapeo para evaluaciones
- **`SolicitudEscalafonMappingProfile.cs`** - Perfil de mapeo para solicitudes de escalafón

---

### 🏛️ `ProyectoAgiles.Domain/` - Capa de Dominio
**Propósito**: Contiene las entidades de negocio, reglas de dominio y interfaces core.

#### 📄 Archivo Principal
- **`ProyectoAgiles.Domain.csproj`** - Configuración del proyecto de dominio

#### 📁 `Entities/` - Entidades de Dominio
- **`BaseEntity.cs`** - Entidad base con propiedades comunes
- **`User.cs`** - Entidad de usuario del sistema
- **`Investigacion.cs`** - Entidad de investigación académica
- **`EvaluacionDesempeno.cs`** - Entidad de evaluación de desempeño
- **`DITIC.cs`** - Entidad de capacitación DITIC
- **`SolicitudEscalafon.cs`** - Entidad de solicitud de escalafón
- **`ExternalTeacher.cs`** - Entidad de docente externo
- **`TTHH.cs`** - Entidad de Talento Humano
- **`TimeConfiguration.cs`** - Configuración de períodos de tiempo
- **`PasswordResetToken.cs`** - Token de restablecimiento de contraseña
- **`ArchivosUtilizadosEscalafon.cs`** - Registro de archivos utilizados

#### 📁 `Enums/` - Enumeraciones
- **`UserType.cs`** - Tipos de usuario del sistema (Admin, Docente, etc.)

#### 📁 `Interfaces/` - Interfaces de Dominio
- **`IRepository.cs`** - Interfaz base de repositorio
- **`IArchivosUtilizadosRepository.cs`** - Interfaz para archivos utilizados
- **`IDiticRepository.cs`** - Interfaz para capacitaciones DITIC
- **`IEvaluacionDesempenoRepository.cs`** - Interfaz para evaluaciones
- **`IExternalTeacherRepository.cs`** - Interfaz para docentes externos
- **`IInvestigacionRepository.cs`** - Interfaz para investigaciones
- **`IPasswordResetTokenRepository.cs`** - Interfaz para tokens de reset
- **`ISolicitudEscalafonRepository.cs`** - Interfaz para solicitudes
- **`ITTHHRepository.cs`** - Interfaz para Talento Humano

---

### 🗃️ `ProyectoAgiles.Infrastructure/` - Capa de Infraestructura
**Propósito**: Implementa el acceso a datos, servicios externos y configuraciones de infraestructura.

#### 📄 Archivo Principal
- **`ProyectoAgiles.Infrastructure.csproj`** - Configuración del proyecto de infraestructura

#### 📁 `Data/` - Contexto de Base de Datos
- **`ApplicationDbContext.cs`** - Contexto de Entity Framework Core
  - Configuración de entidades
  - Configuración de relaciones
  - Configuración de índices y restricciones

#### 📁 `Repositories/` - Implementaciones de Repositorios
- **`Repository.cs`** - Repositorio base genérico
- **`UserRepository.cs`** - Repositorio de usuarios
- **`InvestigacionRepository.cs`** - Repositorio de investigaciones
- **`EvaluacionDesempenoRepository.cs`** - Repositorio de evaluaciones
- **`DiticRepository.cs`** - Repositorio de capacitaciones
- **`SolicitudEscalafonRepository.cs`** - Repositorio de solicitudes
- **`ExternalTeacherRepository.cs`** - Repositorio de docentes externos
- **`TTHHRepository.cs`** - Repositorio de Talento Humano
- **`PasswordResetTokenRepository.cs`** - Repositorio de tokens de reset
- **`ArchivosUtilizadosRepository.cs`** - Repositorio de archivos utilizados

#### 📁 `Services/` - Servicios de Infraestructura
- **`ArchivosUtilizadosInfrastructureService.cs`** - Servicio de infraestructura para archivos utilizados

#### 📁 `Migrations/` - Migraciones de Base de Datos
Archivos de migración de Entity Framework Core:
- **`ApplicationDbContextModelSnapshot.cs`** - Snapshot del modelo actual
- **`20250703212400_AddTimeConfigurationTable.cs`** - Tabla de configuración de tiempo
- **`20250702024808_AddArchivosUtilizadosEscalafon.cs`** - Tabla de archivos utilizados
- **`20250625061044_AddConsejoPropertiesToSolicitudEscalafon.cs`** - Propiedades del consejo
- **`20250624203251_AddSolicitudEscalafon.cs`** - Tabla de solicitudes de escalafón
- **`20250623184108_CreateDiticTable.cs`** - Tabla de capacitaciones DITIC
- **`20250623180128_RenameTableToDAC.cs`** - Renombrado de tabla DAC
- **`20250623175749_AgregarTablaEvaluacionesDesempeno.cs`** - Tabla de evaluaciones
- **`20250623163117_AddArchivoPdfToInvestigacion.cs`** - Campo PDF en investigaciones
- **`20250623054200_AddInvestigacionesTable.cs`** - Tabla de investigaciones
- Y otras migraciones de configuración inicial del sistema

---

## 📜 Documentación Adicional

### 📋 Archivos de Documentación
- **`README.md`** - Información general del proyecto
- **`DOCUMENTACION_REPORTES.md`** - Documentación de reportes
- **`RESUMEN_IMPLEMENTACION.md`** - Resumen de la implementación
- **`Sprint_2_Documentacion_Completa.md`** - Documentación del Sprint 2
- **`DOCUMENTACION_ESTRUCTURA_PROYECTO.md`** - Este archivo de documentación

### 🛠️ Archivos de Configuración
- **`proyectoAgiles.slnx`** - Archivo de solución de Visual Studio
- **`.gitignore`** - Archivos ignorados por Git
- **`.vscode/`** - Configuración de Visual Studio Code

### 🔧 Scripts y Herramientas
- **`InsertScript/`** - Proyecto de scripts de inserción de datos (contiene solo archivos de compilación)
- **`obj/`** - Archivos de compilación temporales del proyecto raíz
- **`bin/`** - Archivos binarios compilados (en cada proyecto)

---

## 🎯 Funcionalidades Principales

### 👨‍🏫 Para Docentes
1. **Gestión de Investigaciones**: Registro, edición, eliminación y visualización de investigaciones
2. **Gestión de Evaluaciones**: Manejo de evaluaciones de desempeño
3. **Gestión de Capacitaciones**: Registro de capacitaciones DITIC
4. **Verificación de Requisitos**: Verificación automática de requisitos para ascenso
5. **Solicitudes de Escalafón**: Creación y seguimiento de solicitudes
6. **Historial de Escalafones**: Visualización del historial de ascensos
7. **Apelaciones**: Sistema de apelaciones para solicitudes rechazadas

### 👤 Para Administradores
1. **Gestión de Usuarios**: Administración de usuarios del sistema
2. **Gestión de Docentes**: Administración de información docente
3. **Gestión de Períodos**: Configuración de períodos de solicitudes
4. **Reportes**: Generación de reportes estadísticos
5. **Configuración del Sistema**: Ajustes generales del sistema

### 🏛️ Para Comisiones
1. **Revisión de Solicitudes**: Evaluación de solicitudes de escalafón
2. **Aprobación/Rechazo**: Decisiones sobre solicitudes
3. **Gestión de Apelaciones**: Revisión de apelaciones
4. **Reportes Especializados**: Reportes para toma de decisiones

---

## 🔧 Tecnologías Utilizadas

### Frontend
- **Blazor WebAssembly** - Framework de aplicaciones web interactivas
- **Bootstrap 5** - Framework CSS para diseño responsivo
- **Font Awesome** - Iconos vectoriales
- **JavaScript** - Funcionalidades del lado del cliente

### Backend
- **ASP.NET Core 9** - Framework web
- **Entity Framework Core** - ORM para acceso a datos
- **SQL Server** - Base de datos relacional
- **AutoMapper** - Mapeo de objetos
- **JWT** - Autenticación y autorización

### Arquitectura
- **Clean Architecture** - Separación de responsabilidades
- **Repository Pattern** - Patrón de acceso a datos
- **Dependency Injection** - Inyección de dependencias
- **CQRS** - Command Query Responsibility Segregation

---

## 🚀 Instrucciones de Ejecución

### Prerrequisitos
1. **.NET 9 SDK** instalado
2. **SQL Server** instalado y configurado
3. **Visual Studio** o **Visual Studio Code**

### Pasos para Ejecutar
1. **Clonar el repositorio**
2. **Configurar la cadena de conexión** en `appsettings.json`
3. **Ejecutar migraciones** de Entity Framework
4. **Ejecutar la API**: `dotnet run --project ProyectoAgiles.Api`
5. **Ejecutar el cliente Blazor**: `dotnet run --project proyectoAgiles`

### Puertos por Defecto
- **API**: `https://localhost:5200`
- **Cliente Blazor**: `https://localhost:5001`

---

## 📝 Notas Importantes

### 🔒 Seguridad
- Autenticación basada en JWT
- Autorización por roles
- Validación de datos en todas las capas
- Protección contra ataques comunes

### 📊 Base de Datos
- Diseño normalizado
- Índices optimizados
- Restricciones de integridad
- Auditoría de cambios

### 🎨 Interfaz de Usuario
- Diseño responsive
- Experiencia de usuario intuitiva
- Notificaciones en tiempo real
- Carga progresiva de datos

---

## 🤝 Contribuciones

Para contribuir al proyecto:
1. Seguir la arquitectura establecida
2. Mantener la separación de responsabilidades
3. Escribir pruebas unitarias
4. Documentar los cambios
5. Seguir las convenciones de código

---

## 📞 Contacto y Soporte

Para soporte técnico o consultas sobre el proyecto, contactar al equipo de desarrollo.

---

*Última actualización: 7 de julio de 2025*
