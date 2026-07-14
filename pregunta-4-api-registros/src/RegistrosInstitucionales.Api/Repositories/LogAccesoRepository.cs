using Microsoft.EntityFrameworkCore;
using RegistrosInstitucionales.Api.Data;
using RegistrosInstitucionales.Api.Entities;

namespace RegistrosInstitucionales.Api.Repositories;

public class LogAccesoRepository : ILogAccesoRepository
{
    private readonly AppDbContext _context;

    public LogAccesoRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<int> ContarAprobadasHoyAsync(int entidadId, CancellationToken ct = default)
    {
        var hoy = DateTime.UtcNow.Date;
        var manana = hoy.AddDays(1);

        return _context.LogAccesos
            .AsNoTracking()
            .CountAsync(l =>
                l.EntidadId == entidadId &&
                l.Resultado == ResultadoAcceso.Aprobado &&
                l.FechaHora >= hoy && l.FechaHora < manana,
                ct);
    }

    public async Task RegistrarAsync(LogAcceso log, CancellationToken ct = default)
    {
        _context.LogAccesos.Add(log);
        await _context.SaveChangesAsync(ct);
    }
}
