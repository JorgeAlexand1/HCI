-- Script para cambiar la collation de la base de datos y tablas a UTF-8
USE [master];

-- Cambiar collation de la base de datos completa
ALTER DATABASE [IncidentesFISEI_Dev] COLLATE SQL_Latin1_General_CP1_CI_AS;

USE [IncidentesFISEI_Dev];

-- Cambiar collation de columnas específicas de la tabla Servicios
ALTER TABLE [Servicios] ALTER COLUMN [Nombre] NVARCHAR(100) COLLATE SQL_Latin1_General_CP1_CI_AS;
ALTER TABLE [Servicios] ALTER COLUMN [Descripcion] NVARCHAR(500) COLLATE SQL_Latin1_General_CP1_CI_AS;
ALTER TABLE [Servicios] ALTER COLUMN [ResponsableArea] NVARCHAR(100) COLLATE SQL_Latin1_General_CP1_CI_AS;
ALTER TABLE [Servicios] ALTER COLUMN [Instrucciones] NVARCHAR(2000) COLLATE SQL_Latin1_General_CP1_CI_AS;
ALTER TABLE [Servicios] ALTER COLUMN [EscalacionProcedure] NVARCHAR(1000) COLLATE SQL_Latin1_General_CP1_CI_AS;

-- Cambiar collation de columnas de la tabla Categorias para consistencia
ALTER TABLE [Categorias] ALTER COLUMN [Nombre] NVARCHAR(100) COLLATE SQL_Latin1_General_CP1_CI_AS;
ALTER TABLE [Categorias] ALTER COLUMN [Descripcion] NVARCHAR(500) COLLATE SQL_Latin1_General_CP1_CI_AS;

-- Cambiar collation de columnas de la tabla Incidentes
ALTER TABLE [Incidentes] ALTER COLUMN [Titulo] NVARCHAR(200) COLLATE SQL_Latin1_General_CP1_CI_AS;
ALTER TABLE [Incidentes] ALTER COLUMN [Descripcion] NVARCHAR(2000) COLLATE SQL_Latin1_General_CP1_CI_AS;
ALTER TABLE [Incidentes] ALTER COLUMN [Solucion] NVARCHAR(2000) COLLATE SQL_Latin1_General_CP1_CI_AS;

-- Verificar las collations actuales
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    COLLATION_NAME,
    DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME IN ('Servicios', 'Categorias', 'Incidentes') 
  AND COLLATION_NAME IS NOT NULL
ORDER BY TABLE_NAME, COLUMN_NAME;