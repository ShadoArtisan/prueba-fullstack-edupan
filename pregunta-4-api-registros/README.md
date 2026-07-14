# Pregunta 4 — Programa que responde consultas de registros

## ¿Qué hace esto?

Imagina que una organización externa (por ejemplo, un banco o una entidad de gobierno)
necesita preguntarle a la institución: "¿existe un registro con este número y este
nombre?". Este programa es quien recibe esa pregunta y responde, pero antes hace varias
comprobaciones para cuidar la información:

1. **¿Quién pregunta?** La organización debe enviar una clave de acceso. Si no la envía, o
   la clave no es válida, el programa no responde nada más y avisa que falta identificarse.
2. **¿Tiene permiso vigente?** Si la clave es válida pero el convenio con esa organización
   ya venció o fue desactivado, tampoco se le da información.
3. **¿Preguntó bien?** Tiene que dar tanto el número de identificación como el nombre, no
   uno solo. Si falta alguno, se le pide que complete los datos.
4. **¿Ya preguntó demasiadas veces hoy?** Cada organización tiene un límite diario de
   consultas. Si ya lo alcanzó, se le avisa que debe esperar a mañana.
5. **¿Existe el registro?** Si pasó todos los pasos anteriores, se busca el registro y se
   le devuelve la información (o se le avisa que no se encontró nada con esos datos).

Cada uno de estos intentos —se le haya dado la información o no— queda guardado, con
fecha y hora, para poder revisar después quién consultó qué.

## Cómo está organizado el código

- Una parte recibe la solicitud y decide qué responder.
- Otra parte tiene las reglas explicadas arriba (permiso, límite diario, búsqueda).
- Otra parte se encarga de leer y guardar los datos en la base de datos.

Separarlo así hace que cada regla se pueda revisar y probar por separado, sin tener que
correr todo el sistema completo cada vez.

También hay un conjunto de pruebas automáticas que verifican, entre otras cosas, que el
límite diario de consultas se calcule correctamente.

> Este programa necesita las tablas que se crean en la carpeta
> [`pregunta-7-esquema-basedatos`](../pregunta-7-esquema-basedatos) para poder guardar y
> consultar datos reales.

## Cómo probarlo

Se necesita tener instalado el **.NET 8 SDK** (el programa que permite ejecutar este tipo
de proyectos) y, si se quiere probar con datos reales, un **SQL Server** disponible.

### 1. Preparar la base de datos (opcional)

Si quieres probarlo contra datos reales, primero crea las tablas y carga algunos datos de
ejemplo (esto está explicado en la carpeta de la pregunta 7):

```bash
sqlcmd -S localhost -i ../pregunta-7-esquema-basedatos/schema.sql
sqlcmd -S localhost -i ../pregunta-7-esquema-basedatos/datos-prueba.sql
```

### 2. Encender el programa

Abre una terminal en esta carpeta y escribe:

```bash
cd src/RegistrosInstitucionales.Api
dotnet run
```

Esto deja el programa corriendo y esperando solicitudes. También se abre una página de
prueba (Swagger) donde se puede probar el endpoint desde el navegador, sin necesidad de
escribir comandos.

### 3. Hacer una prueba real

Con los datos de ejemplo ya cargados, se puede simular una consulta con este comando (la
clave de acceso de prueba es `clave-demo-123`):

```bash
curl -k -X POST https://localhost:7000/api/registros/consulta \
  -H "Content-Type: application/json" \
  -H "X-API-Key: clave-demo-123" \
  -d "{\"identificador\": \"8-123-456\", \"nombre\": \"Juan Pérez\"}"
```

Con esos datos de ejemplo, debería responder con la información del registro. Si cambias
la clave de acceso por cualquier otra, o borras el dato del nombre, vas a ver que el
programa responde explicando qué faltó o qué salió mal, en vez de simplemente fallar.

### 4. Correr las pruebas automáticas

```bash
cd ..
dotnet test
```

Si todo está bien, debería decir que todas las pruebas pasaron correctamente.

## Decisiones que se tomaron y por qué

- **La clave de acceso nunca se guarda tal cual.** Se guarda una versión "encriptada" de
  ella, para que ni siquiera alguien con acceso a la base de datos pueda ver la clave
  original.
- **El límite diario se revisa en dos lugares distintos:** una vez en el programa (que es
  donde se le explica al usuario por qué se le negó la respuesta) y otra vez directamente
  en la base de datos, como respaldo, por si algún día alguien intenta escribir en la base
  de datos sin pasar por este programa.
