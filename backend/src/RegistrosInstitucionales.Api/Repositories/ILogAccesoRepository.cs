using RegistrosInstitucionales.Api.Entities;

namespace RegistrosInstitucionales.Api.Repositories;

public interface ILogAccesoRepository
{
    // Cuenta las consultas APROBADAS del día de hoy para una entidad (base del cálculo de cuota diaria).
    Task<int> ContarAprobadasHoyAsync(int entidadId, CancellationToken ct = default);

    Task RegistrarAsync(LogAcceso log, CancellationToken ct = default);
}
