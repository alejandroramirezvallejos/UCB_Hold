# Development Setup

Full-stack local environment: .NET 8 + Angular 18 + PostgreSQL 14+

---

## ✦ Prerequisites

| Tool | Min version | Install | Verify |
|---|---|---|---|
| .NET SDK | 8.0 LTS | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) | `dotnet --version` |
| Node.js | 18 LTS | [nodejs.org](https://nodejs.org) | `node -v` |
| PostgreSQL | 14 | [postgresql.org](https://www.postgresql.org/download/) | `psql --version` |
| Angular CLI | 18 | `npm install -g @angular/cli@18` | `ng version` |
| Git | any | [git-scm.com](https://git-scm.com) | `git --version` |

---

## ✦ 1 — Database

**Native (recommended on macOS / Linux):**

```bash
psql -U postgres -c "CREATE DATABASE IMT_Reservas;"
psql -U postgres -d IMT_Reservas -f DataBase/database.ddl
psql -U postgres -d IMT_Reservas -c "\dt"   # should list 15 tables
```

**Docker (recommended on Windows):**

```bash
docker run -d \
  --name ucbhold-postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=IMT_Reservas \
  -p 5432:5432 \
  -v ucbhold-pgdata:/var/lib/postgresql/data \
  postgres:14

docker exec -i ucbhold-postgres psql -U postgres -d IMT_Reservas < DataBase/database.ddl
docker exec ucbhold-postgres psql -U postgres -d IMT_Reservas -c "\dt"
```

---

## ✦ 2 — Backend credentials

Credentials are stored in [dotnet user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) — never committed.

```bash
cd Code/Server
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:PostgreSQL" \
  "Host=localhost;Port=5432;Database=IMT_Reservas;Username=postgres;Password=postgres;Pooling=true;MinPoolSize=2;MaxPoolSize=20"
dotnet user-secrets list   # verify
```

| Parameter | Value | Reason |
|---|---|---|
| `MinPoolSize=2` | 2 connections | Pre-warm pool — avoids ~8s cold-start |
| `MaxPoolSize=20` | 20 connections | Reasonable cap for development |
| `Pooling=true` | enabled | Reuse TCP connections |

---

## ✦ 3 — Frontend dependencies

```bash
cd Code/Client
npm install
```

---

## ✦ 4 — Run

**JetBrains Rider (recommended):**

1. Open folder `Code/`
2. Select run configuration `IMT_Reservas.FullStack`
3. Press Run (`Shift+F10`)

| Configuration | Runs |
|---|---|
| `IMT_Reservas.FullStack` | Backend + Frontend simultaneously |
| `IMT_Reservas.Server` | Backend only |
| `IMT_Reservas.Client` | Frontend only (watch mode) |

**CLI (two terminals):**

```bash
# Terminal 1 — Backend
cd Code/Server && dotnet run

# Terminal 2 — Frontend
cd Code/Client && npm start
```

---

## ✦ URLs

| Service | URL |
|---|---|
| Frontend | `http://localhost:4200` |
| Backend API | `https://localhost:7216` |
| Swagger | `https://localhost:7216/swagger` |
| Health check | `https://localhost:7216/api/health` |

> Swagger is only available in `Development`. The frontend dev server proxies `/api/*` to the backend via `proxy.conf.js`.

---

## ✦ Build

```bash
# Backend
cd Code/Server
dotnet build          # debug
dotnet build -c Release

# Frontend
cd Code/Client
ng build              # dev
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

**Port 4200 in use**
```bash
ng serve --port 4300
```

**`Cannot find module '@angular/...'`**
```bash
cd Code/Client && npm install
```
