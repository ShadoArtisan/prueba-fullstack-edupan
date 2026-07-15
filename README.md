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

- .NET 8 SDK
- Node.js 18 o más nuevo (incluye npm)
- SQL Server (Express, Developer Edition, LocalDB o Docker)
- sqlcmd o SQL Server Management Studio (SSMS)
- Opcional: curl, Postman o Insomnia
- Opcional: Visual Studio, Visual Studio Code o Rider
