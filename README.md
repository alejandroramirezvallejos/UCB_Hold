<div align="center">

<img src="Code/Client/public/assets/logo_universidad.png" alt="UCB Hold" width="160" />

# UCB Hold

**Sistema de Reservas — Laboratorio de Mecatrónica UCB**

Plataforma web para gestión de préstamos de equipos, accesorios y herramientas del laboratorio IMT de la Universidad Católica Boliviana.

[![.NET](https://img.shields.io/badge/.NET-8.0-yellow?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-18-yellow?style=flat-square&logo=angular)](https://angular.io/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-14-yellow?style=flat-square&logo=postgresql)](https://www.postgresql.org/)
[![EF Core](https://img.shields.io/badge/EF_Core-8.0-yellow?style=flat-square&logo=nuget)](https://learn.microsoft.com/en-us/ef/core/)
[![Mapperly](https://img.shields.io/badge/Mapperly-3.6-yellow?style=flat-square)](https://mapperly.riok.app/)
[![FluentValidation](https://img.shields.io/badge/FluentValidation-11.9-yellow?style=flat-square)](https://fluentvalidation.net/)

</div>

---

## ✦ Características

| | |
|---|---|
| 📦 **Gestión de equipos** | Inventario con grupos, gaveteros y muebles. Código IMT, estado y costo promedio calculados automáticamente. |
| 📋 **Préstamos con contrato** | Flujo completo: solicitud → aprobación → entrega → devolución. Generación de contrato HTML adjunto. |
| 📅 **Disponibilidad en tiempo real** | Carrito de reservas que calcula unidades disponibles por grupo por día en un rango de fechas. |
| 🔧 **Mantenimientos** | Registro de mantenimientos correctivos y preventivos por empresa externa con detalles por equipo. |
| 🗂️ **Catálogos completos** | Carreras, categorías, accesorios, componentes, empresas y muebles con CRUD completo. |
| ♻️ **Borrado lógico** | Ningún dato se elimina físicamente. `estado_eliminado = true` con cascade automático a detalles. |
| ⚡ **Queries optimizadas** | Single SQL projection con LEFT JOINs en todos los repositorios. `AsNoTracking` + índices compuestos. |

---

## ✦ Stack

| Capa | Tecnología | Versión |
|------|-----------|---------|
| Frontend | Angular | 18 |
| Backend | ASP.NET Core | .NET 8 |
| ORM | Entity Framework Core + Npgsql | 8.0.4 |
| Base de datos | PostgreSQL | 14+ |
| Mapeo | Riok.Mapperly (source-generated) | 3.6.0 |
| Validación | FluentValidation | 11.9.0 |
| Result pattern | Ardalis.Result | 10.1.0 |
| Passwords | BCrypt.Net-Next | 4.0.3 |

---

## ✦ Quick Start

### Prerequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org)
- [PostgreSQL 14+](https://www.postgresql.org/download/) — nativo **o** Docker

### 1 · Base de datos

**Nativo:**
```bash
psql -U postgres -c "CREATE DATABASE IMT_Reservas;"
psql -U postgres -d IMT_Reservas -f DataBase/database.ddl
```

**Docker:**
```bash
docker run -d --name ucbhold-postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=IMT_Reservas \
  -p 5432:5432 \
  postgres:14

docker exec -i ucbhold-postgres psql -U postgres -d IMT_Reservas < DataBase/database.ddl
```

### 2 · Backend — credenciales locales

```bash
cd Code/Server
dotnet user-secrets set "ConnectionStrings:PostgreSQL" \
  "Host=localhost;Port=5432;Database=IMT_Reservas;Username=postgres;Password=postgres;Pooling=true;MinPoolSize=2;MaxPoolSize=20"
```

### 3 · Frontend — dependencias

```bash
cd Code/Client
npm install
```

### 4 · Ejecutar

**JetBrains Rider** — abrir `Code/` → config `IMT_Reservas.FullStack` → Run.

**CLI** — dos terminales:
```bash
# Terminal 1
cd Code/Server && dotnet run

# Terminal 2
cd Code/Client && npm start
```

| Servicio | URL |
|---------|-----|
| Frontend | http://localhost:4200 |
| Backend API | https://localhost:7001 |
| Swagger | https://localhost:7001/swagger |

---

## ✦ Arquitectura

```
┌─────────────────────────────────────────────────────┐
│                    Angular 18                       │
│              (Client — localhost:4200)              │
└──────────────────────┬──────────────────────────────┘
                       │ HTTP /api/*
┌──────────────────────▼──────────────────────────────┐
│              ASP.NET Core 8 — Controllers           │
│         Result<T> · FluentValidation · Mapperly     │
├─────────────────────────────────────────────────────┤
│                    Services                         │
│   Business logic · State machines · Recálculos      │
├─────────────────────────────────────────────────────┤
│                  Repositories                       │
│    EF Core · AsNoTracking · SQL projections         │
├─────────────────────────────────────────────────────┤
│           PostgreSQL 14 — ApplicationDbContext      │
│    Enums nativos · Índices compuestos · Pool 2-20   │
└─────────────────────────────────────────────────────┘
```

---

## ✦ Documentación

| Documento | Contenido |
|-----------|-----------|
| [docs/API.md](docs/API.md) | Todos los endpoints, DTOs, códigos de respuesta |
| [docs/DATABASE.md](docs/DATABASE.md) | Esquema ER, tablas, enums, vistas, índices, transacciones |
| [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md) | Setup local completo, user-secrets, variables de entorno |
| [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md) | Despliegue Oracle Cloud · nginx · systemd |

---

## ✦ Equipo

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
