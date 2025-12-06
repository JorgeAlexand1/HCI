-- Script completo para recrear servicios con UTF-8 correcto
-- Eliminamos y recreamos todos los datos para asegurar codificación correcta

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

USE [IncidentesFISEI_Dev];

-- Eliminar todos los servicios existentes
DELETE FROM [Servicios];

-- Reiniciar el contador de identidad
DBCC CHECKIDENT ('Servicios', RESEED, 0);

-- Insertar servicios con codificación UTF-8 correcta usando NVARCHAR explícito
-- Hardware (CategoriaId = 1)
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [IsDeleted])
VALUES 
(CAST(N'Soporte de Computadoras' AS NVARCHAR(100)), CAST(N'Mantenimiento y reparación de equipos de cómputo' AS NVARCHAR(500)), 'SRV-HW-001', 1, 1, CAST(N'Soporte Técnico' AS NVARCHAR(100)), 'soporte.hw@fisei.uta.edu.ec', 30, 240, CAST(N'Verificar estado físico del equipo, ejecutar diagnósticos básicos' AS NVARCHAR(2000)), 0, GETDATE(), 0),
(CAST(N'Soporte de Impresoras' AS NVARCHAR(100)), CAST(N'Configuración, mantenimiento y reparación de impresoras' AS NVARCHAR(500)), 'SRV-HW-002', 1, 1, CAST(N'Soporte Técnico' AS NVARCHAR(100)), 'soporte.hw@fisei.uta.edu.ec', 15, 120, CAST(N'Verificar conexiones, niveles de tinta/tóner, configuración de red' AS NVARCHAR(2000)), 0, GETDATE(), 0),
(CAST(N'Soporte de Proyectores' AS NVARCHAR(100)), CAST(N'Instalación y mantenimiento de equipos de proyección' AS NVARCHAR(500)), 'SRV-HW-003', 1, 1, 'Audiovisuales', 'audiovisuales@fisei.uta.edu.ec', 45, 180, CAST(N'Verificar conexiones de video, calibración de imagen' AS NVARCHAR(2000)), 0, GETDATE(), 0);

-- Software (CategoriaId = 2)  
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [IsDeleted])
VALUES 
(CAST(N'Instalación de Software Académico' AS NVARCHAR(100)), CAST(N'Instalación de programas especializados para educación' AS NVARCHAR(500)), 'SRV-SW-001', 1, 2, 'Sistemas', 'sistemas@fisei.uta.edu.ec', 60, 480, CAST(N'Verificar licencias, compatibilidad del sistema, permisos de instalación' AS NVARCHAR(2000)), 1, GETDATE(), 0),
(CAST(N'Soporte de Sistema Operativo' AS NVARCHAR(100)), CAST(N'Mantenimiento y configuración del SO' AS NVARCHAR(500)), 'SRV-SW-002', 1, 2, 'Sistemas', 'sistemas@fisei.uta.edu.ec', 30, 360, CAST(N'Ejecutar diagnósticos, verificar actualizaciones, limpiar archivos temporales' AS NVARCHAR(2000)), 0, GETDATE(), 0),
(CAST(N'Soporte de Antivirus' AS NVARCHAR(100)), CAST(N'Gestión y actualización de software antivirus' AS NVARCHAR(500)), 'SRV-SW-003', 1, 2, CAST(N'Seguridad TI' AS NVARCHAR(100)), 'seguridad@fisei.uta.edu.ec', 15, 90, CAST(N'Actualizar definiciones, ejecutar escaneo completo' AS NVARCHAR(2000)), 0, GETDATE(), 0);

-- Red (CategoriaId = 3)
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [IsDeleted])
VALUES 
('Conectividad WiFi', CAST(N'Problemas de conexión a la red inalámbrica institucional' AS NVARCHAR(500)), 'SRV-NET-001', 1, 3, 'Redes y Comunicaciones', 'redes@fisei.uta.edu.ec', 15, 120, CAST(N'Verificar SSID, credenciales, reiniciar adaptador de red' AS NVARCHAR(2000)), 0, GETDATE(), 0),
('Acceso a Internet', CAST(N'Problemas de navegación web y acceso a recursos online' AS NVARCHAR(500)), 'SRV-NET-002', 1, 3, 'Redes y Comunicaciones', 'redes@fisei.uta.edu.ec', 30, 240, CAST(N'Verificar conectividad, DNS, proxy institucional' AS NVARCHAR(2000)), 0, GETDATE(), 0),
('Red Cableada', 'Problemas con conexiones Ethernet en aulas y oficinas', 'SRV-NET-003', 1, 3, 'Infraestructura TI', 'infraestructura@fisei.uta.edu.ec', 45, 360, CAST(N'Verificar cables, switches, configuración de puerto' AS NVARCHAR(2000)), 0, GETDATE(), 0);

-- Seguridad/Acceso (CategoriaId = 4)
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [IsDeleted])
VALUES 
(CAST(N'Gestión de Cuentas de Usuario' AS NVARCHAR(100)), CAST(N'Creación, modificación y desactivación de cuentas' AS NVARCHAR(500)), 'SRV-SEC-001', 1, 4, CAST(N'Administración de Sistemas' AS NVARCHAR(100)), 'admin@fisei.uta.edu.ec', 120, 480, CAST(N'Verificar autorización, validar datos del usuario, aplicar políticas de seguridad' AS NVARCHAR(2000)), 1, GETDATE(), 0),
(CAST(N'Recuperación de Contraseñas' AS NVARCHAR(100)), CAST(N'Reset y recuperación de contraseñas institucionales' AS NVARCHAR(500)), 'SRV-SEC-002', 1, 4, 'Soporte al Usuario', 'soporte@fisei.uta.edu.ec', 30, 60, CAST(N'Verificar identidad del usuario, aplicar procedimiento de reset' AS NVARCHAR(2000)), 0, GETDATE(), 0),
('Permisos y Accesos', CAST(N'Gestión de permisos a sistemas y recursos' AS NVARCHAR(500)), 'SRV-SEC-003', 1, 4, CAST(N'Administración de Sistemas' AS NVARCHAR(100)), 'admin@fisei.uta.edu.ec', 60, 240, CAST(N'Verificar roles del usuario, aplicar principio de menor privilegio' AS NVARCHAR(2000)), 1, GETDATE(), 0);

-- Correo/Aplicaciones Web (CategoriaId = 5)
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [IsDeleted])
VALUES 
(CAST(N'Portal Académico' AS NVARCHAR(100)), CAST(N'Soporte para el sistema de gestión académica' AS NVARCHAR(500)), 'SRV-WEB-001', 1, 5, 'Desarrollo Web', 'desarrollo@fisei.uta.edu.ec', 45, 360, CAST(N'Verificar sesión del usuario, limpiar caché del navegador, revisar logs de aplicación' AS NVARCHAR(2000)), 0, GETDATE(), 0),
('Sistema de Biblioteca', CAST(N'Soporte para el catálogo y servicios digitales de biblioteca' AS NVARCHAR(500)), 'SRV-WEB-002', 1, 5, 'Servicios Digitales', 'biblioteca@fisei.uta.edu.ec', 30, 180, CAST(N'Verificar credenciales, estado de la cuenta, disponibilidad del recurso' AS NVARCHAR(2000)), 0, GETDATE(), 0),
('Plataforma E-Learning', CAST(N'Soporte para aulas virtuales y contenido educativo online' AS NVARCHAR(500)), 'SRV-WEB-003', 1, 5, CAST(N'Educación Virtual' AS NVARCHAR(100)), 'elearning@fisei.uta.edu.ec', 60, 480, CAST(N'Verificar rol de usuario, permisos del curso, compatibilidad del navegador' AS NVARCHAR(2000)), 0, GETDATE(), 0);

-- Verificar inserción con caracteres correctos
SELECT 
    s.Id,
    s.Codigo,
    s.Nombre,
    s.Descripcion,
    c.Nombre as Categoria,
    s.ResponsableArea,
    s.IsActive
FROM [Servicios] s
INNER JOIN [Categorias] c ON s.CategoriaId = c.Id
ORDER BY c.Nombre, s.Codigo;