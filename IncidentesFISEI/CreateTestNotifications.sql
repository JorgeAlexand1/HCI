-- Script para crear notificaciones de prueba
-- Ejecutar en SQL Server Management Studio o con sqlcmd

USE IncidentesFISEI_Dev;
GO

-- Obtener el ID del primer usuario administrador (TipoUsuario = 4)
DECLARE @AdminUserId INT;
SELECT TOP 1 @AdminUserId = Id FROM Usuarios WHERE TipoUsuario = 4 ORDER BY Id;

-- Insertar notificaciones de prueba
IF @AdminUserId IS NOT NULL
BEGIN
    -- Notificación 1: Actividades próximas
    INSERT INTO Notificaciones (UsuarioId, Titulo, Mensaje, TipoNotificacion, Prioridad, Leida, CreatedAt, UpdatedAt, IsDeleted)
    VALUES (@AdminUserId, 
            N'Tiene actividades próximas pendientes', 
            N'Revise sus tareas asignadas que vencen pronto',
            0, -- Info
            1, -- Normal
            0, -- No leída
            DATEADD(DAY, -6, GETDATE()),
            DATEADD(DAY, -6, GETDATE()),
            0);

    -- Notificación 2: Nuevo incidente
    INSERT INTO Notificaciones (UsuarioId, Titulo, Mensaje, TipoNotificacion, Prioridad, Leida, CreatedAt, UpdatedAt, IsDeleted)
    VALUES (@AdminUserId, 
            N'Nuevo incidente asignado', 
            N'Se le ha asignado un nuevo incidente crítico #INC-2024-001',
            1, -- Alerta
            2, -- Alta
            0, 
            DATEADD(DAY, -13, GETDATE()),
            DATEADD(DAY, -13, GETDATE()),
            0);

    -- Notificación 3: SLA próximo a vencer
    INSERT INTO Notificaciones (UsuarioId, Titulo, Mensaje, TipoNotificacion, Prioridad, Leida, CreatedAt, UpdatedAt, IsDeleted)
    VALUES (@AdminUserId, 
            N'SLA próximo a vencer', 
            N'El incidente #INC-2024-002 vence en 2 horas',
            2, -- Advertencia
            2, -- Alta
            0, 
            DATEADD(DAY, -20, GETDATE()),
            DATEADD(DAY, -20, GETDATE()),
            0);

    -- Notificación 4: Incidente resuelto
    INSERT INTO Notificaciones (UsuarioId, Titulo, Mensaje, TipoNotificacion, Prioridad, Leida, CreatedAt, UpdatedAt, IsDeleted)
    VALUES (@AdminUserId, 
            N'Incidente resuelto satisfactoriamente', 
            N'El incidente #INC-2024-003 ha sido cerrado',
            3, -- Exito
            1, -- Normal
            0, 
            DATEADD(DAY, -27, GETDATE()),
            DATEADD(DAY, -27, GETDATE()),
            0);

    PRINT 'Notificaciones de prueba creadas exitosamente para el usuario ID: ' + CAST(@AdminUserId AS VARCHAR);
END
ELSE
BEGIN
    PRINT 'ERROR: No se encontró ningún usuario administrador en la base de datos';
END
GO
