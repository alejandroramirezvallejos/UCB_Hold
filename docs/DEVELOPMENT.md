# Desarrollo

Guía para levantar el entorno local en Windows, macOS o Linux.

---

## Prerequisitos

| Herramienta | Versión mínima | Enlace |
|-------------|---------------|--------|
| .NET SDK | 8.0 | https://dotnet.microsoft.com/download |
| Node.js | 18 | https://nodejs.org |
| PostgreSQL | 14 | https://www.postgresql.org/download/ — o vía Docker |
| Angular CLI | 18 | `npm install -g @angular/cli` |
| JetBrains Rider | Última | Opcional — recomendado para fullstack |

---

## 1. Base de datos PostgreSQL

### Opción A — instalación nativa

```bash
psql -U postgres -c "CREATE DATABASE IMT_Reservas;"
psql -U postgres -d IMT_Reservas -f DataBase/database.ddl
```

### Opción B — Docker

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

Verificar conexión:
```bash
psql -U postgres -d IMT_Reservas -c "\dt"
```

---

## 2. Credenciales del backend (User Secrets)

Las credenciales **no** viven en `appsettings.json`. Se almacenan localmente con `dotnet user-secrets` (encriptado por .NET, nunca se commitea).

```bash
cd Code/Server

dotnet user-secrets init

dotnet user-secrets set "ConnectionStrings:PostgreSQL" \
  "Host=localhost;Port=5432;Database=IMT_Reservas;Username=postgres;Password=postgres;Pooling=true;MinPoolSize=2;MaxPoolSize=20"
```

> **Por qué `MinPoolSize=2;MaxPoolSize=20`:** pre-abre 2 conexiones al arrancar → el primer request no paga el TCP handshake (evita cold-start de ~8s). `MaxPoolSize=20` limita el footprint de memoria.

Verificar secrets configurados:
```bash
dotnet user-secrets list
```

---

## 3. Frontend — dependencias

```bash
cd Code/Client
npm install
```

---

## 4. Ejecutar el proyecto

### Opción A — JetBrains Rider

Abrir la carpeta `Code/` en Rider → seleccionar la run config `IMT_Reservas.FullStack` → **Run**.

Configs disponibles en `Code/.run/`:

| Config | Descripción |
|--------|------------|
| `IMT_Reservas.Server` | Solo backend |
| `IMT_Reservas.Client` | Solo frontend |
| `IMT_Reservas.FullStack` | Backend + frontend simultáneo |

El config del servidor incluye `ASPNETCORE_ENVIRONMENT=Development` automáticamente.

### Opción B — CLI (dos terminales)

```bash
# Terminal 1 — backend
cd Code/Server
dotnet run
```

```bash
# Terminal 2 — frontend
cd Code/Client
npm start
```

### URLs locales

| Servicio | URL |
|---------|-----|
| Frontend | http://localhost:4200 |
| Backend API | https://localhost:7001 |
| Swagger UI | https://localhost:7001/swagger |

---

## 5. Variables de entorno

| Variable | Entorno dev | Entorno prod |
|----------|-------------|--------------|
| `ASPNETCORE_ENVIRONMENT` | `Development` (automático en Rider) | `Production` (via `/etc/imt-reservas.env`) |
| `ConnectionStrings:PostgreSQL` | user-secrets | OS env var |
| `AllowedOrigins:0` | `appsettings.Development.json` (ya configurado) | OS env var |

`appsettings.Development.json` ya trae:
```json
{
  "AllowedOrigins": ["http://localhost:4200", "https://localhost:4200"]
}
```

No necesitas modificarlo para desarrollo local estándar.

---

## 6. Build y tests

```bash
cd Code/Server
dotnet build        # debe terminar con "0 Errores"
dotnet test         # cuando existan tests
```

```bash
cd Code/Client
ng build
ng test
```

---

## 7. Tecnologías del backend

| Paquete | Versión | Propósito |
|---------|---------|-----------|
| Microsoft.EntityFrameworkCore | 8.0.4 | ORM |
| Npgsql.EntityFrameworkCore.PostgreSQL | 8.0.4 | Driver PostgreSQL |
| Riok.Mapperly | 3.6.0 | Mapeo DTO↔Entity (source-generated) |
| FluentValidation | 11.9.0 | Validación de entradas |
| Ardalis.Result | 10.1.0 | Result pattern (sin try-catch) |
| BCrypt.Net-Next | 4.0.3 | Hash de contraseñas |
| Swashbuckle.AspNetCore | 6.6.2 | Swagger (solo Development) |
