# Arquitectura

## Capas

El backend sigue una arquitectura por capas estricta:

```
Presentation (Controllers + Middleware)
    ↓
Application (Services + DTOs + Features)
    ↓
Infrastructure (Repositories + DbContext + PostgreSQL)
    ↓
Core (Entities + Abstractions + Enums)
```

Cada capa solo depende de las inferiores. **Los controllers nunca acceden a Repositories directamente**, siempre vía Service.

### Responsabilidades

| Capa | Responsabilidad |
|------|-----------------|
| **Presentation** | Recibir request, llamar Service, devolver `Response<T>`. Sin lógica de negocio. |
| **Application** | Validaciones de negocio, orquestación, transformación DTO ↔ Entity. |
| **Infrastructure** | Queries Entity Framework optimizadas, proyecciones SQL, mapeo a DTO. |
| **Core** | Entidades de dominio puras, enums, interfaces base. Sin dependencias. |

---

## Patrones de diseño aplicados

### Template Method (Repository + Service genéricos)

Las clases base `Repository<TEntity, TDto>` y `Service<TEntity, TRepository, TDto>` definen el esqueleto del CRUD (`Create`, `Update`, `Delete`, `Get`, `GetAll`). Las clases concretas sobrescriben solo los métodos que requieren lógica específica.

**Ubicación:**
- `Code/Server/src/Infrastructure/Repositories/Abstraction/Repository.cs`
- `Code/Server/src/Application/Abstraction/Service.cs`

**Regla:** todos los Services heredan de `Service<,,>` y todos los Repositories de `Repository<,>`. Excepción legítima: `CarritoService` (no maneja entidad propia, calcula disponibilidad).

### State (transiciones de EstadoPrestamo)

Las transiciones válidas de estado de un préstamo viven en `EstadoPrestamoState` como una tabla de transiciones encapsulada.

**Ubicación:** `Code/Server/src/Application/Features/Prestamo/EstadoPrestamoState.cs`

```csharp
public static bool CanTransition(EstadoPrestamo current, EstadoPrestamo next);
public static EstadoPrestamo? Parse(string? estado);
public static string ToText(EstadoPrestamo estado);
```

Transiciones permitidas:
| Desde | Hacia |
|-------|-------|
| `pendiente` | `aprobado`, `rechazado`, `cancelado` |
| `aprobado` | `activo`, `cancelado` |
| `activo` | `finalizado`, `cancelado` |
| `rechazado`, `finalizado`, `cancelado` | (terminales) |

### Result Pattern (Ardalis.Result)

**Prohibido `try-catch` en services para flujo normal.** Toda operación devuelve `Result<T>` con éxito o error explícito.

```csharp
public async Task<Result<UsuarioDto>> Create(UsuarioEntity entity)
{
    var emailExists = await _dbContext.Usuarios.AnyAsync(...);
    if (emailExists)
        return Result<UsuarioDto>.Error("Email ya existe");

    return await base.Create(entity);
}
```

Excepciones inevitables (race conditions de DB, etc.) las captura el `ExceptionMiddleware` que convierte cualquier excepción no controlada en `Response<object>` con el status HTTP apropiado.

---

## Estructura de respuesta

Todos los endpoints devuelven `Response<T>`:

```json
{
  "Status": 200,
  "Value": { ... } | [...] | null,
  "Errors": ["mensaje 1", "mensaje 2"],
  "ValidationErrors": [],
  "SuccessMessage": null
}
```

Códigos HTTP:
- `200 OK` — éxito con valor
- `201 Created` — creación exitosa
- `204 No Content` — eliminación exitosa
- `400 Bad Request` — error de validación
- `401 Unauthorized` — credenciales inválidas
- `404 Not Found` — recurso no existe
- `409 Conflict` — duplicado o restricción de DB violada
- `500 Internal Server Error` — error no controlado (logueado)

---

## Convención de nombres

**Regla general:** funciones, métodos y variables locales en **inglés**.

**Excepciones (mantener en español):**
- Nombres de entidades: `Prestamo`, `Usuario`, `Equipo`, `Carrera`, etc.
- Atributos de entidades: `Carnet`, `CodigoImt`, `EstadoPrestamo`, `IdCarrera`, etc.

**Ejemplos correctos:**
```csharp
public bool CanTransition(EstadoPrestamo current, EstadoPrestamo next)   // método inglés, EstadoPrestamo es atributo
public async Task<int?> GetEquipoByCodigoImt(int codigoImt)              // GetBy inglés, CodigoImt es atributo
var carnetExists = await _dbContext.Usuarios.AnyAsync(...);               // variable mezcla OK (Carnet es atributo)
.Select(usuario => usuario.Email)                                         // lambda usa nombre de entidad
```

---

## Decisiones clave

### 1. Soft delete universal
Todas las entidades tienen `EstadoEliminado BOOLEAN DEFAULT FALSE`. Los DbContext aplican filtro global `HasQueryFilter(e => !e.EstadoEliminado)`. Para chequeos de unicidad (carnet, email), usar `.IgnoreQueryFilters()` para evitar conflictos con registros soft-deleted.

### 2. Sin AutoMapper
Mappings DTO ↔ Entity son explícitos. Cada Repository tiene `MapToDto` abstracto. Para proyecciones SQL, los repositories usan `.Select(...)` directo en LINQ.

### 3. AsNoTracking por defecto
Toda query de lectura usa `.AsNoTracking()`. Los entities tracked solo en operaciones de Update.

### 4. Proyecciones SQL > client-side
`GetAll` usa `.Join(...).Select(... => new Dto)` en la query para que el JOIN ocurra en PostgreSQL, no en memoria.

### 5. CarritoService como excepción
No tiene entidad propia (`Carrito` es un cálculo dinámico, no se persiste). Por eso no hereda de `Service<,,>` y vive como servicio independiente con `ApplicationDbContext` inyectado.

---

## Performance

- **Connection pooling** habilitado vía `Pooling=true;MinPoolSize=2;MaxPoolSize=20` en connection string
- **Login** completa en < 500ms en warm pool (BCrypt ~250ms es el costo dominante)
- **Cold start** primer request ≤ 2s con `MinPoolSize=2` pre-abriendo conexiones
- **RAM objetivo** < 700MB con carga normal
