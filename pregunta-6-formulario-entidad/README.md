# Pregunta 6 — Formulario de registro para organizaciones

Este es un frontend web para el alta de nuevas entidades que consumirán la API. 

El formulario recopila la siguiente información:
- RUC / Identificación fiscal (incluye validación de dígito verificador).
- Nombre oficial de la organización.
- Dirección IP del servidor origen.
- Contacto técnico y responsable de datos.
- Archivos PDF (Autorización institucional y resolución habilitante).

El cliente implementa validaciones en tiempo real para evitar viajes innecesarios al servidor. Si ingresas una IP con formato incorrecto o un correo sin arroba, la UI bloquea el envío y renderiza errores específicos debajo de cada input.

## Levantando el entorno (Node.js 18+)

Para levantar la aplicación en Vite:

```bash
cd pregunta-6-formulario-entidad
npm install
npm run dev
```

La app quedará corriendo en `http://localhost:5173`. 

Por defecto, el submit del formulario apunta al API de la [pregunta 4](../pregunta-4-api-registros). Asegúrate de tener el backend corriendo en otra terminal. Si el puerto del backend es distinto, se puede crear un archivo `.env` basándose en el `.env.example` y ajustar la variable de entorno correspondiente.