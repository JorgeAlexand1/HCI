-- Script para actualizar directamente los servicios con caracteres correctos
USE IncidentesFISEI_Dev;

-- Actualizar servicios de Hardware
UPDATE Servicios SET Nombre = N'Soporte de Computadoras', Descripcion = N'Mantenimiento y reparación de equipos de cómputo', ResponsableArea = N'Soporte Técnico' WHERE Codigo = 'SRV-HW-001';
UPDATE Servicios SET Nombre = N'Soporte de Impresoras', Descripcion = N'Configuración, mantenimiento y reparación de impresoras', ResponsableArea = N'Soporte Técnico' WHERE Codigo = 'SRV-HW-002';
UPDATE Servicios SET Nombre = N'Soporte de Proyectores', Descripcion = N'Instalación y mantenimiento de equipos de proyección' WHERE Codigo = 'SRV-HW-003';

-- Actualizar servicios de Software  
UPDATE Servicios SET Nombre = N'Instalación de Software Académico', Descripcion = N'Instalación de programas especializados para educación' WHERE Codigo = 'SRV-SW-001';
UPDATE Servicios SET Nombre = N'Soporte de Sistema Operativo', Descripcion = N'Mantenimiento y configuración del SO' WHERE Codigo = 'SRV-SW-002';
UPDATE Servicios SET Nombre = N'Soporte de Antivirus', Descripcion = N'Gestión y actualización de software antivirus', ResponsableArea = N'Seguridad TI' WHERE Codigo = 'SRV-SW-003';

-- Actualizar servicios de Red
UPDATE Servicios SET Nombre = N'Conectividad WiFi', Descripcion = N'Problemas de conexión a la red inalámbrica institucional' WHERE Codigo = 'SRV-NET-001';
UPDATE Servicios SET Nombre = N'Acceso a Internet', Descripcion = N'Problemas de navegación web y acceso a recursos online' WHERE Codigo = 'SRV-NET-002';

-- Actualizar servicios de Seguridad
UPDATE Servicios SET Nombre = N'Gestión de Cuentas de Usuario', Descripcion = N'Creación, modificación y desactivación de cuentas', ResponsableArea = N'Administración de Sistemas' WHERE Codigo = 'SRV-SEC-001';
UPDATE Servicios SET Nombre = N'Recuperación de Contraseñas', Descripcion = N'Reset y recuperación de contraseñas institucionales' WHERE Codigo = 'SRV-SEC-002';
UPDATE Servicios SET Nombre = N'Permisos y Accesos', Descripcion = N'Gestión de permisos a sistemas y recursos', ResponsableArea = N'Administración de Sistemas' WHERE Codigo = 'SRV-SEC-003';

-- Actualizar servicios Web
UPDATE Servicios SET Nombre = N'Portal Académico', Descripcion = N'Soporte para el sistema de gestión académica' WHERE Codigo = 'SRV-WEB-001';
UPDATE Servicios SET Nombre = N'Sistema de Biblioteca', Descripcion = N'Soporte para el catálogo y servicios digitales de biblioteca' WHERE Codigo = 'SRV-WEB-002';
UPDATE Servicios SET Nombre = N'Plataforma E-Learning', Descripcion = N'Soporte para aulas virtuales y contenido educativo online', ResponsableArea = N'Educación Virtual' WHERE Codigo = 'SRV-WEB-003';

-- Verificar los cambios
SELECT Codigo, Nombre, Descripcion, ResponsableArea 
FROM Servicios 
ORDER BY Codigo;