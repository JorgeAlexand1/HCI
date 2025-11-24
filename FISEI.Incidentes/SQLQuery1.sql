-- ===============================================
--  CREACIÓN DE BASE DE DATOS
-- ===============================================
CREATE DATABASE GestorIncidentesFISEI;
GO

USE GestorIncidentesFISEI;
GO

-- ===============================================
--  TABLA: ROL
-- ===============================================
CREATE TABLE ROL (
    IdRol INT IDENTITY(1,1) PRIMARY KEY,
    NombreRol NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(200)
);
GO

-- ===============================================
--  TABLA: USUARIO
-- ===============================================
CREATE TABLE USUARIO (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Correo NVARCHAR(100) UNIQUE NOT NULL,
    Contrasena NVARCHAR(255) NOT NULL,
    IdRol INT NOT NULL,
    FOREIGN KEY (IdRol) REFERENCES ROL(IdRol)
);
GO

-- ===============================================
--  TABLA: NIVEL_SOPORTE
-- ===============================================
CREATE TABLE NIVEL_SOPORTE (
    IdNivel INT IDENTITY(1,1) PRIMARY KEY,
    NombreNivel NVARCHAR(50) NOT NULL, -- Técnico, Experto, Proveedor
    Descripcion NVARCHAR(200)
);
GO

-- ===============================================
--  TABLA: SLA
-- ===============================================
CREATE TABLE SLA (
    IdSLA INT IDENTITY(1,1) PRIMARY KEY,
    TiempoRespuesta INT NOT NULL,     -- en horas
    TiempoResolucion INT NOT NULL,    -- en horas
    Prioridad NVARCHAR(50)
);
GO

-- ===============================================
--  TABLA: SERVICIO
-- ===============================================
CREATE TABLE SERVICIO (
    IdServicio INT IDENTITY(1,1) PRIMARY KEY,
    NombreServicio NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255),
    Responsable NVARCHAR(100),
    Area NVARCHAR(100),
    IdSLA INT NOT NULL,
    FOREIGN KEY (IdSLA) REFERENCES SLA(IdSLA)
);
GO

-- ===============================================
--  TABLA: INCIDENTE
-- ===============================================
CREATE TABLE INCIDENTE (
    IdIncidente INT IDENTITY(1,1) PRIMARY KEY,
    Titulo NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(MAX),
    FechaCreacion DATETIME DEFAULT GETDATE(),
    Estado NVARCHAR(50) DEFAULT 'Pendiente',
    IdUsuarioSolicitante INT NOT NULL,
    IdServicio INT NOT NULL,
    FOREIGN KEY (IdUsuarioSolicitante) REFERENCES USUARIO(IdUsuario),
    FOREIGN KEY (IdServicio) REFERENCES SERVICIO(IdServicio)
);
GO

-- ===============================================
--  TABLA: ASIGNACION
-- ===============================================
CREATE TABLE ASIGNACION (
    IdAsignacion INT IDENTITY(1,1) PRIMARY KEY,
    IdIncidente INT NOT NULL,
    IdUsuarioAsignado INT NOT NULL,
    IdNivel INT NOT NULL,
    FechaAsignacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IdIncidente) REFERENCES INCIDENTE(IdIncidente),
    FOREIGN KEY (IdUsuarioAsignado) REFERENCES USUARIO(IdUsuario),
    FOREIGN KEY (IdNivel) REFERENCES NIVEL_SOPORTE(IdNivel)
);
GO

-- ===============================================
--  TABLA: SOLUCION
-- ===============================================
CREATE TABLE SOLUCION (
    IdSolucion INT IDENTITY(1,1) PRIMARY KEY,
    IdIncidente INT NOT NULL,
    DescripcionSolucion NVARCHAR(MAX),
    FechaSolucion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IdIncidente) REFERENCES INCIDENTE(IdIncidente)
);
GO

-- ===============================================
--  TABLA: CONOCIMIENTO
-- ===============================================
CREATE TABLE CONOCIMIENTO (
    IdConocimiento INT IDENTITY(1,1) PRIMARY KEY,
    Titulo NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(MAX),
    Fuente NVARCHAR(150),
    FechaRegistro DATETIME DEFAULT GETDATE(),
    IdUsuarioAutor INT NOT NULL,
    FOREIGN KEY (IdUsuarioAutor) REFERENCES USUARIO(IdUsuario)
);
GO

-- ===============================================
--  TABLA: NOTIFICACION
-- ===============================================
CREATE TABLE NOTIFICACION (
    IdNotificacion INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario INT NOT NULL,
    Mensaje NVARCHAR(255),
    FechaEnvio DATETIME DEFAULT GETDATE(),
    Leida BIT DEFAULT 0,
    FOREIGN KEY (IdUsuario) REFERENCES USUARIO(IdUsuario)
);
GO

-- ===============================================
--  DATOS INICIALES (Opcional)
-- ===============================================

-- Roles
INSERT INTO ROL (NombreRol, Descripcion)
VALUES 
('Administrador', 'Gestor del sistema y asignador principal de tickets'),
('Técnico', 'Primer nivel de soporte'),
('Experto', 'Segundo nivel de soporte'),
('Proveedor', 'Soporte externo especializado'),
('Solicitante', 'Usuarios finales como estudiantes o docentes');
GO

-- Niveles de soporte
INSERT INTO NIVEL_SOPORTE (NombreNivel, Descripcion)
VALUES 
('Técnico', 'Primer nivel de atención'),
('Experto', 'Segundo nivel de atención'),
('Proveedor', 'Tercer nivel de atención');
GO

-- SLA
INSERT INTO SLA (TiempoRespuesta, TiempoResolucion, Prioridad)
VALUES 
(2, 6, 'Alta'),
(4, 12, 'Media'),
(8, 24, 'Baja');
GO

-- Servicios base
INSERT INTO SERVICIO (NombreServicio, Descripcion, Responsable, Area, IdSLA)
VALUES
('Mantenimiento de PC', 'Soporte técnico a computadoras de laboratorios', 'Juan Pérez', 'Laboratorios', 1),
('Red y Conectividad', 'Gestión de red e Internet en la facultad', 'María Gómez', 'Infraestructura', 2),
('Correo Institucional', 'Problemas con cuentas o acceso institucional', 'Carlos Ruiz', 'Comunicaciones', 3);
GO

PRINT '✅ Base de datos GestorIncidentesFISEI creada correctamente.';



USE FISEI_Incidentes;
GO

-- ================================
-- 1. Tabla de Roles (ITIL: gestión de acceso)
-- ================================
CREATE TABLE ROL (
    IdRol INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255)
);

-- ================================
-- 2. Tabla de Niveles de Soporte
-- ================================
CREATE TABLE NIVEL_SOPORTE (
    IdNivel INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255)
);

-- ================================
-- 3. Tabla de Servicios (Catálogo ITIL)
-- ================================
CREATE TABLE SERVICIO (
    IdServicio INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(255),
    SLA NVARCHAR(100), -- Acuerdo de nivel de servicio
    Responsable INT NULL,
    FOREIGN KEY (Responsable) REFERENCES Usuarios(IdUsuario)
);

-- ================================
-- 4. Tabla de Asignaciones
-- ================================
CREATE TABLE ASIGNACION (
    IdAsignacion INT IDENTITY PRIMARY KEY,
    IdIncidente INT NOT NULL,
    IdUsuario INT NOT NULL,
    FechaAsignacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IdIncidente) REFERENCES Incidentes(IdIncidente),
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario)
);

-- ================================
-- 5. Tabla de Notificaciones
-- ================================
CREATE TABLE NOTIFICACION (
    IdNotificacion INT IDENTITY PRIMARY KEY,
    IdUsuario INT NOT NULL,
    Mensaje NVARCHAR(255) NOT NULL,
    FechaEnvio DATETIME DEFAULT GETDATE(),
    Leido BIT DEFAULT 0,
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario)
);

-- ================================
-- 6. Tabla de Base de Conocimiento (BDC)
-- ================================
CREATE TABLE CONOCIMIENTO (
    IdConocimiento INT IDENTITY PRIMARY KEY,
    Titulo NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(MAX),
    Solucion NVARCHAR(MAX),
    FechaRegistro DATETIME DEFAULT GETDATE(),
    IdUsuario INT NOT NULL,
    IdCategoria INT NULL,
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario),
    FOREIGN KEY (IdCategoria) REFERENCES Categorias(IdCategoria)
);

-- ================================
-- 7. Relacionar Usuarios con Roles y Niveles de Soporte
-- ================================
ALTER TABLE Usuarios
ADD IdRol INT NULL,
    IdNivel INT NULL,
    FOREIGN KEY (IdRol) REFERENCES ROL(IdRol),
    FOREIGN KEY (IdNivel) REFERENCES NIVEL_SOPORTE(IdNivel);


    DROP DATABASE GestorIncidentesFISEI;
