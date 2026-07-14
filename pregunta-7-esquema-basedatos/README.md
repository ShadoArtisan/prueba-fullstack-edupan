# Pregunta 7 — Diseño de las tablas donde se guarda todo

## De qué va esto

`schema.sql` es el "plano" de la base de datos: define las tres cajas donde se guarda la
información, más un par de reglas de seguridad automáticas. `datos-prueba.sql` agrega
algunos datos de ejemplo, para poder probar el programa de la
[pregunta 4](../pregunta-4-api-registros) sin tener que escribir todo a mano.

## Cómo crear las tablas

```bash
sqlcmd -S localhost -i schema.sql
sqlcmd -S localhost -i datos-prueba.sql
```

## Las tres cajas de información

- **Organizaciones autorizadas** — quién puede consultar, si su acceso sigue vigente, y
  cuántas consultas puede hacer por día (cada una puede tener un límite distinto).
- **Registros** — lo que se puede consultar: número de registro, estado, y las fechas
  relacionadas.
- **Historial de accesos** — queda anotado cada intento de consulta, se haya aprobado o
  no, con fecha y motivo. Sirve para dos cosas: revisar después quién consultó qué, y
  calcular cuántas consultas ya hizo cada organización hoy.

## Dos atajos para que las búsquedas no sean lentas

Cuando una tabla tiene muchísimos datos, buscar algo puede ponerse lento si no se le da
una guía al sistema para encontrar las cosas más rápido. Agregué dos de esas guías: una
para cuando alguien busca un registro por identificación y nombre (que es como siempre se
busca, según la pregunta 4), y otra para cuando el sistema necesita contar rápido cuántas
consultas aprobadas hizo una organización hoy.

## Un candado automático para el límite diario

Agregué una regla directo en la base de datos que impide guardar una consulta "aprobada"
si la organización ya llegó a su límite del día. Es un respaldo: la comprobación
principal ya la hace el programa de la pregunta 4 (que además le explica al usuario por
qué se le negó la respuesta), pero esta segunda capa protege la información aunque, por
algún motivo, alguien intente modificar la base de datos directamente, sin pasar por el
programa.
