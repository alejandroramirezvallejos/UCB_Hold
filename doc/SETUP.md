# Setup

<div align="center">

[![Typing SVG](https://readme-typing-svg.demolab.com?font=Fira+Code&size=13&pause=1000&color=6B7786&center=true&vCenter=true&width=480&lines=docker+compose+up+--build;dotnet+run+%2B+npm+start;Press+Run+in+Rider)](https://git.io/typing-svg)

</div>

Full-stack local environment: .NET 8 · Angular 18 · PostgreSQL 14 · Redis 7

---

<h2><img height="20" src="../Images/icons/prerequisites.svg">&nbsp;&nbsp;Prerequisites</h2>

<div align="center">

[![My Skills](https://skillicons.dev/icons?i=dotnet,nodejs,docker,git)](https://skillicons.dev)

</div>

| Tool     | Min version | Install                                                       | Verify             |
| -------- | ----------- | ------------------------------------------------------------- | ------------------ |
| .NET SDK | 8.0 LTS     | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) | `dotnet --version` |
| Node.js  | 18 LTS      | [nodejs.org](https://nodejs.org)                              | `node -v`          |
| Docker   | any         | [docker.com](https://www.docker.com/products/docker-desktop/) | `docker -v`        |
| Git      | any         | [git-scm.com](https://git-scm.com)                            | `git --version`    |

---

<h2><img height="20" src="../Images/icons/setup.svg">&nbsp;&nbsp;One-time setup</h2>

### Docker mode

Create `Code/server.env` — gitignored, never commit it:

```ini
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80
ConnectionStrings__PostgreSQL=Host=ucb_db;Port=5432;Database=IMT_Reservas;Username=postgres;Password=postgres;Pooling=true;MinPoolSize=2;MaxPoolSize=20
Jwt__Key=<KEY>
Redis__ConnectionString=ucb_redis:6379
```

> `Jwt__Key` must be at least 32 characters. Generate one with `openssl rand -base64 32`.

### Rider / Terminal mode

Backend credentials via [dotnet user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):

```bash
cd Code/Server
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:PostgreSQL" \
  "Host=localhost;Port=5432;Database=IMT_Reservas;Username=postgres;Password=postgres;Pooling=true;MinPoolSize=2;MaxPoolSize=20"
dotnet user-secrets set "Jwt:Key" "local_dev_secret_at_least_32_chars!!"
dotnet user-secrets set "Redis:ConnectionString" "localhost:6379"
```

Frontend dependencies:

```bash
cd Code/Client
npm install
```

---

<h2><img height="20" src="../Images/icons/running.svg">&nbsp;&nbsp;Running the project</h2>

### 1. Docker

```bash
cd Code
docker compose up --build
```

| Service     | URL                   |
| ----------- | --------------------- |
| Frontend    | http://localhost:4200 |
| Backend API | http://localhost:5000 |
| PostgreSQL  | localhost:5432        |
| Redis       | localhost:6379        |

```bash
docker compose down        # stop, keep volumes
docker compose down -v     # stop, wipe DB data
docker logs -f ucb_server  # backend logs
```

### 2. Rider

1. Open folder `Code/`
2. Select run configuration `IMT_Reservas.FullStack`
3. Press Run (`Shift+F10`)

DB and Redis start automatically before the backend via the `IMT_Reservas.Database` before-launch step.

### 3. Two terminals

Start infrastructure first:

```bash
cd Code
docker compose up -d ucb_db ucb_redis
```

Then in two terminals:

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

<h2><img height="20" src="../Images/icons/tests.svg">&nbsp;&nbsp;Tests</h2>

All tests run against an **EF Core InMemory** database — no PostgreSQL required.

```bash
dotnet test Code/Tests/IMT_Reservas.Tests.csproj
```

Tests also run automatically on every push to `main` and `develop` via GitHub Actions. A browsable HTML coverage report is uploaded as the `coverage` artifact.

---

<h2><img height="20" src="../Images/icons/troubleshooting.svg">&nbsp;&nbsp;Troubleshooting</h2>

**`dotnet: command not found`** — install .NET 8 SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)

**`Connection refused` on PostgreSQL or Redis**

```bash
cd Code && docker compose up -d ucb_db ucb_redis
docker ps --format "table {{.Names}}\t{{.Status}}"
```

**`User Secrets not initialized`**

```bash
cd Code/Server && dotnet user-secrets init
```

**Docker — backend container restarting**

```bash
docker logs ucb_server
```

**Port 4200 in use**

```bash
ng serve --port 4300
```

**`Cannot find module '@angular/...'`**

```bash
cd Code/Client && npm install
```
