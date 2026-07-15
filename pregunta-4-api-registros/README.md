# Pregunta 4 — API de Consulta de Registros

Este proyecto es una API en .NET 8 que permite a organizaciones externas (como bancos o entidades gubernamentales) consultar si existe un registro específico usando un identificador y un nombre. 

Para proteger la información, el endpoint valida varios puntos antes de devolver cualquier dato:
1. **Autenticación:** Exige un API Key válido en los headers.
2. **Estado del convenio:** Verifica que la organización tenga los permisos activos.
3. **Integridad del request:** Asegura que vengan tanto el identificador como el nombre en la petición.
4. **Rate limiting (Cuota diaria):** Controla que la entidad no exceda su límite de consultas por día.

Cualquier intento de consulta, exitoso o fallido, queda registrado en la base de datos para temas de auditoría.

## Arquitectura

El código está estructurado en capas para separar responsabilidades: el controlador maneja la petición http, los servicios aplican las reglas de negocio (rate limiting, validaciones) y la capa de acceso a datos interactúa con SQL Server. Esto hace que el código sea mucho más fácil de testear de forma aislada.

> **Nota:** Para que esta API funcione con datos reales, necesitas ejecutar primero los scripts de la [`pregunta-7-esquema-basedatos`](../pregunta-7-esquema-basedatos).

## Levantando el entorno

Necesitas el **SDK de .NET 8** y una instancia de **SQL Server**.

1. **Preparar la base de datos:**  
   Ejecuta los scripts de la pregunta 7 para montar el esquema y datos mock:
   ```bash
   sqlcmd -S localhost -i ../pregunta-7-esquema-basedatos/schema.sql
   sqlcmd -S localhost -i ../pregunta-7-esquema-basedatos/datos-prueba.sql
   ```

2. **Ejecutar el API:**  
   En la terminal, levanta el proyecto:
   ```bash
   cd src/RegistrosInstitucionales.Api
   dotnet run
   ```
   Esto levantará el servidor y habilitará Swagger para que puedas hacer pruebas directamente desde el navegador.

3. **Hacer una petición manual:**  
   Puedes usar `curl` con la clave de prueba (`clave-demo-123`) para ver cómo responde:
   ```bash
   curl -k -X POST https://localhost:7000/api/registros/consulta \
     -H "Content-Type: application/json" \
     -H "X-API-Key: clave-demo-123" \
     -d "{\"identificador\": \"8-123-456\", \"nombre\": \"Juan Pérez\"}"
   ```

4. **Tests automatizados:**  
   Para correr la suite de pruebas unitarias:
   ```bash
   cd ..
   dotnet test
   ```

## Detalles técnicos adicionales
- **Seguridad de las credenciales:** Las API Keys se guardan hasheadas/encriptadas en la base de datos.
- **Doble validación de la cuota:** El límite de consultas diarias se valida en la capa de servicios (para dar feedback claro al usuario) y tiene un respaldo a nivel de base de datos mediante constraints, por si algún proceso externo intenta saltarse la API.