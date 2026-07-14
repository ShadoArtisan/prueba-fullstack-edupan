# Pregunta 4 — Programa que responde consultas de registros

## De qué va esto

Pensemos en una organización externa (un banco, una entidad de gobierno, lo que sea) que
necesita preguntarle a la institución "¿existe un registro con este número y este
nombre?". Este programa es el que recibe esa pregunta y contesta, pero antes hace unas
cuantas verificaciones:

1. **¿Quién pregunta?** Tiene que mandar una clave de acceso. Sin ella, o con una clave
   que no es válida, ahí queda: se le avisa que falta identificarse y no se sigue.
2. **¿Tiene permiso vigente?** Puede que la clave sea válida pero el convenio con esa
   organización ya venció o lo desactivaron. En ese caso tampoco se le da nada.
3. **¿Preguntó bien?** Tiene que mandar número de identificación y nombre juntos, no uno
   solo. Si falta alguno, se le pide que complete.
4. **¿Ya preguntó demasiado hoy?** Cada organización tiene un tope diario de consultas. Si
   ya lo llegó, se le avisa que tiene que esperar al día siguiente.
5. **¿Existe el registro?** Si pasó todo lo anterior, ahí sí se busca el dato y se le
   devuelve, o se le avisa que no hay nada con esos datos.

Cada intento queda guardado con fecha y hora —le haya dado la información o no— para
poder revisar después quién preguntó qué.

## Cómo está armado el código

- Una parte recibe la solicitud y decide qué contestar.
- Otra tiene las reglas de arriba (permiso, tope diario, búsqueda).
- Otra se encarga de leer y guardar en la base de datos.

Separarlo así permite revisar y probar cada regla por su cuenta, sin tener que levantar
todo el sistema para probar una sola cosa. También hay pruebas automáticas que confirman,
entre otras cosas, que el tope diario se calcula bien.

> Este programa necesita las tablas de la carpeta
> [`pregunta-7-esquema-basedatos`](../pregunta-7-esquema-basedatos) para poder guardar y
> consultar datos de verdad.

## Cómo probarlo

Hace falta el **.NET 8 SDK** instalado y, si quieres probarlo con datos reales, un **SQL
Server** a mano.

### 1. Preparar la base de datos (opcional)

Si quieres probarlo con datos reales, primero crea las tablas y carga algunos datos de
ejemplo (los pasos completos están en la carpeta de la pregunta 7):

```bash
sqlcmd -S localhost -i ../pregunta-7-esquema-basedatos/schema.sql
sqlcmd -S localhost -i ../pregunta-7-esquema-basedatos/datos-prueba.sql
```

### 2. Prender el programa

Abre una terminal en esta carpeta:

```bash
cd src/RegistrosInstitucionales.Api
dotnet run
```

Con esto el programa queda corriendo, esperando solicitudes. También se abre una página
de prueba (Swagger) para probarlo desde el navegador sin escribir comandos.

### 3. Probarlo de verdad

Con los datos de ejemplo cargados, se puede simular una consulta así (la clave de prueba
es `clave-demo-123`):

```bash
curl -k -X POST https://localhost:7000/api/registros/consulta \
  -H "Content-Type: application/json" \
  -H "X-API-Key: clave-demo-123" \
  -d "{\"identificador\": \"8-123-456\", \"nombre\": \"Juan Pérez\"}"
```

Debería devolver la información del registro. Si cambias la clave por cualquier otra, o
borras el nombre, vas a ver que el programa explica qué faltó en vez de simplemente
fallar sin decir nada.

### 4. Correr las pruebas automáticas

```bash
cd ..
dotnet test
```

Si todo salió bien, va a decir que todas las pruebas pasaron.

## Un par de decisiones que vale la pena explicar

- **La clave de acceso nunca se guarda tal cual.** Se guarda una versión encriptada, así
  ni alguien con acceso directo a la base de datos puede ver la clave original.
- **El tope diario se revisa dos veces:** una en el programa (ahí es donde se le explica
  al usuario por qué se le negó la respuesta) y otra directamente en la base de datos,
  como respaldo, por si alguna vez alguien intenta escribir ahí sin pasar por este
  programa.
