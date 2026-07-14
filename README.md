# Prueba técnica — Desarrollador Fullstack

Repositorio de código para el **Módulo II** de la evaluación técnica (preguntas 4 a 15). El
**Módulo I** (análisis contextual y comunicación técnica, preguntas 1 a 3) se entrega aparte,
en un documento de texto (Word/PDF), tal como pide el enunciado.

Cada pregunta con ejercicio de código tiene su **propia carpeta**, independiente de las
demás, con su código, su análisis escrito y su propio README con instrucciones de ejecución.

## Índice de respuestas por pregunta

| Pregunta | Carpeta |
|---|---|
| 4 — API REST .NET 8 (endpoint de consulta) | [`pregunta-4-api-registros/`](pregunta-4-api-registros) |
| 5 — Migración SQL Sybase → SQL Server + LINQ | [`pregunta-5-migracion-sql/`](pregunta-5-migracion-sql) |
| 6 — Formulario frontend de registro de entidad | [`pregunta-6-formulario-entidad/`](pregunta-6-formulario-entidad) |
| 7 — Esquema de base de datos SQL Server | [`pregunta-7-esquema-basedatos/`](pregunta-7-esquema-basedatos) |

Entra a cada carpeta para ver el análisis escrito (problemas identificados, justificación de
índices, decisiones de diseño, etc.) y las instrucciones puntuales para correrla.

## Estructura del repositorio

```
pregunta-4-api-registros/     -> API .NET 8: endpoint de consulta + tests
pregunta-5-migracion-sql/     -> Consulta migrada Sybase -> SQL Server + equivalente LINQ
pregunta-6-formulario-entidad/-> Formulario React de registro de entidad
pregunta-7-esquema-basedatos/ -> Script SQL Server: tablas, índices y trigger
```

Las preguntas 4 y 7 comparten el mismo modelo de datos (la Pregunta 4 corre contra las tablas
que crea la Pregunta 7), y la Pregunta 5 reutiliza esas mismas tablas para su reporte — cada
README lo señala donde corresponde.

## Cómo ejecutar todo

1. **Pregunta 7** — corre `schema.sql` y `datos-prueba.sql` para tener la base de datos.
2. **Pregunta 4** — `dotnet run` para levantar la API, `dotnet test` para los tests.
3. **Pregunta 5** — `dotnet build` dentro de `linq-equivalente/` para verificar que compila.
4. **Pregunta 6** — `npm install && npm run dev` para el formulario.

El detalle completo de cada paso está en el README de la carpeta correspondiente.

## Sobre este código

Esta prueba se resolvió con el nivel de un desarrollador fullstack junior: se priorizó que
cada pieza funcione, esté bien organizada en capas y sea fácil de seguir, más que mostrar
patrones avanzados o abstracciones que no pide el enunciado.
