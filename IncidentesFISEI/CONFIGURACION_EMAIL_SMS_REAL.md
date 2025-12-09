# 📧 GUÍA DE CONFIGURACIÓN - Notificaciones por Email

## ✅ ESTADO ACTUAL

### ✅ **EMAIL - LISTO PARA CONFIGURAR**
**Sistema:** Configurado para usar Outlook (@uta.edu.ec)

**Estado:** Requiere que agregues tu email y contraseña en `appsettings.json`

### ❌ **SMS - ELIMINADO**
**Decisión:** Se eliminó completamente la funcionalidad SMS del sistema por no ser necesaria y evitar simulaciones.

---

## 📧 CÓMO CONFIGURAR TU EMAIL DE OUTLOOK (@uta.edu.ec)

### Paso 1: Abrir appsettings.json

**Ubicación del archivo:**
```
C:\Users\USUARIO\Documents\IHC\proyecto\HCI\IncidentesFISEI\IncidentesFISEI.Api\appsettings.json
```

### Paso 2: Agregar tus credenciales

Busca la sección `EmailSettings` y reemplaza con TU información:

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp-mail.outlook.com",
    "SmtpPort": 587,
    "SmtpUser": "tu_nombre@uta.edu.ec",           // ← TU EMAIL REAL
    "SmtpPassword": "tu_contraseña_aqui",         // ← TU CONTRASEÑA REAL
    "FromEmail": "tu_nombre@uta.edu.ec",          // ← TU EMAIL REAL
    "FromName": "Sistema IncidentesFISEI - UTA"
  }
}
```

**Ejemplo con email real:**
```json
{
  "EmailSettings": {
    "SmtpHost": "smtp-mail.outlook.com",
    "SmtpPort": 587,
    "SmtpUser": "jperez@uta.edu.ec",
    "SmtpPassword": "MiContraseña123!",
    "FromEmail": "jperez@uta.edu.ec",
    "FromName": "Sistema IncidentesFISEI - UTA"
  }
}
```

### Paso 3: Verificar configuración de Outlook

#### Opción A: Si tu cuenta de Outlook funciona normalmente

✅ Solo necesitas poner tu email y contraseña directamente.

#### Opción B: Si tu cuenta tiene verificación en 2 pasos

Si tienes activada la verificación en 2 pasos en tu cuenta @uta.edu.ec:

1. **Ir a:** https://account.microsoft.com/security
2. **Crear contraseña de aplicación:**
   - Busca "Contraseñas de aplicación"
   - Crea una nueva
   - Copia la contraseña generada
3. **Usar esa contraseña** en lugar de tu contraseña normal

### Paso 4: Reiniciar la API

```powershell
# Detener la API si está corriendo (Ctrl+C en la terminal)
# Volver a iniciar:
cd "c:\Users\USUARIO\Documents\IHC\proyecto\HCI\IncidentesFISEI\IncidentesFISEI.Api"
dotnet run --urls "http://localhost:7001"
```

### Paso 5: Probar envío de Email

1. Iniciar sesión como administrador
2. Ir a **Configuración → Notificaciones**
3. Verificar que Email esté **habilitado** ✅
4. Crear un incidente de prueba
5. Verificar que llegue el email a tu bandeja

---

## 🔐 SEGURIDAD IMPORTANTE

### ⚠️ NO SUBIR CREDENCIALES A GIT

Tu archivo `appsettings.json` con credenciales reales **NO DEBE** subirse a GitHub.

**Solución: Agregar al .gitignore**

Edita el archivo `.gitignore` en la raíz del proyecto y agrega:

```gitignore
# Configuración con credenciales
**/appsettings.json
**/appsettings.*.json
!**/appsettings.Development.json.template
```

**O mejor aún, crear un archivo de plantilla:**

1. Copia `appsettings.json` a `appsettings.json.template`
2. En el template, deja las credenciales vacías
3. Sube el template a Git
4. Mantén tu `appsettings.json` real en `.gitignore`

---

## 🧪 PROBAR QUE FUNCIONA

### Test 1: Email de Incidente Nuevo

1. Login como usuario normal
2. Crear un nuevo incidente
3. Asignar a un técnico
4. **Verificar:** El técnico debe recibir email de notificación

### Test 2: Email de Escalamiento

1. Login como técnico
2. Dejar que un incidente supere su SLA
3. **Verificar:** El supervisor debe recibir email de escalamiento

### Test 3: Centro de Notificaciones

1. Login con cualquier usuario
2. Click en el ícono de campana 🔔
3. **Verificar:** Las notificaciones también aparecen en sistema

---

## 💰 COSTOS

### Email con Outlook @uta.edu.ec
- **Costo:** GRATIS (parte de tu cuenta institucional)
- **Límite:** ~300 emails por día
- **Recomendación:** Suficiente para un sistema universitario

---

## 📊 CANALES DE NOTIFICACIÓN DISPONIBLES

| Canal | Estado | Descripción |
|-------|--------|-------------|
| **Email** | ✅ Configurado | Notificaciones por correo electrónico |
| **Sistema** | ✅ Activo | Centro de notificaciones en la aplicación (campana 🔔) |
| ~~**SMS**~~ | ❌ Eliminado | Se removió para mantener el sistema 100% real |

---

## ❓ SOLUCIÓN DE PROBLEMAS

### ❌ No llegan los emails

**Verificar:**
1. ¿Pusiste tu email y contraseña correctos?
2. ¿Reiniciaste la API después de modificar appsettings.json?
3. ¿Revisaste la carpeta de spam?
4. ¿Tu cuenta de Outlook está activa?

**Revisar logs:**
```powershell
# En la terminal donde corre la API, busca errores como:
# [ERROR] EmailService: Failed to send email
```

### ❌ Error: "Authentication failed"

**Solución:**
- Si tienes verificación en 2 pasos, usa contraseña de aplicación (ver Paso 3, Opción B)
- Verifica que tu cuenta @uta.edu.ec esté activa

### ❌ Error: "SmtpHost could not be resolved"

**Solución:**
- Verifica que `SmtpHost` sea exactamente: `smtp-mail.outlook.com`
- Verifica tu conexión a internet

---

## 📝 RESUMEN RÁPIDO

```json
// 1. Abre: IncidentesFISEI.Api/appsettings.json

// 2. Modifica EmailSettings:
{
  "EmailSettings": {
    "SmtpHost": "smtp-mail.outlook.com",
    "SmtpPort": 587,
    "SmtpUser": "TU_EMAIL@uta.edu.ec",        // ← Cambia esto
    "SmtpPassword": "TU_CONTRASEÑA",          // ← Cambia esto
    "FromEmail": "TU_EMAIL@uta.edu.ec",       // ← Cambia esto
    "FromName": "Sistema IncidentesFISEI - UTA"
  }
}

// 3. Guarda el archivo

// 4. Reinicia la API

// 5. ¡Listo! Ya puedes recibir notificaciones por email
```

---

## ✅ VENTAJAS DE SOLO EMAIL + SISTEMA

1. **Sin costos adicionales** - No hay que pagar SMS
2. **100% real** - Todo funciona sin simulaciones
3. **Suficiente para el proyecto** - Email institucional + notificaciones en sistema
4. **Fácil de mantener** - Menos código, menos bugs
5. **Cumple ITIL v3** - Registro y trazabilidad completa

---

**Última actualización:** 7 de Diciembre de 2025  
**Sistema:** IncidentesFISEI - Universidad Técnica de Ambato
