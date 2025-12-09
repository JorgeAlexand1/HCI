-- Delete existing services
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
DELETE FROM Servicios;

-- Insert Services into Servicios table with explicit Modern_Spanish_CI_AS collation
-- Category 1: Hardware
INSERT INTO Servicios (Nombre, Descripcion, Codigo, IsActive, CategoriaId, ResponsableArea, ContactoTecnico, TiempoRespuestaMinutos, TiempoResolucionMinutos, Instrucciones, RequiereAprobacion, CreatedAt, UpdatedAt, IsDeleted) 
VALUES 
(N'Mantenimiento de Computadoras' COLLATE Modern_Spanish_CI_AS, N'Reparación y mantenimiento de equipos de cómputo' COLLATE Modern_Spanish_CI_AS, 'SRV-HW-001', 1, 1, N'Infraestructura' COLLATE Modern_Spanish_CI_AS, 'soporte@ejemplo.com', 120, 480, N'Contacte a soporte técnico' COLLATE Modern_Spanish_CI_AS, 0, GETDATE(), GETDATE(), 0),
(N'Reemplazo de Hardware' COLLATE Modern_Spanish_CI_AS, N'Cambio e instalación de componentes de hardware' COLLATE Modern_Spanish_CI_AS, 'SRV-HW-002', 1, 1, N'Infraestructura' COLLATE Modern_Spanish_CI_AS, 'hardware@ejemplo.com', 240, 1440, N'Solicite reemplazo de componentes' COLLATE Modern_Spanish_CI_AS, 1, GETDATE(), GETDATE(), 0),
(N'Impresoras y Periféricos' COLLATE Modern_Spanish_CI_AS, N'Soporte para impresoras, escáneres y otros periféricos' COLLATE Modern_Spanish_CI_AS, 'SRV-HW-003', 1, 1, N'Tecnología' COLLATE Modern_Spanish_CI_AS, 'perifericos@ejemplo.com', 60, 240, N'Reinicie el dispositivo' COLLATE Modern_Spanish_CI_AS, 0, GETDATE(), GETDATE(), 0),

-- Category 2: Software
(N'Instalación de Software' COLLATE Modern_Spanish_CI_AS, N'Instalación y configuración de aplicaciones' COLLATE Modern_Spanish_CI_AS, 'SRV-SW-001', 1, 2, N'Desarrollo' COLLATE Modern_Spanish_CI_AS, 'software@ejemplo.com', 90, 360, N'Proporcione licencias válidas' COLLATE Modern_Spanish_CI_AS, 1, GETDATE(), GETDATE(), 0),
(N'Actualización de Software' COLLATE Modern_Spanish_CI_AS, N'Parches y actualizaciones de sistemas' COLLATE Modern_Spanish_CI_AS, 'SRV-SW-002', 1, 2, N'Tecnología' COLLATE Modern_Spanish_CI_AS, 'updates@ejemplo.com', 60, 300, N'Se requiere planificación' COLLATE Modern_Spanish_CI_AS, 1, GETDATE(), GETDATE(), 0),
(N'Aplicaciones Empresariales' COLLATE Modern_Spanish_CI_AS, N'Soporte a aplicaciones personalizadas corporativas' COLLATE Modern_Spanish_CI_AS, 'SRV-SW-003', 1, 2, N'Desarrollo' COLLATE Modern_Spanish_CI_AS, 'apps@ejemplo.com', 120, 600, N'Abra ticket de soporte' COLLATE Modern_Spanish_CI_AS, 1, GETDATE(), GETDATE(), 0),

-- Category 3: Network
(N'Conectividad de Red' COLLATE Modern_Spanish_CI_AS, N'Configuración y diagnóstico de conectividad' COLLATE Modern_Spanish_CI_AS, 'SRV-NET-001', 1, 3, N'Infraestructura' COLLATE Modern_Spanish_CI_AS, 'redes@ejemplo.com', 30, 180, N'Verifique cables y switch' COLLATE Modern_Spanish_CI_AS, 0, GETDATE(), GETDATE(), 0),
(N'WiFi Corporativo' COLLATE Modern_Spanish_CI_AS, N'Soporte a redes inalámbricas' COLLATE Modern_Spanish_CI_AS, 'SRV-NET-002', 1, 3, N'Infraestructura' COLLATE Modern_Spanish_CI_AS, 'wifi@ejemplo.com', 45, 240, N'Reinicie enrutador' COLLATE Modern_Spanish_CI_AS, 0, GETDATE(), GETDATE(), 0),
(N'VPN y Acceso Remoto' COLLATE Modern_Spanish_CI_AS, N'Acceso remoto y conectividad VPN' COLLATE Modern_Spanish_CI_AS, 'SRV-NET-003', 1, 3, N'Infraestructura' COLLATE Modern_Spanish_CI_AS, 'vpn@ejemplo.com', 45, 240, N'Reinstale cliente VPN' COLLATE Modern_Spanish_CI_AS, 0, GETDATE(), GETDATE(), 0),

-- Category 4: Access/Security
(N'Control de Acceso' COLLATE Modern_Spanish_CI_AS, N'Gestión de permisos y acceso a recursos' COLLATE Modern_Spanish_CI_AS, 'SRV-ACC-001', 1, 4, N'Seguridad' COLLATE Modern_Spanish_CI_AS, 'acceso@ejemplo.com', 60, 240, N'Solicite cambio de permisos' COLLATE Modern_Spanish_CI_AS, 1, GETDATE(), GETDATE(), 0),
(N'Seguridad y Antivirus' COLLATE Modern_Spanish_CI_AS, N'Protección antivirus y antimalware' COLLATE Modern_Spanish_CI_AS, 'SRV-ACC-002', 1, 4, N'Seguridad' COLLATE Modern_Spanish_CI_AS, 'seguridad@ejemplo.com', 30, 120, N'Ejecute escaneo completo' COLLATE Modern_Spanish_CI_AS, 0, GETDATE(), GETDATE(), 0),
(N'Backup y Recuperación' COLLATE Modern_Spanish_CI_AS, N'Servicios de copia de seguridad y recuperación' COLLATE Modern_Spanish_CI_AS, 'SRV-ACC-003', 1, 4, N'Infraestructura' COLLATE Modern_Spanish_CI_AS, 'backup@ejemplo.com', 120, 1440, N'Contacte a equipo de backup' COLLATE Modern_Spanish_CI_AS, 1, GETDATE(), GETDATE(), 0),

-- Category 5: Email
(N'Correo Corporativo' COLLATE Modern_Spanish_CI_AS, N'Servicio de correo electrónico institucional' COLLATE Modern_Spanish_CI_AS, 'SRV-EMAIL-001', 1, 5, N'Tecnología' COLLATE Modern_Spanish_CI_AS, 'correo@ejemplo.com', 60, 240, N'Verifique credenciales' COLLATE Modern_Spanish_CI_AS, 0, GETDATE(), GETDATE(), 0),
(N'Configuración de Outlook' COLLATE Modern_Spanish_CI_AS, N'Configuración de clientes de correo' COLLATE Modern_Spanish_CI_AS, 'SRV-EMAIL-002', 1, 5, N'Tecnología' COLLATE Modern_Spanish_CI_AS, 'outlook@ejemplo.com', 45, 180, N'Resetee contraseña si es necesario' COLLATE Modern_Spanish_CI_AS, 0, GETDATE(), GETDATE(), 0),
(N'Recuperación de Correo' COLLATE Modern_Spanish_CI_AS, N'Recuperación de mensajes eliminados' COLLATE Modern_Spanish_CI_AS, 'SRV-EMAIL-003', 1, 5, N'Tecnología' COLLATE Modern_Spanish_CI_AS, 'recovery@ejemplo.com', 90, 480, N'Contacte a administrador' COLLATE Modern_Spanish_CI_AS, 1, GETDATE(), GETDATE(), 0);

-- Verify inserted records
SELECT COUNT(*) as TotalInsertados FROM Servicios;
SELECT Id, Nombre, Codigo, CategoriaId, IsActive FROM Servicios ORDER BY CategoriaId, Id;
