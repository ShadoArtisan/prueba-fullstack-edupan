# Pregunta 4 — API REST .NET 8: endpoint de consulta con control de acceso (25 pts)

Implementa `POST /api/registros/consulta`: valida la API Key y el convenio de la entidad,
exige identificador + nombre, revisa la cuota diaria, busca el registro y deja todo
registrado en el log de accesos.

> Necesita las tablas de [`../pregunta-7-esquema-basedatos`](../pregunta-7-esquema-basedatos)
> para correr contra una base real.

## Estructura

```
src/RegistrosInstitucionales.Api/
  Controllers/RegistrosController.cs   -> el endpoint en sí
  Services/RegistroConsultaService.cs  -> la lógica de negocio (cuota, búsqueda, log)
  Repositories/                        -> acceso a datos con EF Core
  Auth/ApiKeyAuthFilter.cs             -> valida X-API-Key antes de llegar al controller
  Data/AppDbContext.cs                 -> mapeo EF Core -> tablas de la Pregunta 7
tests/RegistrosInstitucionales.Api.Tests/
  CuotaDiariaTests.cs                  -> test unitario de la cuota diaria (pedido en el enunciado)
  RegistroConsultaServiceTests.cs      -> el servicio completo con repositorios mockeados (Moq)
```

## Cómo ejecutarlo

Requisitos: .NET 8 SDK. SQL Server solo hace falta para probar contra datos reales; los
tests no lo necesitan.

### 1. Base de datos (opcional, solo para probar el endpoint de verdad)

Ejecuta primero el esquema y los datos de prueba de la Pregunta 7:

```bash
sqlcmd -S localhost -i ../pregunta-7-esquema-basedatos/schema.sql
sqlcmd -S localhost -i ../pregunta-7-esquema-basedatos/datos-prueba.sql
```

Ajusta `ConnectionStrings:SqlServer` en `src/RegistrosInstitucionales.Api/appsettings.json`
si tu servidor no es `localhost` con autenticación de Windows.

### 2. Levantar la API

```bash
cd src/RegistrosInstitucionales.Api
dotnet run
```

En modo desarrollo, Swagger queda disponible en `/swagger`.

### 3. Probar el endpoint

Con los datos de `datos-prueba.sql` cargados, la API Key de prueba (texto plano) es
`clave-demo-123`:

```bash
curl -k -X POST https://localhost:7000/api/registros/consulta \
  -H "Content-Type: application/json" \
  -H "X-API-Key: clave-demo-123" \
  -d "{\"identificador\": \"8-123-456\", \"nombre\": \"Juan Pérez\"}"
```

| Caso | Cómo provocarlo | Respuesta esperada |
|---|---|---|
| Éxito | Datos de arriba, tal cual | `200 OK` con el registro |
| Falta la API Key | Quitar el header `X-API-Key` | `401 Unauthorized` |
| API Key inválida | Usar cualquier otro texto en `X-API-Key` | `401 Unauthorized` |
| Convenio vencido/inactivo | `FechaVencimiento` en el pasado o `Activo = 0` | `403 Forbidden` |
| Falta el nombre o el identificador | Omitir `nombre` en el body | `400 Bad Request` |
| Registro no encontrado | Identificador inexistente | `404 Not Found` |
| Cuota diaria superada | Bajar `CuotaDiaria` de la entidad a un número ya alcanzado | `429 Too Many Requests` |

Cada intento (aprobado o rechazado) queda en `dbo.LogAccesos`.

### 4. Correr los tests

```bash
cd ..    # a la raíz de esta carpeta (pregunta-4-api-registros)
dotnet test
```

## Cómo se cubrió cada criterio de evaluación

- **Separación de capas**: `Controller` → `Service` → `Repository`, cada uno con su interfaz.
- **Errores HTTP**: 401 (sin API Key o inválida), 403 (convenio inactivo/vencido), 400
  (validación de campos vía `[ApiController]` + DataAnnotations), 404 (no encontrado), 429
  (cuota superada), 500 (excepción no controlada, capturada por un middleware global en
  `Program.cs` para no filtrar detalles internos al cliente).
- **Validaciones de entrada**: DataAnnotations en `Dtos/ConsultaRegistroRequest.cs`
  (`[Required]` en ambos campos, así nunca se permite buscar solo por identificador).
- **Acceso a datos**: Entity Framework Core sobre SQL Server.
- **Inyección de dependencias**: todo registrado en `Program.cs`.
- **Test unitario de cuota diaria**: `CuotaDiariaTests.cs` prueba la función pura
  `RegistroConsultaService.SuperoCuotaDiaria(...)` con varios casos límite.

## Decisiones de diseño

- **Autenticación por API Key** como un `IAsyncActionFilter`
  (`Auth/ApiKeyAuthFilter.cs`) que corre antes del controller, resuelve la entidad y la deja
  en `HttpContext.Items`. Así el controller no repite la búsqueda ni mezcla autenticación con
  lógica de negocio. Solo se guarda el hash SHA-256 de la API Key, nunca el texto plano
  (`Auth/ApiKeyHasher.cs`).
- **El servicio no conoce HTTP**: devuelve un resultado de negocio neutro
  (`Services/ResultadoConsultaRegistro.cs`) y es el controller quien traduce eso a un código
  HTTP. Facilita testear el servicio sin necesitar `HttpContext`.
- **Cuota diaria**: la validación "real" vive acá (para poder responder 429 con contexto). El
  trigger SQL de la Pregunta 7 (punto 15) es una segunda barrera a nivel de base de datos por
  si algo escribe directo a la tabla sin pasar por esta API.
