# Database

PostgreSQL 14+ with Entity Framework Core 8. Schema DDL: [`DataBase/schema.ddl`](../DataBase/schema.ddl)

---

## ✦ Entity-Relation Diagram

![ER Diagram](../Images/diagram.png)

---

## ✦ Tables

| Table                     | Purpose                           | Soft Delete |
| ------------------------- | --------------------------------- | ----------- |
| `usuarios`                | Users and authentication          | Yes         |
| `prestamos`               | Loan lifecycle                    | Yes         |
| `detalles_prestamos`      | Line items per loan               | Yes         |
| `categorias`              | Equipment categories              | Yes         |
| `carreras`                | Academic programs                 | Yes         |
| `empresas_mantenimiento`  | Third-party maintenance providers | Yes         |
| `mantenimientos`          | Maintenance records               | Yes         |
| `detalles_mantenimientos` | Line items per maintenance        | Yes         |
| `grupos_equipos`          | Logical equipment groups          | Yes         |
| `equipos`                 | Individual equipment units        | Yes         |
| `gaveteros`               | Storage compartments              | Yes         |
| `muebles`                 | Furniture containing compartments | Yes         |
| `accesorios`              | Complementary accessories         | Yes         |
| `componentes`             | Internal components / spares      | Yes         |
| `contratos`               | Generated HTML contracts          | No          |

`contratos` contains only `id INTEGER PRIMARY KEY` and `contrato TEXT`. Referenced via FK from `prestamos.id_contrato`.

---

## ✦ Enums

| SQL Enum             | Values                                                                         | Used in                   |
| -------------------- | ------------------------------------------------------------------------------ | ------------------------- |
| `estado_prestamo`    | `pendiente` · `aprobado` · `activo` · `finalizado` · `rechazado` · `cancelado` | `prestamos`               |
| `estado_equipo`      | `operativo` · `parcialmente_operativo` · `inoperativo`                         | `equipos`                 |
| `tipo_usuario`       | `docente` · `administrador` · `estudiante`                                     | `usuarios`                |
| `tipo_mantenimiento` | `correctivo` · `preventivo`                                                    | `detalles_mantenimientos` |

C# mapping: `[PgName]` attribute + `NpgsqlDataSourceBuilder.MapEnum<T>()` in `Program.cs`.

---

## ✦ Business Logic

Derived counters are maintained by two mechanisms. DB triggers fire first; the backend recalculates as a consistency backstop.

| Level      | Event                              | Mechanism                                                                                                           | Field maintained                                           |
| ---------- | ---------------------------------- | ------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| DB trigger | Equipo INSERT / UPDATE             | `fn_incrementar_cantidad_equipos`, `fn_actualizar_cantidad_equipo_por_estado`, `fn_actualizar_costo_promedio_grupo` | `grupos_equipos.cantidad`, `grupos_equipos.costo_promedio` |
| DB trigger | Gavetero INSERT / UPDATE           | `fn_incrementar_numero_gaveteros`, `fn_actualizar_conteo_gaveteros_por_estado`                                      | `muebles.numero_gaveteros`                                 |
| DB trigger | Prestamo UPDATE (soft-delete)      | `fn_estado_eliminado_prestamo_a_detalle`                                                                            | `detalles_prestamos.estado_eliminado`                      |
| DB trigger | Mantenimiento UPDATE (soft-delete) | `fn_estado_eliminado_mantenimiento_a_detalle`                                                                       | `detalles_mantenimientos.estado_eliminado`                 |
| Backend    | Equipo Create / Update / Delete    | `EquipoRepository.RecalcGrupoStats()`                                                                               | `grupos_equipos.cantidad`, `costo_promedio`                |
| Backend    | Gavetero Create / Delete           | `GaveteroRepository.RecalcMuebleCount()`                                                                            | `muebles.numero_gaveteros`                                 |

---

## ✦ Indexes

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

## ✦ SQL Views

| View                                 | Purpose                                                               |
| ------------------------------------ | --------------------------------------------------------------------- |
| `vw_equipos_necesitan_mantenimiento` | Equipment requiring preventive maintenance in the last N months       |
| `vw_ubicaciones_grupos_equipos`      | Physical location of equipment (mueble → gavetero → equipo hierarchy) |

---

## ✦ Restore

```bash
# Create database
psql -U postgres -c "CREATE DATABASE IMT_Reservas;"

# Load schema
psql -U postgres -d IMT_Reservas -f DataBase/schema.ddl

# Verify (should list 15 tables)
psql -U postgres -d IMT_Reservas -c "\dt"
```

Docker alternative:

```bash
docker run -d --name ucbhold-postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=IMT_Reservas \
  -p 5432:5432 \
  -v ucbhold-pgdata:/var/lib/postgresql/data \
  postgres:14

docker exec -i ucbhold-postgres psql -U postgres -d IMT_Reservas < DataBase/schema.ddl
```
