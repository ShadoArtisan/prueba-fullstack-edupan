// Todas las funciones devuelven `null` cuando el valor es válido, o un
// mensaje de error en lenguaje simple (sin jerga técnica) cuando no lo es.

export function validarRequerido(valor, etiqueta) {
  if (!valor || !valor.toString().trim()) {
    return `Este campo es obligatorio.`;
  }
  return null;
}

// Calcula un dígito verificador simple (módulo 11) sobre los dígitos
// anteriores al último. Es la misma idea que usan cédulas o RUC: el
// último dígito debe coincidir con un cálculo hecho sobre el resto del
// número, para detectar errores de tipeo.
function calcularDigitoVerificador(digitos) {
  let suma = 0;
  let peso = 2;
  for (let i = digitos.length - 1; i >= 0; i--) {
    suma += digitos[i] * peso;
    peso = peso === 7 ? 2 : peso + 1;
  }
  const resto = suma % 11;
  const digito = 11 - resto;
  if (digito === 10) return 0;
  if (digito === 11) return 0;
  return digito;
}

export function validarIdentificacionFiscal(valor) {
  const requerido = validarRequerido(valor);
  if (requerido) return requerido;

  const soloDigitos = /^[0-9]+$/;
  if (!soloDigitos.test(valor)) {
    return 'Escribe solo números, sin espacios ni guiones.';
  }

  if (valor.length < 2) {
    return 'El número debe tener al menos un dígito verificador al final.';
  }

  const digitos = valor.slice(0, -1).split('').map(Number);
  const digitoIngresado = Number(valor.slice(-1));
  const digitoEsperado = calcularDigitoVerificador(digitos);

  if (digitoIngresado !== digitoEsperado) {
    return `El último dígito no coincide con el dígito verificador. Revisa que el número esté bien escrito (dígito esperado: ${digitoEsperado}).`;
  }

  return null;
}

export function validarIPv4(valor) {
  const requerido = validarRequerido(valor);
  if (requerido) return requerido;

  const partes = valor.trim().split('.');
  if (partes.length !== 4) {
    return 'La dirección IP debe tener el formato 000.000.000.000, por ejemplo 192.168.1.1.';
  }

  for (const parte of partes) {
    if (!/^\d{1,3}$/.test(parte)) {
      return 'La dirección IP solo debe contener números separados por puntos.';
    }
    const numero = Number(parte);
    if (numero < 0 || numero > 255) {
      return 'Cada parte de la dirección IP debe estar entre 0 y 255.';
    }
  }

  return null;
}

export function validarCorreo(valor) {
  const requerido = validarRequerido(valor);
  if (requerido) return requerido;

  const patronCorreo = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!patronCorreo.test(valor.trim())) {
    return 'Escribe un correo válido, por ejemplo nombre@institucion.gob.';
  }

  return null;
}

export const TAMANO_MAXIMO_ARCHIVO_MB = 5;
const TAMANO_MAXIMO_BYTES = TAMANO_MAXIMO_ARCHIVO_MB * 1024 * 1024;

export function validarArchivoPdf(archivo) {
  if (!archivo) {
    return 'Debes adjuntar este documento.';
  }

  const esPdf = archivo.type === 'application/pdf' || archivo.name.toLowerCase().endsWith('.pdf');
  if (!esPdf) {
    return 'El archivo debe estar en formato PDF.';
  }

  if (archivo.size > TAMANO_MAXIMO_BYTES) {
    return `El archivo pesa demasiado. El tamaño máximo permitido es ${TAMANO_MAXIMO_ARCHIVO_MB} MB.`;
  }

  return null;
}
