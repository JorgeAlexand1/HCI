-- Script para crear un usuario administrador SPOC inicial
-- Ejecuta este script en SQL Server Management Studio o Azure Data Studio

USE FISEI_Incidentes;
GO

-- Obtener el ID del rol SPOC
DECLARE @IdRolSPOC INT;
SELECT @IdRolSPOC = IdRol FROM ROL WHERE Nombre = 'SPOC';

-- Verificar si ya existe el usuario admin
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'admin@uta.edu.ec')
BEGIN
    -- Crear usuario administrador SPOC
    -- Contraseña: Admin123! (ya hasheada con PBKDF2)
    INSERT INTO Usuarios (Nombre, Correo, Contrasena, Activo, IdRol, EmailVerificado, IdentityUserId, ResetToken, ResetTokenExpiry, EmailVerificationToken)
    VALUES (
        'Administrador FISEI',
        'admin@uta.edu.ec',
        'AQAAAAIAAYagAAAAEJfgqU7vJ9uXNqH8Z9f8CwLKj9mNXC5VPq1jqBxRqHm8vF7QN9zH3xZ8mQ5vR2wA==', -- Contraseña: Admin123!
        1, -- Activo
        @IdRolSPOC,
        1, -- EmailVerificado
        NULL,
        NULL,
        NULL,
        NULL
    );
    
    PRINT 'Usuario administrador creado exitosamente';
    PRINT 'Correo: admin@uta.edu.ec';
    PRINT 'Contraseña: Admin123!';
END
ELSE
BEGIN
    PRINT 'El usuario admin@uta.edu.ec ya existe';
END
GO

-- Verificar roles creados
SELECT * FROM ROL ORDER BY IdRol;

-- Verificar usuarios con sus roles
SELECT 
    u.IdUsuario,
    u.Nombre,
    u.Correo,
    u.Activo,
    u.EmailVerificado,
    r.Nombre AS Rol,
    r.Descripcion AS DescripcionRol
FROM Usuarios u
LEFT JOIN ROL r ON u.IdRol = r.IdRol
ORDER BY u.IdUsuario;
