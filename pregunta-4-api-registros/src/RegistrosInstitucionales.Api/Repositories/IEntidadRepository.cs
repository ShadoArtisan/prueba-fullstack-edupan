using RegistrosInstitucionales.Api.Entities;

namespace RegistrosInstitucionales.Api.Repositories;

public interface IEntidadRepository
{
    Task<Entidad?> ObtenerPorApiKeyHashAsync(string apiKeyHash, CancellationToken ct = default);
}
