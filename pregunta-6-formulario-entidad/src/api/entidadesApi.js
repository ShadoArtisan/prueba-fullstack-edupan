// URL del backend configurable por variable de entorno (ver .env.example);
// por defecto apunta al puerto por defecto de la API en desarrollo local.
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7000';

export async function registrarEntidad(datosFormulario) {
  const payload = new FormData();
  payload.append('identificacionFiscal', datosFormulario.identificacionFiscal);
  payload.append('nombreOficial', datosFormulario.nombreOficial);
  payload.append('ipPublica', datosFormulario.ipPublica);
  payload.append('enlaceTecnico', datosFormulario.enlaceTecnico);
  payload.append('correoResponsable', datosFormulario.correoResponsable);
  payload.append('documentoAutorizacion', datosFormulario.documentoAutorizacion);
  payload.append('resolucionHabilitante', datosFormulario.resolucionHabilitante);

  const respuesta = await fetch(`${API_BASE_URL}/api/entidades`, {
    method: 'POST',
    body: payload,
  });

  if (!respuesta.ok) {
    let mensaje = 'No se pudo registrar la entidad. Intenta de nuevo en unos minutos.';
    try {
      const cuerpoError = await respuesta.json();
      if (cuerpoError?.detail || cuerpoError?.title) {
        mensaje = cuerpoError.detail || cuerpoError.title;
      }
    } catch {
      // El backend no devolvió JSON (por ejemplo un 500 sin cuerpo); se usa el mensaje genérico.
    }
    throw new Error(mensaje);
  }

  return respuesta.json();
}
