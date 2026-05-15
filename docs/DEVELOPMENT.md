# Setup de Desarrollo

Guía para levantar el entorno local en Windows, macOS o Linux.

---

## Prerequisitos

| Herramienta | Versión | Notas |
|-------------|---------|-------|
| .NET SDK | 8.0+ | https://dotnet.microsoft.com/download |
| Node.js | 18+ | https://nodejs.org |
| PostgreSQL | 14+ | Instalación nativa o vía Docker |
| JetBrains Rider | Última | Opcional, recomendado para fullstack |

---

## 1. Base de datos PostgreSQL

### Opción A: instalación nativa
Instala PostgreSQL desde https://www.postgresql.org/download/ y crea la base:
```bash
psql -U postgres -c "CREATE DATABASE IMT_Reservas;"
psql -U postgres -d IMT_Reservas -f DataBase/database.ddl
```

### Opción B: Docker
```bash
docker run -d \
  --name ucbhold-postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=IMT_Reservas \
  -p 5432:5432 \
  -v ucbhold-pgdata:/var/lib/postgresql/data \
  postgres:14

docker exec -i ucbhold-postgres psql -U postgres -d IMT_Reservas < DataBase/database.ddl
```

---

## 2. User Secrets

Las credenciales **no** viven en `appsettings.json`. Se almacenan localmente con `dotnet user-secrets`.

```bash
cd Code/Server

dotnet user-secrets init

dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=localhost;Port=5432;Database=IMT_Reservas;Username=postgres;Password=postgres;Pooling=true;MinPoolSize=2;MaxPoolSize=20"
```

Los parámetros `Pooling=true;MinPoolSize=2;MaxPoolSize=20` son críticos para performance:
- **MinPoolSize=2** — pre-abre 2 conexiones al arrancar → el primer request no paga el handshake (resuelve cold-start de 8s)
- **MaxPoolSize=20** — limita el footprint de memoria

Para inspeccionar secrets configurados:
```bash
dotnet user-secrets list
```

---

## 3. Frontend dependencies

```bash
cd Code/Client
npm install
```

---

## 4. Ejecutar el proyecto

### Opción A: JetBrains Rider 
Abrir `Code/` en Rider → seleccionar config `IMT_Reservas.FullStack` → Run. Levanta backend + frontend simultáneamente.

Configs disponibles en `Code/.run/`:
- `IMT_Reservas.Server` — solo backend
- `IMT_Reservas.Client` — solo frontend
- `IMT_Reservas.FullStack` — ambos (compound)

El config del servidor incluye `ASPNETCORE_ENVIRONMENT=Development` automáticamente.

### Opción B: CLI
```bash
cd Code/Server && dotnet run
```
En otra terminal:
```bash
cd Code/Client && npm start
```

### URLs
- Backend: `https://localhost:7001` (puerto puede variar)
- Frontend: `http://localhost:4200`
- Swagger: `https://localhost:7001/swagger`

---

## 5. Variables de entorno

| Variable | Valor dev | Valor prod |
|----------|-----------|------------|
| `ASPNETCORE_ENVIRONMENT` | `Development` | `Production` |
| `ConnectionStrings__PostgreSQL` | (user-secrets) | (OS env) |
| `AllowedOrigins__0` | (en `appsettings.Development.json`) | (OS env) |

En Development, `appsettings.Development.json` ya tiene `AllowedOrigins: ["http://localhost:4200", "https://localhost:4200"]`.

---

## 6. Build y tests

```bash
cd Code/Server
dotnet build           # debe terminar con "0 Errores"
dotnet test            # (cuando existan tests)
```

```bash
cd Code/Client
ng build
ng test
```
