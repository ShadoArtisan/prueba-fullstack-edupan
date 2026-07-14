using Microsoft.EntityFrameworkCore;
using RegistrosInstitucionales.Api.Data;
using RegistrosInstitucionales.Api.Entities;

namespace RegistrosInstitucionales.Api.Repositories;

public class RegistroRepository : IRegistroRepository
{
    private readonly AppDbContext _context;

    public RegistroRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Registro?> BuscarAsync(string identificador, string nombre, CancellationToken ct = default)
    {
        return _context.Registros
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Identificador == identificador && r.Nombre == nombre, ct);
    }
}
