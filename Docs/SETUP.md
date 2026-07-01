<div align="center">

# Setup

Ambiente local de UCB Hold: `.NET 8`, `Angular 19.2`, `PostgreSQL 14+`, `Redis 7` y `Docker Compose`.

[Volver al README](../README.md) · [API](API.md) · [Base de datos](DATABASE.md)

</div>

---

## <img height="22" src="../Images/readme-icons/prerequisites.svg" alt="" /> Requisitos

| Herramienta    | Versión mínima | Verificar          |
| -------------- | -------------- | ------------------ |
| .NET SDK       | 8.0 LTS        | `dotnet --version` |
| Node.js        | 18.19+         | `node -v`          |
| Docker Desktop | Actual         | `docker -v`        |
| Git            | Actual         | `git --version`    |

---

## <img height="22" src="../Images/readme-icons/setup.svg" alt="" /> Configuración Inicial

### Docker

Crear `Code/server.env`. Este archivo está ignorado por Git y no debe versionarse:

```ini
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80
ConnectionStrings__PostgreSQL=Host=ucb_db;Port=5432;Database=IMT_Reservas;Username=postgres;Password=postgres;Pooling=true;MinPoolSize=2;MaxPoolSize=20
Jwt__Key=<clave-local-de-32-caracteres-o-mas>
Redis__ConnectionString=ucb_redis:6379
```

Generar una clave local:

```bash
openssl rand -base64 32
```

### Rider o Terminal

Configurar secretos del backend:

```bash
cd Code/Server
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=localhost;Port=5432;Database=IMT_Reservas;Username=postgres;Password=postgres;Pooling=true;MinPoolSize=2;MaxPoolSize=20"
dotnet user-secrets set "Jwt:Key" "local_dev_secret_at_least_32_chars!!"
dotnet user-secrets set "Redis:ConnectionString" "localhost:6379"
```

Instalar dependencias del frontend:

```bash
cd Code/Client
npm install
```

---

## <img height="22" src="../Images/readme-icons/running.svg" alt="" /> Ejecución

### Opción 1: Docker

```bash
cd Code
docker compose up --build
```

| Servicio    | URL                   |
| ----------- | --------------------- |
| Frontend    | http://localhost:4200 |
| Backend API | http://localhost:5000 |
| PostgreSQL  | localhost:5432        |
| Redis       | localhost:6379        |

Comandos útiles:

```bash
docker compose down
docker compose down -v
docker logs -f ucb_server
```

### Opción 2: Rider

1. Abrir la carpeta `Code/`.
2. Seleccionar la configuración `IMT_Reservas.FullStack`.
3. Ejecutar con `Shift+F10`.

La base de datos y Redis se levantan como pasos previos desde la configuración del IDE.

### Opción 3: Dos Terminales

```bash
cd Code
docker compose up -d ucb_db ucb_redis
```

Backend:

```bash
cd Code/Server
dotnet run
```

Frontend:

```bash
cd Code/Client
npm start
```

| Servicio    | URL                            |
| ----------- | ------------------------------ |
| Frontend    | http://localhost:4200          |
| Backend API | https://localhost:7216         |
| Swagger     | https://localhost:7216/swagger |

---

## <img height="22" src="../Images/readme-icons/verification.svg" alt="" /> Verificación

Backend:

```bash
dotnet test Code/Tests/IMT_Reservas.Tests.csproj
```

Frontend:

```bash
cd Code/Client
npm run format:check
npx tsc -p tsconfig.app.json --noEmit
npx tsc -p tsconfig.spec.json --noEmit
npm run test:coverage
npm run build
```

---

## <img height="22" src="../Images/readme-icons/troubleshooting.svg" alt="" /> Solución de Problemas

| Problema                            | Acción                                                       |
| ----------------------------------- | ------------------------------------------------------------ |
| `dotnet: command not found`         | Instalar .NET 8 SDK desde `dotnet.microsoft.com`.            |
| PostgreSQL o Redis rechaza conexión | Ejecutar `cd Code && docker compose up -d ucb_db ucb_redis`. |
| `User Secrets not initialized`      | Ejecutar `cd Code/Server && dotnet user-secrets init`.       |
| Backend en Docker reinicia          | Revisar `docker logs ucb_server`.                            |
| Puerto `4200` ocupado               | Ejecutar `ng serve --port 4300`.                             |
| Faltan paquetes Angular             | Ejecutar `cd Code/Client && npm install`.                    |
