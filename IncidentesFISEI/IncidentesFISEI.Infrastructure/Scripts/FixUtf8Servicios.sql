-- Script para actualizar los servicios con caracteres UTF-8 correctos
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- Actualizar servicios con caracteres especiales corregidos
UPDATE Servicios 
SET Nombre = 'Gestión de Cuentas de Usuario',
    Descripcion = 'Creación, modificación y desactivación de cuentas',
    ResponsableArea = 'Administración de Sistemas'
WHERE Codigo = 'SRV-SEC-001';

UPDATE Servicios 
SET Nombre = 'Recuperación de Contraseñas',
    Descripcion = 'Reset y recuperación de contraseñas institucionales'
WHERE Codigo = 'SRV-SEC-002';

UPDATE Servicios 
SET Nombre = 'Gestión de permisos a sistemas y recursos',
    ResponsableArea = 'Administración de Sistemas'
WHERE Codigo = 'SRV-SEC-003';

UPDATE Servicios 
SET Nombre = 'Portal Académico',
    Descripcion = 'Soporte para el sistema de gestión académica'
WHERE Codigo = 'SRV-WEB-001';

UPDATE Servicios 
SET Nombre = 'Sistema de Biblioteca',
    Descripcion = 'Soporte para el catálogo y servicios digitales de biblioteca'
WHERE Codigo = 'SRV-WEB-002';

UPDATE Servicios 
SET Nombre = 'Educación Virtual'
WHERE Codigo = 'SRV-WEB-003';

UPDATE Servicios 
SET Descripcion = 'Mantenimiento y reparación de equipos de cómputo',
    ResponsableArea = 'Soporte Técnico'
WHERE Codigo = 'SRV-HW-001';

UPDATE Servicios 
SET Descripcion = 'Configuración, mantenimiento y reparación de impresoras',
    ResponsableArea = 'Soporte Técnico'
WHERE Codigo = 'SRV-HW-002';

UPDATE Servicios 
SET Descripcion = 'Instalación y mantenimiento de equipos de proyección'
WHERE Codigo = 'SRV-HW-003';

UPDATE Servicios 
SET Nombre = 'Conectividad WiFi',
    Descripcion = 'Problemas de conexión a la red inalámbrica institucional'
WHERE Codigo = 'SRV-NET-001';

UPDATE Servicios 
SET Descripcion = 'Problemas de navegación web y acceso a recursos online'
WHERE Codigo = 'SRV-NET-002';

UPDATE Servicios 
SET Nombre = 'Instalación de Software Académico',
    Descripcion = 'Instalación de programas especializados para educación'
WHERE Codigo = 'SRV-SW-001';

UPDATE Servicios 
SET Descripcion = 'Mantenimiento y configuración del SO'
WHERE Codigo = 'SRV-SW-002';

UPDATE Servicios 
SET Descripcion = 'Gestión y actualización de software antivirus'
WHERE Codigo = 'SRV-SW-003';

-- Verificar los cambios
SELECT Codigo, Nombre, Descripcion, ResponsableArea 
FROM Servicios 
ORDER BY Codigo;