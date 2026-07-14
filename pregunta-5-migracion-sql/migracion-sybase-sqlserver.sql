/*
    Pregunta 5 — Migración de consulta SQL: Sybase a SQL Server

    -------------------------------------------------------------
    Consulta original (Sybase ASE)
    -------------------------------------------------------------
    SELECT TOP 100
        e.nombre_entidad, l.tipo_consulta, COUNT(*) AS total_consultas,
        CONVERT(VARCHAR, l.fecha_hora, 103) AS fecha_formato
    FROM LogAccesos l
    INNER JOIN Entidades e ON l.entidad_id = e.id
    WHERE l.fecha_hora BETWEEN '2025-01-01' AND '2025-12-31'
      AND l.resultado = 'APROBADO'
    GROUP BY e.nombre_entidad, l.tipo_consulta, CONVERT(VARCHAR, l.fecha_hora, 103)
    ORDER BY total_consultas DESC

    -------------------------------------------------------------
    9. Problemas identificados en la consulta original
    -------------------------------------------------------------

    Problema 1 (CORRECTITUD, no solo rendimiento):
    "BETWEEN '2025-01-01' AND '2025-12-31'" sobre una columna DATETIME
    equivale a "BETWEEN '2025-01-01 00:00:00.000' AND '2025-12-31
    00:00:00.000'". Cualquier acceso registrado el 31 de diciembre
    después de la medianoche (ej. 31/12/2025 14:30) queda FUERA del
    reporte sin que nadie lo note. Es un bug silencioso de rango de
    fechas, no solo un tema de rendimiento.
    Solución: usar un rango medio-abierto con el primer día del año
    siguiente como límite superior exclusivo (>= inicio AND < fin).

    Problema 2 (RENDIMIENTO / DISEÑO):
    El GROUP BY agrupa por "CONVERT(VARCHAR, l.fecha_hora, 103)", es
    decir, por una cadena de texto calculada fila por fila. Esto impide
    que el motor use cualquier índice sobre fecha_hora para la agrupación
    (tiene que materializar el string para cada fila antes de agrupar) y
    además calcula el mismo CONVERT dos veces (en el SELECT y en el
    GROUP BY). Solución: agrupar por la fecha "pura" (CAST a DATE, que sí
    es sargable y aprovechable por índice) y dejar el formateo de texto
    solo para la columna de salida del SELECT.
    Además, conviene apoyar la consulta con un índice como
    IX_LogAccesos_EntidadId_FechaHora_Aprobado (ver Pregunta 7, punto 14)
    filtrado por Resultado = 'APROBADO', ya que ese es siempre el filtro
    de esta consulta.

    -------------------------------------------------------------
    8. Consulta reescrita para SQL Server 2019
    -------------------------------------------------------------
*/

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

/*
    -------------------------------------------------------------
    10. Equivalente en LINQ / Entity Framework Core
    -------------------------------------------------------------
    Implementado como código real y compilable en:
    backend/src/RegistrosInstitucionales.Api/Reportes/ReporteAccesosRepository.cs

    (el DTO tipado está en Reportes/LogAccesoResumenDto.cs). Resumen:

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

    Nota de diseño: el DTO expone la fecha como DateOnly (dato puro), no
    como el string "dd/MM/yyyy" ya formateado. Formatear para mostrar es
    responsabilidad de la capa de presentación (frontend o el controller
    que sirva este reporte), no del repositorio.
*/
