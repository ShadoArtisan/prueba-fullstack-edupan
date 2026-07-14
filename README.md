# Prueba técnica — Desarrollador Fullstack

Repositorio de código para el **Módulo II** de la evaluación técnica (preguntas 4 a 15). El
**Módulo I** (análisis contextual y comunicación técnica, preguntas 1 a 3) se entrega aparte,
en un documento de texto (Word/PDF), tal como pide el enunciado.

## Índice de respuestas por pregunta

| Pregunta | Dónde está | 
|---|---|
| 4 — API REST .NET 8 (endpoint de consulta) | [`backend/src/RegistrosInstitucionales.Api`](backend/src/RegistrosInstitucionales.Api) — instrucciones en [`backend/README.md`](backend/README.md) |
| 5 — Migración SQL Sybase → SQL Server + LINQ | [`backend/sql/02_migracion_sybase_sqlserver.sql`](backend/sql/02_migracion_sybase_sqlserver.sql) y [`backend/src/RegistrosInstitucionales.Api/Reportes`](backend/src/RegistrosInstitucionales.Api/Reportes) |
| 6 — Formulario frontend de registro de entidad | [`frontend/entidad-registro-form`](frontend/entidad-registro-form) — instrucciones en su propio README |
| 7 — Esquema de base de datos SQL Server | [`backend/sql/01_schema.sql`](backend/sql/01_schema.sql) |

El análisis escrito que acompaña al código (problemas identificados, justificación de
índices, etc.) está en [`docs/respuestas-modulo2.md`](docs/respuestas-modulo2.md), organizado
por número de pregunta.

## Estructura del repositorio

```
backend/    -> API .NET 8 (Pregunta 4), scripts SQL (Preguntas 5 y 7), tests
frontend/   -> Formulario React (Pregunta 6)
docs/       -> Análisis escrito, indexado por pregunta
```

## Cómo ejecutar todo

1. **Base de datos y API** — ver [`backend/README.md`](backend/README.md) para crear el
   esquema, cargar datos de prueba, correr la API y correr los tests (`dotnet test`).
2. **Frontend** — ver [`frontend/entidad-registro-form/README.md`](frontend/entidad-registro-form/README.md)
   para instalar dependencias y levantar el formulario con `npm run dev`.

## Sobre este código

Esta prueba se resolvió con el nivel de un desarrollador fullstack junior: se priorizó que
cada pieza funcione, esté bien organizada en capas y sea fácil de seguir, más que mostrar
patrones avanzados o abstracciones que no pide el enunciado.
