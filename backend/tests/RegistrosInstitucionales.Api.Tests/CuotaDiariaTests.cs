using RegistrosInstitucionales.Api.Services;
using Xunit;

namespace RegistrosInstitucionales.Api.Tests;

// Pregunta 4: al menos un test unitario para la lógica de validación de cuota diaria.
public class CuotaDiariaTests
{
    [Theory]
    [InlineData(0, 5000, false)]
    [InlineData(4999, 5000, false)]
    [InlineData(5000, 5000, true)]
    [InlineData(5001, 5000, true)]
    public void SuperoCuotaDiaria_ComparaConsultasHoyContraElLimiteDeLaEntidad(
        int consultasHoy, int cuotaDiaria, bool esperado)
    {
        var resultado = RegistroConsultaService.SuperoCuotaDiaria(consultasHoy, cuotaDiaria);

        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void SuperoCuotaDiaria_ConCuotaCero_SiempreRechaza()
    {
        // Una entidad con cuota configurada en 0 no debería poder hacer ninguna consulta.
        var resultado = RegistroConsultaService.SuperoCuotaDiaria(consultasAprobadasHoy: 0, cuotaDiaria: 0);

        Assert.True(resultado);
    }
}
