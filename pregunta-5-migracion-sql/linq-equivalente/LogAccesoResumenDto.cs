namespace LinqEquivalente;

// DTO tipado equivalente a las columnas de la consulta original en Sybase
// (ver Pregunta 5, punto 10). La fecha se expone como DateOnly: formatearla
// como texto (dd/MM/yyyy) es responsabilidad de la capa de presentación,
// no del repositorio.
public record LogAccesoResumenDto(
    string NombreEntidad,
    string TipoConsulta,
    int TotalConsultas,
    DateOnly Fecha);
