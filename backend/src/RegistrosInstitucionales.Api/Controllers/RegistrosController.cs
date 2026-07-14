using Microsoft.AspNetCore.Mvc;
using RegistrosInstitucionales.Api.Auth;
using RegistrosInstitucionales.Api.Dtos;
using RegistrosInstitucionales.Api.Entities;
using RegistrosInstitucionales.Api.Services;

namespace RegistrosInstitucionales.Api.Controllers;

[ApiController]
[Route("api/registros")]
[ServiceFilter(typeof(ApiKeyAuthFilter))]
public class RegistrosController : ControllerBase
{
    private readonly IRegistroConsultaService _consultaService;

    public RegistrosController(IRegistroConsultaService consultaService)
    {
        _consultaService = consultaService;
    }

    // POST /api/registros/consulta
    // El [ApiController] valida automáticamente el ConsultaRegistroRequest (DataAnnotations)
    // y responde 400 si falta identificador o nombre, antes de llegar aquí.
    [HttpPost("consulta")]
    [ProducesResponseType(typeof(ConsultaRegistroResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Consultar(
        [FromBody] ConsultaRegistroRequest request,
        CancellationToken ct)
    {
        var entidad = (Entidad)HttpContext.Items[ApiKeyAuthFilter.HttpContextItemKey]!;

        var resultado = await _consultaService.ConsultarAsync(entidad, request, ct);

        return resultado.Tipo switch
        {
            TipoResultadoConsulta.Aprobado => Ok(resultado.Datos),
            TipoResultadoConsulta.NoEncontrado => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Registro no encontrado",
                Detail = "No existe un registro con el identificador y nombre proporcionados."
            }),
            TipoResultadoConsulta.CuotaSuperada => StatusCode(StatusCodes.Status429TooManyRequests, new ProblemDetails
            {
                Status = StatusCodes.Status429TooManyRequests,
                Title = "Cuota diaria superada",
                Detail = "La entidad ya alcanzó su cuota diaria de consultas configurada."
            }),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
