# Pregunta 5 — Migración y optimización de reportes

Este ejercicio consistía en migrar una consulta del sistema legacy (Sybase) a SQL Server. La consulta genera un reporte anual del total de peticiones por organización, agrupadas por día y por tipo. Aproveché la migración para resolver un par de problemas de la versión original.

## Optimizaciones implementadas

1. **Corrección de fechas límite (Edge cases):**  
   La consulta original filtraba "del 1 de enero al 31 de diciembre", lo que implícitamente cortaba el 31 de diciembre a las 00:00:00, dejando por fuera cualquier registro de ese último día. Lo ajusté usando el rango desde el 1 de enero del año actual hasta el 1 de enero del año siguiente (`<`).

2. **Mejora en el Group By:**  
   Anteriormente, la fecha se parseaba a texto (string) *antes* de agrupar, lo cual obligaba al motor de la base de datos a hacer conversiones fila por fila de forma ineficiente. Refactoricé el query para agrupar primero usando el tipo `Date` nativo y delegar el casteo a string solo en la proyección final (SELECT).

El script SQL actualizado y comentado está en [`migracion-sybase-sqlserver.sql`](migracion-sybase-sqlserver.sql).

## Implementación en LINQ (.NET 8)

Adicional al SQL crudo, armé la lógica equivalente utilizando LINQ, pensando en cómo se integraría esto en el nuevo backend. El código está en la carpeta [`linq-equivalente/`](linq-equivalente). Es un proyecto independiente, así que lo puede compilar por su cuenta.

Para verificar que compila correctamente:
```bash
cd linq-equivalente
dotnet build
```