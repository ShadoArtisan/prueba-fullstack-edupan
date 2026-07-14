using RegistrosInstitucionales.Api.Dtos;

namespace RegistrosInstitucionales.Api.Services;

public enum TipoResultadoConsulta
{
    Aprobado,
    NoEncontrado,
    CuotaSuperada
}

// Envuelve el resultado de negocio sin acoplar el servicio a códigos HTTP;
// es el controller quien traduce cada tipo al status code correspondiente.
public class ResultadoConsultaRegistro
{
    public TipoResultadoConsulta Tipo { get; private init; }
    public ConsultaRegistroResponse? Datos { get; private init; }

    public static ResultadoConsultaRegistro Aprobado(ConsultaRegistroResponse datos) =>
        new() { Tipo = TipoResultadoConsulta.Aprobado, Datos = datos };

    public static ResultadoConsultaRegistro NoEncontrado() =>
        new() { Tipo = TipoResultadoConsulta.NoEncontrado };

    public static ResultadoConsultaRegistro CuotaSuperada() =>
        new() { Tipo = TipoResultadoConsulta.CuotaSuperada };
}
