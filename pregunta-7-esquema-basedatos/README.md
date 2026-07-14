# Pregunta 7 — Esquema de base de datos en SQL Server (10 pts)

Script completo en [`schema.sql`](schema.sql): `CREATE TABLE`, índices y el trigger de cuota
diaria, todo comentado. [`datos-prueba.sql`](datos-prueba.sql) tiene una entidad y un registro
de ejemplo para poder probar el endpoint de la [Pregunta 4](../pregunta-4-api-registros) a
mano.

## Cómo ejecutarlo

```bash
sqlcmd -S localhost -i schema.sql
sqlcmd -S localhost -i datos-prueba.sql
```

## 11. Tabla principal de registros

`Registros`: los cuatro campos de salida del endpoint (`Estado`, `NumeroRegistro`,
`FechaEvento`, `FechaInscripcion`) más `Identificador` y `Nombre`, que son los campos por los
que se busca.

## 12. Tabla de entidades autorizadas

`Entidades`: `FechaInicioConvenio` y `FechaVencimiento` (vigencia del convenio), `CuotaDiaria`
—configurable por entidad, a diferencia del legado donde estaba fija en 5000 para todas— y
`Activo`.

## 13. Tabla de log de accesos

`LogAccesos`: entidad, fecha/hora, tipo de consulta, identificador consultado, resultado y
motivo. Con esos campos alcanza tanto para auditoría (quién consultó qué y cuándo) como para
calcular la cuota diaria (contar aprobados de hoy por entidad).

## 14. Índices justificados

- **`IX_Registros_Identificador_Nombre`** sobre `(Identificador, Nombre)`, con columnas
  incluidas para cubrir toda la respuesta del endpoint. Cubre el patrón de búsqueda puntual:
  la especificación de la Pregunta 4 prohíbe buscar solo por identificador, así que los dos
  campos siempre llegan juntos.
- **`IX_LogAccesos_EntidadId_FechaHora_Aprobado`** sobre `(EntidadId, FechaHora)`, **filtrado**
  por `Resultado = 'APROBADO'`. El cálculo de cuota diaria solo cuenta consultas aprobadas de
  hoy; filtrar directamente en el índice lo mantiene chico (no indexa los rechazos, que son la
  mayoría de las filas en un escenario con abuso de cuota) y más rápido de mantener.

## 15. Constraint / trigger de cuota diaria

`TR_LogAccesos_ValidarCuotaDiaria`: si un `INSERT` dejaría a una entidad con más consultas
`APROBADO` hoy que su `CuotaDiaria`, se revierte la transacción (`ROLLBACK` + `THROW`).

La validación "de verdad" vive en el servicio C# de la
[Pregunta 4](../pregunta-4-api-registros) (`RegistroConsultaService`), porque ahí se puede
responder `429` con un mensaje claro antes de tocar la base. Este trigger es una **segunda
barrera** (defensa en profundidad): protege la integridad del dato aunque algo escriba
directo a la tabla sin pasar por la API, o ante una condición de carrera entre dos requests
simultáneos.
