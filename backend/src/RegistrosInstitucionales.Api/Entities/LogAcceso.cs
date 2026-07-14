namespace RegistrosInstitucionales.Api.Entities;

public static class ResultadoAcceso
{
    public const string Aprobado = "APROBADO";
    public const string RechazadoVencido = "RECHAZADO_VENCIDO";
    public const string RechazadoCuota = "RECHAZADO_CUOTA";
    public const string RechazadoNoEncontrado = "RECHAZADO_NO_ENCONTRADO";
    public const string RechazadoApiKeyInvalida = "RECHAZADO_API_KEY_INVALIDA";
}

public class LogAcceso
{
    public long LogId { get; set; }
    public int EntidadId { get; set; }
    public DateTime FechaHora { get; set; }
    public string TipoConsulta { get; set; } = string.Empty;
    public string? IdentificadorConsultado { get; set; }
    public string Resultado { get; set; } = string.Empty;
    public string? Motivo { get; set; }
}
