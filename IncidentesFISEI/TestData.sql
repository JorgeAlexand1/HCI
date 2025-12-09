-- Script para insertar datos de prueba en la base de datos
USE IncidentesFISEI_Dev;
GO

-- Verificar cuántos incidentes hay
SELECT COUNT(*) AS TotalIncidentes FROM Incidentes;
SELECT COUNT(*) AS TotalUsuarios FROM Usuarios;

-- Si no hay incidentes, insertar algunos de prueba
-- (Solo ejecutar si la consulta anterior devuelve 0)

-- Insertar incidentes de prueba (ejemplo)
/*
INSERT INTO Incidentes (Titulo, Descripcion, Estado, Prioridad, FechaReporte, FechaResolucion, ReportadoPorId, AsignadoAId, CategoriaId, ServicioId, NumeroIncidente)
VALUES 
('Problema de red en aula 101', 'No hay conexión a internet', 0, 2, DATEADD(day, -5, GETDATE()), DATEADD(day, -2, GETDATE()), 1, 2, 1, 1, 'INC-001'),
('Error en sistema de gestión', 'No se puede acceder al módulo de reportes', 0, 1, DATEADD(day, -10, GETDATE()), DATEADD(day, -8, GETDATE()), 2, 3, 2, 2, 'INC-002'),
('Solicitud de nuevo equipo', 'Se necesita laptop para nuevo docente', 1, 3, DATEADD(day, -3, GETDATE()), NULL, 3, 4, 1, 1, 'INC-003'),
('Problema con proyector', 'El proyector no enciende', 0, 2, DATEADD(day, -1, GETDATE()), GETDATE(), 4, 2, 3, 3, 'INC-004'),
('Actualización de software', 'Actualizar software de laboratorio', 1, 3, DATEADD(day, -7, GETDATE()), NULL, 1, 3, 2, 2, 'INC-005');
*/

-- Verificar datos después de insertar
SELECT * FROM Incidentes ORDER BY FechaReporte DESC;
SELECT Id, FirstName, LastName, Email, LastLoginAt, IsActive FROM Usuarios WHERE IsActive = 1;
