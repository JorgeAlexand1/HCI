# Script PowerShell para poblar datos de prueba
# Ejecutar desde PowerShell como administrador

$server = ".\SQLEXPRESS"
$database = "IncidentesFISEI_Dev"

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "POBLACIÓN DE DATOS DE PRUEBA" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Artículos de Conocimiento
Write-Host "Insertando Artículos de Conocimiento..." -ForegroundColor Yellow
$sqlArticulos = @"
INSERT INTO ArticulosConocimiento (Titulo, Contenido, Resumen, AutorId, CategoriaId, Estado, TipoArticulo, VecesUtilizado, Visualizaciones, VotosPositivos, IsDeleted, CreatedAt)
VALUES 
('Configuración de WiFi Institucional', '<h2>WiFi FISEI</h2><p>Guía de conexión WiFi</p>', 'Guía WiFi institucional', 3, 3, 2, 0, 12, 150, 45, 0, DATEADD(DAY, -30, GETUTCDATE())),
('Recuperación de Contraseña', '<h2>Recuperar Password</h2>', 'Reseteo de contraseña correo', 3, 5, 2, 0, 25, 280, 68, 0, DATEADD(DAY, -45, GETUTCDATE())),
('Instalación de Software', '<h2>Solicitud Software</h2>', 'Proceso instalación software', 4, 2, 2, 1, 8, 95, 22, 0, DATEADD(DAY, -20, GETUTCDATE()));
SELECT '✓ Artículos: ' + CAST(@@ROWCOUNT AS VARCHAR) AS Resultado;
"@
sqlcmd -S $server -d $database -E -Q $sqlArticulos

# Incidentes
Write-Host "Insertando Incidentes..." -ForegroundColor Yellow
$sqlInc1 = "INSERT INTO Incidentes (NumeroIncidente, Titulo, Descripcion, ReportadoPorId, AsignadoAId, CategoriaId, Prioridad, Estado, Urgencia, Impacto, FechaReporte, FechaAsignacion, FechaResolucion, FechaCierre, Solucion, NivelActual, ServicioDITICId, IsDeleted, CreatedAt) VALUES ('INC-2024-001', 'Servidor de correo no responde', 'Error de conexión al servidor mail.uta.edu.ec', 5, 2, 5, 3, 4, 3, 3, DATEADD(DAY, -7, GETUTCDATE()), DATEADD(MINUTE, 5, DATEADD(DAY, -7, GETUTCDATE())), DATEADD(MINUTE, 45, DATEADD(DAY, -7, GETUTCDATE())), DATEADD(MINUTE, 50, DATEADD(DAY, -7, GETUTCDATE())), 'Servidor reiniciado', 2, 2, 0, DATEADD(DAY, -7, GETUTCDATE()));"
sqlcmd -S $server -d $database -E -Q $sqlInc1

$sqlInc2 = "INSERT INTO Incidentes (NumeroIncidente, Titulo, Descripcion, ReportadoPorId, AsignadoAId, CategoriaId, Prioridad, Estado, Urgencia, Impacto, FechaReporte, FechaAsignacion, FechaResolucion, FechaCierre, Solucion, NivelActual, ServicioDITICId, ArticuloConocimientoId, IsDeleted, CreatedAt) VALUES ('INC-2024-002', 'WiFi intermitente en Lab 3', 'Desconexiones frecuentes del WiFi', 6, 4, 3, 2, 4, 2, 2, DATEADD(DAY, -5, GETUTCDATE()), DATEADD(MINUTE, 20, DATEADD(DAY, -5, GETUTCDATE())), DATEADD(HOUR, 2, DATEADD(DAY, -5, GETUTCDATE())), DATEADD(HOUR, 2, DATEADD(MINUTE, 10, DATEADD(DAY, -5, GETUTCDATE()))), 'Cambio de canal WiFi solucionó interferencia', 1, 1, 1, 0, DATEADD(DAY, -5, GETUTCDATE()));"
sqlcmd -S $server -d $database -E -Q $sqlInc2

$sqlInc3 = "INSERT INTO Incidentes (NumeroIncidente, Titulo, Descripcion, ReportadoPorId, AsignadoAId, CategoriaId, Prioridad, Estado, Urgencia, Impacto, FechaReporte, FechaAsignacion, FechaResolucion, FechaCierre, Solucion, NivelActual, IsDeleted, CreatedAt) VALUES ('INC-2024-003', 'Impresora sin toner', 'Mensaje: Toner bajo en impresora Lab 2', 7, 3, 1, 1, 4, 1, 1, DATEADD(DAY, -3, GETUTCDATE()), DATEADD(MINUTE, 40, DATEADD(DAY, -3, GETUTCDATE())), DATEADD(HOUR, 4, DATEADD(DAY, -3, GETUTCDATE())), DATEADD(HOUR, 4, DATEADD(MINUTE, 5, DATEADD(DAY, -3, GETUTCDATE()))), 'Cartucho de toner reemplazado', 1, 0, DATEADD(DAY, -3, GETUTCDATE()));"
sqlcmd -S $server -d $database -E -Q $sqlInc3

$sqlInc4 = "INSERT INTO Incidentes (NumeroIncidente, Titulo, Descripcion, ReportadoPorId, AsignadoAId, CategoriaId, Prioridad, Estado, Urgencia, Impacto, FechaReporte, FechaAsignacion, NivelActual, ServicioDITICId, IsDeleted, CreatedAt) VALUES ('INC-2024-004', 'Error 403 en Aula Virtual', 'No puedo ingresar al aula virtual. Error 403 Acceso prohibido', 5, 3, 2, 2, 2, 2, 2, DATEADD(MINUTE, -45, GETUTCDATE()), DATEADD(MINUTE, -30, GETUTCDATE()), 1, 6, 0, DATEADD(MINUTE, -45, GETUTCDATE()));"
sqlcmd -S $server -d $database -E -Q $sqlInc4

$sqlInc5 = "INSERT INTO Incidentes (NumeroIncidente, Titulo, Descripcion, ReportadoPorId, CategoriaId, Prioridad, Estado, Urgencia, Impacto, FechaReporte, NivelActual, IsDeleted, CreatedAt) VALUES ('INC-2024-005', 'Solicitud AutoCAD 2024', 'Instalación de AutoCAD 2024 en Lab 5, 20 licencias', 5, 2, 0, 0, 0, 1, DATEADD(MINUTE, -15, GETUTCDATE()), 0, 0, DATEADD(MINUTE, -15, GETUTCDATE()));"
sqlcmd -S $server -d $database -E -Q $sqlInc5

$sqlInc6 = "INSERT INTO Incidentes (NumeroIncidente, Titulo, Descripcion, ReportadoPorId, AsignadoAId, CategoriaId, Prioridad, Estado, Urgencia, Impacto, FechaReporte, FechaAsignacion, NivelActual, NumeroEscalaciones, FechaUltimaEscalacion, RazonEscalacion, IsDeleted, CreatedAt) VALUES ('INC-2024-006', 'URGENTE: Servidor de proyectos', 'Servidor no responde. Posible pérdida de datos. 150+ proyectos', 2, 2, 1, 3, 3, 3, 3, DATEADD(MINUTE, -120, GETUTCDATE()), DATEADD(MINUTE, -118, GETUTCDATE()), 2, 1, DATEADD(MINUTE, -115, GETUTCDATE()), 'Incidente crítico requiere intervención especializada', 0, DATEADD(MINUTE, -120, GETUTCDATE()));"
sqlcmd -S $server -d $database -E -Q $sqlInc6

# Comentarios
Write-Host "Insertando Comentarios..." -ForegroundColor Yellow
$sqlComentarios = @"
INSERT INTO ComentariosIncidente (IncidenteId, AutorId, Contenido, EsInterno, Tipo, IsDeleted, CreatedAt) VALUES
(1, 2, 'Confirmado. Servicio Exchange Server no responde.', 1, 0, 0, DATEADD(MINUTE, 10, DATEADD(DAY, -7, GETUTCDATE()))),
(1, 2, 'Identificado proceso consumiendo 100% CPU.', 1, 0, 0, DATEADD(MINUTE, 20, DATEADD(DAY, -7, GETUTCDATE()))),
(1, 2, 'Problema resuelto. Servicio restaurado.', 0, 1, 0, DATEADD(MINUTE, 50, DATEADD(DAY, -7, GETUTCDATE()))),
(2, 4, 'En camino al Laboratorio 3.', 1, 0, 0, DATEADD(MINUTE, 25, DATEADD(DAY, -5, GETUTCDATE()))),
(2, 4, 'Detectada interferencia en canal 6. Reconfigurando.', 1, 0, 0, DATEADD(MINUTE, 45, DATEADD(DAY, -5, GETUTCDATE()))),
(4, 3, 'Revisando permisos de usuario en Moodle.', 1, 0, 0, DATEADD(MINUTE, -25, GETUTCDATE())),
(6, 2, 'ESCALADO. Contactando proveedor. Verificando backups.', 1, 2, 0, DATEADD(MINUTE, -110, GETUTCDATE()));
SELECT '✓ Comentarios: ' + CAST(@@ROWCOUNT AS VARCHAR) AS Resultado;
"@
sqlcmd -S $server -d $database -E -Q $sqlComentarios

# Notificaciones
Write-Host "Insertando Notificaciones..." -ForegroundColor Yellow
$sqlNotif = @"
INSERT INTO Notificaciones (UsuarioId, Titulo, Mensaje, Tipo, Prioridad, Leida, IncidenteId, IsDeleted, CreatedAt) VALUES
(5, 'Incidente Cerrado', 'Su incidente INC-2024-001 ha sido resuelto.', 3, 2, 1, 1, 0, DATEADD(DAY, -7, GETUTCDATE())),
(5, 'Incidente Asignado', 'Su incidente INC-2024-004 ha sido asignado a técnico.', 1, 1, 1, 4, 0, DATEADD(MINUTE, -30, GETUTCDATE())),
(5, 'Nuevo Comentario', 'Hay un nuevo comentario en su incidente INC-2024-004.', 2, 1, 0, 4, 0, DATEADD(MINUTE, -10, GETUTCDATE())),
(3, 'Incidente Asignado', 'Se le ha asignado INC-2024-003.', 1, 1, 1, 3, 0, DATEADD(DAY, -3, GETUTCDATE())),
(3, 'Incidente Asignado', 'Se le ha asignado INC-2024-004.', 1, 2, 0, 4, 0, DATEADD(MINUTE, -30, GETUTCDATE())),
(2, 'Incidente Crítico', 'ALERTA: Incidente crítico INC-2024-006.', 0, 3, 0, 6, 0, DATEADD(MINUTE, -120, GETUTCDATE()));
SELECT '✓ Notificaciones: ' + CAST(@@ROWCOUNT AS VARCHAR) AS Resultado;
"@
sqlcmd -S $server -d $database -E -Q $sqlNotif

# Logs de Auditoría
Write-Host "Insertando Logs de Auditoría..." -ForegroundColor Yellow
$sqlAudit = @"
INSERT INTO AuditLogs (UsuarioId, UsuarioNombre, DireccionIP, UserAgent, TipoAccion, TipoEntidad, EntidadId, EntidadDescripcion, Descripcion, NivelSeveridad, EsExitoso, FechaHora, Modulo, Endpoint, IsDeleted, CreatedAt) VALUES
(1, 'admin', '192.168.1.100', 'Mozilla/5.0', 3, 0, 1, 'Usuario: admin', 'Login exitoso del administrador', 0, 1, DATEADD(HOUR, -8, GETUTCDATE()), 'API', '/api/auth/login', 0, DATEADD(HOUR, -8, GETUTCDATE())),
(2, 'Supervisor Carlos Mendoza', '192.168.1.105', 'Mozilla/5.0', 0, 1, 6, 'Incidente: INC-2024-006', 'Creación de incidente crítico', 4, 1, DATEADD(MINUTE, -120, GETUTCDATE()), 'API', '/api/incidentes', 0, DATEADD(MINUTE, -120, GETUTCDATE())),
(2, 'Supervisor Carlos Mendoza', '192.168.1.105', 'Mozilla/5.0', 7, 1, 6, 'Incidente: INC-2024-006', 'Escalación de incidente', 4, 1, DATEADD(MINUTE, -115, GETUTCDATE()), 'API', '/api/incidentes/escalar', 0, DATEADD(MINUTE, -115, GETUTCDATE())),
(2, 'Supervisor Carlos Mendoza', '192.168.1.105', 'Mozilla/5.0', 1, 1, 1, 'Incidente: INC-2024-001', 'Cierre de incidente resuelto', 1, 1, DATEADD(DAY, -7, GETUTCDATE()), 'API', '/api/incidentes/cerrar', 0, DATEADD(DAY, -7, GETUTCDATE())),
(3, 'Técnico María González', '192.168.1.102', 'Mozilla/5.0', 0, 3, 1, 'Artículo: Configuración WiFi', 'Publicación de artículo de conocimiento', 1, 1, DATEADD(DAY, -30, GETUTCDATE()), 'API', '/api/conocimiento', 0, DATEADD(DAY, -30, GETUTCDATE()));
SELECT '✓ Auditoría: ' + CAST(@@ROWCOUNT AS VARCHAR) AS Resultado;
"@
sqlcmd -S $server -d $database -E -Q $sqlAudit

# Resumen
Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "RESUMEN FINAL" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green

$sqlResumen = @"
SELECT 'Artículos' AS Tabla, COUNT(*) AS Total FROM ArticulosConocimiento WHERE IsDeleted = 0
UNION ALL
SELECT 'Incidentes', COUNT(*) FROM Incidentes WHERE IsDeleted = 0
UNION ALL
SELECT 'Comentarios', COUNT(*) FROM ComentariosIncidente WHERE IsDeleted = 0
UNION ALL
SELECT 'Notificaciones', COUNT(*) FROM Notificaciones WHERE IsDeleted = 0
UNION ALL
SELECT 'Logs Auditoría', COUNT(*) FROM AuditLogs WHERE IsDeleted = 0;
"@
sqlcmd -S $server -d $database -E -Q $sqlResumen

Write-Host ""
Write-Host "✅ POBLACIÓN DE DATOS COMPLETADA" -ForegroundColor Green
