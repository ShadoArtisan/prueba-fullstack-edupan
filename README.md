# Prueba técnica del módulo 2 - vicente urriola

Acá están las cuatro respuestas de la parte práctica (las que piden código, no solo
texto). Cada una tiene su propia carpeta, separada del resto, para que sea fácil ubicar
cuál es cuál.

## Qué hay en cada carpeta

| Pregunta | Carpeta | En corto |
|---|---|---|
| 4 | [`pregunta-4-api-registros/`](pregunta-4-api-registros) | Un programa que recibe una solicitud, revisa si quien pregunta tiene permiso, y responde si el registro existe o no. |
| 5 | [`pregunta-5-migracion-sql/`](pregunta-5-migracion-sql) | Una consulta del sistema viejo, corregida y adaptada al sistema nuevo. |
| 6 | [`pregunta-6-formulario-entidad/`](pregunta-6-formulario-entidad) | Un formulario web para registrar una organización nueva, con avisos claros si algo falta o está mal. |
| 7 | [`pregunta-7-esquema-basedatos/`](pregunta-7-esquema-basedatos) | El diseño de las tablas donde se guarda toda esa información. |

## Cómo se conecta todo

No son cuatro piezas sueltas, son parte del mismo sistema:

1. Primero se crean las tablas de la **pregunta 7** (ahí se guarda todo).
2. El programa de la **pregunta 4** lee y escribe en esas mismas tablas.
3. La consulta de la **pregunta 5** también usa esas tablas, para armar un reporte.
4. El formulario de la **pregunta 6** es la parte visual: lo que vería alguien usando el
   sistema para registrar una organización nueva.

Cada carpeta tiene su propio README con los pasos para probar esa parte.

## Qué hace falta tener instalado

Para poder correr todo el código de este repositorio (no solo leerlo) hace falta:

- **.NET 8 SDK** — corre el programa de la pregunta 4 y el proyecto de la pregunta 5.
  Se descarga gratis desde el sitio oficial de Microsoft (dotnet.microsoft.com). No hace
  falta un IDE especial: alcanza con la terminal, aunque si quieres editar el código
  cómodamente, **Visual Studio**, **Visual Studio Code** (con la extensión de C#) o
  **Rider** funcionan bien.
- **Node.js 18 o más nuevo** (incluye `npm`) — corre el formulario de la pregunta 6.
  También gratis, desde nodejs.org.
- **SQL Server** — donde se crean las tablas de la pregunta 7 y contra el que corre el
  programa de la pregunta 4. Puede ser SQL Server Express, Developer Edition, LocalDB, o
  un contenedor de Docker con SQL Server; cualquiera de esas opciones sirve.
- **sqlcmd** (para ejecutar los archivos `.sql` desde la terminal) o, si lo prefieres,
  **SQL Server Management Studio (SSMS)** para hacerlo con una interfaz gráfica.
- Opcional: **curl** o algo como **Postman**/**Insomnia** para probar el programa de la
  pregunta 4 a mano, enviándole solicitudes de prueba.

Nada de esto tiene costo, todas son herramientas gratuitas.
