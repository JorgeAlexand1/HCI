-- Script para insertar servicios de ejemplo por categoría
-- Este script debe ejecutarse después de aplicar la migración AddServiciosTable

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- Servicios para Categoría: Hardware (ID: 1)
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [UpdatedAt], [IsDeleted])
VALUES 
('Soporte de Computadoras', 'Mantenimiento y reparación de equipos de cómputo', 'SRV-HW-001', 1, 1, 'Soporte Técnico', 'soporte.hw@fisei.uta.edu.ec', 30, 240, 'Verificar estado físico del equipo, ejecutar diagnósticos básicos', 0, GETDATE(), NULL, 0),
('Soporte de Impresoras', 'Configuración, mantenimiento y reparación de impresoras', 'SRV-HW-002', 1, 1, 'Soporte Técnico', 'soporte.hw@fisei.uta.edu.ec', 15, 120, 'Verificar conexiones, niveles de tinta/tóner, configuración de red', 0, GETDATE(), NULL, 0),
('Soporte de Proyectores', 'Instalación y mantenimiento de equipos de proyección', 'SRV-HW-003', 1, 1, 'Audiovisuales', 'audiovisuales@fisei.uta.edu.ec', 45, 180, 'Verificar conexiones de video, calibración de imagen', 0, GETDATE(), NULL, 0);

-- Servicios para Categoría: Software (ID: 2)
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [UpdatedAt], [IsDeleted])
VALUES 
('Instalación de Software Académico', 'Instalación de programas especializados para educación', 'SRV-SW-001', 1, 2, 'Sistemas', 'sistemas@fisei.uta.edu.ec', 60, 480, 'Verificar licencias, compatibilidad del sistema, permisos de instalación', 1, GETDATE(), NULL, 0),
('Soporte de Sistema Operativo', 'Mantenimiento y configuración del SO', 'SRV-SW-002', 1, 2, 'Sistemas', 'sistemas@fisei.uta.edu.ec', 30, 360, 'Ejecutar diagnósticos, verificar actualizaciones, limpiar archivos temporales', 0, GETDATE(), NULL, 0),
('Soporte de Antivirus', 'Gestión y actualización de software antivirus', 'SRV-SW-003', 1, 2, 'Seguridad TI', 'seguridad@fisei.uta.edu.ec', 15, 90, 'Actualizar definiciones, ejecutar escaneo completo', 0, GETDATE(), NULL, 0);

-- Servicios para Categoría: Red (ID: 3)
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [UpdatedAt], [IsDeleted])
VALUES 
('Conectividad WiFi', 'Problemas de conexión a la red inalámbrica institucional', 'SRV-NET-001', 1, 3, 'Redes y Comunicaciones', 'redes@fisei.uta.edu.ec', 15, 120, 'Verificar SSID, credenciales, reiniciar adaptador de red', 0, GETDATE(), NULL, 0),
('Acceso a Internet', 'Problemas de navegación web y acceso a recursos online', 'SRV-NET-002', 1, 3, 'Redes y Comunicaciones', 'redes@fisei.uta.edu.ec', 30, 240, 'Verificar conectividad, DNS, proxy institucional', 0, GETDATE(), NULL, 0),
('Red Cableada', 'Problemas con conexiones Ethernet en aulas y oficinas', 'SRV-NET-003', 1, 3, 'Infraestructura TI', 'infraestructura@fisei.uta.edu.ec', 45, 360, 'Verificar cables, switches, configuración de puerto', 0, GETDATE(), NULL, 0);

-- Servicios para Categoría: Seguridad (ID: 4)
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [UpdatedAt], [IsDeleted])
VALUES 
('Gestión de Cuentas de Usuario', 'Creación, modificación y desactivación de cuentas', 'SRV-SEC-001', 1, 4, 'Administración de Sistemas', 'admin@fisei.uta.edu.ec', 120, 480, 'Verificar autorización, validar datos del usuario, aplicar políticas de seguridad', 1, GETDATE(), NULL, 0),
('Recuperación de Contraseñas', 'Reset y recuperación de contraseñas institucionales', 'SRV-SEC-002', 1, 4, 'Soporte al Usuario', 'soporte@fisei.uta.edu.ec', 30, 60, 'Verificar identidad del usuario, aplicar procedimiento de reset', 0, GETDATE(), NULL, 0),
('Permisos y Accesos', 'Gestión de permisos a sistemas y recursos', 'SRV-SEC-003', 1, 4, 'Administración de Sistemas', 'admin@fisei.uta.edu.ec', 60, 240, 'Verificar roles del usuario, aplicar principio de menor privilegio', 1, GETDATE(), NULL, 0);

-- Servicios para Categoría: Aplicaciones Web (ID: 5)
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [UpdatedAt], [IsDeleted])
VALUES 
('Portal Académico', 'Soporte para el sistema de gestión académica', 'SRV-WEB-001', 1, 5, 'Desarrollo Web', 'desarrollo@fisei.uta.edu.ec', 45, 360, 'Verificar sesión del usuario, limpiar caché del navegador, revisar logs de aplicación', 0, GETDATE(), NULL, 0),
('Sistema de Biblioteca', 'Soporte para el catálogo y servicios digitales de biblioteca', 'SRV-WEB-002', 1, 5, 'Servicios Digitales', 'biblioteca@fisei.uta.edu.ec', 30, 180, 'Verificar credenciales, estado de la cuenta, disponibilidad del recurso', 0, GETDATE(), NULL, 0),
('Plataforma E-Learning', 'Soporte para aulas virtuales y contenido educativo online', 'SRV-WEB-003', 1, 5, 'Educación Virtual', 'elearning@fisei.uta.edu.ec', 60, 480, 'Verificar rol de usuario, permisos del curso, compatibilidad del navegador', 0, GETDATE(), NULL, 0);

-- Verificar la inserción
SELECT 
    s.Id,
    s.Codigo,
    s.Nombre,
    s.Descripcion,
    c.Nombre as Categoria,
    s.ResponsableArea,
    s.ContactoTecnico,
    s.TiempoRespuestaMinutos,
    s.TiempoResolucionMinutos,
    s.RequiereAprobacion,
    s.IsActive
FROM Servicios s
INNER JOIN Categorias c ON s.CategoriaId = c.Id
ORDER BY c.Nombre, s.Codigo;