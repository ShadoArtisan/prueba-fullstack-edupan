using RegistrosInstitucionales.Api.Entities;

namespace RegistrosInstitucionales.Api.Repositories;

public interface IRegistroRepository
{
    Task<Registro?> BuscarAsync(string identificador, string nombre, CancellationToken ct = default);
}
