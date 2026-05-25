<div align="center">

<img src="Images/logo.png" alt="UCB Hold" width="160" />

# UCB Hold

**Reservation System — UCB Mechatronics Lab**

Web platform for managing loans of equipment, accessories, and tools from the **Universidad Católica Boliviana** Mechatronics Lab.

[![Live](https://img.shields.io/badge/Live-ucbhold.dev-22c55e?style=flat-square&logo=vercel&logoColor=white)](https://ucbhold.dev)
[![Tests](https://github.com/alejandroramirezvallejos/UCB_Hold/actions/workflows/tests.yml/badge.svg)](https://github.com/alejandroramirezvallejos/UCB_Hold/actions/workflows/tests.yml)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-18-DD0031?style=flat-square&logo=angular)](https://angular.io/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-14-4169E1?style=flat-square&logo=postgresql)](https://www.postgresql.org/)
[![EF Core](https://img.shields.io/badge/EF_Core-8.0-512BD4?style=flat-square&logo=nuget)](https://learn.microsoft.com/en-us/ef/core/)
[![Mapperly](https://img.shields.io/badge/Mapperly-3.6-00ADB5?style=flat-square)](https://mapperly.riok.app/)
[![FluentValidation](https://img.shields.io/badge/FluentValidation-11.9-00B4AB?style=flat-square)](https://fluentvalidation.net/)

</div>

---

## ✦ Features

|                                  |                                                                                                        |
| -------------------------------- | ------------------------------------------------------------------------------------------------------ |
| 📦 **Equipment management**      | Inventory with groups, drawers, and cabinets. IMT code, status, and average cost auto-calculated.      |
| 📋 **Loans with contract**       | End-to-end flow: request → approval → checkout → return. Generates an attached HTML contract.          |
| 📅 **Real-time availability**    | Cart calculates available units per group per day. Only `aprobado` and `activo` loans block capacity.  |
| 🔒 **Conflict prevention**       | Pre-create check + approval-time conflict check prevent double-booking at every transition.             |
| 🔧 **Maintenance**               | Records corrective and preventive maintenance by external company with per-item details.               |
| 🗂️ **Full catalogs**             | Degrees, categories, accessories, components, companies, and cabinets with full CRUD.                  |
| 🛡️ **Validation**                | Password strength (uppercase + number + special char). Carnet, email, NIT, and phone uniqueness.       |
| ♻️ **Logical delete**            | No data is physically deleted. `estado_eliminado = true` with automatic cascade to details.            |
| ⚡ **Optimized queries**         | Single SQL projection with LEFT JOINs across repositories. `AsNoTracking` + composite indexes.         |
| 🧪 **Tested**                    | 90 integration + unit tests with NUnit, FluentAssertions, and EF Core InMemory. CI on every push.      |
| 🐳 **Docker**                    | One-command full-stack deployment via `docker compose up`.                                             |

---

## ✦ Stack

| Layer          | Technology                       | Version  |
| -------------- | -------------------------------- | -------- |
| Frontend       | Angular                          | 18       |
| Backend        | ASP.NET Core                     | .NET 8   |
| ORM            | Entity Framework Core + Npgsql   | 8.0.10   |
| Database       | PostgreSQL                       | 14+      |
| Mapping        | Riok.Mapperly (source-generated) | 3.6.0    |
| Validation     | FluentValidation                 | 11.9.0   |
| Result pattern | Ardalis.Result                   | 10.1.0   |
| Passwords      | BCrypt.Net-Next                  | 4.0.3    |
| Testing        | NUnit + FluentAssertions         | 3.14 / 6.12 |
| CI             | GitHub Actions                   | —        |

---

## ✦ Quick Start

### Option A — Docker (recommended)

Requires [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Windows / macOS) or Docker Engine (Linux).

**1. Create `Code/server.env`** (never committed):

```ini
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80
ConnectionStrings__PostgreSQL=Host=ucb_db;Port=5432;Database=IMT_Reservas;Username=postgres;Password=postgres;Pooling=true;MinPoolSize=2;MaxPoolSize=20
```

**2. Start the full stack:**

```bash
cd Code
docker compose up --build
```

| Service     | URL                    |
| ----------- | ---------------------- |
| Frontend    | http://localhost:4200  |
| Backend API | http://localhost:5000  |

> The database is initialized automatically from `Database/server.sql` on first run.

---

### Option B — Local development

#### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org)
- [PostgreSQL 14+](https://www.postgresql.org/download/)

#### 1 — Database

```bash
psql -U postgres -c "CREATE DATABASE IMT_Reservas;"
psql -U postgres -d IMT_Reservas -f Database/server.sql
```

#### 2 — Backend credentials

```bash
cd Code/Server
dotnet user-secrets set "ConnectionStrings:PostgreSQL" \
  "Host=localhost;Port=5432;Database=IMT_Reservas;Username=postgres;Password=postgres;Pooling=true;MinPoolSize=2;MaxPoolSize=20"
```

#### 3 — Frontend dependencies

```bash
cd Code/Client && npm install
```

#### 4 — Run

```bash
# Terminal 1 — Backend
cd Code/Server && dotnet run

# Terminal 2 — Frontend
cd Code/Client && npm start
```

| Service     | URL                            |
| ----------- | ------------------------------ |
| Frontend    | http://localhost:4200          |
| Backend API | https://localhost:7216         |
| Swagger     | https://localhost:7216/swagger |

---

## ✦ Tests

```bash
# Run all 90 tests
dotnet test Code/Tests/IMT_Reservas.Tests.csproj

# With detailed output
dotnet test Code/Tests/IMT_Reservas.Tests.csproj --logger "console;verbosity=normal"
```

Tests run automatically on every push and pull request to `main` and `develop` via GitHub Actions. Coverage is uploaded as an artifact.

| Suite                        | Count | Description                                           |
| ---------------------------- | ----- | ----------------------------------------------------- |
| `Unit/PrestamoStateTests`    | 17    | State machine transitions and string parsing          |
| `Integration/UsuarioService` | 13    | User creation, login, validation, soft-delete         |
| `Integration/EquipoService`  | 10    | IMT code assignment, date, group stat recalculation   |
| `Integration/PrestamoService`| 12    | Availability checks, status transitions, history      |
| `Integration/CarritoService` | 8     | Capacity math with concurrent loans                   |

---

## ✦ Architecture

```
┌─────────────────────────────────────────────────────┐
│                    Angular 18                       │
│              (Client - localhost:4200)              │
└──────────────────────┬──────────────────────────────┘
                       │ HTTP /api/*
┌──────────────────────▼──────────────────────────────┐
│              ASP.NET Core 8 — Controllers           │
│         Result<T> · FluentValidation · Mapperly     │
├─────────────────────────────────────────────────────┤
│                    Services                         │
│   Business logic · State machines · Recalculations  │
├─────────────────────────────────────────────────────┤
│                  Repositories                       │
│    EF Core · AsNoTracking · SQL projections         │
├─────────────────────────────────────────────────────┤
│           PostgreSQL 14 — ApplicationDbContext      │
│    Native enums · Composite indexes · Pool 2–20     │
└─────────────────────────────────────────────────────┘
```

---

## ✦ Documentation

| Document                                   | Content                                                        |
| ------------------------------------------ | -------------------------------------------------------------- |
| [Docs/API.md](Docs/API.md)                 | All endpoints, DTOs, validation rules, and response codes      |
| [Docs/DATABASE.md](Docs/DATABASE.md)       | ER schema, tables, enums, views, indexes, business logic       |
| [Docs/DEVELOPMENT.md](Docs/DEVELOPMENT.md) | Full local setup, Docker, user-secrets, test runner            |
| [Docs/DEPLOYMENT.md](Docs/DEPLOYMENT.md)   | Oracle Cloud deployment · nginx · systemd · production URL     |

---

## ✦ Team

<table>
  <tr>
    <td align="center">
      <a href="https://github.com/josue-balbontin">
        <b>Josue Galo Balbontin Ugarteche</b>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/alejandroramirezvallejos">
        <b>Alejandro Ramirez Vallejos</b>
      </a>
    </td>
    <td align="center">
      <a href="https://github.com/FernandoTerrazasLl">
        <b>Fernando Terrazas Llanos</b>
      </a>
    </td>
  </tr>
</table>

---

## ✦ Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for the workflow, standards, and review expectations.

---

## ✦ Code of Conduct

Please review [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md).
