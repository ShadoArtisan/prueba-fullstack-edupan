# Pregunta 6 — Formulario para registrar una nueva organización

## De qué va esto

Una página web con un formulario para dar de alta a una organización nueva que va a usar
el sistema. Pide estos datos:

- Número de identificación fiscal (con un dígito de control, para detectar si alguien se
  equivocó al escribirlo).
- Nombre oficial de la organización.
- Dirección de internet (IP) del servidor desde el que se va a conectar.
- Nombre de la persona técnica encargada.
- Correo de la persona responsable de proteger los datos.
- Dos documentos en PDF: la autorización institucional y la resolución que la habilita.

Antes de enviar, la página revisa que cada dato esté completo y bien escrito. Si algo
falta o está mal, muestra un aviso claro al lado de ese campo (por ejemplo: "la dirección
de internet no tiene el formato correcto") en vez de un mensaje confuso. Recién cuando
todo está en orden, envía la información y avisa si el registro se guardó bien o si hubo
algún problema.

## Cómo probarlo

Hace falta tener **Node.js** (versión 18 o más nueva).

```bash
cd pregunta-6-formulario-entidad
npm install
npm run dev
```

El primer comando entra a la carpeta, el segundo instala lo que hace falta, y el tercero
prende la página. Después, abre esto en el navegador:

```
http://localhost:5173
```

Por defecto, el formulario manda los datos al programa de la
[pregunta 4](../pregunta-4-api-registros), que tiene que estar prendido en otra ventana
para que el envío funcione de punta a punta. Si ese programa corre en otra dirección, se
puede ajustar copiando `.env.example` a un archivo nuevo llamado `.env` y cambiando el
valor de adentro.

## Cómo lo probé

Abrí la página en el navegador y probé a mano:

- Enviar el formulario vacío: aparece un aviso debajo de cada campo, diciendo qué falta.
- Escribir una dirección de internet inventada (`999.999.1.1`) o un correo sin arroba:
  cada uno tira su propio aviso, en palabras simples.
- Corregir un solo campo: solo desaparece el aviso de ese campo, los demás quedan hasta
  que también se corrijan.
