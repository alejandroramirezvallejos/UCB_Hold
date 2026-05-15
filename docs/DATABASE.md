# Base de Datos

PostgreSQL 14+ con Entity Framework Core 8. Schema DDL completo en `DataBase/database.ddl`.

---

## Modelo Entidad–Relación

![ER Diagram](../Images/bd.png)

---

## Tablas

`usuarios`, `prestamos`, `detalles_prestamos`, `categorias`, `carreras`, `empresas_mantenimiento`, `mantenimientos`, `detalles_mantenimientos`, `grupos_equipos`, `equipos`, `gaveteros`, `muebles`, `accesorios`, `componentes`, `contratos`.

Todas incluyen `estado_eliminado BOOLEAN DEFAULT FALSE` para borrado lógico. **Excepción**: la tabla `contratos` no tiene `estado_eliminado` — solo `id INTEGER PRIMARY KEY` y `contrato TEXT`. En C# el entity `Contrato` tiene `ContratoHtml` mapeado vía `HasColumnName("contrato")`. La FK desde `prestamos.id_contrato` (INTEGER) referencia `contratos.id`.

---

## Enums

| Enum SQL | Valores | Uso |
|----------|---------|-----|
| `estado_prestamo` | pendiente, rechazado, aprobado, activo, finalizado, cancelado | Tabla `prestamos` |
| `estado_equipo` | operativo, parcialmente_operativo, inoperativo | Tabla `equipos` |
| `tipo_usuario` | docente, administrador, estudiante | Tabla `usuarios` |
| `tipo_mantenimiento` | correctivo, preventivo | Tabla `detalles_mantenimientos` |
| `estado_disponibilidad` | disponible, mantenimiento, ocupado | **Reservado** — definido en DDL pero sin uso actual en ninguna tabla |

Mapeados en C# vía `[PgName]` y registrados en `Program.cs`.

---

## Triggers y lógica de negocio

El DDL define varias funciones (`fn_actualizar_cantidad_equipo_por_estado`, `fn_actualizar_costo_promedio_grupo`, `fn_actualizar_conteo_gaveteros_por_estado`, `fn_estado_eliminado_prestamo_a_detalle`, `fn_estado_eliminado_mantenimiento_a_detalle`, etc.) pero **NO están attached como triggers** — no se ejecutan automáticamente. La lógica equivalente vive en los Services del backend:

| Acción | Service responsable | Método |
|--------|---------------------|--------|
| Recalcular `grupos_equipos.cantidad` y `costo_promedio` tras Create/Update/Delete de `equipos` | `EquipoService` | `RecalcGrupoStats` |
| Recalcular `muebles.numero_gaveteros` tras Create/Update/Delete de `gaveteros` | `GaveteroService` | `RecalcMuebleCount` |
| Cascade soft-delete `prestamos.estado_eliminado=true` → `detalles_prestamos.estado_eliminado=true` | `PrestamoService.Delete` | inline |
| Cascade soft-delete `mantenimientos.estado_eliminado=true` → `detalles_mantenimientos.estado_eliminado=true` | `MantenimientoService.Delete` | inline |

Backend es la única fuente de verdad para esta sincronización.

---

## Vistas

- **`vw_equipos_necesitan_mantenimiento`** — equipos sin mantenimiento en los últimos N meses
- **`vw_ubicaciones_grupos_equipos`** — agrupación de equipos por mueble/gavetero

---

## Índices y justificación

**Usuarios**
Índice sobre `email` y `estado_eliminado` acelera login y validaciones. Índice sobre `nombre`+`estado_eliminado` garantiza listados rápidos sin cargar soft-deleted.

**Prestamos**
Índice compuesto sobre `fecha_prestamo`, `fecha_devolucion_esperada`, `carnet`, `estado_eliminado` optimiza rangos temporales y filtrado por usuario.

**Mantenimientos**
Índice combinado `fecha_inicio`, `fecha_final`, `id_empresa`, `estado_eliminado` segmenta históricos por compañía y período.

**Grupos_equipos**
Índice sobre `categoria`, `nombre`, `modelo`, `marca`, `estado_eliminado` mejora búsquedas combinadas tipo "todas las impresoras HP activas".

**Gaveteros**
Índice `nombre`, `id_mueble`, `estado_eliminado` agiliza asignación y consultas de espacio.

**Equipos**
Índice `id_grupo_equipo`, `codigo_imt`, `estado_eliminado` fundamental para joins y filtros de equipos activos.

**Empresas_mantenimiento**
Índice `nombre`, `estado` acelera selección de proveedores activos.

**Detalles_prestamos**
Índice por `id_prestamo`, `estado_eliminado` optimiza obtención de items de un préstamo.

**Detalles_mantenimientos**
Índice por `id_mantenimiento`, `estado_eliminado` para histórico de intervenciones.

**Componentes**
Índice `nombre`, `id_equipo`, `estado_eliminado` facilita verificación de stock y compatibilidad.

**Categorias / Carreras / Accesorios**
Índices sobre `nombre` + `estado_eliminado` para validación de unicidad y poblado de dropdowns.

---

## Análisis de plan de ejecución

### Consulta pesada sin índices
![Sin índices](https://github.com/user-attachments/assets/90820cbc-6f9d-4186-8b0d-4777f01e61f9)

### Misma consulta con índices
![Con índices](https://github.com/user-attachments/assets/102805fd-879f-4817-845c-df516831b876)

---

## Transacciones y aislamiento

- **Nivel:** `SERIALIZABLE`
- **Justificación:** garantiza ausencia de lecturas no repetibles y lecturas fantasmas

Operaciones críticas (creación de préstamo + contrato + detalles) se ejecutan en una sola transacción vía `SaveChangesAsync` agrupados.

---

## Restauración inicial

```bash
psql -U postgres -c "CREATE DATABASE IMT_Reservas;"
psql -U postgres -d IMT_Reservas -f DataBase/database.ddl
```
