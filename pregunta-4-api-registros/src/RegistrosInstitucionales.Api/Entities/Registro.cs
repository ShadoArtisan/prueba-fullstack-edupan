namespace RegistrosInstitucionales.Api.Entities;

public class Registro
{
    public long RegistroId { get; set; }

    // Campos de búsqueda (identificador + nombre, ambos obligatorios según la especificación).
    public string Identificador { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    // Campos de salida del endpoint de consulta.
    public string Estado { get; set; } = string.Empty;
    public string NumeroRegistro { get; set; } = string.Empty;
    public DateTime FechaEvento { get; set; }
    public DateTime FechaInscripcion { get; set; }
}
