import { useState } from 'react';
import {
  validarIdentificacionFiscal,
  validarRequerido,
  validarIPv4,
  validarCorreo,
  validarArchivoPdf,
  TAMANO_MAXIMO_ARCHIVO_MB,
} from '../utils/validaciones';
import { registrarEntidad } from '../api/entidadesApi';

const CAMPOS_INICIALES = {
  identificacionFiscal: '',
  nombreOficial: '',
  ipPublica: '',
  enlaceTecnico: '',
  correoResponsable: '',
};

const VALIDADORES_TEXTO = {
  identificacionFiscal: validarIdentificacionFiscal,
  nombreOficial: (valor) => validarRequerido(valor),
  ipPublica: validarIPv4,
  enlaceTecnico: (valor) => validarRequerido(valor),
  correoResponsable: validarCorreo,
};

const ETIQUETAS = {
  identificacionFiscal: 'Número de identificación fiscal',
  nombreOficial: 'Nombre oficial de la entidad',
  ipPublica: 'Dirección IP pública del servidor de consumo',
  enlaceTecnico: 'Nombre del enlace técnico designado',
  correoResponsable: 'Correo del responsable de protección de datos',
  documentoAutorizacion: 'Documento de autorización institucional',
  resolucionHabilitante: 'Resolución o acto administrativo habilitante',
};

function EstadoEnvio(estado, mensajeError) {
  if (estado === 'enviando') {
    return <p className="estado estado-cargando" role="status">Enviando la información, un momento...</p>;
  }
  if (estado === 'exito') {
    return <p className="estado estado-exito" role="status">¡Listo! La entidad quedó registrada correctamente.</p>;
  }
  if (estado === 'error') {
    return <p className="estado estado-error" role="alert">{mensajeError}</p>;
  }
  return null;
}

export default function FormularioEntidad() {
  const [valores, setValores] = useState(CAMPOS_INICIALES);
  const [archivos, setArchivos] = useState({ documentoAutorizacion: null, resolucionHabilitante: null });
  const [errores, setErrores] = useState({});
  const [estadoEnvio, setEstadoEnvio] = useState('inicial'); // inicial | enviando | exito | error
  const [mensajeError, setMensajeError] = useState('');

  function manejarCambioTexto(campo, valor) {
    setValores((anterior) => ({ ...anterior, [campo]: valor }));
  }

  function manejarCambioArchivo(campo, archivo) {
    setArchivos((anterior) => ({ ...anterior, [campo]: archivo }));
  }

  function validarTodoElFormulario() {
    const nuevosErrores = {};

    for (const campo of Object.keys(VALIDADORES_TEXTO)) {
      const error = VALIDADORES_TEXTO[campo](valores[campo]);
      if (error) nuevosErrores[campo] = error;
    }

    const errorDocumento = validarArchivoPdf(archivos.documentoAutorizacion);
    if (errorDocumento) nuevosErrores.documentoAutorizacion = errorDocumento;

    const errorResolucion = validarArchivoPdf(archivos.resolucionHabilitante);
    if (errorResolucion) nuevosErrores.resolucionHabilitante = errorResolucion;

    setErrores(nuevosErrores);
    return Object.keys(nuevosErrores).length === 0;
  }

  async function manejarEnvio(evento) {
    evento.preventDefault();

    if (!validarTodoElFormulario()) {
      return;
    }

    setEstadoEnvio('enviando');
    setMensajeError('');

    try {
      await registrarEntidad({ ...valores, ...archivos });
      setEstadoEnvio('exito');
      setValores(CAMPOS_INICIALES);
      setArchivos({ documentoAutorizacion: null, resolucionHabilitante: null });
      setErrores({});
      evento.target.reset();
    } catch (error) {
      setEstadoEnvio('error');
      setMensajeError(error.message);
    }
  }

  const estaEnviando = estadoEnvio === 'enviando';

  return (
    <form className="formulario-entidad" onSubmit={manejarEnvio} noValidate>
      <h1>Registro de entidad</h1>
      <p className="subtitulo">
        Completa los datos de la entidad que va a consumir el servicio de consulta de registros.
      </p>

      <CampoTexto
        id="identificacionFiscal"
        etiqueta={ETIQUETAS.identificacionFiscal}
        valor={valores.identificacionFiscal}
        error={errores.identificacionFiscal}
        deshabilitado={estaEnviando}
        onChange={(valor) => manejarCambioTexto('identificacionFiscal', valor)}
        placeholder="Ej: 155000123"
      />

      <CampoTexto
        id="nombreOficial"
        etiqueta={ETIQUETAS.nombreOficial}
        valor={valores.nombreOficial}
        error={errores.nombreOficial}
        deshabilitado={estaEnviando}
        onChange={(valor) => manejarCambioTexto('nombreOficial', valor)}
        placeholder="Ej: Ministerio de Ejemplo"
      />

      <CampoTexto
        id="ipPublica"
        etiqueta={ETIQUETAS.ipPublica}
        valor={valores.ipPublica}
        error={errores.ipPublica}
        deshabilitado={estaEnviando}
        onChange={(valor) => manejarCambioTexto('ipPublica', valor)}
        placeholder="Ej: 190.34.10.25"
      />

      <CampoTexto
        id="enlaceTecnico"
        etiqueta={ETIQUETAS.enlaceTecnico}
        valor={valores.enlaceTecnico}
        error={errores.enlaceTecnico}
        deshabilitado={estaEnviando}
        onChange={(valor) => manejarCambioTexto('enlaceTecnico', valor)}
        placeholder="Ej: María González"
      />

      <CampoTexto
        id="correoResponsable"
        etiqueta={ETIQUETAS.correoResponsable}
        tipo="email"
        valor={valores.correoResponsable}
        error={errores.correoResponsable}
        deshabilitado={estaEnviando}
        onChange={(valor) => manejarCambioTexto('correoResponsable', valor)}
        placeholder="Ej: proteccion.datos@institucion.gob"
      />

      <CampoArchivo
        id="documentoAutorizacion"
        etiqueta={ETIQUETAS.documentoAutorizacion}
        error={errores.documentoAutorizacion}
        deshabilitado={estaEnviando}
        onChange={(archivo) => manejarCambioArchivo('documentoAutorizacion', archivo)}
      />

      <CampoArchivo
        id="resolucionHabilitante"
        etiqueta={ETIQUETAS.resolucionHabilitante}
        error={errores.resolucionHabilitante}
        deshabilitado={estaEnviando}
        onChange={(archivo) => manejarCambioArchivo('resolucionHabilitante', archivo)}
      />

      <button type="submit" className="boton-enviar" disabled={estaEnviando}>
        {estaEnviando ? 'Enviando...' : 'Registrar entidad'}
      </button>

      {EstadoEnvio(estadoEnvio, mensajeError)}
    </form>
  );
}

function CampoTexto({ id, etiqueta, valor, error, deshabilitado, onChange, placeholder, tipo = 'text' }) {
  return (
    <div className="campo">
      <label htmlFor={id}>{etiqueta}</label>
      <input
        id={id}
        name={id}
        type={tipo}
        value={valor}
        placeholder={placeholder}
        disabled={deshabilitado}
        aria-invalid={Boolean(error)}
        aria-describedby={error ? `${id}-error` : undefined}
        onChange={(evento) => onChange(evento.target.value)}
      />
      {error && (
        <span id={`${id}-error`} className="mensaje-error">
          {error}
        </span>
      )}
    </div>
  );
}

function CampoArchivo({ id, etiqueta, error, deshabilitado, onChange }) {
  return (
    <div className="campo">
      <label htmlFor={id}>
        {etiqueta} <span className="ayuda">(PDF, máximo {TAMANO_MAXIMO_ARCHIVO_MB} MB)</span>
      </label>
      <input
        id={id}
        name={id}
        type="file"
        accept="application/pdf"
        disabled={deshabilitado}
        aria-invalid={Boolean(error)}
        aria-describedby={error ? `${id}-error` : undefined}
        onChange={(evento) => onChange(evento.target.files?.[0] ?? null)}
      />
      {error && (
        <span id={`${id}-error`} className="mensaje-error">
          {error}
        </span>
      )}
    </div>
  );
}
