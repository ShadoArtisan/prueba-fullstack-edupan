# Backend — Servicio de consulta de registros institucionales

Cubre la **Pregunta 4** (API REST), la **Pregunta 5** (migración SQL + LINQ) y la
**Pregunta 7** (esquema de base de datos). Los scripts SQL de las preguntas 5 y 7 están en
[`sql/`](sql).

## Requisitos

- .NET 8 SDK
- SQL Server (local, Docker o LocalDB) — solo hace falta para probar contra datos reales;
  los tests unitarios no necesitan base de datos.

## Estructura

```
backend/
  src/RegistrosInstitucionales.Api/     -> la API (Controllers/Services/Repositories/Data)
  tests/RegistrosInstitucionales.Api.Tests/ -> tests unitarios (xUnit + Moq)
  sql/
    01_schema.sql                 -> Pregunta 7: tablas, índices y trigger
    02_migracion_sybase_sqlserver.sql -> Pregunta 5: consulta migrada + análisis + LINQ
    03_datos_prueba.sql           -> datos de ejemplo para probar el endpoint a mano
```

## 1. Preparar la base de datos

Ejecuta en orden, contra tu instancia de SQL Server (por ejemplo con `sqlcmd` o SSMS):

```bash
sqlcmd -S localhost -i sql/01_schema.sql
sqlcmd -S localhost -i sql/03_datos_prueba.sql
```

Ajusta la cadena de conexión en `src/RegistrosInstitucionales.Api/appsettings.json`
(`ConnectionStrings:SqlServer`) si tu servidor no es `localhost` con autenticación de Windows.

## 2. Ejecutar la API

```bash
cd src/RegistrosInstitucionales.Api
dotnet run
```

Con el proyecto en modo desarrollo, Swagger queda disponible en `/swagger` para probar el
endpoint desde el navegador.

### Probar el endpoint manualmente

Con los datos de `03_datos_prueba.sql` cargados, la API Key de prueba (texto plano) es
`clave-demo-123`:

```bash
curl -k -X POST https://localhost:7000/api/registros/consulta \
  -H "Content-Type: application/json" \
  -H "X-API-Key: clave-demo-123" \
  -d "{\"identificador\": \"8-123-456\", \"nombre\": \"Juan Pérez\"}"
```

Casos para probar cada código de respuesta:

| Caso | Cómo provocarlo | Respuesta esperada |
|---|---|---|
| Éxito | Datos de arriba, tal cual | `200 OK` con el registro |
| Falta la API Key | Quitar el header `X-API-Key` | `401 Unauthorized` |
| API Key inválida | Usar cualquier otro texto en `X-API-Key` | `401 Unauthorized` |
| Convenio vencido/inactivo | Poner `FechaVencimiento` en el pasado o `Activo = 0` en la entidad | `403 Forbidden` |
| Falta el nombre o el identificador | Omitir `nombre` en el body | `400 Bad Request` (validación automática) |
| Registro no encontrado | Cambiar el identificador a uno inexistente | `404 Not Found` |
| Cuota diaria superada | Bajar `CuotaDiaria` de la entidad a un número ya alcanzado | `429 Too Many Requests` |

Cada intento (aprobado o rechazado) queda registrado en `dbo.LogAccesos`.

## 3. Ejecutar los tests

```bash
cd backend
dotnet test
```

Incluye el test unitario pedido en la Pregunta 4 sobre la lógica de cuota diaria
(`tests/RegistrosInstitucionales.Api.Tests/CuotaDiariaTests.cs`) y pruebas de la capa de
servicio completa con mocks de los repositorios
(`RegistroConsultaServiceTests.cs`).

## Decisiones de diseño (resumen)

- **Capas**: `Controllers` → `Services` → `Repositories`, cada una con su interfaz, para que
  el servicio se pueda testear sin base de datos (ver Moq en los tests).
- **Autenticación por API Key**: se implementó como un `IAsyncActionFilter`
  (`Auth/ApiKeyAuthFilter.cs`) que corre antes del controller, resuelve la entidad y la deja
  en `HttpContext.Items` — así el controller no repite la búsqueda ni mezcla lógica de
  autenticación con lógica de negocio. La API Key nunca se guarda en texto plano, solo su
  hash SHA-256 (`Auth/ApiKeyHasher.cs`).
- **Errores HTTP**: el servicio devuelve un resultado de negocio neutro
  (`Services/ResultadoConsultaRegistro.cs`) y es el controller quien decide el código HTTP
  (200/404/429). Los errores no controlados los captura un middleware global en
  `Program.cs` y responden 500 sin filtrar detalles internos.
- **Cuota diaria**: la validación "de verdad" vive en `RegistroConsultaService` (para poder
  responder 429 con contexto), y el trigger SQL (`sql/01_schema.sql`, Pregunta 7 punto 15) es
  una segunda barrera a nivel de base de datos por si algo escribe directo a la tabla.
