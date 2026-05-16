<div align="center">

<img src="Images/logo.png" alt="UCB Hold" width="160" />

# UCB Hold

**Reservation System - UCB Mechatronics Lab**

Web platform for managing loans of equipment, accessories, and tools from the **Universidad Catolica Boliviana** Mechatronics Lab.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-18-DD0031?style=flat-square&logo=angular)](https://angular.io/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-14-4169E1?style=flat-square&logo=postgresql)](https://www.postgresql.org/)
[![EF Core](https://img.shields.io/badge/EF_Core-8.0-512BD4?style=flat-square&logo=nuget)](https://learn.microsoft.com/en-us/ef/core/)
[![Mapperly](https://img.shields.io/badge/Mapperly-3.6-00ADB5?style=flat-square)](https://mapperly.riok.app/)
[![FluentValidation](https://img.shields.io/badge/FluentValidation-11.9-00B4AB?style=flat-square)](https://fluentvalidation.net/)

</div>

---

## ✦ Features

|                               |                                                                                                   |
| ----------------------------- | ------------------------------------------------------------------------------------------------- |
| 📦 **Equipment management**   | Inventory with groups, drawers, and cabinets. IMT code, status, and average cost auto-calculated. |
| 📋 **Loans with contract**    | End-to-end flow: request -> approval -> checkout -> return. Generates an attached HTML contract.  |
| 📅 **Real-time availability** | Reservation cart calculates available units per group per day within a date range.                |
| 🔧 **Maintenance**            | Records corrective and preventive maintenance by external company with per-item details.          |
| 🗂️ **Full catalogs**          | Degrees, categories, accessories, components, companies, and cabinets with full CRUD.             |
| ♻️ **Logical delete**         | No data is physically deleted. `estado_eliminado = true` with automatic cascade to details.       |
| ⚡ **Optimized queries**      | Single SQL projection with LEFT JOINs across repositories. `AsNoTracking` + composite indexes.    |

---

## ✦ Stack

| Capa           | Tecnología                       | Versión |
| -------------- | -------------------------------- | ------- |
| Frontend       | Angular                          | 18      |
| Backend        | ASP.NET Core                     | .NET 8  |
| ORM            | Entity Framework Core + Npgsql   | 8.0.4   |
| Database       | PostgreSQL                       | 14+     |
| Mapping        | Riok.Mapperly (source-generated) | 3.6.0   |
| Validation     | FluentValidation                 | 11.9.0  |
| Result pattern | Ardalis.Result                   | 10.1.0  |
| Passwords      | BCrypt.Net-Next                  | 4.0.3   |

---

## ✦ Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org)
- [PostgreSQL 14+](https://www.postgresql.org/download/) - native **or** Docker

### 1 - Database

**Nativo:**

```bash
psql -U postgres -c "CREATE DATABASE IMT_Reservas;"
psql -U postgres -d IMT_Reservas -f DataBase/schema.ddl
```

**Docker:**

```bash
docker run -d --name ucbhold-postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=IMT_Reservas \
  -p 5432:5432 \
  postgres:14

docker exec -i ucbhold-postgres psql -U postgres -d IMT_Reservas < DataBase/schema.ddl
```

### 2 - Backend - local credentials

```bash
cd Code/Server
dotnet user-secrets set "ConnectionStrings:PostgreSQL" \
  "Host=localhost;Port=5432;Database=IMT_Reservas;Username=postgres;Password=postgres;Pooling=true;MinPoolSize=2;MaxPoolSize=20"
```

### 3 - Frontend - dependencies

```bash
cd Code/Client
npm install
```

### 4 - Run

**JetBrains Rider** - open `Code/` -> config `IMT_Reservas.FullStack` -> Run.

**CLI** - two terminals:

```bash
# Terminal 1
cd Code/Server && dotnet run

# Terminal 2
cd Code/Client && npm start
```

| Servicio    | URL                            |
| ----------- | ------------------------------ |
| Frontend    | http://localhost:4200          |
| Backend API | https://localhost:7216         |
| Swagger     | https://localhost:7216/swagger |

---

## ✦ Architecture

```
┌─────────────────────────────────────────────────────┐
│                    Angular 18                       │
│              (Client - localhost:4200)              │
└──────────────────────┬──────────────────────────────┘
                       │ HTTP /api/*
┌──────────────────────▼──────────────────────────────┐
│              ASP.NET Core 8 - Controllers           │
│         Result<T> - FluentValidation - Mapperly     │
├─────────────────────────────────────────────────────┤
│                    Services                         │
│   Business logic - State machines - Recalculations  │
├─────────────────────────────────────────────────────┤
│                  Repositories                       │
│    EF Core - AsNoTracking - SQL projections         │
├─────────────────────────────────────────────────────┤
│           PostgreSQL 14 - ApplicationDbContext      │
│    Native enums - Composite indexes - Pool 2-20     │
└─────────────────────────────────────────────────────┘
```

---

## ✦ Documentation

| Document                                   | Content                                                |
| ------------------------------------------ | ------------------------------------------------------ |
| [docs/API.md](docs/API.md)                 | All endpoints, DTOs, and response codes                |
| [docs/DATABASE.md](docs/DATABASE.md)       | ER schema, tables, enums, views, indexes, transactions |
| [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md) | Full local setup, user-secrets, environment variables  |
| [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md)   | Oracle Cloud deployment - nginx - systemd              |

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

