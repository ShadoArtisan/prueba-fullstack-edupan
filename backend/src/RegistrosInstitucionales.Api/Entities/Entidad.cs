namespace RegistrosInstitucionales.Api.Entities;

public class Entidad
{
    public int EntidadId { get; set; }
    public string NombreEntidad { get; set; } = string.Empty;

    // Se guarda el hash de la API Key, nunca el valor en texto plano.
    public string ApiKeyHash { get; set; } = string.Empty;

    public DateTime FechaInicioConvenio { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public int CuotaDiaria { get; set; }
    public bool Activo { get; set; }
}
