# Sistema de Registro y Asignación de Roles - FISEI Incidentes

## 📋 Descripción General

El sistema de gestión de usuarios implementa un flujo seguro donde:
1. Los usuarios se registran sin rol asignado
2. Deben verificar su correo electrónico
3. Un administrador (rol SPOC) asigna el rol correspondiente
4. Solo entonces pueden iniciar sesión y usar el sistema

## 🔐 Roles del Sistema

| Rol | Descripción | Nivel |
|-----|-------------|-------|
| **SPOC** | Single Point of Contact - Administrador principal que asigna tickets | Admin |
| **ServiceDesk** | Personal del Service Desk que gestiona tickets | Soporte |
| **SupportN1** | Técnicos de Nivel 1 - Primer nivel de soporte | Nivel 1 |
| **SupportN2** | Técnicos de Nivel 2 - Segundo nivel de soporte | Nivel 2 |
| **SupportN3** | Expertos/Proveedores - Tercer nivel de soporte | Nivel 3 |

## 📝 Flujo de Registro

### 1. Registro de Usuario Nuevo

**Endpoint:** `POST /api/auth/register`

**Body:**
```json
{
  "nombre": "Juan Pérez",
  "correo": "juan.perez@uta.edu.ec",
  "contrasena": "Contraseña123!",
  "confirmarContrasena": "Contraseña123!"
}
```

**Respuesta exitosa:**
```json
{
  "message": "Registro exitoso. Por favor verifica tu correo. Un administrador asignará tu rol posteriormente.",
  "idUsuario": 5
}
```

**Correos permitidos:**
- Dominio UTA: `@uta.edu.ec` (estudiantes/docentes)
- Outlook: `@outlook.com`, `@hotmail.com`, `@live.com`

### 2. Verificación de Correo

El usuario recibirá un correo con un enlace para verificar su cuenta.

**Endpoint:** `GET /api/auth/verify-email?email={correo}&token={token}`

### 3. Intentar Iniciar Sesión (Sin Rol)

**Endpoint:** `POST /api/auth/login`

**Body:**
```json
{
  "email": "juan.perez@uta.edu.ec",
  "password": "Contraseña123!"
}
```

**Respuesta sin rol:**
```json
{
  "code": "NO_ROLE_ASSIGNED",
  "message": "No tienes un rol asignado. Contacta al administrador."
}
```

## 👨‍💼 Funciones del Administrador (SPOC)

### 1. Obtener Todos los Usuarios

**Endpoint:** `GET /api/usuarios`

**Headers:**
```
Authorization: Bearer {token_admin}
```

**Respuesta:**
```json
[
  {
    "idUsuario": 5,
    "nombre": "Juan Pérez",
    "correo": "juan.perez@uta.edu.ec",
    "activo": true,
    "emailVerificado": true,
    "idRol": null,
    "nombreRol": null,
    "descripcionRol": null
  }
]
```

### 2. Obtener Roles Disponibles

**Endpoint:** `GET /api/usuarios/roles`

**Headers:**
```
Authorization: Bearer {token_admin}
```

**Respuesta:**
```json
[
  {
    "idRol": 1,
    "nombre": "SPOC",
    "descripcion": "Rol SPOC"
  },
  {
    "idRol": 2,
    "nombre": "ServiceDesk",
    "descripcion": "Rol ServiceDesk"
  },
  {
    "idRol": 3,
    "nombre": "SupportN1",
    "descripcion": "Rol SupportN1"
  },
  {
    "idRol": 4,
    "nombre": "SupportN2",
    "descripcion": "Rol SupportN2"
  },
  {
    "idRol": 5,
    "nombre": "SupportN3",
    "descripcion": "Rol SupportN3"
  }
]
```

### 3. Asignar Rol a Usuario

**Endpoint:** `POST /api/usuarios/asignar-rol`

**Headers:**
```
Authorization: Bearer {token_admin}
```

**Body:**
```json
{
  "idUsuario": 5,
  "idRol": 3
}
```

**Respuesta:**
```json
{
  "message": "Rol 'SupportN1' asignado exitosamente al usuario 'Juan Pérez'",
  "usuario": {
    "idUsuario": 5,
    "nombre": "Juan Pérez",
    "correo": "juan.perez@uta.edu.ec",
    "activo": true,
    "emailVerificado": true,
    "idRol": 3,
    "nombreRol": "SupportN1",
    "descripcionRol": "Rol SupportN1"
  }
}
```

### 4. Remover Rol de Usuario

**Endpoint:** `POST /api/usuarios/{id}/remover-rol`

**Headers:**
```
Authorization: Bearer {token_admin}
```

**Ejemplo:** `POST /api/usuarios/5/remover-rol`

### 5. Activar/Desactivar Usuario

**Endpoint:** `POST /api/usuarios/{id}/toggle-activo`

**Headers:**
```
Authorization: Bearer {token_admin}
```

**Ejemplo:** `POST /api/usuarios/5/toggle-activo`

## 🔑 Login Exitoso (Con Rol Asignado)

**Endpoint:** `POST /api/auth/login`

**Body:**
```json
{
  "email": "juan.perez@uta.edu.ec",
  "password": "Contraseña123!"
}
```

**Respuesta exitosa:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "roles": ["SupportN1"],
  "usuario": {
    "idUsuario": 5,
    "nombre": "Juan Pérez",
    "correo": "juan.perez@uta.edu.ec",
    "rol": "SupportN1"
  }
}
```

## 👤 Endpoints para Usuarios Autenticados

### Obtener Mi Perfil

**Endpoint:** `GET /api/usuarios/mi-perfil`

**Headers:**
```
Authorization: Bearer {token}
```

## 🧪 Probar con Swagger

1. Inicia la aplicación: `dotnet run`
2. Abre: http://localhost:5023/swagger
3. Prueba los endpoints en este orden:
   - `POST /api/auth/register` - Registrar usuario
   - `GET /api/auth/verify-email` - Verificar correo (copiar token del email)
   - `POST /api/auth/login` - Intentar login con admin SPOC
   - `GET /api/usuarios` - Ver usuarios registrados
   - `GET /api/usuarios/roles` - Ver roles disponibles
   - `POST /api/usuarios/asignar-rol` - Asignar rol al nuevo usuario
   - `POST /api/auth/login` - Login con el nuevo usuario

## 🔒 Seguridad

- Las contraseñas se hashean con PBKDF2 antes de almacenarse
- Los tokens JWT expiran en 60 minutos (configurable en appsettings.json)
- Los endpoints de administración requieren rol SPOC
- Los usuarios deben verificar su correo antes de iniciar sesión
- Los usuarios deben tener un rol asignado para iniciar sesión

## 📊 Base de Datos

### Usuario Demo (Creado automáticamente)

- **Correo:** demo@fisei.local
- **Contraseña:** (ya está hasheada en la base de datos)
- **Rol:** SupportN1
- **Email Verificado:** Sí

Para crear un usuario SPOC inicial, debes modificar el archivo `DomainSeeder.cs` o insertarlo manualmente en la base de datos.

## 🛠️ Arquitectura del Código

```
Core/
  DTOs/
    RegisterDTO.cs          - DTOs para registro y asignación de roles
  Entities/
    Usuario.cs              - Entidad de usuario con relación a Rol
    Rol.cs                  - Entidad de rol
  Interfaces/
    IRepositories/
      IUsuarioRepository.cs - Interfaz para operaciones de usuario
      IRolRepository.cs     - Interfaz para operaciones de rol

Infrastructure/
  Data/
    Repositories/
      UsuarioRepository.cs  - Implementación con Include(Rol)
      RolRepository.cs      - Implementación de repositorio de roles

Presentation/
  Controllers/
    AuthController.cs       - Registro, login, verificación
    UsuariosController.cs   - Administración de usuarios y roles
```

## 📌 Notas Importantes

1. El rol **SPOC** es el único que puede asignar roles a otros usuarios
2. Los usuarios recién registrados **NO PUEDEN** iniciar sesión hasta que:
   - Verifiquen su correo
   - Un administrador les asigne un rol
3. El sistema valida que solo correos de UTA u Outlook puedan registrarse
4. Las contraseñas deben tener mínimo 6 caracteres
