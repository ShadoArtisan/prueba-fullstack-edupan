using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RegistrosInstitucionales.Api.Entities;
using RegistrosInstitucionales.Api.Repositories;

namespace RegistrosInstitucionales.Api.Auth;

// Filtro de acción: valida la API Key contra la entidad y su convenio ANTES
// de que el controller/servicio se ejecute. Deja la entidad resuelta en
// HttpContext.Items para que el controller no tenga que repetir la búsqueda.
public class ApiKeyAuthFilter : IAsyncActionFilter
{
    public const string HeaderName = "X-API-Key";
    public const string HttpContextItemKey = "Entidad";

    private readonly IEntidadRepository _entidadRepository;

    public ApiKeyAuthFilter(IEntidadRepository entidadRepository)
    {
        _entidadRepository = entidadRepository;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var apiKeyValues) ||
            string.IsNullOrWhiteSpace(apiKeyValues.ToString()))
        {
            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "API Key requerida",
                Detail = $"Debe enviar el header '{HeaderName}'."
            })
            { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }

        var apiKeyHash = ApiKeyHasher.Hash(apiKeyValues.ToString());
        var entidad = await _entidadRepository.ObtenerPorApiKeyHashAsync(apiKeyHash, context.HttpContext.RequestAborted);

        if (entidad is null)
        {
            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "API Key inválida",
                Detail = "La API Key enviada no corresponde a ninguna entidad registrada."
            })
            { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }

        if (!EsConvenioActivoYVigente(entidad))
        {
            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Convenio inactivo o vencido",
                Detail = "La entidad no tiene un convenio activo y vigente para realizar consultas."
            })
            { StatusCode = StatusCodes.Status403Forbidden };
            return;
        }

        context.HttpContext.Items[HttpContextItemKey] = entidad;
        await next();
    }

    private static bool EsConvenioActivoYVigente(Entidad entidad) =>
        entidad.Activo && entidad.FechaVencimiento >= DateTime.UtcNow.Date;
}
