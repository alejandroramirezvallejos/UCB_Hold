# UCB Hold — Sistema de Reservas IMT

Sistema fullstack para gestión de préstamos de equipos del laboratorio de Mecatrónica UCB.

**Stack:** Angular 18 · .NET 8 · PostgreSQL 14 · Entity Framework Core

---

## Quick Start

### Prerequisitos
- .NET 8 SDK
- Node 18+
- PostgreSQL 14+
- JetBrains Rider (recomendado) o Visual Studio / VS Code

### Setup
```bash
psql -U postgres -f DataBase/database.ddl

cd Code/Server
dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=localhost;Port=5432;Database=IMT_Reservas;Username=postgres;Password=postgres;Pooling=true;MinPoolSize=2;MaxPoolSize=20"

cd ../Client
npm install
```

### Ejecutar
**Rider:** abrir `Code/` → run config `IMT_Reservas.FullStack` (compound: backend + frontend).

**CLI:**
```bash
cd Code/Server && dotnet run
cd Code/Client && npm start
```

Swagger en `https://localhost:{puerto}/swagger` (solo Development).

---

## Documentación

| Documento | Contenido |
|-----------|-----------|
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | Capas, patrones, decisiones de diseño |
| [docs/DATABASE.md](docs/DATABASE.md) | Esquema, triggers, vistas, índices |
| [docs/API.md](docs/API.md) | Endpoints disponibles y formato de respuesta |
| [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md) | Setup local, user-secrets, convenciones |
| [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md) | Despliegue Oracle Cloud + nginx |

---

## Miembros

- [Josue Galo Balbontin Ugarteche](https://github.com/josue-balbontin)
- [Alejandro Ramirez Vallejos](https://github.com/alejandroramirezvallejos)
- [Fernando Terrazas Llanos](https://github.com/FernandoTerrazasLl)
