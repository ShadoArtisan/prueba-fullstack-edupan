# Pregunta 5 — Actualizar una consulta del sistema viejo al nuevo

## De qué va esto

El sistema anterior tenía una consulta que armaba un reporte: cuántas veces consultó cada
organización, agrupado por tipo de consulta y por día, durante el año. Había que pasar esa
consulta al sistema nuevo, aprovechando de paso para corregir un par de errores que tenía.

## Los problemas que le encontré

**Se estaba perdiendo información sin que nadie lo notara.** La consulta original pedía
el reporte "del 1 de enero al 31 de diciembre". El detalle es que, técnicamente, eso
significa "hasta el 31 de diciembre a las 00:00 en punto". O sea que cualquier consulta
hecha el 31 de diciembre después de la medianoche quedaba afuera del reporte, sin ningún
aviso. Lo arreglé pidiendo el reporte "desde el 1 de enero de este año hasta el 1 de enero
del año siguiente", así no se escapa nada.

**El reporte era más lento de lo que tenía que ser.** La consulta original agrupaba los
resultados usando la fecha ya convertida a texto (tipo "31/12/2025"), y encima hacía esa
conversión dos veces por cada fila. Convertir la fecha a texto antes de agrupar obliga a
revisar cada resultado de la forma más lenta posible. Lo arreglé agrupando primero por la
fecha tal cual, y dejando la conversión a texto solo para el resultado final que se
muestra.

## La consulta ya corregida

Está en [`migracion-sybase-sqlserver.sql`](migracion-sybase-sqlserver.sql), comentada
para que se entienda cada cambio.

## La misma idea, pero como parte del programa nuevo

Aparte de la consulta corregida, escribí la misma lógica en código, tal como se usaría
dentro del sistema nuevo. Está en la carpeta [`linq-equivalente/`](linq-equivalente), y es
un proyecto aparte que no depende de la carpeta de la pregunta 4 — se puede revisar y
compilar solo.

### Para comprobar que compila

Hace falta el **.NET 8 SDK** instalado. Con eso, en una terminal:

```bash
cd linq-equivalente
dotnet build
```

Si sale "Compilación correcta", quiere decir que el código está bien y listo para usarse.
