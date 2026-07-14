# Pregunta 5 — Migración de consulta SQL: Sybase a SQL Server (15 pts)

## Consulta original (Sybase ASE)

```sql
SELECT TOP 100
    e.nombre_entidad, l.tipo_consulta, COUNT(*) AS total_consultas,
    CONVERT(VARCHAR, l.fecha_hora, 103) AS fecha_formato
FROM LogAccesos l
INNER JOIN Entidades e ON l.entidad_id = e.id
WHERE l.fecha_hora BETWEEN '2025-01-01' AND '2025-12-31'
  AND l.resultado = 'APROBADO'
GROUP BY e.nombre_entidad, l.tipo_consulta, CONVERT(VARCHAR, l.fecha_hora, 103)
ORDER BY total_consultas DESC
```

## 9. Problemas identificados en la consulta original

**Problema 1 (de corrección, no solo de rendimiento).** `BETWEEN '2025-01-01' AND
'2025-12-31'` sobre una columna de fecha-y-hora en realidad significa "hasta el 31 de
diciembre a las 00:00:00 en punto". Cualquier acceso registrado el 31 de diciembre después de
la medianoche queda fuera del reporte sin que nadie lo note: es un bug silencioso de rango de
fechas. Se corrige con un rango medio-abierto: desde el 1 de enero hasta el 1 de enero del
año siguiente (sin incluir ese límite).

**Problema 2 (rendimiento del `GROUP BY`).** La consulta agrupa por el resultado de formatear
la fecha como texto (`CONVERT(VARCHAR, ..., 103)`), calculado además dos veces (una en el
`SELECT` y otra en el `GROUP BY`). Eso obliga a calcular esa conversión fila por fila antes de
poder agrupar, y ningún índice sobre la fecha ayuda a esa agrupación. Se corrige agrupando por
la fecha "pura" (`CAST(... AS DATE)`) y dejando el formateo de texto solo para mostrar el
resultado. También ayuda un índice filtrado por `Resultado = 'APROBADO'` sobre
`(EntidadId, FechaHora)` — ver [`../pregunta-7-esquema-basedatos/schema.sql`](../pregunta-7-esquema-basedatos/schema.sql),
punto 14 — porque ese filtro está presente siempre en esta consulta.

## 8. Consulta reescrita para SQL Server 2019

Ver [`migracion-sybase-sqlserver.sql`](migracion-sybase-sqlserver.sql).

```sql
SELECT TOP (100)
    e.NombreEntidad,
    l.TipoConsulta,
    COUNT(*) AS TotalConsultas,
    CONVERT(VARCHAR(10), CAST(l.FechaHora AS DATE), 103) AS FechaFormato
FROM dbo.LogAccesos AS l
INNER JOIN dbo.Entidades AS e ON l.EntidadId = e.EntidadId
WHERE l.FechaHora >= '20250101'
  AND l.FechaHora <  '20260101'          -- límite exclusivo: no trunca el 31/12
  AND l.Resultado = 'APROBADO'
GROUP BY e.NombreEntidad, l.TipoConsulta, CAST(l.FechaHora AS DATE)
ORDER BY TotalConsultas DESC;
```

## 10. Equivalente en LINQ / Entity Framework Core

Código real y compilable en [`linq-equivalente/`](linq-equivalente). Es un proyecto aparte
(no depende de `pregunta-4-api-registros`) con una copia mínima de las entidades
`Entidad`/`LogAcceso` — los mismos campos que en el esquema de la Pregunta 7, solo que acá
sirven nada más para esta consulta.

```csharp
var inicio = new DateTime(anio, 1, 1);
var fin = inicio.AddYears(1);

var resultado = await _context.LogAccesos
    .Where(l => l.FechaHora >= inicio && l.FechaHora < fin && l.Resultado == "APROBADO")
    .Join(_context.Entidades, log => log.EntidadId, ent => ent.EntidadId, (log, ent) => new { log, ent })
    .GroupBy(x => new { x.ent.NombreEntidad, x.log.TipoConsulta, Fecha = x.log.FechaHora.Date })
    .Select(g => new LogAccesoResumenDto(g.Key.NombreEntidad, g.Key.TipoConsulta, g.Count(), DateOnly.FromDateTime(g.Key.Fecha)))
    .OrderByDescending(r => r.TotalConsultas)
    .Take(100)
    .ToListAsync();
```

El DTO expone la fecha como `DateOnly` (dato puro), no como el string `dd/MM/yyyy` ya
formateado — formatear para mostrar es responsabilidad de la capa de presentación, no del
repositorio.

### Cómo compilarlo

```bash
cd linq-equivalente
dotnet build
```
