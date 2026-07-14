# Módulo II — Respuestas (preguntas 4 a 15)

Este documento indexa dónde está resuelta cada pregunta del Módulo II y agrega el análisis
escrito que no vive directamente en el código. El Módulo I se entrega aparte, en el
documento de texto correspondiente.

---

## Pregunta 4 — API REST .NET 8: endpoint de consulta con control de acceso (25 pts)

**Código:** [`backend/src/RegistrosInstitucionales.Api`](../backend/src/RegistrosInstitucionales.Api)
**Tests:** [`backend/tests/RegistrosInstitucionales.Api.Tests`](../backend/tests/RegistrosInstitucionales.Api.Tests)
**Cómo correrlo:** [`backend/README.md`](../backend/README.md)

Resumen de cómo se cubrió cada criterio de evaluación:

- **Separación de capas** — `Controllers/RegistrosController.cs` → `Services/RegistroConsultaService.cs`
  → `Repositories/*Repository.cs`. El controller no toca EF Core ni SQL directamente.
- **Manejo de errores HTTP** — 401 (sin API Key o API Key inválida), 403 (convenio inactivo o
  vencido), 400 (falta `identificador` o `nombre`, vía `[ApiController]` + DataAnnotations),
  404 (registro no encontrado), 429 (cuota diaria superada), 500 (excepción no controlada,
  capturada por el middleware global en `Program.cs`).
- **Validaciones de entrada** — DataAnnotations en `Dtos/ConsultaRegistroRequest.cs`
  (`[Required]` en ambos campos, así que nunca se permite buscar solo por identificador).
- **Acceso a datos** — Entity Framework Core sobre SQL Server (`Data/AppDbContext.cs`).
- **Inyección de dependencias** — todo registrado en `Program.cs` (repositorios, servicio,
  filtro de autenticación, `DbContext`).
- **Test unitario de cuota diaria** — `CuotaDiariaTests.cs` prueba la función pura
  `RegistroConsultaService.SuperoCuotaDiaria(...)`, y `RegistroConsultaServiceTests.cs` prueba
  el flujo completo del servicio con los repositorios mockeados (Moq).

---

## Pregunta 5 — Migración de consulta SQL: Sybase a SQL Server (15 pts)

**Código:** [`backend/sql/02_migracion_sybase_sqlserver.sql`](../backend/sql/02_migracion_sybase_sqlserver.sql)
y [`backend/src/RegistrosInstitucionales.Api/Reportes`](../backend/src/RegistrosInstitucionales.Api/Reportes)

**8. Consulta reescrita** — está completa en el archivo `.sql`, con comentarios explicando
cada cambio.

**9. Problemas identificados en la consulta original:**

1. **Corrección, no solo rendimiento** — `BETWEEN '2025-01-01' AND '2025-12-31'` sobre una
   columna de tipo fecha-hora en realidad significa "hasta el 31 de diciembre a las 00:00:00
   en punto". Cualquier acceso registrado el 31 de diciembre después de la medianoche queda
   fuera del reporte sin que nadie lo note. Es un bug silencioso de rango de fechas. Se
   corrige usando un rango medio-abierto: desde el 1 de enero hasta el 1 de enero del año
   siguiente (sin incluir).
2. **Rendimiento del `GROUP BY`** — la consulta original agrupa por el resultado de formatear
   la fecha como texto (`CONVERT(VARCHAR, ..., 103)`), calculado además dos veces (en el
   `SELECT` y en el `GROUP BY`). Eso obliga a calcular esa conversión fila por fila antes de
   poder agrupar, y ningún índice sobre la fecha ayuda a esa agrupación. La solución es
   agrupar por la fecha "pura" (`CAST(... AS DATE)`) y dejar el formateo de texto solo para
   mostrar el resultado. También ayuda tener un índice filtrado por
   `Resultado = 'APROBADO'` sobre `(EntidadId, FechaHora)` (ver Pregunta 7, punto 14), porque
   ese filtro está presente siempre en esta consulta.

**10. Equivalente en LINQ / EF Core** — código real y compilable en
`Reportes/ReporteAccesosRepository.cs`, con el DTO tipado en `Reportes/LogAccesoResumenDto.cs`.

---

## Pregunta 6 — Frontend: formulario de registro de entidad (15 pts)

**Código:** [`frontend/entidad-registro-form`](../frontend/entidad-registro-form)
**Cómo correrlo:** [`frontend/entidad-registro-form/README.md`](../frontend/entidad-registro-form/README.md)

Formulario en React con los 7 campos pedidos, validación en el cliente con mensajes en
lenguaje simple, envío mediante `fetch` a `POST /api/entidades` y manejo de los estados de
carga, éxito y error. Se probó a mano en el navegador (detalle de lo probado en el README del
proyecto).

---

## Pregunta 7 — Esquema de base de datos en SQL Server (10 pts)

**Código:** [`backend/sql/01_schema.sql`](../backend/sql/01_schema.sql)

- **11.** Tabla `Registros`: campos de salida (`Estado`, `NumeroRegistro`, `FechaEvento`,
  `FechaInscripcion`) más `Identificador` y `Nombre` para la búsqueda.
- **12.** Tabla `Entidades`: `FechaInicioConvenio`, `FechaVencimiento`, `CuotaDiaria`
  (configurable por entidad, a diferencia del legado donde estaba fija en 5000) y `Activo`.
- **13.** Tabla `LogAccesos`: entidad, fecha/hora, tipo de consulta, identificador consultado,
  resultado y motivo — suficiente para auditoría y para calcular la cuota diaria.
- **14.** Dos índices justificados:
  - `IX_Registros_Identificador_Nombre` — cubre la búsqueda puntual del endpoint (siempre se
    busca por los dos campos juntos).
  - `IX_LogAccesos_EntidadId_FechaHora_Aprobado` — índice **filtrado** por
    `Resultado = 'APROBADO'`, porque el cálculo de cuota diaria solo cuenta consultas
    aprobadas de hoy; filtrar en el índice lo mantiene chico y rápido.
- **15.** Trigger `TR_LogAccesos_ValidarCuotaDiaria`: si un `INSERT` dejaría a una entidad con
  más consultas `APROBADO` hoy que su `CuotaDiaria`, se revierte la transacción. Es una
  segunda barrera además de la validación que ya hace el servicio C# (Pregunta 4): protege la
  integridad del dato aunque alguien escriba directo a la base sin pasar por la API.
