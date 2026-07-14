# Prueba técnica del módulo 2 - vicente urriola

Este repositorio junta las cuatro respuestas de la parte práctica de la prueba (las que
piden entregar código, no solo texto). Cada respuesta vive en su propia carpeta, separada
de las demás, para que sea fácil ver dónde está cada una.

## ¿Qué hay en cada carpeta?

| Pregunta | Carpeta | En simple |
|---|---|---|
| 4 | [`pregunta-4-api-registros/`](pregunta-4-api-registros) | Un programa que recibe una solicitud, revisa que quien pregunta tenga permiso, y responde si el registro existe o no. |
| 5 | [`pregunta-5-migracion-sql/`](pregunta-5-migracion-sql) | Una consulta a una base de datos vieja que se corrigió y se adaptó al sistema nuevo. |
| 6 | [`pregunta-6-formulario-entidad/`](pregunta-6-formulario-entidad) | Un formulario web para registrar una nueva organización, con avisos claros si algo falta o está mal escrito. |
| 7 | [`pregunta-7-esquema-basedatos/`](pregunta-7-esquema-basedatos) | El diseño de las tablas donde se guarda toda esa información. |

## ¿Cómo se conecta todo?

Estas cuatro piezas no son independientes entre sí: son parte de un mismo sistema.

1. Primero se crean las tablas de la carpeta de la **pregunta 7** (ahí se guarda todo).
2. El programa de la **pregunta 4** lee y escribe en esas mismas tablas.
3. La consulta de la **pregunta 5** también usa esas tablas, para armar un reporte.
4. El formulario de la **pregunta 6** es la parte visual: lo que vería una persona usando
   el sistema para registrar una organización nueva.

Cada carpeta tiene su propio archivo de instrucciones (README) explicando, paso a paso y
en lenguaje simple, cómo probar esa parte.
