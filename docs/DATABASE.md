# Database

PostgreSQL 14+ with Entity Framework Core 8. Schema: [`Database/server.sql`](../Database/server.sql)

---

## <img height="20" src="../Images/icons/architecture.svg">&nbsp;&nbsp;Entity-Relation Diagram

![ER Diagram](../Images/diagram.png)

---

## <img height="20" src="../Images/icons/features.svg">&nbsp;&nbsp;Tables

| Table                     | Purpose                              | Soft Delete | Notable columns                    |
| ------------------------- | ------------------------------------ | ----------- | ---------------------------------- |
| `usuarios`                | Users and authentication             | Yes         | `carnet` PK, `email` (unique), `telefono` (unique) |
| `prestamos`               | Loan lifecycle                       | Yes         | `estado_prestamo` enum, FK to `usuarios` |
| `detalles_prestamos`      | Line items per loan                  | Yes         | FK to `prestamos`, FK to `equipos` |
| `categorias`              | Equipment categories                 | Yes         | —                                  |
| `carreras`                | Academic programs                    | Yes         | —                                  |
| `empresas_mantenimiento`  | Third-party maintenance providers    | Yes         | `nit` (unique when provided)       |
| `mantenimientos`          | Maintenance records                  | Yes         | FK to `empresas_mantenimiento`     |
| `detalles_mantenimientos` | Line items per maintenance           | Yes         | `tipo_mantenimiento` enum          |
| `grupos_equipos`          | Logical equipment groups             | Yes         | `cantidad`, `costo_promedio` (derived) |
| `equipos`                 | Individual equipment units           | Yes         | `codigo_imt` (unique, sequential), `estado_equipo` enum |
| `gaveteros`               | Storage compartments                 | Yes         | FK to `muebles`                    |
| `muebles`                 | Furniture containing compartments    | Yes         | `numero_gaveteros` (derived)       |
| `accesorios`              | Complementary accessories            | Yes         | —                                  |
| `componentes`             | Internal components / spares         | Yes         | —                                  |
| `contratos`               | Generated HTML contracts             | No          | `contrato TEXT`, FK from `prestamos` |

---

## <img height="20" src="../Images/icons/stack.svg">&nbsp;&nbsp;Enums

| SQL Enum             | Values                                                                         | Used in                   |
| -------------------- | ------------------------------------------------------------------------------ | ------------------------- |
| `estado_prestamo`    | `pendiente` · `aprobado` · `activo` · `finalizado` · `rechazado` · `cancelado` | `prestamos`               |
| `estado_equipo`      | `operativo` · `parcialmente_operativo` · `inoperativo`                         | `equipos`                 |
| `tipo_usuario`       | `docente` · `administrador` · `estudiante`                                     | `usuarios`                |
| `tipo_mantenimiento` | `correctivo` · `preventivo`                                                    | `detalles_mantenimientos` |

C# mapping: `[PgName]` attribute + `NpgsqlDataSourceBuilder.MapEnum<T>()` in `Program.cs`.
Mapperly uses `EnumMappingStrategy.ByName` with a `StringToEstadoEquipo` helper for SQL-named enums with underscores.

---

## <img height="20" src="../Images/icons/documentation.svg">&nbsp;&nbsp;Business Logic

### Derived counters

Maintained by two mechanisms. DB triggers fire first; the backend recalculates as a consistency backstop.

| Level      | Event                              | Mechanism                                                                                                           | Field maintained                                           |
| ---------- | ---------------------------------- | ------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| DB trigger | Equipo INSERT / UPDATE             | `fn_incrementar_cantidad_equipos`, `fn_actualizar_cantidad_equipo_por_estado`, `fn_actualizar_costo_promedio_grupo` | `grupos_equipos.cantidad`, `grupos_equipos.costo_promedio` |
| DB trigger | Gavetero INSERT / UPDATE           | `fn_incrementar_numero_gaveteros`, `fn_actualizar_conteo_gaveteros_por_estado`                                      | `muebles.numero_gaveteros`                                 |
| DB trigger | Prestamo UPDATE (soft-delete)      | `fn_estado_eliminado_prestamo_a_detalle`                                                                            | `detalles_prestamos.estado_eliminado`                      |
| DB trigger | Mantenimiento UPDATE (soft-delete) | `fn_estado_eliminado_mantenimiento_a_detalle`                                                                       | `detalles_mantenimientos.estado_eliminado`                 |
| Backend    | Equipo Create / Update / Delete    | `EquipoRepository.RecalcGrupoStats()`                                                                               | `grupos_equipos.cantidad`, `costo_promedio`                |
| Backend    | Gavetero Create / Delete           | `GaveteroRepository.RecalcMuebleCount()`                                                                            | `muebles.numero_gaveteros`                                 |

### Availability logic

An equipment unit is considered **unavailable** for a date range when an existing loan for that unit has `estado_prestamo IN ('aprobado', 'activo')` and the date ranges overlap.

`pendiente` loans do **not** block availability — they are not confirmed reservations.

Two checks run per `Prestamo`:

1. **Pre-create** (`HasAvailableEquipo`): verified before saving the loan. Returns error if no units in the requested group are available.
2. **Pre-approve** (`HasEquipoConflictoAlAprobar`): re-verified when an admin transitions `pendiente → aprobado`. Prevents race conditions between concurrent approvals.

### IMT code assignment

`equipos.codigo_imt` is assigned sequentially (max existing + 1) on Create. It cannot be changed via Update — the field is read-only after creation.

---

## <img height="20" src="../Images/icons/prerequisites.svg">&nbsp;&nbsp;Indexes

| Index                                           | Columns                                                               | Justification                  |
| ----------------------------------------------- | --------------------------------------------------------------------- | ------------------------------ |
| `idx_usuarios_email_estado`                     | `email, estado_eliminado`                                             | Login lookup                   |
| `idx_usuarios_nombre_estado`                    | `nombre, estado_eliminado`                                            | User listing                   |
| `idx_prestamos_temporal_usuario_estado`         | `fecha_prestamo, fecha_devolucion_esperada, carnet, estado_eliminado` | Date-range and history filters |
| `idx_mantenimientos_temporal_empresa_estado`    | `fecha_inicio, fecha_final, id_empresa, estado_eliminado`             | Maintenance history            |
| `idx_grupos_equipos_búsqueda`                   | `categoria, nombre, modelo, marca, estado_eliminado`                  | Equipment group search         |
| `idx_gaveteros_ubicación`                       | `nombre, id_mueble, estado_eliminado`                                 | Storage location queries       |
| `idx_equipos_agrupación`                        | `id_grupo_equipo, codigo_imt, estado_eliminado`                       | Joins with groups              |
| `idx_empresas_mantenimiento_activas`            | `nombre, estado`                                                      | Provider selection             |
| `idx_detalles_prestamos_por_prestamo`           | `id_prestamo, estado_eliminado`                                       | Loan line items                |
| `idx_detalles_mantenimientos_por_mantenimiento` | `id_mantenimiento, estado_eliminado`                                  | Maintenance line items         |
| `idx_categorias_nombre`                         | `nombre, estado_eliminado`                                            | Dropdown population            |
| `idx_carreras_nombre`                           | `nombre, estado_eliminado`                                            | Dropdown population            |
| `idx_accesorios_nombre`                         | `nombre, estado_eliminado`                                            | Accessory listing              |

---

## <img height="20" src="../Images/icons/stack.svg">&nbsp;&nbsp;SQL Views

| View                                 | Purpose                                                               |
| ------------------------------------ | --------------------------------------------------------------------- |
| `vw_equipos_necesitan_mantenimiento` | Equipment requiring preventive maintenance in the last N months       |
| `vw_ubicaciones_grupos_equipos`      | Physical location of equipment (mueble → gavetero → equipo hierarchy) |

---

## <img height="20" src="../Images/icons/setup.svg">&nbsp;&nbsp;Restore

```bash
psql -U postgres -c "CREATE DATABASE IMT_Reservas;"
psql -U postgres -d IMT_Reservas -f Database/server.sql
psql -U postgres -d IMT_Reservas -c "\dt"
```

Docker alternative:

```bash
cd Code && docker compose up -d ucb_db
```

The `ucb_db` container runs `DataBase/server.sql` automatically on first startup.
