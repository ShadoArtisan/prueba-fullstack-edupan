# Pregunta 7 — Diseño del esquema de Base de Datos

En el archivo `schema.sql` definí la estructura relacional del sistema y algunas restricciones de integridad. Además, armé un `datos-prueba.sql` con data mockeada para poder testear los endpoints de la [pregunta 4](../pregunta-4-api-registros) sin problemas.

## Ejecutar scripts

```bash
sqlcmd -S localhost -i schema.sql
sqlcmd -S localhost -i datos-prueba.sql
```

## Estructura de tablas

El esquema se centra en tres entidades principales:
- **Organizaciones:** Almacena los clientes autorizados, su estado de vigencia y el límite de requests (rate limit) que tienen permitido por día.
- **Registros:** Contiene el catálogo consultable (número de registro, estado, fechas).
- **Historial de accesos:** Tabla transaccional para auditoría. Registra cada intento de consulta contra la API (aprobado o rechazado) y es la tabla que se usa para calcular el consumo diario por entidad.

## Optimización y Rendimiento

Agregué un par de índices no agrupados (Non-Clustered Indexes) para acelerar las lecturas en tablas que tienden a crecer mucho:
1. Un índice compuesto para optimizar las búsquedas conjuntas por `identificación` y `nombre` (el caso de uso principal de la API).
2. Un índice sobre el historial de consultas para agilizar el conteo del rate limiting diario (COUNT) de cada organización.

## Integridad de datos (Constraints)

Para hacer el diseño más robusto, implementé un trigger/constraint a nivel de base de datos que aborta la inserción de una consulta "aprobada" en el historial si la organización ya superó su cuota del día. El backend ya maneja esto limpiamente, pero sirve como última barrera de defensa para mantener la consistencia de los datos en caso de una intervención directa en la base de datos.