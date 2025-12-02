-- ========================================
-- SCRIPT DE POBLACIÓN DE DATOS DE PRUEBA
-- Sistema de Gestión de Incidentes FISEI
-- ========================================

USE IncidentesFISEI_Dev;
GO

-- ========================================
-- 1. ARTÍCULOS DE CONOCIMIENTO
-- ========================================
INSERT INTO ArticulosConocimiento (Titulo, Contenido, Resumen, AutorId, CategoriaId, Estado, Tags, Relevancia, VecesUtilizado, CreatedAt)
VALUES 
(
    'Configuración de WiFi Institucional en Windows 10/11',
    '<h2>Configuración WiFi FISEI</h2><p>Para conectarse a la red WiFi institucional, siga estos pasos:</p><ol><li>Abrir configuración de red</li><li>Seleccionar red "UTA-FISEI"</li><li>Ingresar credenciales institucionales</li><li>Verificar conexión</li></ol>',
    'Guía paso a paso para configurar la conexión WiFi institucional en equipos Windows',
    3, -- Técnico María González
    3, -- Categoría: Red
    2, -- Estado: Publicado
    'wifi,red,windows,configuracion',
    5,
    12,
    DATEADD(DAY, -30, GETUTCDATE())
),
(
    'Recuperación de Contraseña de Correo Institucional',
    '<h2>Recuperar Contraseña</h2><p>Procedimiento oficial:</p><ol><li>Acceder a https://correo.uta.edu.ec/recuperar</li><li>Ingresar número de cédula</li><li>Verificar código enviado a teléfono registrado</li><li>Establecer nueva contraseña</li></ol>',
    'Procedimiento para restablecer la contraseña del correo @fisei.uta.edu.ec',
    3,
    5, -- Categoría: Correo
    2,
    'correo,password,recuperacion,email',
    5,
    25,
    DATEADD(DAY, -45, GETUTCDATE())
),
(
    'Instalación de Software en Laboratorios',
    '<h2>Solicitud de Software</h2><p>Para solicitar instalación de software especializado:</p><ol><li>Llenar formulario en intranet</li><li>Adjuntar licencia si aplica</li><li>Esperar aprobación del coordinador</li><li>Instalación programada en 48-72h</li></ol>',
    'Proceso para solicitar instalación de software en laboratorios de FISEI',
    4, -- Técnico Luis Ramírez
    2, -- Categoría: Software
    2,
    'software,laboratorio,instalacion',
    4,
    8,
    DATEADD(DAY, -20, GETUTCDATE())
),
(
    'Error "Impresora No Disponible" en Multifuncionales',
    '<h2>Solución Rápida</h2><p>Si aparece el error:</p><ol><li>Verificar que la impresora esté encendida</li><li>Revisar conexión de red</li><li>Reiniciar servicio de cola de impresión</li><li>Reinstalar driver si persiste</li></ol><p><strong>Comando PowerShell:</strong><br><code>Restart-Service Spooler</code></p>',
    'Solución al error común de impresoras multifuncionales en red',
    3,
    1, -- Categoría: Hardware
    2,
    'impresora,error,multifuncional,driver',
    5,
    18,
    DATEADD(DAY, -15, GETUTCDATE())
),
(
    'Acceso a Aula Virtual desde Casa',
    '<h2>VPN y Aula Virtual</h2><p>Para acceder desde fuera del campus:</p><ol><li>Descargar cliente VPN institucional</li><li>Configurar con credenciales UTA</li><li>Conectar a VPN</li><li>Acceder a https://aulavirtual.fisei.uta.edu.ec</li></ol>',
    'Guía para acceder al aula virtual Moodle desde fuera de la universidad',
    2, -- Supervisor Carlos Mendoza
    2,
    2,
    'vpn,aula virtual,moodle,remoto',
    5,
    35,
    DATEADD(DAY, -60, GETUTCDATE())
);

-- ========================================
-- 2. INCIDENTES DE PRUEBA
-- ========================================

-- Incidente 1: CRÍTICO - Caída de servidor de correo (CERRADO)
INSERT INTO Incidentes (NumeroIncidente, Titulo, Descripcion, ReportadoPorId, AsignadoAId, CategoriaId, 
    Prioridad, Estado, Urgencia, Impacto, FechaReporte, FechaAsignacion, FechaResolucion, FechaCierre,
    Solucion, TiempoRespuestaMinutos, TiempoResolucionMinutos, ServicioDITICId, ArticuloConocimientoId, CreatedAt)
VALUES 
(
    'INC-2024-001',
    'Servidor de correo institucional no responde',
    'Los usuarios reportan que no pueden acceder al correo institucional desde las 8:00 AM. Error: "No se puede conectar al servidor mail.uta.edu.ec". Afecta a toda la comunidad FISEI.',
    5, -- Reportado por: Docente Ana Pérez
    2, -- Asignado a: Supervisor Carlos Mendoza
    5, -- Categoría: Correo
    3, -- Prioridad: Crítica
    4, -- Estado: Cerrado
    3, -- Urgencia: Crítica
    3, -- Impacto: Crítico
    DATEADD(DAY, -7, GETUTCDATE()),
    DATEADD(MINUTE, 5, DATEADD(DAY, -7, GETUTCDATE())),
    DATEADD(MINUTE, 45, DATEADD(DAY, -7, GETUTCDATE())),
    DATEADD(MINUTE, 50, DATEADD(DAY, -7, GETUTCDATE())),
    'Se reinició el servicio Exchange Server. El problema fue causado por un proceso que consumía el 100% de CPU. Se identificó un bucle en script de sincronización. Se desactivó temporalmente y se programa mantenimiento preventivo.',
    5,   -- Respuesta en 5 minutos
    45,  -- Resolución en 45 minutos
    2,   -- Servicio: Correo Institucional
    NULL,
    DATEADD(DAY, -7, GETUTCDATE())
);

-- Incidente 2: ALTA - WiFi intermitente en Lab 3 (CERRADO)
INSERT INTO Incidentes (NumeroIncidente, Titulo, Descripcion, ReportadoPorId, AsignadoAId, CategoriaId, 
    Prioridad, Estado, Urgencia, Impacto, FechaReporte, FechaAsignacion, FechaResolucion, FechaCierre,
    Solucion, TiempoRespuestaMinutos, TiempoResolucionMinutos, ServicioDITICId, ArticuloConocimientoId, CreatedAt)
VALUES 
(
    'INC-2024-002',
    'Conexión WiFi intermitente en Laboratorio 3',
    'Estudiantes reportan desconexiones frecuentes del WiFi en Lab 3. La señal se pierde cada 5-10 minutos. Afecta a 30 estudiantes en clase de Programación Web.',
    6, -- Reportado por: Estudiante José Morales
    4, -- Asignado a: Técnico Luis Ramírez
    3, -- Categoría: Red
    2, -- Prioridad: Alta
    4, -- Estado: Cerrado
    2, -- Urgencia: Alta
    2, -- Impacto: Alto
    DATEADD(DAY, -5, GETUTCDATE()),
    DATEADD(MINUTE, 20, DATEADD(DAY, -5, GETUTCDATE())),
    DATEADD(HOUR, 2, DATEADD(DAY, -5, GETUTCDATE())),
    DATEADD(HOUR, 2, DATEADD(MINUTE, 10, DATEADD(DAY, -5, GETUTCDATE()))),
    'Se identificó interferencia en el canal 6. Access Point configurado en canal saturado. Se cambió a canal 11 y se ajustó potencia de transmisión. Se verificó estabilidad por 30 minutos. Problema resuelto.',
    20,  -- Respuesta en 20 minutos
    120, -- Resolución en 2 horas
    1,   -- Servicio: Acceso a Internet WiFi
    1,   -- Artículo: Configuración WiFi
    DATEADD(DAY, -5, GETUTCDATE())
);

-- Incidente 3: MEDIA - Impresora sin tinta (CERRADO)
INSERT INTO Incidentes (NumeroIncidente, Titulo, Descripcion, ReportadoPorId, AsignadoAId, CategoriaId, 
    Prioridad, Estado, Urgencia, Impacto, FechaReporte, FechaAsignacion, FechaResolucion, FechaCierre,
    Solucion, TiempoRespuestaMinutos, TiempoResolucionMinutos, ArticuloConocimientoId, CreatedAt)
VALUES 
(
    'INC-2024-003',
    'Impresora HP LaserJet Lab 2 sin tóner',
    'La impresora del Laboratorio 2 muestra mensaje "Toner bajo". Los estudiantes no pueden imprimir sus trabajos.',
    7, -- Reportado por: Estudiante Carmen Torres
    3, -- Asignado a: Técnico María González
    1, -- Categoría: Hardware
    1, -- Prioridad: Media
    4, -- Estado: Cerrado
    1, -- Urgencia: Media
    1, -- Impacto: Medio
    DATEADD(DAY, -3, GETUTCDATE()),
    DATEADD(MINUTE, 40, DATEADD(DAY, -3, GETUTCDATE())),
    DATEADD(HOUR, 4, DATEADD(DAY, -3, GETUTCDATE())),
    DATEADD(HOUR, 4, DATEADD(MINUTE, 5, DATEADD(DAY, -3, GETUTCDATE()))),
    'Se reemplazó cartucho de tóner. Se solicitó a bodega. Instalación completada y calibración realizada. Impresora operativa.',
    40,  -- Respuesta en 40 minutos
    240, -- Resolución en 4 horas
    4,   -- Artículo: Error impresora
    DATEADD(DAY, -3, GETUTCDATE())
);

-- Incidente 4: ALTA - No puede acceder al Aula Virtual (EN PROCESO)
INSERT INTO Incidentes (NumeroIncidente, Titulo, Descripcion, ReportadoPorId, AsignadoAId, CategoriaId, 
    Prioridad, Estado, Urgencia, Impacto, FechaReporte, FechaAsignacion, ServicioDITICId, CreatedAt)
VALUES 
(
    'INC-2024-004',
    'No puedo acceder al Aula Virtual - Error 403',
    'Desde ayer no puedo ingresar al aula virtual. Me aparece "Error 403: Acceso prohibido". He intentado desde diferentes navegadores y el error persiste. Necesito entregar una tarea hoy.',
    5, -- Reportado por: Docente Ana Pérez
    3, -- Asignado a: Técnico María González
    2, -- Categoría: Software
    2, -- Prioridad: Alta
    2, -- Estado: En Proceso
    2, -- Urgencia: Alta
    2, -- Impacto: Alto
    DATEADD(MINUTE, -45, GETUTCDATE()),
    DATEADD(MINUTE, -30, GETUTCDATE()),
    6, -- Servicio: Aula Virtual (Moodle)
    DATEADD(MINUTE, -45, GETUTCDATE())
);

-- Incidente 5: BAJA - Solicitud de software (NUEVO)
INSERT INTO Incidentes (NumeroIncidente, Titulo, Descripcion, ReportadoPorId, CategoriaId, 
    Prioridad, Estado, Urgencia, Impacto, FechaReporte, CreatedAt)
VALUES 
(
    'INC-2024-005',
    'Solicitud instalación AutoCAD 2024 en Lab 5',
    'Requiero que se instale AutoCAD 2024 en el Laboratorio 5 para la materia de Diseño Asistido por Computadora. Necesito 20 licencias. La materia inicia la próxima semana.',
    5, -- Reportado por: Docente Ana Pérez
    2, -- Categoría: Software
    0, -- Prioridad: Baja
    0, -- Estado: Nuevo
    0, -- Urgencia: Baja
    1, -- Impacto: Medio
    DATEADD(MINUTE, -15, GETUTCDATE()),
    DATEADD(MINUTE, -15, GETUTCDATE())
);

-- Incidente 6: CRÍTICA - Pérdida de datos en servidor (ESCALADO)
INSERT INTO Incidentes (NumeroIncidente, Titulo, Descripcion, ReportadoPorId, AsignadoAId, CategoriaId, 
    Prioridad, Estado, Urgencia, Impacto, FechaReporte, FechaAsignacion, NivelEscalacion, FueEscalado, CreatedAt)
VALUES 
(
    'INC-2024-006',
    'URGENTE: Posible pérdida de datos en servidor de proyectos',
    'El servidor de almacenamiento de proyectos de titulación (srv-proyectos.fisei.uta.edu.ec) no responde. Al intentar acceder, aparece error de disco. Contiene 150+ proyectos de titulación de los últimos 3 años. CRÍTICO.',
    2, -- Reportado por: Supervisor Carlos Mendoza
    2, -- Asignado a: Supervisor Carlos Mendoza (escalado a sí mismo)
    1, -- Categoría: Hardware
    3, -- Prioridad: Crítica
    3, -- Estado: Escalado
    3, -- Urgencia: Crítica
    3, -- Impacto: Crítico
    DATEADD(MINUTE, -120, GETUTCDATE()),
    DATEADD(MINUTE, -118, GETUTCDATE()),
    2, -- Nivel de escalación: 2
    1, -- Fue escalado: Sí
    DATEADD(MINUTE, -120, GETUTCDATE())
);

-- ========================================
-- 3. COMENTARIOS EN INCIDENTES
-- ========================================

-- Comentarios Incidente 1 (Correo caído)
INSERT INTO ComentariosIncidente (IncidenteId, AutorId, Contenido, EsInterno, CreatedAt)
VALUES 
(1, 2, 'Confirmado. Servicio Exchange Server no responde. Iniciando diagnóstico.', 1, DATEADD(MINUTE, 10, DATEADD(DAY, -7, GETUTCDATE()))),
(1, 2, 'Identificado proceso w3wp.exe consumiendo 100% CPU. Investigando causa raíz.', 1, DATEADD(MINUTE, 20, DATEADD(DAY, -7, GETUTCDATE()))),
(1, 2, 'Problema resuelto. Servicio restaurado. Se requiere mantenimiento preventivo programado.', 0, DATEADD(MINUTE, 50, DATEADD(DAY, -7, GETUTCDATE())));

-- Comentarios Incidente 2 (WiFi intermitente)
INSERT INTO ComentariosIncidente (IncidenteId, AutorId, Contenido, EsInterno, CreatedAt)
VALUES 
(2, 4, 'En camino al Laboratorio 3 para verificar la señal WiFi.', 1, DATEADD(MINUTE, 25, DATEADD(DAY, -5, GETUTCDATE()))),
(2, 4, 'Detectada interferencia en el canal 6. AP vecino también en canal 6. Reconfigurando.', 1, DATEADD(MINUTE, 45, DATEADD(DAY, -5, GETUTCDATE()))),
(2, 4, 'Cambio de canal completado. Monitoreando estabilidad.', 0, DATEADD(HOUR, 1, DATEADD(MINUTE, 30, DATEADD(DAY, -5, GETUTCDATE()))));

-- Comentarios Incidente 4 (Aula Virtual)
INSERT INTO ComentariosIncidente (IncidenteId, AutorId, Contenido, EsInterno, CreatedAt)
VALUES 
(4, 3, 'Revisando permisos de usuario en Moodle. Posible problema de sesión.', 1, DATEADD(MINUTE, -25, GETUTCDATE())),
(4, 3, 'Identificado bloqueo de IP por intentos fallidos. Limpiando cache y desbloqueando.', 1, DATEADD(MINUTE, -10, GETUTCDATE()));

-- Comentarios Incidente 6 (Servidor crítico)
INSERT INTO ComentariosIncidente (IncidenteId, AutorId, Contenido, EsInterno, CreatedAt)
VALUES 
(6, 2, 'ESCALADO A CRÍTICO. Contactando con proveedor de hardware. Verificando backups.', 1, DATEADD(MINUTE, -110, GETUTCDATE())),
(6, 2, 'Backup más reciente: hace 24 horas. Contactado técnico de Dell. ETA: 2 horas.', 1, DATEADD(MINUTE, -90, GETUTCDATE()));

-- ========================================
-- 4. ARCHIVOS ADJUNTOS
-- ========================================

INSERT INTO ArchivosAdjuntos (IncidenteId, NombreOriginal, NombreArchivo, RutaArchivo, TipoMime, TamanioBytes, SubidoPorId, CreatedAt)
VALUES 
(1, 'screenshot-error-correo.png', 'INC001_1701234567_screenshot.png', '/uploads/incidentes/2024/12/INC001_1701234567_screenshot.png', 'image/png', 245678, 5, DATEADD(DAY, -7, GETUTCDATE())),
(2, 'analisis-wifi-lab3.pdf', 'INC002_1701345678_analisis.pdf', '/uploads/incidentes/2024/12/INC002_1701345678_analisis.pdf', 'application/pdf', 512340, 4, DATEADD(DAY, -5, GETUTCDATE())),
(4, 'error-403-captura.jpg', 'INC004_1701456789_error.jpg', '/uploads/incidentes/2024/12/INC004_1701456789_error.jpg', 'image/jpeg', 156789, 5, DATEADD(MINUTE, -40, GETUTCDATE()));

-- ========================================
-- 5. REGISTROS DE TIEMPO
-- ========================================

INSERT INTO RegistrosTiempo (IncidenteId, TecnicoId, MinutosTrabajados, Descripcion, FechaRegistro, CreatedAt)
VALUES 
(1, 2, 45, 'Diagnóstico y resolución de caída del servidor Exchange', DATEADD(DAY, -7, GETUTCDATE()), DATEADD(DAY, -7, GETUTCDATE())),
(2, 4, 30, 'Diagnóstico inicial de interferencia WiFi', DATEADD(DAY, -5, GETUTCDATE()), DATEADD(DAY, -5, GETUTCDATE())),
(2, 4, 90, 'Reconfiguración de Access Point y monitoreo', DATEADD(DAY, -5, GETUTCDATE()), DATEADD(DAY, -5, GETUTCDATE())),
(3, 3, 15, 'Reemplazo de cartucho de tóner', DATEADD(DAY, -3, GETUTCDATE()), DATEADD(DAY, -3, GETUTCDATE())),
(4, 3, 35, 'Diagnóstico de error 403 en Moodle', DATEADD(MINUTE, -30, GETUTCDATE()), DATEADD(MINUTE, -30, GETUTCDATE())),
(6, 2, 120, 'Diagnóstico crítico servidor + contacto con proveedor', DATEADD(MINUTE, -120, GETUTCDATE()), DATEADD(MINUTE, -120, GETUTCDATE()));

-- ========================================
-- 6. HISTORIAL DE ESCALACIONES
-- ========================================

INSERT INTO HistorialEscalaciones (IncidenteId, TecnicoOrigenId, TecnicoDestinoId, NivelAnterior, NivelNuevo, Razon, FechaEscalacion, CreatedAt)
VALUES 
(6, 3, 2, 1, 2, 'Incidente crítico con posible pérdida masiva de datos. Requiere intervención de supervisor y contacto con proveedores externos.', 
    DATEADD(MINUTE, -115, GETUTCDATE()), DATEADD(MINUTE, -115, GETUTCDATE()));

-- ========================================
-- 7. NOTIFICACIONES
-- ========================================

INSERT INTO Notificaciones (UsuarioId, Titulo, Mensaje, Tipo, Prioridad, Leida, IncidenteId, CreatedAt)
VALUES 
-- Notificaciones para Docente Ana Pérez (Usuario 5)
(5, 'Incidente Cerrado', 'Su incidente INC-2024-001 "Servidor de correo institucional no responde" ha sido resuelto.', 3, 2, 1, 1, DATEADD(DAY, -7, GETUTCDATE())),
(5, 'Incidente Asignado', 'Su incidente INC-2024-004 ha sido asignado al técnico María González.', 1, 1, 1, 4, DATEADD(MINUTE, -30, GETUTCDATE())),
(5, 'Nuevo Comentario', 'El técnico ha agregado un comentario en su incidente INC-2024-004.', 2, 1, 0, 4, DATEADD(MINUTE, -10, GETUTCDATE())),

-- Notificaciones para Técnico María González (Usuario 3)
(3, 'Incidente Asignado', 'Se le ha asignado el incidente INC-2024-003 "Impresora HP LaserJet Lab 2 sin tóner".', 1, 1, 1, 3, DATEADD(DAY, -3, GETUTCDATE())),
(3, 'Incidente Asignado', 'Se le ha asignado el incidente INC-2024-004 "No puedo acceder al Aula Virtual - Error 403".', 1, 2, 0, 4, DATEADD(MINUTE, -30, GETUTCDATE())),

-- Notificaciones para Supervisor Carlos Mendoza (Usuario 2)
(2, 'Incidente Crítico', 'ALERTA: Nuevo incidente crítico INC-2024-006 requiere atención inmediata.', 0, 3, 0, 6, DATEADD(MINUTE, -120, GETUTCDATE())),
(2, 'Escalación Requerida', 'El incidente INC-2024-006 ha sido escalado a su nivel.', 4, 3, 0, 6, DATEADD(MINUTE, -115, GETUTCDATE()));

-- ========================================
-- 8. VOTACIONES EN ARTÍCULOS
-- ========================================

INSERT INTO VotacionesArticulo (ArticuloConocimientoId, UsuarioId, EsUtil, Calificacion, Comentario, CreatedAt)
VALUES 
(1, 6, 1, 5, 'Muy clara la explicación, pude conectarme sin problemas.', DATEADD(DAY, -25, GETUTCDATE())),
(1, 7, 1, 4, 'Funcionó perfecto, aunque en mi laptop fue un poco diferente.', DATEADD(DAY, -20, GETUTCDATE())),
(2, 5, 1, 5, 'Excelente, recuperé mi contraseña en minutos.', DATEADD(DAY, -40, GETUTCDATE())),
(4, 3, 1, 5, 'Esta solución me ha salvado varias veces.', DATEADD(DAY, -10, GETUTCDATE())),
(5, 5, 1, 5, 'Perfecto para trabajo remoto durante la pandemia.', DATEADD(DAY, -55, GETUTCDATE()));

-- ========================================
-- 9. COMENTARIOS EN ARTÍCULOS
-- ========================================

INSERT INTO ComentariosArticulo (ArticuloConocimientoId, UsuarioId, Contenido, CreatedAt)
VALUES 
(1, 6, '¿Esta configuración también sirve para conectar desde Linux?', DATEADD(DAY, -24, GETUTCDATE())),
(1, 3, 'Sí, el proceso es similar. En Ubuntu: NetworkManager > Add Network > WPA2-Enterprise', DATEADD(DAY, -23, GETUTCDATE())),
(2, 7, 'No me llega el código al celular, ¿qué hago?', DATEADD(DAY, -35, GETUTCDATE())),
(2, 2, 'Verifica que tu número esté actualizado en el sistema. Acércate a Secretaría si persiste.', DATEADD(DAY, -35, GETUTCDATE()));

-- ========================================
-- 10. VERSIONES DE ARTÍCULOS
-- ========================================

INSERT INTO VersionesArticulo (ArticuloConocimientoId, NumeroVersion, Titulo, Contenido, ModificadoPorId, CambiosRealizados, CreatedAt)
VALUES 
(1, 1, 'Configuración de WiFi Institucional en Windows 10/11', 
    '<h2>Configuración WiFi FISEI</h2><p>Para conectarse a la red WiFi institucional, siga estos pasos:</p><ol><li>Abrir configuración de red</li><li>Seleccionar red "UTA-FISEI"</li><li>Ingresar credenciales institucionales</li><li>Verificar conexión</li></ol>',
    3, 'Versión inicial del artículo', DATEADD(DAY, -30, GETUTCDATE())),
(5, 1, 'Acceso a Aula Virtual desde Casa',
    '<h2>VPN y Aula Virtual</h2><p>Para acceder desde fuera del campus:</p><ol><li>Descargar cliente VPN institucional</li><li>Configurar con credenciales UTA</li><li>Conectar a VPN</li><li>Acceder a https://aulavirtual.fisei.uta.edu.ec</li></ol>',
    2, 'Versión inicial', DATEADD(DAY, -60, GETUTCDATE()));

-- ========================================
-- 11. LOGS DE AUDITORÍA
-- ========================================

INSERT INTO AuditLogs (UsuarioId, UsuarioNombre, DireccionIP, UserAgent, TipoAccion, TipoEntidad, EntidadId, EntidadDescripcion, 
    Descripcion, NivelSeveridad, EsExitoso, FechaHora, Modulo, Endpoint, CreatedAt)
VALUES 
-- Login exitoso del administrador
(1, 'admin', '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)', 0, 0, 1, 'Usuario: admin', 
    'Login exitoso del administrador', 0, 1, DATEADD(HOUR, -8, GETUTCDATE()), 'API', '/api/auth/login', DATEADD(HOUR, -8, GETUTCDATE())),

-- Creación de incidente crítico
(2, 'Supervisor Carlos Mendoza', '192.168.1.105', 'Mozilla/5.0', 0, 1, 6, 'Incidente: INC-2024-006',
    'Creación de incidente crítico: Posible pérdida de datos en servidor', 4, 1, DATEADD(MINUTE, -120, GETUTCDATE()), 'API', '/api/incidentes', DATEADD(MINUTE, -120, GETUTCDATE())),

-- Escalación del incidente 6
(2, 'Supervisor Carlos Mendoza', '192.168.1.105', 'Mozilla/5.0', 7, 1, 6, 'Incidente: INC-2024-006',
    'Escalación de incidente a nivel 2 por severidad crítica', 4, 1, DATEADD(MINUTE, -115, GETUTCDATE()), 'API', '/api/incidentes/6/escalar', DATEADD(MINUTE, -115, GETUTCDATE())),

-- Cierre de incidente resuelto
(2, 'Supervisor Carlos Mendoza', '192.168.1.105', 'Mozilla/5.0', 1, 1, 1, 'Incidente: INC-2024-001',
    'Cierre de incidente resuelto: Servidor de correo restaurado', 1, 1, DATEADD(DAY, -7, GETUTCDATE()), 'API', '/api/incidentes/1/cerrar', DATEADD(DAY, -7, GETUTCDATE())),

-- Publicación de artículo
(3, 'Técnico María González', '192.168.1.102', 'Mozilla/5.0', 0, 3, 1, 'Artículo: Configuración WiFi',
    'Publicación de artículo de conocimiento sobre WiFi', 1, 1, DATEADD(DAY, -30, GETUTCDATE()), 'API', '/api/conocimiento', DATEADD(DAY, -30, GETUTCDATE())),

-- Intento de login fallido
(NULL, 'usuario_desconocido', '192.168.1.200', 'curl/7.68.0', 0, 0, NULL, NULL,
    'Intento de login fallido: credenciales incorrectas', 3, 0, DATEADD(HOUR, -3, GETUTCDATE()), 'API', '/api/auth/login', DATEADD(HOUR, -3, GETUTCDATE()));

-- ========================================
-- VERIFICACIÓN DE DATOS INSERTADOS
-- ========================================

PRINT '============================================';
PRINT 'RESUMEN DE DATOS INSERTADOS:';
PRINT '============================================';

SELECT 'Artículos de Conocimiento' AS Tabla, COUNT(*) AS Registros FROM ArticulosConocimiento
UNION ALL
SELECT 'Incidentes', COUNT(*) FROM Incidentes
UNION ALL
SELECT 'Comentarios en Incidentes', COUNT(*) FROM ComentariosIncidente
UNION ALL
SELECT 'Archivos Adjuntos', COUNT(*) FROM ArchivosAdjuntos
UNION ALL
SELECT 'Registros de Tiempo', COUNT(*) FROM RegistrosTiempo
UNION ALL
SELECT 'Escalaciones', COUNT(*) FROM HistorialEscalaciones
UNION ALL
SELECT 'Notificaciones', COUNT(*) FROM Notificaciones
UNION ALL
SELECT 'Votaciones en Artículos', COUNT(*) FROM VotacionesArticulo
UNION ALL
SELECT 'Comentarios en Artículos', COUNT(*) FROM ComentariosArticulo
UNION ALL
SELECT 'Versiones de Artículos', COUNT(*) FROM VersionesArticulo
UNION ALL
SELECT 'Logs de Auditoría', COUNT(*) FROM AuditLogs;

PRINT '';
PRINT '============================================';
PRINT 'DISTRIBUCIÓN DE INCIDENTES POR ESTADO:';
PRINT '============================================';

SELECT 
    CASE Estado
        WHEN 0 THEN 'Nuevo'
        WHEN 1 THEN 'Asignado'
        WHEN 2 THEN 'En Proceso'
        WHEN 3 THEN 'Escalado'
        WHEN 4 THEN 'Cerrado'
        ELSE 'Desconocido'
    END AS Estado,
    COUNT(*) AS Cantidad
FROM Incidentes
GROUP BY Estado
ORDER BY Estado;

PRINT '';
PRINT '============================================';
PRINT 'DISTRIBUCIÓN DE INCIDENTES POR PRIORIDAD:';
PRINT '============================================';

SELECT 
    CASE Prioridad
        WHEN 0 THEN 'Baja'
        WHEN 1 THEN 'Media'
        WHEN 2 THEN 'Alta'
        WHEN 3 THEN 'Crítica'
        ELSE 'Desconocida'
    END AS Prioridad,
    COUNT(*) AS Cantidad
FROM Incidentes
GROUP BY Prioridad
ORDER BY Prioridad;

PRINT '';
PRINT '============================================';
PRINT '✅ POBLACIÓN DE DATOS COMPLETADA';
PRINT '============================================';
