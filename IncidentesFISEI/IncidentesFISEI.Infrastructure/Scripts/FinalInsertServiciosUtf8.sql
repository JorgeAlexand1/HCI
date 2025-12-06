-- Script final para insertar servicios con caracteres UTF-8 correctos
-- Ahora que tenemos la collation correcta SQL_Latin1_General_CP1_CI_AS

USE [IncidentesFISEI_Dev];

-- Limpiar servicios existentes
DELETE FROM [Servicios];
DBCC CHECKIDENT ('Servicios', RESEED, 0);

-- Insertar servicios con caracteres españoles correctos
-- Hardware (CategoriaId = 1)
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [IsDeleted])
VALUES 
(N'Soporte de Computadoras', N'Mantenimiento y reparación de equipos de cómputo', 'SRV-HW-001', 1, 1, N'Soporte Técnico', 'soporte.hw@fisei.uta.edu.ec', 30, 240, N'Verificar estado físico del equipo, ejecutar diagnósticos básicos', 0, GETDATE(), 0),
(N'Soporte de Impresoras', N'Configuración, mantenimiento y reparación de impresoras', 'SRV-HW-002', 1, 1, N'Soporte Técnico', 'soporte.hw@fisei.uta.edu.ec', 15, 120, N'Verificar conexiones, niveles de tinta/tóner, configuración de red', 0, GETDATE(), 0),
(N'Soporte de Proyectores', N'Instalación y mantenimiento de equipos de proyección', 'SRV-HW-003', 1, 1, N'Audiovisuales', 'audiovisuales@fisei.uta.edu.ec', 45, 180, N'Verificar conexiones de video, calibración de imagen', 0, GETDATE(), 0);

-- Software (CategoriaId = 2)  
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [IsDeleted])
VALUES 
(N'Instalación de Software Académico', N'Instalación de programas especializados para educación', 'SRV-SW-001', 1, 2, N'Sistemas', 'sistemas@fisei.uta.edu.ec', 60, 480, N'Verificar licencias, compatibilidad del sistema, permisos de instalación', 1, GETDATE(), 0),
(N'Soporte de Sistema Operativo', N'Mantenimiento y configuración del SO', 'SRV-SW-002', 1, 2, N'Sistemas', 'sistemas@fisei.uta.edu.ec', 30, 360, N'Ejecutar diagnósticos, verificar actualizaciones, limpiar archivos temporales', 0, GETDATE(), 0),
(N'Soporte de Antivirus', N'Gestión y actualización de software antivirus', 'SRV-SW-003', 1, 2, N'Seguridad TI', 'seguridad@fisei.uta.edu.ec', 15, 90, N'Actualizar definiciones, ejecutar escaneo completo', 0, GETDATE(), 0);

-- Red (CategoriaId = 3)
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [IsDeleted])
VALUES 
(N'Conectividad WiFi', N'Problemas de conexión a la red inalámbrica institucional', 'SRV-NET-001', 1, 3, N'Redes y Comunicaciones', 'redes@fisei.uta.edu.ec', 15, 120, N'Verificar SSID, credenciales, reiniciar adaptador de red', 0, GETDATE(), 0),
(N'Acceso a Internet', N'Problemas de navegación web y acceso a recursos online', 'SRV-NET-002', 1, 3, N'Redes y Comunicaciones', 'redes@fisei.uta.edu.ec', 30, 240, N'Verificar conectividad, DNS, proxy institucional', 0, GETDATE(), 0),
(N'Red Cableada', N'Problemas con conexiones Ethernet en aulas y oficinas', 'SRV-NET-003', 1, 3, N'Infraestructura TI', 'infraestructura@fisei.uta.edu.ec', 45, 360, N'Verificar cables, switches, configuración de puerto', 0, GETDATE(), 0);

-- Seguridad/Acceso (CategoriaId = 4)
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [IsDeleted])
VALUES 
(N'Gestión de Cuentas de Usuario', N'Creación, modificación y desactivación de cuentas', 'SRV-SEC-001', 1, 4, N'Administración de Sistemas', 'admin@fisei.uta.edu.ec', 120, 480, N'Verificar autorización, validar datos del usuario, aplicar políticas de seguridad', 1, GETDATE(), 0),
(N'Recuperación de Contraseñas', N'Reset y recuperación de contraseñas institucionales', 'SRV-SEC-002', 1, 4, N'Soporte al Usuario', 'soporte@fisei.uta.edu.ec', 30, 60, N'Verificar identidad del usuario, aplicar procedimiento de reset', 0, GETDATE(), 0),
(N'Permisos y Accesos', N'Gestión de permisos a sistemas y recursos', 'SRV-SEC-003', 1, 4, N'Administración de Sistemas', 'admin@fisei.uta.edu.ec', 60, 240, N'Verificar roles del usuario, aplicar principio de menor privilegio', 1, GETDATE(), 0);

-- Correo/Aplicaciones Web (CategoriaId = 5)
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [IsDeleted])
VALUES 
(N'Portal Académico', N'Soporte para el sistema de gestión académica', 'SRV-WEB-001', 1, 5, N'Desarrollo Web', 'desarrollo@fisei.uta.edu.ec', 45, 360, N'Verificar sesión del usuario, limpiar caché del navegador, revisar logs de aplicación', 0, GETDATE(), 0),
(N'Sistema de Biblioteca', N'Soporte para el catálogo y s ervicios digitales de biblioteca', 'SRV-WEB-002', 1, 5, N'Servicios Digitales', 'biblioteca@fisei.uta.edu.ec', 30, 180, N'Verificar credenciales, estado de la cuenta, disponibilidad del recurso', 0, GETDATE(), 0),
(N'Plataforma E-Learning', N'Soporte para aulas virtuales y contenido educativo online', 'SRV-WEB-003', 1, 5, N'Educación Virtual', 'elearning@fisei.uta.edu.ec', 60, 480, N'Verificar rol de usuario, permisos del curso, compatibilidad del navegador', 0, GETDATE(), 0);

-- Verificar inserción final
SELECT 
    s.Id,
    s.Codigo,
    s.Nombre,
    s.Descripcion,
    c.Nombre as Categoria,
    s.ResponsableArea
FROM [Servicios] s
INNER JOIN [Categorias] c ON s.CategoriaId = c.Id
ORDER BY s.Codigo;