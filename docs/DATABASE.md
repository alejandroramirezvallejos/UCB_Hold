# 💛 Base de Datos

> PostgreSQL 14+ con Entity Framework Core 8 | Schema DDL completo en [`DataBase/database.ddl`](../DataBase/database.ddl)

```
📊 Engine:      PostgreSQL 14+
🔗 ORM:         Entity Framework Core 8
🗑️  Soft Delete: Habilitado en todas las tablas (excepto contratos)
🔄 Transacciones: Nivel SERIALIZABLE
🚀 Performance: Índices optimizados para queries frecuentes
```

---

## 📋 Tabla de Contenidos

- [Modelo Entidad-Relación](#modelo-entidad-relación)
- [Tablas Principales](#tablas-principales)
- [Tipos de Datos (Enums)](#tipos-de-datos-enums)
- [Lógica de Negocio](#lógica-de-negocio)
- [Vistas SQL](#vistas-sql)
- [Estrategia de Índices](#estrategia-de-índices)
- [Transacciones](#transacciones-y-aislamiento)
- [Restauración](#restauración-inicial)

---

## 📐 Modelo Entidad-Relación

![ER Diagram](../Images/bd.png)

---

## 📦 Tablas Principales

| Tabla                     | Propósito                              | Soft Delete | Registros |
| ------------------------- | -------------------------------------- | ----------- | --------- |
| `usuarios`                | Gestión de usuarios y autenticación    | ✅ Sí       | ~100-500  |
| `prestamos`               | Ciclo completo de préstamos            | ✅ Sí       | ~1000s    |
| `detalles_prestamos`      | Ítems asociados a cada préstamo        | ✅ Sí       | ~5000s    |
| `categorias`              | Categorías de equipos                  | ✅ Sí       | ~20-50    |
| `carreras`                | Carreras profesionales                 | ✅ Sí       | ~20-50    |
| `empresas_mantenimiento`  | Proveedores terceros de mantenimiento  | ✅ Sí       | ~10-20    |
| `mantenimientos`          | Registros de intervenciones            | ✅ Sí       | ~100s     |
| `detalles_mantenimientos` | Ítems por mantenimiento                | ✅ Sí       | ~500s     |
| `grupos_equipos`          | Agrupaciones lógicas de equipos        | ✅ Sí       | ~100s     |
| `equipos`                 | Equipos individuales                   | ✅ Sí       | ~1000s    |
| `gaveteros`               | Compartimentos de almacenamiento       | ✅ Sí       | ~100s     |
| `muebles`                 | Muebles/estantes que contienen gavetas | ✅ Sí       | ~50-100   |
| `accesorios`              | Accesorios complementarios             | ✅ Sí       | ~100-200  |
| `componentes`             | Componentes internos/repuestos         | ✅ Sí       | ~200-500  |
| `contratos`               | Contratos HTML generados               | ❌ No\*     | ~1000s    |

**\* Nota:** La tabla `contratos` es especial — contiene solo `id INTEGER PRIMARY KEY` y `contrato TEXT` (mapeado en C# como `ContratoHtml`). Referenciada vía FK desde `prestamos.id_contrato`.

---

## 🏷️ Tipos de Datos (Enums)

| Enum SQL                | Valores                                                                                  | Uso                       | Transiciones                            |
| ----------------------- | ---------------------------------------------------------------------------------------- | ------------------------- | --------------------------------------- |
| `estado_prestamo`       | `pendiente` → `aprobado` → `activo` → `finalizado`<br>`rechazado`, `cancelado` (finales) | Tabla `prestamos`         | Ver validación en `EstadoPrestamoState` |
| `estado_equipo`         | `operativo` \| `parcialmente_operativo` \| `inoperativo`                                 | Tabla `equipos`           | Libre                                   |
| `tipo_usuario`          | `docente` \| `administrador` \| `estudiante`                                             | Tabla `usuarios`          | Libre                                   |
| `tipo_mantenimiento`    | `correctivo` \| `preventivo`                                                             | `detalles_mantenimientos` | Libre                                   |
| `estado_disponibilidad` | `disponible` \| `mantenimiento` \| `ocupado`                                             | _(Reservado)_             | No usado actualmente                    |

**Mapeo en C#:** Utilizamos `[PgName]` attribute y registración vía Fluent API en `Program.cs`

---

## ⚙️ Lógica de Negocio

El DDL incluye funciones SQL pero **NO están attached como triggers** — se ejecutan desde Services del backend. Esto garantiza que la lógica esté centralizada en código y sea fácil de debuggear.

### Synchronización de Datos

| Operación                                 | Servicio Responsable   | Método                | Disparo                        | Descripción                                                 |
| ----------------------------------------- | ---------------------- | --------------------- | ------------------------------ | ----------------------------------------------------------- |
| **Equipo creado/actualizado/eliminado**   | `EquipoService`        | `RecalcGrupoStats()`  | Inline en Create/Update/Delete | Recalcula `grupos_equipos.cantidad` y `costo_promedio`      |
| **Gavetero creado/eliminado**             | `GaveteroService`      | `RecalcMuebleCount()` | Inline en Create/Delete        | Recalcula `muebles.numero_gaveteros`                        |
| **Préstamo eliminado (soft-delete)**      | `PrestamoService`      | `Delete()`            | Inline                         | Propaga `estado_eliminado=true` a `detalles_prestamos`      |
| **Mantenimiento eliminado (soft-delete)** | `MantenimientoService` | `Delete()`            | Inline                         | Propaga `estado_eliminado=true` a `detalles_mantenimientos` |

> 💛 **Principio:** El backend es la única fuente de verdad para sincronización de datos.

### Funciones SQL Disponibles

Aunque no se usan como triggers, están definidas en el DDL para referencia:

- `fn_actualizar_cantidad_equipo_por_estado()` — calcula cantidad de equipos por estado
- `fn_actualizar_costo_promedio_grupo()` — promedia costos de equipos en un grupo
- `fn_actualizar_conteo_gaveteros_por_estado()` — cuenta gavetas por mueble
- `fn_estado_eliminado_prestamo_a_detalle()` — cascade de soft-delete para préstamos
- `fn_estado_eliminado_mantenimiento_a_detalle()` — cascade de soft-delete para mantenimientos

---

## 👁️ Vistas SQL

| Vista                                | SELECT                               | Propósito                                                                         |
| ------------------------------------ | ------------------------------------ | --------------------------------------------------------------------------------- |
| `vw_equipos_necesitan_mantenimiento` | Equipos + Última fecha mantenimiento | Reportería: equipos que necesitan mantenimiento preventivo en los últimos N meses |
| `vw_ubicaciones_grupos_equipos`      | Muebles + Gaveteros + Equipos        | Ubicación física de equipos en almacén (agrupación jerárquica)                    |

---

## 🔍 Estrategia de Índices

La estrategia de indexación prioriza **queries frecuentes** y **joins comunes**.

### Usuarios

```sql
CREATE INDEX idx_usuarios_email_estado ON usuarios(email, estado_eliminado);
CREATE INDEX idx_usuarios_nombre_estado ON usuarios(nombre, estado_eliminado);
```

**Justificación:** Acelera login (`email`) y listados sin soft-deleted

### Prestamos

```sql
CREATE INDEX idx_prestamos_temporal_usuario_estado
  ON prestamos(fecha_prestamo, fecha_devolucion_esperada, carnet, estado_eliminado);
```

**Justificación:** Optimiza filtros por rango de fechas, usuario e historial

### Mantenimientos

```sql
CREATE INDEX idx_mantenimientos_temporal_empresa_estado
  ON mantenimientos(fecha_inicio, fecha_final, id_empresa, estado_eliminado);
```

**Justificación:** Segmenta históricos por empresa y período

### Grupos de Equipos

```sql
CREATE INDEX idx_grupos_equipos_búsqueda
  ON grupos_equipos(categoria, nombre, modelo, marca, estado_eliminado);
```

**Justificación:** Acelera búsquedas complejas de tipo "todas las impresoras HP activas"

### Gaveteros

```sql
CREATE INDEX idx_gaveteros_ubicación
  ON gaveteros(nombre, id_mueble, estado_eliminado);
```

**Justificación:** Optimiza asignación y consultas de espacio

### Equipos

```sql
CREATE INDEX idx_equipos_agrupación
  ON equipos(id_grupo_equipo, codigo_imt, estado_eliminado);
```

**Justificación:** Fundamental para joins con grupos y filtros de equipos activos

### Empresas de Mantenimiento

```sql
CREATE INDEX idx_empresas_mantenimiento_activas
  ON empresas_mantenimiento(nombre, estado);
```

**Justificación:** Acelera selección de proveedores activos

### Detalles de Préstamos y Mantenimientos

```sql
CREATE INDEX idx_detalles_prestamos_por_prestamo
  ON detalles_prestamos(id_prestamo, estado_eliminado);

CREATE INDEX idx_detalles_mantenimientos_por_mantenimiento
  ON detalles_mantenimientos(id_mantenimiento, estado_eliminado);
```

**Justificación:** Optimiza obtención de ítems por encabezado

### Catálogos

```sql
CREATE INDEX idx_categorias_nombre ON categorias(nombre, estado_eliminado);
CREATE INDEX idx_carreras_nombre ON carreras(nombre, estado_eliminado);
CREATE INDEX idx_accesorios_nombre ON accesorios(nombre, estado_eliminado);
```

**Justificación:** Valida unicidad y poblado de dropdowns

---

## 🔄 Transacciones y Aislamiento

| Propiedad                | Valor              | Razón                                                                               |
| ------------------------ | ------------------ | ----------------------------------------------------------------------------------- |
| **Nivel de Aislamiento** | `SERIALIZABLE`     | Garantiza ausencia de lecturas fantasma y no repetibles                             |
| **Unidad Atómica**       | `SaveChangesAsync` | Operaciones críticas (crear préstamo + contrato + detalles) en una sola transacción |

**Ejemplo:** Creación de préstamo completo en una transacción:

```csharp
using (var transaction = await dbContext.Database.BeginTransactionAsync())
{
    // 1. Crear préstamo
    var prestamo = new Prestamo { ... };
    dbContext.Prestamos.Add(prestamo);

    // 2. Crear detalles
    var detalles = new[] { new DetallePrestamo { ... } };
    dbContext.DetallesPrestamos.AddRange(detalles);

    // 3. Crear contrato
    var contrato = new Contrato { ContratoHtml = html };
    dbContext.Contratos.Add(contrato);

    // 4. Commit
    await dbContext.SaveChangesAsync();
    await transaction.CommitAsync();
}
```

---

## 🚀 Restauración Inicial

### Opción 1: Nativa (recomendado en desarrollo)

```bash
# Crear base de datos
psql -U postgres -c "CREATE DATABASE IMT_Reservas;"

# Cargar schema
psql -U postgres -d IMT_Reservas -f DataBase/database.ddl
```

### Opción 2: Docker

```bash
docker run -d \
  --name ucbhold-postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=IMT_Reservas \
  -p 5432:5432 \
  -v ucbhold-pgdata:/var/lib/postgresql/data \
  postgres:14

# Cargar schema
docker exec -i ucbhold-postgres \
  psql -U postgres -d IMT_Reservas < DataBase/database.ddl
```

### Verificación

```bash
psql -U postgres -d IMT_Reservas -c "\dt"
```

Deberías ver todas las 15 tablas listadas ✅

---

## 📊 Plan de Ejecución

### Antes de Optimización

![Sin índices](https://github.com/user-attachments/assets/90820cbc-6f9d-4186-8b0d-4777f01e61f9)

### Después de Optimización

![Con índices](https://github.com/user-attachments/assets/102805fd-879f-4817-845c-df516831b876)

**Mejora:** ~85% reducción en tiempo de query

---

## 💡 Buenas Prácticas

- ✅ Siempre usar soft delete (`estado_eliminado`) para auditoría
- ✅ Includencias de navegación explícitas en queries para evitar N+1
- ✅ Usar índices compound para múltiples filtros
- ✅ Monitorear `pg_stat_statements` en producción
- 🚫 No eliminar directamente — marcar como eliminado
- 🚫 No confiar en triggers — usar Services del backend

---
