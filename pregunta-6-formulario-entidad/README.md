# Pregunta 6 — Formulario de registro de entidad

Formulario en React (Vite) para registrar una nueva entidad en el módulo administrativo.

## Qué incluye

- Los 7 campos pedidos en el enunciado, con validación en el cliente antes de enviar.
- Mensajes de error por campo, en español simple (sin términos técnicos).
- Envío al backend con `fetch` a `POST /api/entidades` (usando `FormData` porque hay dos archivos adjuntos).
- Manejo de los tres estados de la petición: cargando, éxito y error.

Todo el código relevante está en `src/`:

| Archivo | Qué hace |
|---|---|
| `src/utils/validaciones.js` | Reglas de validación de cada campo (identificador con dígito verificador, IPv4, correo, tamaño/tipo de PDF) |
| `src/components/FormularioEntidad.jsx` | El formulario en sí: estado, validación y envío |
| `src/api/entidadesApi.js` | Llamada `fetch` al backend |

## Cómo ejecutarlo

Requisitos: Node.js 18 o superior.

```bash
cd pregunta-6-formulario-entidad
npm install
npm run dev
```

Abre `http://localhost:5173`.

Por defecto el formulario intenta enviar los datos a `https://localhost:7000` (la URL por
defecto del backend en desarrollo, ver [`../pregunta-4-api-registros/README.md`](../pregunta-4-api-registros/README.md)).
Si tu backend corre en otra URL, copia `.env.example` a `.env` y ajusta `VITE_API_BASE_URL`.

## Cómo se probó

Se levantó el servidor de desarrollo y se verificó a mano en el navegador:

- Enviar el formulario vacío muestra un mensaje de error específico debajo de cada campo
  (identificador, nombre, IP, enlace técnico, correo y los dos adjuntos).
- Un identificador con el dígito verificador incorrecto, una IP con octetos fuera de rango
  (`999.999.1.1`) y un correo sin `@` muestran su propio mensaje, sin jerga técnica.
- Al corregir un campo, solo desaparece el error de ese campo; los demás se mantienen hasta
  corregirse.

No se dejó corriendo un backend real durante la prueba manual, así que el camino "feliz"
completo (200 OK del servidor) se valida ejecutando el backend según su propio README y
repitiendo el envío con los dos PDF adjuntos.
