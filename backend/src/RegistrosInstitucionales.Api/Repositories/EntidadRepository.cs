using Microsoft.EntityFrameworkCore;
using RegistrosInstitucionales.Api.Data;
using RegistrosInstitucionales.Api.Entities;

namespace RegistrosInstitucionales.Api.Repositories;

public class EntidadRepository : IEntidadRepository
{
    private readonly AppDbContext _context;

    public EntidadRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Entidad?> ObtenerPorApiKeyHashAsync(string apiKeyHash, CancellationToken ct = default)
    {
        return _context.Entidades
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.ApiKeyHash == apiKeyHash, ct);
    }
}
