-- Script para agregar usuarios estudiantes de prueba
-- Insertar estudiantes con TipoUsuario = 1 (Usuario) y email que contenga "estudiante"

INSERT INTO Usuarios (Username, Email, PasswordHash, FirstName, LastName, Phone, Department, TipoUsuario, IsActive, IsEmailConfirmed, CreatedAt, UpdatedAt)
VALUES 
('estudiante1', 'estudiante1@fisei.uta.edu.ec', 'AQAAAAIAAYagAAAAEKKx8VfK7n7gOZHzr7L9YjOWxM8VjY9lKfS9OFdMP8qTzr6lGHpRf7xJgW4pFdGZGg==', 'Carlos', 'Ramírez', '0984123456', 'Estudiante', 1, 1, 1, GETUTCDATE(), GETUTCDATE()),
('estudiante2', 'estudiante2@fisei.uta.edu.ec', 'AQAAAAIAAYagAAAAEKKx8VfK7n7gOZHzr7L9YjOWxM8VjY9lKfS9OFdMP8qTzr6lGHpRf7xJgW4pFdGZGg==', 'María', 'López', '0984123457', 'Estudiante', 1, 1, 1, GETUTCDATE(), GETUTCDATE());

-- Verificar la inserción
SELECT * FROM Usuarios WHERE Email LIKE '%estudiante%';