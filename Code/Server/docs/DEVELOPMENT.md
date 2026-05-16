# Guía de Desarrollo

## Requisitos

- .NET 8 SDK
- PostgreSQL 14+
- Node.js 20+ (para el cliente Angular)

## Setup local

### 1. Configurar la cadena de conexión

Crear o editar `Code/Server/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=imt_reservas;Username=postgres;Password=TU_PASSWORD"
  },
  "AllowedOrigins": ["http://localhost:4200", "https://localhost:4200"],
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### 2. Levantar el backend

```bash
cd Code/Server
dotnet run
```

### 3. Levantar el frontend

```bash
cd Code/Client
npm install
npm start
```

## URLs locales

| Servicio | URL |
|----------|-----|
| API | https://localhost:7001 |
| Swagger | https://localhost:7001/swagger |
| Health Check | https://localhost:7001/api/health |
| Angular | http://localhost:4200 |

## Estructura del proyecto

```
src/
├── Application/
│   ├── Abstraction/          # Service<>, IMapper<>
│   └── Features/             # Un directorio por feature (Dto, Service, Mapper, Validator)
├── Core/
│   ├── Entities/             # Entidades EF Core
│   └── Abstraction/          # QueryFilter, interfaces
├── Infrastructure/
│   ├── Config/               # ApplicationDbContext
│   └── Repositories/
│       ├── Abstraction/      # Repository<TEntity,TDto>
│       └── Implementations/  # Repos concretos
└── Presentation/
    ├── Controllers/
    ├── Middleware/            # ExceptionMiddleware
    └── Program.cs
```

## Convenciones

### Mappers (Mapperly)

- Decorar la clase con `[Mapper]`
- Usar `[MapProperty]` para mapear propiedades con nombre distinto
- Usar `[MapperIgnoreTarget]` / `[MapperIgnoreSource]` para campos sin mapeo directo
- Exponer `ProjectTo(IQueryable<TEntity>)` para proyección en EF (evita cargar toda la entidad)

### Validadores (FluentValidation)

- Heredar de `AbstractValidator<TDto>`
- Usar `Cascade(CascadeMode.Stop)` cuando una regla DB depende de que pase la validación simple primero
- La validación se llama desde el Service, nunca desde el Controller

### Repositorios

- Heredar de `Repository<TEntity, TDto>`
- `DbContext` disponible como `protected` en la base
- Overrides opcionales: `GetAll`, `Get`, `Delete`
- No inyectar `ApplicationDbContext` en Services — toda lógica DB va en el repositorio

### BCrypt

- Work factor 10 para hashes nuevos
- Al hacer login, si el hash tiene cost > 10 se rehashea en background (fire-and-forget)

## Build

```bash
cd Code/Server
dotnet build
# Esperado: 0 errores (posibles warnings de Npgsql)
```
