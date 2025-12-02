# 🗃️ Configuración de Base de Datos - IncidentesFISEI

## 📋 Pasos para crear y configurar la base de datos

### 1️⃣ Prerrequisitos

Asegúrate de tener instalado:
- **SQL Server Express LocalDB** o **SQL Server completo**
- **.NET 9 SDK**
- **Entity Framework Core Tools**

### 2️⃣ Instalar EF Core Tools (si no lo tienes)

```bash
dotnet tool install --global dotnet-ef
```

### 3️⃣ Verificar la cadena de conexión

Edita el archivo `IncidentesFISEI.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=IncidentesFISEI;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

**Opciones de cadena de conexión:**

**Para LocalDB (Recomendado para desarrollo):**
```
Server=(localdb)\\mssqllocaldb;Database=IncidentesFISEI;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true
```

**Para SQL Server Express:**
```
Server=.\\SQLEXPRESS;Database=IncidentesFISEI;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true
```

**Para SQL Server completo:**
```
Server=localhost;Database=IncidentesFISEI;User Id=tu_usuario;Password=tu_password;TrustServerCertificate=true;MultipleActiveResultSets=true
```

### 4️⃣ Crear la migración inicial

Abre terminal en la carpeta raíz del proyecto y ejecuta:

```bash
cd IncidentesFISEI.Api
dotnet ef migrations add InitialCreate
```

### 5️⃣ Aplicar la migración y crear la base de datos

```bash
dotnet ef database update
```

### 6️⃣ Verificar que la base de datos se creó correctamente

Puedes usar **SQL Server Management Studio (SSMS)** o **Azure Data Studio** para conectarte y verificar:

**Conexión LocalDB:**
- Servidor: `(localdb)\mssqllocaldb`
- Autenticación: Windows Authentication
- Base de datos: `IncidentesFISEI`

## 📊 Estructura de la base de datos creada

Las siguientes tablas serán creadas automáticamente:

### Tablas principales:
- `Usuarios` - Información de usuarios del sistema
- `Categorias` - Categorías de incidentes  
- `Incidentes` - Registro de incidentes
- `ArticulosConocimiento` - Base de conocimiento
- `ComentariosIncidente` - Comentarios de incidentes
- `ComentariosArticulo` - Comentarios de artículos
- `ArchivosAdjuntos` - Archivos adjuntos
- `IncidentesRelacionados` - Relaciones entre incidentes
- `RegistrosTiempo` - Registro de tiempo trabajado
- `SLAs` - Configuración de SLA
- `EscalacionesSLA` - Escalaciones de SLA
- `VotacionesArticulo` - Votaciones de artículos

### Datos iniciales incluidos:
- **Usuario administrador**: admin / Admin123!
- **5 Categorías predefinidas**: Hardware, Software, Red, Acceso, Correo
- **4 SLAs por defecto**: Crítico, Alto, Medio, Bajo

## 🔧 Comandos útiles de Entity Framework

### Ver migraciones pendientes:
```bash
dotnet ef migrations list
```

### Crear nueva migración:
```bash
dotnet ef migrations add NombreDeLaMigracion
```

### Revertir migración:
```bash
dotnet ef database update NombreMigracionAnterior
```

### Eliminar última migración (si no se ha aplicado):
```bash
dotnet ef migrations remove
```

### Generar script SQL:
```bash
dotnet ef migrations script
```

### Ver información de la base de datos:
```bash
dotnet ef dbcontext info
```

## 🚨 Solución de problemas comunes

### Error: "LocalDB no está instalado"
```bash
# Descargar e instalar SQL Server Express LocalDB desde:
# https://www.microsoft.com/en-us/sql-server/sql-server-downloads
```

### Error: "Cannot connect to LocalDB"
```bash
# Verificar que LocalDB esté funcionando:
sqllocaldb info mssqllocaldb
sqllocaldb start mssqllocaldb
```

### Error: "Database already exists"
```bash
# Eliminar base de datos existente:
dotnet ef database drop
# Luego aplicar migración nuevamente:
dotnet ef database update
```

### Error: "Build failed"
```bash
# Compilar el proyecto primero:
dotnet build
# Luego ejecutar la migración:
dotnet ef database update
```

## 🔐 Usuario administrador por defecto

Después de crear la base de datos, podrás acceder con:
- **Usuario**: admin
- **Contraseña**: Admin123!
- **Email**: admin@fisei.uta.edu.ec

## 🎯 Siguientes pasos

1. ✅ Crear la base de datos (este paso)
2. ▶️ Ejecutar la API: `dotnet run` en `IncidentesFISEI.Api`
3. ▶️ Ejecutar el cliente: `dotnet run` en `IncidentesFISEI.Blazor`
4. 🌐 Abrir navegador en `https://localhost:5001`
5. 🔑 Iniciar sesión con las credenciales del administrador