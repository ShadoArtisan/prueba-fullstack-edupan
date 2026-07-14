namespace LinqEquivalente;

// Versión mínima de las tablas de la Pregunta 7 (solo los campos que usa
// esta consulta). El esquema completo está en
// pregunta-7-esquema-basedatos/schema.sql.

public class Entidad
{
    public int EntidadId { get; set; }
    public string NombreEntidad { get; set; } = string.Empty;
}

public class LogAcceso
{
    public long LogId { get; set; }
    public int EntidadId { get; set; }
    public DateTime FechaHora { get; set; }
    public string TipoConsulta { get; set; } = string.Empty;
    public string Resultado { get; set; } = string.Empty;
}
