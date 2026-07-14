using Microsoft.EntityFrameworkCore;
using RegistrosInstitucionales.Api.Data;
using RegistrosInstitucionales.Api.Entities;

namespace RegistrosInstitucionales.Api.Reportes;

public class ReporteAccesosRepository : IReporteAccesosRepository
{
    private readonly AppDbContext _context;

    public ReporteAccesosRepository(AppDbContext context)
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
            .Where(l => l.FechaHora >= inicio && l.FechaHora < fin && l.Resultado == ResultadoAcceso.Aprobado)
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
