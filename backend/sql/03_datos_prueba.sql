/*
    Datos de prueba para ejercitar manualmente el endpoint
    POST /api/registros/consulta (ver backend/README.md).

    La API Key de prueba es el texto plano: clave-demo-123
    (el hash de abajo es SHA-256 en mayúsculas, igual a como lo calcula
    ApiKeyHasher.Hash en el backend con Convert.ToHexString).
*/

USE RegistrosInstitucionales;
GO

INSERT INTO dbo.Entidades (NombreEntidad, ApiKeyHash, FechaInicioConvenio, FechaVencimiento, CuotaDiaria, Activo)
VALUES (
    'Entidad de Prueba',
    '9273604F77B0AB6544529EB4D51BBA176F55C270E7BDBFDB30B8DC50D526C266',
    '2025-01-01',
    '2027-12-31',
    5000,
    1
);
GO

INSERT INTO dbo.Registros (Identificador, Nombre, Estado, NumeroRegistro, FechaEvento, FechaInscripcion)
VALUES (
    '8-123-456',
    'Juan Pérez',
    'VIGENTE',
    'REG-2024-001',
    '2024-01-10',
    '2024-01-15'
);
GO
