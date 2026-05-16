# Base de Datos

PostgreSQL 14. ORM: Entity Framework Core 8 + Npgsql.

## Enums PostgreSQL

| Tipo PG | Valores |
|---------|---------|
| `estado_prestamo` | `Pendiente`, `Aprobado`, `Activo`, `Rechazado`, `Finalizado`, `Cancelado` |
| `tipo_usuario` | `admin`, `usuario` |
| `estado_equipo` | `operativo`, `parcialmente_operativo`, `inoperativo` |

Registrados con `NpgsqlConnection.GlobalTypeMapper.MapEnum<>()` en `Program.cs`.

## Entidades principales

| Entidad | Tabla | Notas |
|---------|-------|-------|
| `Usuario` | `usuarios` | Soft-delete (`estado_eliminado`). Imágenes de carnet en bytea. |
| `Carrera` | `carreras` | Soft-delete. |
| `Categoria` | `categorias` | Soft-delete. |
| `GrupoEquipo` | `grupos_equipos` | Soft-delete. `cantidad` y `costo_promedio` son campos derivados. |
| `Equipo` | `equipos` | Soft-delete. `codigo_imt` asignado automáticamente. |
| `Gavetero` | `gaveteros` | Soft-delete. `numero_gaveteros` es campo derivado. |
| `Mueble` | `muebles` | Soft-delete. |
| `Prestamo` | `prestamos` | Soft-delete. Estado controlado por State pattern. |
| `DetallePrestamo` | `detalles_prestamos` | Relación equipo↔préstamo. |
| `Contrato` | `contratos` | HTML del contrato firmado. |
| `EmpresaMantenimiento` | `empresas_mantenimiento` | Soft-delete. |
| `Mantenimiento` | `mantenimientos` | |
| `Accesorio` | `accesorios` | Accesorios de equipo. |
| `Componente` | `componentes` | Componentes de equipo. |

## Campos derivados

`cantidad`/`costo_promedio` en `GrupoEquipo` y `numero_gaveteros` en `Mueble` son mantenidos por:

1. **Triggers de BD** — SQL functions attached como triggers en la DB.
2. **Backend (EquipoRepository / GaveteroRepository)** — `RecalcGrupoStats` y `RecalcMuebleCount` se llaman desde los servicios en Create/Update/Delete como capa de redundancia.

No confiar en un solo mecanismo; ambos coexisten intencionalmente.

## Soft-delete

La mayoría de entidades usan `estado_eliminado = true` en lugar de `DELETE`. El repositorio base aplica `HasQueryFilter(e => !e.EstadoEliminado)` automáticamente. Para queries que necesiten ignorarlo, usar `.IgnoreQueryFilters()`.

## Conexión

```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=imt_reservas;Username=postgres;Password=..."
  }
}
```
