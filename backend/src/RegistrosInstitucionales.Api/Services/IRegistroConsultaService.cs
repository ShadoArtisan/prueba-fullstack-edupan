using RegistrosInstitucionales.Api.Dtos;
using RegistrosInstitucionales.Api.Entities;

namespace RegistrosInstitucionales.Api.Services;

public interface IRegistroConsultaService
{
    Task<ResultadoConsultaRegistro> ConsultarAsync(
        Entidad entidad,
        ConsultaRegistroRequest request,
        CancellationToken ct = default);
}
