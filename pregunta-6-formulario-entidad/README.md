# Pregunta 6 — Formulario para registrar una nueva organización

## ¿Qué hace esto?

Es una página web con un formulario para dar de alta a una organización nueva que va a
usar el sistema. Pide estos datos:

- Número de identificación fiscal de la organización (con un dígito de control, para
  detectar si alguien se equivocó al escribirlo).
- Nombre oficial de la organización.
- Dirección de internet (IP) del servidor desde el que va a conectarse.
- Nombre de la persona técnica encargada.
- Correo de la persona responsable de proteger los datos.
- Dos documentos en PDF: la autorización institucional y la resolución que habilita a la
  organización.

Antes de enviar el formulario, la página revisa que cada dato esté completo y bien escrito,
y si algo falta o está mal, muestra un aviso claro al lado de ese campo (por ejemplo: "la
dirección de internet no tiene el formato correcto"), en vez de un mensaje confuso. Recién
cuando todo está correcto, la página envía la información al sistema y muestra si el
registro se guardó con éxito o si hubo algún problema.

## Cómo probarlo

Se necesita tener instalado **Node.js** (versión 18 o más nueva).

```bash
cd pregunta-6-formulario-entidad
npm install
npm run dev
```

El primer comando entra a la carpeta, el segundo instala lo necesario para que funcione, y
el tercero enciende la página. Después de eso, abre esta dirección en el navegador:

```
http://localhost:5173
```

Por defecto, el formulario intenta enviar los datos al programa de la
[pregunta 4](../pregunta-4-api-registros), que debe estar encendido en otra ventana para
que el envío funcione de principio a fin. Si ese programa corre en otra dirección, se
puede ajustar copiando el archivo `.env.example` a uno nuevo llamado `.env` y cambiando el
valor de adentro.

## Cómo se comprobó que funciona

Se abrió la página en el navegador y se probó a mano:

- Enviar el formulario vacío: aparece un aviso debajo de cada campo, explicando qué falta.
- Escribir una dirección de internet inventada (como `999.999.1.1`) o un correo sin
  arroba: cada uno muestra su propio aviso, en palabras simples.
- Corregir un solo campo: solo desaparece el aviso de ese campo, los demás se mantienen
  hasta corregirse también.
