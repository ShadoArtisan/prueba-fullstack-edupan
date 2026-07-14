/*
    Pregunta 7 — Esquema de base de datos en SQL Server
    Servicio de consulta de registros institucionales

    Este script crea las tres tablas necesarias para el endpoint
    POST /api/registros/consulta (Pregunta 4), más los índices y el
    trigger que blindan la regla de cuota diaria también a nivel de base
    de datos (no solo en el código C#).
*/

IF DB_ID('RegistrosInstitucionales') IS NULL
BEGIN
    CREATE DATABASE RegistrosInstitucionales;
END
GO

USE RegistrosInstitucionales;
GO

-- =============================================================
-- 12. Entidades autorizadas (convenio: vigencia, cuota, estado)
-- =============================================================
IF OBJECT_ID('dbo.Entidades', 'U') IS NOT NULL DROP TABLE dbo.Entidades;
GO

CREATE TABLE dbo.Entidades
(
    EntidadId           INT IDENTITY(1,1)     NOT NULL,
    NombreEntidad       NVARCHAR(200)         NOT NULL,
    -- Solo se guarda el hash de la API Key (SHA-256), nunca el valor en texto plano.
    ApiKeyHash          CHAR(64)              NOT NULL,
    FechaInicioConvenio DATE                  NOT NULL,
    FechaVencimiento    DATE                  NOT NULL,
    -- Cuota configurable por entidad, tal como pide la especificación
    -- (en el legado estaba hardcodeada en 5000 para todas).
    CuotaDiaria         INT                   NOT NULL CONSTRAINT DF_Entidades_CuotaDiaria DEFAULT (5000),
    Activo              BIT                   NOT NULL CONSTRAINT DF_Entidades_Activo DEFAULT (1),
    FechaCreacion       DATETIME2(0)          NOT NULL CONSTRAINT DF_Entidades_FechaCreacion DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_Entidades PRIMARY KEY CLUSTERED (EntidadId),
    CONSTRAINT CK_Entidades_CuotaDiaria CHECK (CuotaDiaria >= 0),
    CONSTRAINT CK_Entidades_Vigencia CHECK (FechaVencimiento >= FechaInicioConvenio)
);
GO

-- Búsqueda por API Key en cada request: debe ser rapidísima y única.
CREATE UNIQUE NONCLUSTERED INDEX UX_Entidades_ApiKeyHash
    ON dbo.Entidades (ApiKeyHash);
GO

-- =============================================================
-- 11. Tabla principal de registros institucionales
-- =============================================================
IF OBJECT_ID('dbo.Registros', 'U') IS NOT NULL DROP TABLE dbo.Registros;
GO

CREATE TABLE dbo.Registros
(
    RegistroId       BIGINT IDENTITY(1,1) NOT NULL,
    -- Campos de identificación usados para la búsqueda (identificador + nombre, ambos obligatorios).
    Identificador    NVARCHAR(20)         NOT NULL,
    Nombre           NVARCHAR(200)        NOT NULL,
    -- Campos de salida exactos del endpoint de consulta.
    Estado           NVARCHAR(50)         NOT NULL,
    NumeroRegistro   NVARCHAR(50)         NOT NULL,
    FechaEvento      DATE                 NOT NULL,
    FechaInscripcion DATE                 NOT NULL,

    CONSTRAINT PK_Registros PRIMARY KEY CLUSTERED (RegistroId)
);
GO

-- 14a. Índice para el patrón de consulta más frecuente: búsqueda puntual
-- por identificador + nombre (la especificación prohíbe buscar solo por
-- identificador, así que siempre llegan los dos juntos).
CREATE NONCLUSTERED INDEX IX_Registros_Identificador_Nombre
    ON dbo.Registros (Identificador, Nombre)
    INCLUDE (Estado, NumeroRegistro, FechaEvento, FechaInscripcion);
GO

-- =============================================================
-- 13. Log de accesos (auditoría + cálculo de cuota diaria)
-- =============================================================
IF OBJECT_ID('dbo.LogAccesos', 'U') IS NOT NULL DROP TABLE dbo.LogAccesos;
GO

CREATE TABLE dbo.LogAccesos
(
    LogId                   BIGINT IDENTITY(1,1) NOT NULL,
    EntidadId               INT                   NOT NULL,
    FechaHora               DATETIME2(3)          NOT NULL CONSTRAINT DF_LogAccesos_FechaHora DEFAULT (SYSUTCDATETIME()),
    TipoConsulta            NVARCHAR(50)          NOT NULL,
    IdentificadorConsultado NVARCHAR(20)          NULL,
    -- APROBADO | RECHAZADO_VENCIDO | RECHAZADO_CUOTA | RECHAZADO_NO_ENCONTRADO | RECHAZADO_API_KEY_INVALIDA
    Resultado               NVARCHAR(30)          NOT NULL,
    Motivo                  NVARCHAR(200)         NULL,

    CONSTRAINT PK_LogAccesos PRIMARY KEY CLUSTERED (LogId),
    CONSTRAINT FK_LogAccesos_Entidades FOREIGN KEY (EntidadId)
        REFERENCES dbo.Entidades (EntidadId)
);
GO

-- 14b. Índice filtrado: el cálculo de cuota diaria SOLO cuenta consultas
-- APROBADO de HOY para una entidad. Filtrar por Resultado en el propio
-- índice evita indexar los rechazos (que no participan en la cuota) y
-- mantiene el índice mucho más pequeño y rápido de mantener.
CREATE NONCLUSTERED INDEX IX_LogAccesos_EntidadId_FechaHora_Aprobado
    ON dbo.LogAccesos (EntidadId, FechaHora)
    WHERE Resultado = 'APROBADO';
GO

-- =============================================================
-- 15. Regla de negocio a nivel de base de datos: nunca permitir un
-- log APROBADO si la entidad ya superó su cuota diaria configurada.
--
-- La validación "real" ocurre en el servicio C# (RegistroConsultaService,
-- Pregunta 4) porque ahí se puede responder 429 con un mensaje claro.
-- Este trigger es una segunda barrera (defensa en profundidad) para
-- proteger la integridad del dato ante condiciones de carrera o accesos
-- directos a la base que no pasen por la API.
-- =============================================================
GO
CREATE TRIGGER dbo.TR_LogAccesos_ValidarCuotaDiaria
ON dbo.LogAccesos
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN dbo.Entidades e ON e.EntidadId = i.EntidadId
        WHERE i.Resultado = 'APROBADO'
          AND (
                SELECT COUNT(*)
                FROM dbo.LogAccesos la
                WHERE la.EntidadId = i.EntidadId
                  AND la.Resultado = 'APROBADO'
                  AND CAST(la.FechaHora AS DATE) = CAST(i.FechaHora AS DATE)
              ) > e.CuotaDiaria
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50001, 'La entidad ya superó su cuota diaria configurada; no se puede registrar un acceso APROBADO adicional.', 1;
    END
END;
GO
