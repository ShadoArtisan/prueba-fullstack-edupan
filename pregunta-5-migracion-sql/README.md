# Pregunta 5 — Actualizar una consulta del sistema viejo al sistema nuevo

## ¿De qué se trata?

El sistema anterior tenía una consulta que armaba un reporte: cuántas veces consultó cada
organización, agrupado por tipo de consulta y por día, durante el año. Esa consulta hay
que pasarla al nuevo sistema, corrigiendo un par de errores que tenía.

## Los problemas que se encontraron

**Problema 1 — se perdía información sin que nadie lo notara.** La consulta original
pedía el reporte "del 1 de enero al 31 de diciembre". El detalle es que, técnicamente,
esa instrucción significa "hasta el 31 de diciembre a las 00:00 horas en punto". Es decir,
cualquier consulta hecha el 31 de diciembre después de la medianoche quedaba fuera del
reporte, sin ningún aviso ni error. Se corrigió pidiendo el reporte "desde el 1 de enero
de este año hasta el 1 de enero del año siguiente", así no se pierde nada.

**Problema 2 — el reporte era más lento de lo necesario.** La consulta original agrupaba
los resultados usando la fecha ya convertida a texto (por ejemplo, "31/12/2025"), y además
hacía esa conversión dos veces por cada fila. Convertir la fecha a texto antes de agrupar
obliga a revisar cada resultado uno por uno de la forma más lenta posible. Se corrigió
agrupando primero por la fecha "en bruto" y dejando la conversión a texto solo para el
resultado final que se muestra.

## La consulta ya corregida

Está en el archivo [`migracion-sybase-sqlserver.sql`](migracion-sybase-sqlserver.sql), con
comentarios explicando cada cambio.

## La misma idea, escrita como parte del programa nuevo

Además de la consulta corregida, se escribió la misma lógica pero como código, tal como se
usaría dentro del sistema nuevo. Este código está en la carpeta
[`linq-equivalente/`](linq-equivalente) y es un proyecto aparte y autocontenido —no
depende de la carpeta de la pregunta 4— para que se pueda revisar y compilar por sí solo.

### Cómo comprobar que compila

Se necesita tener instalado el **.NET 8 SDK**. Con eso, en una terminal:

```bash
cd linq-equivalente
dotnet build
```

Si aparece el mensaje "Compilación correcta", significa que el código está bien escrito y
listo para usarse.
