# ✅ Datos de Prueba Poblados Exitosamente

## 📊 Resumen de Población

La base de datos `IncidentesFISEI_Dev` ha sido poblada con datos de prueba para validar el funcionamiento completo del sistema.

---

## 🎯 Datos Insertados

### **Incidentes (6 totales)**

| Número | Título | Estado | Prioridad | Reportado Por | Asignado A |
|--------|--------|--------|-----------|---------------|------------|
| INC-2024-001 | Servidor de correo no responde | ✅ CERRADO | 🔴 Crítica | Ana Pérez (Docente) | Carlos Mendoza (Supervisor) |
| INC-2024-002 | WiFi intermitente Lab 3 | ✅ CERRADO | 🟡 Alta | José Morales (Estudiante) | Luis Ramírez (Técnico) |
| INC-2024-003 | Impresora sin toner Lab 2 | ✅ CERRADO | 🟢 Media | Carmen Torres (Estudiante) | María González (Técnico) |
| INC-2024-004 | Error 403 Aula Virtual | 🔄 EN PROCESO | 🟡 Alta | Ana Pérez (Docente) | María González (Técnico) |
| INC-2024-005 | Solicitud AutoCAD Lab 5 | 🆕 NUEVO | ⚪ Baja | Ana Pérez (Docente) | Sin asignar |
| INC-2024-006 | URGENTE: Servidor proyectos | ⚡ ESCALADO | 🔴 Crítica | Carlos Mendoza (Supervisor) | Carlos Mendoza (Supervisor) |

### **Distribución por Estado**
- ✅ **Cerrados**: 3 (50%)
- 🔄 **En Proceso**: 1 (17%)
- ⚡ **Escalados**: 1 (17%)
- 🆕 **Nuevos**: 1 (16%)

### **Distribución por Prioridad**
- 🔴 **Crítica**: 2 incidentes
- 🟡 **Alta**: 2 incidentes
- 🟢 **Media**: 1 incidente
- ⚪ **Baja**: 1 incidente

---

## 📩 Notificaciones (3 totales)

| Usuario | Título | Tipo | Estado | Incidente |
|---------|--------|------|--------|-----------|
| Ana Pérez | Incidente Asignado | Asignación | No leída | INC-2024-004 |
| María González | Incidente Asignado | Asignación | Leída | INC-2024-003 |
| Carlos Mendoza | Incidente Crítico | Alerta | No leída | INC-2024-006 |

---

## 👥 Usuarios del Sistema (7)

| ID | Username | Nombre Completo | Rol | Estado |
|----|----------|----------------|-----|--------|
| 1 | admin | Administrador Sistema | Administrador | ✅ Activo |
| 2 | supervisor1 | Carlos Mendoza | Supervisor | ✅ Activo |
| 3 | tecnico1 | María González | Técnico | ✅ Activo |
| 4 | tecnico2 | Luis Ramírez | Técnico | ✅ Activo |
| 5 | docente1 | Ana Pérez | Usuario | ✅ Activo |
| 6 | estudiante1 | José Morales | Usuario | ❌ Inactivo |
| 7 | estudiante2 | Carmen Torres | Usuario | ❌ Inactivo |

---

## 📂 Categorías de Incidentes (5)

1. **Hardware** - Problemas relacionados con hardware (🔴 #dc3545)
2. **Software** - Problemas relacionados con software (🔵 #007bff)
3. **Red** - Problemas de conectividad y red (🟢 #28a745)
4. **Acceso** - Problemas de autenticación y permisos (🟡 #ffc107)
5. **Correo** - Problemas con correo electrónico (🔷 #17a2b8)

---

## 🛠️ Servicios DITIC (10)

| Código | Nombre | Tipo | SLA | Disponibilidad |
|--------|--------|------|-----|----------------|
| SRV-001 | Acceso a Internet WiFi | Redes | Alta | 99.0% |
| SRV-002 | Correo Institucional | Comunicaciones | Crítico | 99.9% |
| SRV-003 | Laboratorios de Computación | Hardware | Medio | 98.0% |
| SRV-004 | Soporte Técnico Help Desk | Soporte | Alta | 100% |
| SRV-005 | Sistema de Gestión Académica | Aplicaciones | Crítico | 99.5% |
| SRV-006 | Aula Virtual (Moodle) | Aplicaciones | Alta | 99.0% |
| SRV-007 | Impresión y Fotocopiado | Hardware | Medio | 95.0% |
| SRV-008 | VPN Institucional | Seguridad | Medio | 98.0% |
| SRV-009 | Repositorio Digital | Datos | Bajo | 99.5% |
| SRV-010 | Licencias de Software | Aplicaciones | Medio | 99.0% |

---

## 🔍 Casos de Prueba Cubiertos

### **✅ Flujo Completo: Incidente Cerrado**
- **INC-2024-001** (Servidor correo)
  - ✓ Reportado por usuario
  - ✓ Asignado a técnico
  - ✓ Tiempo de respuesta registrado
  - ✓ Solución aplicada
  - ✓ Incidente cerrado
  - ✓ Servicio DITIC vinculado

### **🔄 Flujo en Progreso: Incidente Activo**
- **INC-2024-004** (Aula Virtual)
  - ✓ Reportado y asignado
  - ✓ Técnico trabajando
  - ✓ Notificaciones enviadas
  - ⏳ Pendiente de resolución

### **⚡ Flujo de Escalación: Incidente Crítico**
- **INC-2024-006** (Servidor proyectos)
  - ✓ Prioridad crítica
  - ✓ Escalado a supervisor
  - ✓ Notificación de alerta
  - ✓ Nivel de soporte elevado
  - ⏳ En atención prioritaria

### **🆕 Flujo Inicial: Incidente Nuevo**
- **INC-2024-005** (Solicitud software)
  - ✓ Reportado
  - ⏳ Pendiente de asignación
  - ⏳ Sin actividad registrada

---

## 🧪 Pruebas Sugeridas

### **1. Autenticación**
```
Usuario: admin
Password: Admin123!
```

### **2. Consulta de Incidentes**
```sql
SELECT * FROM Incidentes WHERE IsDeleted = 0;
```

### **3. Verificar Relaciones**
```sql
-- Incidentes con usuario reportador
SELECT i.NumeroIncidente, i.Titulo, u.FirstName + ' ' + u.LastName AS Usuario
FROM Incidentes i
INNER JOIN Usuarios u ON i.ReportadoPorId = u.Id;

-- Incidentes con servicio DITIC
SELECT i.NumeroIncidente, s.Nombre AS Servicio
FROM Incidentes i
INNER JOIN ServiciosDITIC s ON i.ServicioDITICId = s.Id
WHERE i.ServicioDITICId IS NOT NULL;
```

### **4. Probar Notificaciones**
```sql
SELECT u.Username, n.Titulo, n.Mensaje, n.Leida
FROM Notificaciones n
INNER JOIN Usuarios u ON n.UsuarioId = u.Id
WHERE n.IsDeleted = 0;
```

---

## 📝 Notas Importantes

### **Integridad Referencial**
✅ Todas las FK están correctamente vinculadas:
- Incidentes → Usuarios (ReportadoPor, AsignadoA)
- Incidentes → Categorías
- Incidentes → Servicios DITIC
- Notificaciones → Usuarios
- Notificaciones → Incidentes

### **Campos con Datos Realistas**
- Fechas relativas (hace 7 días, hace 45 minutos, etc.)
- Descripciones técnicas reales
- Nombres de servicios DITIC reales de FISEI-UTA
- Estados variados para cubrir todo el flujo

### **Soft Deletes**
Todos los registros tienen `IsDeleted = 0` (activos)

---

## 🚀 Próximos Pasos

1. **Probar API REST**:
   ```powershell
   cd IncidentesFISEI.Api
   dotnet run
   ```
   Navegar a: `https://localhost:7xxx/swagger`

2. **Verificar Endpoints**:
   - GET /api/incidentes
   - GET /api/incidentes/INC-2024-004
   - POST /api/incidentes (crear nuevo)
   - PUT /api/incidentes/{id}
   - GET /api/notificaciones
   - GET /api/auditlog

3. **Probar Autenticación**:
   - POST /api/auth/login con usuario `admin`
   - Verificar JWT token
   - Probar endpoints protegidos

4. **Validar Búsquedas**:
   - Filtrar incidentes por estado
   - Filtrar por prioridad
   - Búsqueda por número de incidente
   - Búsqueda por usuario asignado

---

## ✅ Validación Final

**Base de Datos**: `IncidentesFISEI_Dev` ✓  
**Servidor**: `.\SQLEXPRESS` ✓  
**Registros Insertados**: 26+ registros ✓  
**Integridad Referencial**: Validada ✓  
**Datos Listos para Testing**: ✓

---

**Sistema listo para pruebas funcionales y de integración** 🎉
