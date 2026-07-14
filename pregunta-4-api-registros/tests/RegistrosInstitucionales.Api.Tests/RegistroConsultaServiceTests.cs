using Moq;
using RegistrosInstitucionales.Api.Dtos;
using RegistrosInstitucionales.Api.Entities;
using RegistrosInstitucionales.Api.Repositories;
using RegistrosInstitucionales.Api.Services;
using Xunit;

namespace RegistrosInstitucionales.Api.Tests;

public class RegistroConsultaServiceTests
{
    private readonly Mock<IRegistroRepository> _registroRepository = new();
    private readonly Mock<ILogAccesoRepository> _logAccesoRepository = new();
    private readonly RegistroConsultaService _service;

    public RegistroConsultaServiceTests()
    {
        _service = new RegistroConsultaService(_registroRepository.Object, _logAccesoRepository.Object);
    }

    private static Entidad NuevaEntidad(int cuotaDiaria) => new()
    {
        EntidadId = 1,
        NombreEntidad = "Entidad de prueba",
        ApiKeyHash = "hash",
        FechaInicioConvenio = DateTime.UtcNow.AddYears(-1),
        FechaVencimiento = DateTime.UtcNow.AddYears(1),
        CuotaDiaria = cuotaDiaria,
        Activo = true
    };

    [Fact]
    public async Task ConsultarAsync_CuandoYaAlcanzoLaCuotaDiaria_RechazaSinConsultarElRegistro()
    {
        var entidad = NuevaEntidad(cuotaDiaria: 100);
        _logAccesoRepository.Setup(r => r.ContarAprobadasHoyAsync(entidad.EntidadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(100);

        var resultado = await _service.ConsultarAsync(entidad, new ConsultaRegistroRequest
        {
            Identificador = "8-123-456",
            Nombre = "Juan Pérez"
        });

        Assert.Equal(TipoResultadoConsulta.CuotaSuperada, resultado.Tipo);
        _registroRepository.Verify(r => r.BuscarAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _logAccesoRepository.Verify(r => r.RegistrarAsync(
            It.Is<LogAcceso>(l => l.Resultado == ResultadoAcceso.RechazadoCuota),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConsultarAsync_CuandoNoSuperaCuotaYElRegistroExiste_DevuelveAprobadoYRegistraElLog()
    {
        var entidad = NuevaEntidad(cuotaDiaria: 5000);
        _logAccesoRepository.Setup(r => r.ContarAprobadasHoyAsync(entidad.EntidadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);
        _registroRepository.Setup(r => r.BuscarAsync("8-123-456", "Juan Pérez", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Registro
            {
                Identificador = "8-123-456",
                Nombre = "Juan Pérez",
                Estado = "VIGENTE",
                NumeroRegistro = "REG-001",
                FechaEvento = new DateTime(2024, 1, 10),
                FechaInscripcion = new DateTime(2024, 1, 15)
            });

        var resultado = await _service.ConsultarAsync(entidad, new ConsultaRegistroRequest
        {
            Identificador = "8-123-456",
            Nombre = "Juan Pérez"
        });

        Assert.Equal(TipoResultadoConsulta.Aprobado, resultado.Tipo);
        Assert.Equal("REG-001", resultado.Datos?.NumeroRegistro);
        _logAccesoRepository.Verify(r => r.RegistrarAsync(
            It.Is<LogAcceso>(l => l.Resultado == ResultadoAcceso.Aprobado),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConsultarAsync_CuandoElRegistroNoExiste_DevuelveNoEncontrado()
    {
        var entidad = NuevaEntidad(cuotaDiaria: 5000);
        _logAccesoRepository.Setup(r => r.ContarAprobadasHoyAsync(entidad.EntidadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _registroRepository.Setup(r => r.BuscarAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Registro?)null);

        var resultado = await _service.ConsultarAsync(entidad, new ConsultaRegistroRequest
        {
            Identificador = "8-999-999",
            Nombre = "Nadie"
        });

        Assert.Equal(TipoResultadoConsulta.NoEncontrado, resultado.Tipo);
        _logAccesoRepository.Verify(r => r.RegistrarAsync(
            It.Is<LogAcceso>(l => l.Resultado == ResultadoAcceso.RechazadoNoEncontrado),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
