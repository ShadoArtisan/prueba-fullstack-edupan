# Pregunta 7 — Diseño de las tablas donde se guarda todo

## ¿Qué hace esto?

Este archivo (`schema.sql`) es como el "plano" de la base de datos: define las tres
"cajas" donde se guarda la información, más un par de reglas de seguridad automáticas.
`datos-prueba.sql` agrega un par de datos de ejemplo, para poder probar el programa de la
[pregunta 4](../pregunta-4-api-registros) sin tener que escribir todo a mano.

## Cómo crear las tablas

```bash
sqlcmd -S localhost -i schema.sql
sqlcmd -S localhost -i datos-prueba.sql
```

## Las tres "cajas" de información

- **Organizaciones autorizadas** — quién puede consultar, si su acceso sigue vigente, y
  cuántas consultas puede hacer por día (cada organización puede tener un límite distinto).
- **Registros** — la información que se puede consultar: el número de registro, su
  estado, y las fechas relacionadas.
- **Historial de accesos** — queda anotado cada intento de consulta, se haya aprobado o
  no, con la fecha y el motivo. Esto sirve para dos cosas: poder revisar después quién
  consultó qué, y calcular cuántas consultas ya hizo cada organización hoy.

## Dos atajos para que las búsquedas sean rápidas

Cuando una tabla tiene muchísimos datos, buscar algo puede ser lento si no se le da una
"guía" al sistema para encontrar las cosas más rápido. Se agregaron dos de esas guías:

- Una para cuando alguien busca un registro por número de identificación y nombre (que es
  como siempre se busca, según lo pedido en la pregunta 4).
- Otra para cuando el sistema necesita contar rápidamente cuántas consultas aprobadas hizo
  una organización en el día de hoy.

## Un candado automático para el límite diario

Se agregó una regla directamente en la base de datos que impide guardar una consulta
"aprobada" si la organización ya alcanzó su límite del día. Esta regla es un respaldo: la
comprobación principal ya la hace el programa de la pregunta 4 (que además le explica al
usuario por qué se le negó la respuesta), pero esta segunda capa de seguridad protege la
información aunque, por algún motivo, alguien intente modificar la base de datos
directamente, sin pasar por el programa.
