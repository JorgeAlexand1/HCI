# 🚀 Guía de Inicio Rápido - Sistema de Registro y Roles

## ✅ Estado Actual

**La aplicación está corriendo en:** http://localhost:5023
**Swagger UI:** http://localhost:5023/swagger

## 🔑 Credenciales de Acceso

### Usuario Administrador (SPOC) - **ÚSALO PARA PROBAR**
```
📧 Correo:     admin@uta.edu.ec
🔒 Contraseña: Admin123!
👤 Rol:        SPOC (Administrador)
✅ Estado:     Email verificado, Activo
```

### Usuario Demo (Soporte Nivel 1)
```
📧 Correo:     demo@fisei.local
🔒 Contraseña: Demo#2025
👤 Rol:        SupportN1
✅ Estado:     Email verificado, Activo
```

## 🧪 Prueba Rápida en Swagger

### 1️⃣ Iniciar Sesión como Admin
1. Abre: http://localhost:5023/swagger
2. Busca el endpoint `POST /api/auth/login`
3. Click en "Try it out"
4. Pega este JSON:
   ```json
   {
     "email": "admin@uta.edu.ec",
     "password": "Admin123!"
   }
   ```
5. Click en "Execute"
6. **Copia el token** de la respuesta

### 2️⃣ Autorizar Requests
1. Click en el botón verde "Authorize" (arriba a la derecha)
2. Pega el token
3. Click en "Authorize"
4. Click en "Close"

### 3️⃣ Ver Todos los Usuarios
1. Busca `GET /api/usuarios`
2. Click en "Try it out"
3. Click en "Execute"
4. Verás los usuarios existentes

### 4️⃣ Ver Roles Disponibles
1. Busca `GET /api/usuarios/roles`
2. Click en "Try it out"
3. Click en "Execute"
4. Verás los 5 roles:
   - SPOC (ID: 1)
   - ServiceDesk (ID: 2)
   - SupportN1 (ID: 3)
   - SupportN2 (ID: 4)
   - SupportN3 (ID: 5)

### 5️⃣ Registrar un Nuevo Usuario
1. Busca `POST /api/auth/register`
2. Click en "Try it out"
3. Pega este JSON:
   ```json
   {
     "nombre": "María García",
     "correo": "maria.garcia@uta.edu.ec",
     "contrasena": "Maria123!",
     "confirmarContrasena": "Maria123!"
   }
   ```
4. Click en "Execute"
5. Anota el `idUsuario` de la respuesta (probablemente 3)

### 6️⃣ Verificar Email Manualmente (Para Testing)
Ejecuta esta consulta SQL en SQL Server:
```sql
UPDATE Usuarios 
SET EmailVerificado = 1, EmailVerificationToken = NULL
WHERE Correo = 'maria.garcia@uta.edu.ec';
```

### 7️⃣ Intentar Login Sin Rol
1. Busca `POST /api/auth/login`
2. Intenta con las credenciales de María:
   ```json
   {
     "email": "maria.garcia@uta.edu.ec",
     "password": "Maria123!"
   }
   ```
3. **Debería fallar** con mensaje: "No tienes un rol asignado"

### 8️⃣ Asignar Rol como Admin
1. Asegúrate de estar autenticado como admin (paso 1-2)
2. Busca `POST /api/usuarios/asignar-rol`
3. Click en "Try it out"
4. Pega este JSON (ajusta el idUsuario si es necesario):
   ```json
   {
     "idUsuario": 3,
     "idRol": 3
   }
   ```
5. Click en "Execute"

### 9️⃣ Login Exitoso con Rol
1. Busca `POST /api/auth/login`
2. Ahora María sí puede iniciar sesión:
   ```json
   {
     "email": "maria.garcia@uta.edu.ec",
     "password": "Maria123!"
   }
   ```
3. **Debería funcionar** y recibir un token

## 📊 Consultas SQL Útiles

### Ver todos los usuarios con sus roles
```sql
SELECT 
    u.IdUsuario,
    u.Nombre,
    u.Correo,
    u.EmailVerificado,
    u.Activo,
    r.Nombre AS Rol
FROM Usuarios u
LEFT JOIN ROL r ON u.IdRol = r.IdRol
ORDER BY u.IdUsuario;
```

### Ver todos los roles
```sql
SELECT * FROM ROL ORDER BY IdRol;
```

### Verificar email de un usuario
```sql
UPDATE Usuarios 
SET EmailVerificado = 1, EmailVerificationToken = NULL
WHERE Correo = 'usuario@ejemplo.com';
```

### Asignar rol manualmente
```sql
-- Asignar rol SPOC (ID 1) a un usuario
UPDATE Usuarios 
SET IdRol = 1
WHERE Correo = 'usuario@ejemplo.com';
```

## 🎯 Flujo Completo de Registro

```
┌─────────────────────┐
│ 1. Usuario se       │
│    registra         │──► Sin rol asignado
│    (público)        │   Email no verificado
└─────────────────────┘
         │
         ▼
┌─────────────────────┐
│ 2. Usuario verifica │
│    su email         │──► Correo verificado
└─────────────────────┘   Aún sin rol
         │
         ▼
┌─────────────────────┐
│ 3. Usuario intenta  │
│    hacer login      │──► ❌ Error: Sin rol asignado
└─────────────────────┘
         │
         ▼
┌─────────────────────┐
│ 4. Admin (SPOC)     │
│    asigna rol       │──► Rol asignado
└─────────────────────┘
         │
         ▼
┌─────────────────────┐
│ 5. Usuario hace     │
│    login            │──► ✅ Acceso concedido
└─────────────────────┘   Token JWT generado
```

## 🔐 Endpoints Protegidos por Rol

### Solo SPOC (Administrador)
- `GET /api/usuarios` - Listar usuarios
- `GET /api/usuarios/{id}` - Ver usuario
- `GET /api/usuarios/roles` - Listar roles
- `POST /api/usuarios/asignar-rol` - Asignar rol
- `POST /api/usuarios/{id}/remover-rol` - Quitar rol
- `POST /api/usuarios/{id}/toggle-activo` - Activar/desactivar

### Cualquier Usuario Autenticado
- `GET /api/usuarios/mi-perfil` - Ver mi perfil
- `GET /api/usuarios/tecnicos/nivel/{id}` - Ver técnicos
- `GET /api/usuarios/{id}/es-spoc` - Verificar si es SPOC

### Públicos (Sin Autenticación)
- `POST /api/auth/register` - Registrarse
- `POST /api/auth/login` - Iniciar sesión
- `GET /api/auth/verify-email` - Verificar email
- `POST /api/auth/request-password-reset` - Solicitar reset
- `POST /api/auth/confirm-password-reset` - Confirmar reset

## 📝 Documentación Adicional

- **REGISTRO_USUARIOS.md** - Documentación completa de la API
- **IMPLEMENTACION_COMPLETADA.md** - Detalles técnicos de la implementación
- **Scripts/CreateAdminUser.sql** - Script SQL manual (ya no necesario)

## ❓ Solución de Problemas

### No puedo ver los endpoints de usuarios en Swagger
- Asegúrate de hacer click en "Authorize" con el token del admin

### El usuario no puede iniciar sesión
1. Verifica que EmailVerificado = 1
2. Verifica que IdRol no sea NULL
3. Verifica que Activo = 1

### Olvidé la contraseña del admin
Ejecuta en SQL Server:
```sql
UPDATE Usuarios 
SET Contrasena = 'NUEVO_HASH_AQUI'
WHERE Correo = 'admin@uta.edu.ec';
```

O reinicia la base de datos y vuelve a ejecutar las migraciones.

---

## 🎉 ¡Todo Listo!

El sistema está completamente funcional. Puedes:
- ✅ Registrar nuevos usuarios
- ✅ Verificar correos
- ✅ Asignar roles (como SPOC)
- ✅ Gestionar usuarios activos/inactivos
- ✅ Control de acceso basado en roles

**Siguiente paso:** Implementar la interfaz de usuario en Blazor para facilitar estas operaciones.
