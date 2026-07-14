namespace RegistrosInstitucionales.Api.Reportes;

public interface IReporteAccesosRepository
{
    // Equivalente LINQ de la consulta migrada de Sybase a SQL Server (Pregunta 5, punto 10).
    Task<List<LogAccesoResumenDto>> ObtenerTop100PorAnioAsync(int anio, CancellationToken ct = default);
}
