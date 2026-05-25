# Development Setup

Full-stack local environment: .NET 8 + Angular 18 + PostgreSQL 14+

---

## ✦ Prerequisites

| Tool         | Min version | Install                                               | Verify            |
| ------------ | ----------- | ----------------------------------------------------- | ----------------- |
| .NET SDK     | 8.0 LTS     | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) | `dotnet --version` |
| Node.js      | 18 LTS      | [nodejs.org](https://nodejs.org)                      | `node -v`         |
| PostgreSQL   | 14          | [postgresql.org](https://www.postgresql.org/download/) | `psql --version`  |
| Angular CLI  | 18          | `npm install -g @angular/cli@18`                      | `ng version`      |
| Docker       | any         | [docker.com](https://www.docker.com/products/docker-desktop/) | `docker -v`       |
| Git          | any         | [git-scm.com](https://git-scm.com)                   | `git --version`   |

---

## ✦ Option A — Docker (one command)

The fastest way to run the complete stack locally.

### 1 — Create `Code/server.env`

This file holds the backend configuration and is **gitignored** — never commit it.

```ini
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80
ConnectionStrings__PostgreSQL=Host=ucb_db;Port=5432;Database=IMT_Reservas;Username=<USERNAME>;Password=<PASSWORD>;Pooling=true;MinPoolSize=2;MaxPoolSize=20
```

### 2 — Start

```bash
cd Code
docker compose up --build
```

Docker will:
1. Pull `postgres:14` and start the database
2. Run `Database/server.sql` automatically on first startup (schema + seed data)
3. Build and start the .NET 8 backend
4. Build the Angular app and serve it via nginx

| Service     | URL                   |
| ----------- | --------------------- |
| Frontend    | http://localhost:4200 |
| Backend API | http://localhost:5000 |

### Useful commands

```bash
# Stop without removing volumes
docker compose down

# Stop and remove database volume (fresh start)
docker compose down -v

# Follow backend logs
docker logs -f ucb_server

# Follow all services
docker compose logs -f
```

---

## ✦ Option B — Native (recommended for active development)

### 1 — Database

**Native (macOS / Linux):**

```bash
psql -U postgres -c "CREATE DATABASE IMT_Reservas;"
psql -U postgres -d IMT_Reservas -f Database/server.sql
psql -U postgres -d IMT_Reservas -c "\dt"   # should list 15 tables
```

**Docker (Windows):**

```bash
docker run -d \
  --name ucbhold-postgres \
  -e POSTGRES_PASSWORD=<PASSWORD> \
  -e POSTGRES_DB=IMT_Reservas \
  -p 5432:5432 \
  -v ucbhold-pgdata:/var/lib/postgresql/data \
  postgres:14

docker exec -i ucbhold-postgres psql -U postgres -d IMT_Reservas < Database/server.sql
docker exec ucbhold-postgres psql -U postgres -d IMT_Reservas -c "\dt"
```

### 2 — Backend credentials

Credentials are stored in [dotnet user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) — never committed.

```bash
cd Code/Server
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:PostgreSQL" \
  "Host=localhost;Port=5432;Database=IMT_Reservas;Username=<USERNAME>;Password=<PASSWORD>;Pooling=true;MinPoolSize=2;MaxPoolSize=20"
dotnet user-secrets list   # verify
```

| Parameter      | Value | Reason                                |
| -------------- | ----- | ------------------------------------- |
| `MinPoolSize=2` | 2     | Pre-warm pool — avoids ~8s cold-start |
| `MaxPoolSize=20`| 20    | Reasonable cap for development        |
| `Pooling=true`  | —     | Reuse TCP connections                 |

### 3 — Frontend dependencies

```bash
cd Code/Client
npm install
```

### 4 — Run

**JetBrains Rider (recommended):**

1. Open folder `Code/`
2. Select run configuration `IMT_Reservas.FullStack`
3. Press Run (`Shift+F10`)

| Configuration              | Runs                              |
| -------------------------- | --------------------------------- |
| `IMT_Reservas.FullStack`   | Backend + Frontend simultaneously |
| `IMT_Reservas.Server`      | Backend only                      |
| `IMT_Reservas.Client`      | Frontend only (watch mode)        |

**CLI (two terminals):**

```bash
# Terminal 1 — Backend
cd Code/Server && dotnet run

# Terminal 2 — Frontend
cd Code/Client && npm start
```

### URLs

| Service      | URL                            |
| ------------ | ------------------------------ |
| Frontend     | `http://localhost:4200`        |
| Backend API  | `https://localhost:7216`       |
| Swagger      | `https://localhost:7216/swagger` |
| Health check | `https://localhost:7216/api/health` |

> Swagger is only available in `Development`. The frontend dev server proxies `/api/*` to the backend via `proxy.conf.js`.

---

## ✦ Tests

All 90 tests run against an **EF Core InMemory** database — no PostgreSQL required.

```bash
# Run all tests
dotnet test Code/Tests/IMT_Reservas.Tests.csproj

# Verbose output
dotnet test Code/Tests/IMT_Reservas.Tests.csproj --logger "console;verbosity=normal"

# Filter by suite
dotnet test Code/Tests/IMT_Reservas.Tests.csproj --filter "FullyQualifiedName~Integration"
dotnet test Code/Tests/IMT_Reservas.Tests.csproj --filter "FullyQualifiedName~Unit"
```

Tests also run automatically on every push and PR to `main` and `develop` via GitHub Actions. Coverage output (`coverage.cobertura.xml`) is uploaded as an artifact.

---

## ✦ Build

```bash
# Backend
cd Code/Server
dotnet build              # debug
dotnet build -c Release

# Frontend
cd Code/Client
ng build                  # dev
ng build --configuration production
```

---

## ✦ Troubleshooting

**`dotnet: command not found`** — install .NET 8 SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)

**`Connection refused` on PostgreSQL**
```bash
psql -U postgres -c "SELECT 1"        # test connection
# Docker: docker start ucbhold-postgres
# Linux:  sudo systemctl start postgresql
# macOS:  brew services start postgresql
```

**`User Secrets not initialized`**
```bash
cd Code/Server && dotnet user-secrets init
```

**Docker — backend container restarting**
```bash
docker logs ucb_server   # check for missing server.env or wrong connection string
```

**Port 4200 in use**
```bash
ng serve --port 4300
```

**`Cannot find module '@angular/...'`**
```bash
cd Code/Client && npm install
```
