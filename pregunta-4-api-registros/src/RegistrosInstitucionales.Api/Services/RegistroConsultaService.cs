using RegistrosInstitucionales.Api.Dtos;
using RegistrosInstitucionales.Api.Entities;
using RegistrosInstitucionales.Api.Repositories;

namespace RegistrosInstitucionales.Api.Services;

public class RegistroConsultaService : IRegistroConsultaService
{
    private const string TipoConsultaRegistro = "CONSULTA_REGISTRO";

    private readonly IRegistroRepository _registroRepository;
    private readonly ILogAccesoRepository _logAccesoRepository;

    public RegistroConsultaService(
        IRegistroRepository registroRepository,
        ILogAccesoRepository logAccesoRepository)
    {
        _registroRepository = registroRepository;
        _logAccesoRepository = logAccesoRepository;
    }

    // Lógica pura y aislada a propósito: es lo que cubre el test unitario
    // pedido en la Pregunta 4 sin necesitar base de datos ni mocks.
    public static bool SuperoCuotaDiaria(int consultasAprobadasHoy, int cuotaDiaria) =>
        consultasAprobadasHoy >= cuotaDiaria;

    public async Task<ResultadoConsultaRegistro> ConsultarAsync(
        Entidad entidad,
        ConsultaRegistroRequest request,
        CancellationToken ct = default)
    {
        var consultasHoy = await _logAccesoRepository.ContarAprobadasHoyAsync(entidad.EntidadId, ct);

        if (SuperoCuotaDiaria(consultasHoy, entidad.CuotaDiaria))
        {
            await _logAccesoRepository.RegistrarAsync(new LogAcceso
            {
                EntidadId = entidad.EntidadId,
                FechaHora = DateTime.UtcNow,
                TipoConsulta = TipoConsultaRegistro,
                IdentificadorConsultado = request.Identificador,
                Resultado = ResultadoAcceso.RechazadoCuota,
                Motivo = $"Cuota diaria de {entidad.CuotaDiaria} consultas superada."
            }, ct);

            return ResultadoConsultaRegistro.CuotaSuperada();
        }

        var registro = await _registroRepository.BuscarAsync(request.Identificador, request.Nombre, ct);

        if (registro is null)
        {
            await _logAccesoRepository.RegistrarAsync(new LogAcceso
            {
                EntidadId = entidad.EntidadId,
                FechaHora = DateTime.UtcNow,
                TipoConsulta = TipoConsultaRegistro,
                IdentificadorConsultado = request.Identificador,
                Resultado = ResultadoAcceso.RechazadoNoEncontrado,
                Motivo = "No existe un registro con ese identificador y nombre."
            }, ct);

            return ResultadoConsultaRegistro.NoEncontrado();
        }

        await _logAccesoRepository.RegistrarAsync(new LogAcceso
        {
            EntidadId = entidad.EntidadId,
            FechaHora = DateTime.UtcNow,
            TipoConsulta = TipoConsultaRegistro,
            IdentificadorConsultado = request.Identificador,
            Resultado = ResultadoAcceso.Aprobado
        }, ct);

        return ResultadoConsultaRegistro.Aprobado(new ConsultaRegistroResponse
        {
            Estado = registro.Estado,
            NumeroRegistro = registro.NumeroRegistro,
            FechaEvento = registro.FechaEvento,
            FechaInscripcion = registro.FechaInscripcion
        });
    }
}
