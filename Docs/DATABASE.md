<div align="center">

# Database

PostgreSQL 14+ con Entity Framework Core 8. El esquema inicial vive en [`Code/DataBase/server.sql`](../Code/DataBase/server.sql).

[Volver al README](../README.md) · [Setup](SETUP.md) · [API](API.md)

</div>

---

## <img height="22" src="../Images/readme-icons/database.svg" alt="" /> Diagrama ER

![Diagrama entidad-relación](../Images/diagram.png)

---

## <img height="22" src="../Images/readme-icons/tables.svg" alt="" /> Tablas

| Tabla                     | Propósito                        | Soft delete | Columnas relevantes                |
| ------------------------- | -------------------------------- | ----------- | ---------------------------------- |
| `usuarios`                | Usuarios y autenticación         | Sí          | `carnet`, `email`, `telefono`      |
| `prestamos`               | Ciclo de vida del préstamo       | Sí          | `estado_prestamo`, FK a `usuarios` |
| `detalles_prestamos`      | Equipos por préstamo             | Sí          | FK a `prestamos`, FK a `equipos`   |
| `categorias`              | Categorías de equipos            | Sí          | `nombre`                           |
| `carreras`                | Programas académicos             | Sí          | `nombre`                           |
| `empresas_mantenimiento`  | Proveedores de mantenimiento     | Sí          | `nit`                              |
| `mantenimientos`          | Registros de mantenimiento       | Sí          | FK a `empresas_mantenimiento`      |
| `detalles_mantenimientos` | Equipos por mantenimiento        | Sí          | `tipo_mantenimiento`               |
| `grupos_equipos`          | Agrupación lógica de equipos     | Sí          | `cantidad`, `costo_promedio`       |
| `equipos`                 | Unidades físicas                 | Sí          | `codigo_imt`, `estado_equipo`      |
| `gaveteros`               | Compartimentos de almacenamiento | Sí          | FK a `muebles`                     |
| `muebles`                 | Muebles contenedores             | Sí          | `numero_gaveteros`                 |
| `accesorios`              | Accesorios complementarios       | Sí          | `codigo_imt`                       |
| `componentes`             | Componentes internos             | Sí          | `codigo_imt`                       |
| `contratos`               | Contratos HTML generados         | No          | `contrato`, FK a `prestamos`       |

---

## <img height="22" src="../Images/readme-icons/enums.svg" alt="" /> Enums

| Enum SQL             | Valores                                                                   | Uso                       |
| -------------------- | ------------------------------------------------------------------------- | ------------------------- |
| `estado_prestamo`    | `pendiente`, `aprobado`, `activo`, `finalizado`, `rechazado`, `cancelado` | `prestamos`               |
| `estado_equipo`      | `operativo`, `parcialmente_operativo`, `inoperativo`                      | `equipos`                 |
| `tipo_usuario`       | `docente`, `administrador`, `estudiante`                                  | `usuarios`                |
| `tipo_mantenimiento` | `correctivo`, `preventivo`                                                | `detalles_mantenimientos` |

El backend mapea enums con `PgName` y `NpgsqlDataSourceBuilder.MapEnum<T>()`.

---

## <img height="22" src="../Images/readme-icons/business-logic.svg" alt="" /> Reglas de Negocio

### Contadores derivados

| Nivel   | Evento                       | Mecanismo                          | Campo mantenido                             |
| ------- | ---------------------------- | ---------------------------------- | ------------------------------------------- |
| DB      | `Equipo INSERT/UPDATE`       | Triggers de grupo y costo promedio | `grupos_equipos.cantidad`, `costo_promedio` |
| DB      | `Gavetero INSERT/UPDATE`     | Triggers de conteo                 | `muebles.numero_gaveteros`                  |
| DB      | Soft-delete de préstamo      | Cascada lógica a detalles          | `detalles_prestamos.estado_eliminado`       |
| DB      | Soft-delete de mantenimiento | Cascada lógica a detalles          | `detalles_mantenimientos.estado_eliminado`  |
| Backend | Crear/editar/eliminar equipo | Recalculo de repositorio           | `cantidad`, `costo_promedio`                |
| Backend | Crear/eliminar gavetero      | Recalculo de repositorio           | `numero_gaveteros`                          |

### Disponibilidad

Una unidad no está disponible cuando existe un préstamo del mismo equipo en estado `aprobado` o `activo` con fechas superpuestas.

Los préstamos `pendiente` no bloquean capacidad porque todavía no son reservas confirmadas.

Validaciones relevantes:

| Momento          | Validación                                                               |
| ---------------- | ------------------------------------------------------------------------ |
| Antes de crear   | Verifica que exista al menos una unidad disponible del grupo solicitado. |
| Antes de aprobar | Revalida conflictos para evitar aprobaciones concurrentes incompatibles. |

### Código IMT

`equipos.codigo_imt` se asigna de forma secuencial al crear una unidad. No debe cambiarse en actualizaciones posteriores.

---

## <img height="22" src="../Images/readme-icons/indexes.svg" alt="" /> Índices

| Índice                                          | Uso                                             |
| ----------------------------------------------- | ----------------------------------------------- |
| `idx_usuarios_email_estado`                     | Login y búsqueda por email.                     |
| `idx_usuarios_nombre_estado`                    | Listado de usuarios.                            |
| `idx_prestamos_temporal_usuario_estado`         | Historial y filtros por rango de fechas.        |
| `idx_mantenimientos_temporal_empresa_estado`    | Historial de mantenimiento.                     |
| `idx_grupos_equipos_busqueda`                   | Búsqueda por categoría, nombre, modelo y marca. |
| `idx_equipos_agrupacion`                        | Joins entre equipos y grupos.                   |
| `idx_detalles_prestamos_por_prestamo`           | Detalle por préstamo.                           |
| `idx_detalles_mantenimientos_por_mantenimiento` | Detalle por mantenimiento.                      |

---

## <img height="22" src="../Images/readme-icons/views.svg" alt="" /> Vistas SQL

| Vista                                | Propósito                                          |
| ------------------------------------ | -------------------------------------------------- |
| `vw_equipos_necesitan_mantenimiento` | Equipos que requieren mantenimiento preventivo.    |
| `vw_ubicaciones_grupos_equipos`      | Ubicación física de equipos por mueble y gavetero. |

---

## <img height="22" src="../Images/readme-icons/restoration.svg" alt="" /> Restauración

```bash
psql -U postgres -c "CREATE DATABASE IMT_Reservas;"
psql -U postgres -d IMT_Reservas -f Code/DataBase/server.sql
psql -U postgres -d IMT_Reservas -c "\dt"
```

Con Docker:

```bash
cd Code
docker compose up -d ucb_db
```

El contenedor `ucb_db` ejecuta `Code/DataBase/server.sql` en el primer arranque.
