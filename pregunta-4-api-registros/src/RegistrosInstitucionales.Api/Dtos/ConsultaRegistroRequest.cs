using System.ComponentModel.DataAnnotations;

namespace RegistrosInstitucionales.Api.Dtos;

// La especificación exige que ambos campos vengan juntos: no se permite
// buscar solo por identificador, así que los dos son [Required] explícitamente.
public class ConsultaRegistroRequest
{
    [Required(ErrorMessage = "El identificador es obligatorio.")]
    public string Identificador { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;
}
