-- Script para insertar servicios con caracteres UTF-8 correctos
-- Ejecutar desde la aplicación .NET para asegurar codificación correcta

-- Servicios para Hardware (CategoriaId = 1)
INSERT INTO [Servicios] ([Nombre], [Descripcion], [Codigo], [IsActive], [CategoriaId], [ResponsableArea], [ContactoTecnico], [TiempoRespuestaMinutos], [TiempoResolucionMinutos], [Instrucciones], [RequiereAprobacion], [CreatedAt], [IsDeleted])
VALUES 
(N'Soporte de Computadoras', N'Mantenimiento y reparación de equipos de cómputo', 'SRV-HW-001', 1, 1, N'Soporte Técnico', 'soporte.hw@fisei.uta.edu.ec', 30, 240, N'Verificar estado físico del equipo, ejecutar diagnósticos básicos', 0, GETDATE(), 0);

-- Verificar inserción
SELECT TOP 1 Nombre, Descripcion, ResponsableArea, Instrucciones 
FROM Servicios 
WHERE Codigo = 'SRV-HW-001';