using Microsoft.EntityFrameworkCore;

namespace LinqEquivalente;

// Pregunta 5, punto 10: equivalente en LINQ / EF Core de la consulta SQL
// migrada (ver ../migracion-sybase-sqlserver.sql).
public class ReporteAccesosRepository
{
    private const string Aprobado = "APROBADO";

    private readonly ReporteAccesosDbContext _context;

    public ReporteAccesosRepository(ReporteAccesosDbContext context)
    {
        _context = context;
    }

    public async Task<List<LogAccesoResumenDto>> ObtenerTop100PorAnioAsync(int anio, CancellationToken ct = default)
    {
        // Rango medio-abierto [inicio, fin) para no truncar el 31 de diciembre
        // (ver el problema de la Pregunta 5, punto 9, sobre el BETWEEN original).
        var inicio = new DateTime(anio, 1, 1);
        var fin = inicio.AddYears(1);

        return await _context.LogAccesos
            .Where(l => l.FechaHora >= inicio && l.FechaHora < fin && l.Resultado == Aprobado)
            .Join(
                _context.Entidades,
                log => log.EntidadId,
                entidad => entidad.EntidadId,
                (log, entidad) => new { log, entidad })
            .GroupBy(x => new { x.entidad.NombreEntidad, x.log.TipoConsulta, Fecha = x.log.FechaHora.Date })
            .Select(g => new LogAccesoResumenDto(
                g.Key.NombreEntidad,
                g.Key.TipoConsulta,
                g.Count(),
                DateOnly.FromDateTime(g.Key.Fecha)))
            .OrderByDescending(r => r.TotalConsultas)
            .Take(100)
            .ToListAsync(ct);
    }
}
