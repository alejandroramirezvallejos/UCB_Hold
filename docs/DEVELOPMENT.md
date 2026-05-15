# 💛 Desarrollo Local

> Guía para levantar el entorno completo en Windows, macOS o Linux | Full-Stack .NET 8 + Angular 18

```
🎯 Stack:        .NET 8 + Angular 18 + PostgreSQL 14+
🛠️  IDE:         JetBrains Rider (recomendado) o VS Code
💾 Base de datos: PostgreSQL local o Docker
📦 Runtime:      .NET SDK 8.0+, Node 18+
⏱️  Tiempo setup: ~10-15 minutos (con Docker)
```

---

## 📋 Tabla de Contenidos

- [Requisitos Previos](#requisitos-previos)
- [1. Base de Datos](#1-base-de-datos)
- [2. Credenciales Seguras](#2-credenciales-seguras)
- [3. Dependencias Frontend](#3-dependencias-frontend)
- [4. Ejecutar Proyecto](#4-ejecutar-proyecto)
- [5. Variables de Entorno](#5-variables-de-entorno)
- [6. Build y Tests](#6-build-y-tests)
- [7. Stack Tecnológico](#7-stack-tecnológico)
- [Troubleshooting](#troubleshooting)

---

## 🔧 Requisitos Previos

| Herramienta         | Versión mínima | Descarga                                                      | Verificación                  |
| ------------------- | -------------- | ------------------------------------------------------------- | ----------------------------- |
| **.NET SDK**        | 8.0 LTS        | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) | `dotnet --version`            |
| **Node.js**         | 18 LTS         | [nodejs.org](https://nodejs.org)                              | `node -v && npm -v`           |
| **PostgreSQL**      | 14             | [postgresql.org](https://www.postgresql.org/download/)        | `psql --version`              |
| **Angular CLI**     | 18             | `npm install -g @angular/cli@18`                              | `ng version`                  |
| **Git**             | Última         | [git-scm.com](https://git-scm.com)                            | `git --version`               |
| **JetBrains Rider** | Última         | [jetbrains.com](https://www.jetbrains.com/rider/)             | _(Opcional pero recomendado)_ |

**Verificar instalación rápida:**

```bash
dotnet --version          # >= 8.0
node --version            # >= 18.0
npm --version             # >= 9.0
psql --version            # >= 14
git --version             # >= 2.30
ng version                # >= 18.0
```

---

## 1️⃣ Base de Datos

### Opción A — PostgreSQL Nativa (recomendado en macOS/Linux)

```bash
# Crear base de datos
psql -U postgres -c "CREATE DATABASE IMT_Reservas;"

# Cargar schema DDL
psql -U postgres -d IMT_Reservas -f DataBase/database.ddl

# Verificar
psql -U postgres -d IMT_Reservas -c "\dt"
```

Deberías ver 15 tablas listadas ✅

### Opción B — Docker (recomendado en Windows)

```bash
# Descargar imagen y levantar contenedor
docker run -d \
  --name ucbhold-postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=IMT_Reservas \
  -p 5432:5432 \
  -v ucbhold-pgdata:/var/lib/postgresql/data \
  postgres:14

# Cargar schema
docker exec -i ucbhold-postgres \
  psql -U postgres -d IMT_Reservas < DataBase/database.ddl

# Verificar
docker exec ucbhold-postgres \
  psql -U postgres -d IMT_Reservas -c "\dt"
```

**Detener contenedor:**

```bash
docker stop ucbhold-postgres
docker rm ucbhold-postgres    # eliminar permanentemente
```

> 💛 Con Docker, los datos persisten en el volumen `ucbhold-pgdata` incluso reiniciando

---

## 2️⃣ Credenciales Seguras

Las credenciales **NO se commitean**. Usamos `dotnet user-secrets` para almacenarlas encriptadas localmente.

### Inicializar User Secrets

```bash
cd Code/Server
dotnet user-secrets init    # crea archivo oculto en %APPDATA%\...
```

### Configurar Connection String

```bash
dotnet user-secrets set "ConnectionStrings:PostgreSQL" \
  "Host=localhost;Port=5432;Database=IMT_Reservas;Username=postgres;Password=postgres;Pooling=true;MinPoolSize=2;MaxPoolSize=20"
```

### Verificar Secrets Guardados

```bash
dotnet user-secrets list
```

Deberías ver:

```
ConnectionStrings:PostgreSQL = Host=localhost;...
```

### ¿Por qué estos parámetros?

| Parámetro        | Valor         | Razón                                 |
| ---------------- | ------------- | ------------------------------------- |
| `MinPoolSize=2`  | 2 conexiones  | Precalienta — evita cold-start de ~8s |
| `MaxPoolSize=20` | 20 conexiones | Límite razonable para desarrollo      |
| `Pooling=true`   | Sí            | Reutiliza conexiones TCP              |

---

## 3️⃣ Dependencias Frontend

```bash
cd Code/Client
npm install    # instala desde package.json (~350MB, toma ~2min)
npm list       # verifica dependencias
```

Principales dependencias:

- `@angular/*` — framework
- `typescript` — lenguaje
- `rxjs` — programación reactiva
- `bootstrap`, `ng-bootstrap` — UI
- `axios` — HTTP client (si aplica)

---

## 4️⃣ Ejecutar Proyecto

### Opción A — JetBrains Rider (Recomendado)

1. **Abrir proyecto:**
   - File → Open → seleccionar carpeta `Code/`

2. **Seleccionar Run Configuration:**
   - Dropdown superior derecha → `IMT_Reservas.FullStack`
3. **Ejecutar:**
   - Botón verde ▶️ o `Shift+F10`

**Run Configs disponibles:**

| Config                   | Descripción                      |
| ------------------------ | -------------------------------- |
| `IMT_Reservas.FullStack` | Backend + Frontend simultáneo ⭐ |
| `IMT_Reservas.Server`    | Solo API .NET                    |
| `IMT_Reservas.Client`    | Solo Angular (modo watch)        |

> Rider inyecta automáticamente `ASPNETCORE_ENVIRONMENT=Development`

### Opción B — CLI (Dos Terminales)

**Terminal 1 — Backend:**

```bash
cd Code/Server
dotnet run
```

Esperado:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
```

**Terminal 2 — Frontend:**

```bash
cd Code/Client
npm start
```

Esperado:

```
✔ Browser application bundle generation complete.
Local: http://localhost:4200
```

---

## 🌐 URLs Locales

| Servicio         | URL                              | Función                   |
| ---------------- | -------------------------------- | ------------------------- |
| **Frontend**     | `http://localhost:4200`          | Interfaz Angular          |
| **Backend API**  | `https://localhost:7001`         | REST API .NET             |
| **Swagger UI**   | `https://localhost:7001/swagger` | Documentación interactiva |
| **Health Check** | `https://localhost:7001/health`  | Estado de API             |

**Nota:** Swagger solo está disponible en `Development` — se deshabilita en `Production`

---

## 5️⃣ Variables de Entorno

### Desarrollo Local

La configuración se hereda automáticamente:

```json
{
  "Logging": { "LogLevel": { "Default": "Debug" } },
  "AllowedOrigins": ["http://localhost:4200", "https://localhost:4200"],
  "ASPNETCORE_ENVIRONMENT": "Development"
}
```

**No necesitas configurar nada adicional** — funciona out-of-the-box.

### Producción

Se configura vía `/etc/imt-reservas.env` en la VM (ver [DEPLOYMENT.md](DEPLOYMENT.md))

---

## 6️⃣ Build y Tests

### Compilar Backend

```bash
cd Code/Server

# Debug build (rápido)
dotnet build

# Release build (optimizado)
dotnet build -c Release

# Debe terminar sin errores
# ❌ Error CS1234... → revisar archivo
# ✅ Build succeeded
```

### Tests del Backend

```bash
cd Code/Tests

# Ejecutar todos
dotnet test

# Ejecutar con pattern
dotnet test --filter "Usuario"

# Ver cobertura
dotnet test /p:CollectCoverage=true
```

**Tests disponibles:**

- `ControllerTests/` — Endpoints REST
- `RepositoryTests/` — Acceso a datos
- `ServiceTests/` — Lógica de negocio

### Build Frontend

```bash
cd Code/Client

# Dev build (rápido, sin minify)
ng build

# Prod build (optimizado, minificado)
ng build --configuration production

# Watch mode (recarga automática)
ng build --watch
```

### Tests del Frontend

```bash
cd Code/Client

# Ejecutar tests
ng test

# Con cobertura
ng test --code-coverage

# Modo headless (CI)
ng test --watch=false --browsers=ChromeHeadless
```

---

## 📚 Stack Tecnológico

### Backend (.NET 8)

| Paquete                                   | Versión | Propósito                               | Enlace                                                              |
| ----------------------------------------- | ------- | --------------------------------------- | ------------------------------------------------------------------- |
| **Microsoft.EntityFrameworkCore**         | 8.0.4   | ORM (Object-Relational Mapping)         | [docs](https://learn.microsoft.com/ef/)                             |
| **Npgsql.EntityFrameworkCore.PostgreSQL** | 8.0.4   | Driver PostgreSQL para EF Core          | [docs](https://www.npgsql.org/)                                     |
| **Riok.Mapperly**                         | 3.6.0   | DTO ↔ Entity mapping (source-generated) | [github](https://github.com/riok/mapperly)                          |
| **FluentValidation**                      | 11.9.0  | Validación fluida de entradas           | [docs](https://fluentvalidation.net/)                               |
| **Ardalis.Result**                        | 10.1.0  | Result pattern (sin try-catch)          | [github](https://github.com/ardalis/Result)                         |
| **BCrypt.Net-Next**                       | 4.0.3   | Hash seguro de contraseñas              | [nuget](https://www.nuget.org/packages/BCrypt.Net-Next/)            |
| **Swashbuckle.AspNetCore**                | 6.6.2   | Swagger/OpenAPI (Development only)      | [github](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) |

### Frontend (Angular 18)

| Paquete                | Versión | Propósito                          |
| ---------------------- | ------- | ---------------------------------- |
| `@angular/core`        | 18.x    | Framework principal                |
| `@angular/router`      | 18.x    | Routing SPA                        |
| `@angular/forms`       | 18.x    | Formularios reactivos              |
| `@angular/common/http` | 18.x    | Cliente HTTP                       |
| `rxjs`                 | 7.x     | Programación reactiva              |
| `bootstrap`            | 5.x     | Estilos CSS                        |
| `ng-bootstrap`         | 14.x    | Componentes Bootstrap para Angular |
| `typescript`           | 5.4.x   | Lenguaje tipado                    |

### Base de Datos

| Componente | Versión | Rol                              |
| ---------- | ------- | -------------------------------- |
| PostgreSQL | 14+     | Base de datos relacional         |
| pgAdmin    | 8.x     | Administración visual (opcional) |

---

## 🐛 Troubleshooting

### Error: "dotnet: command not found"

```bash
# Instalar .NET SDK
# Windows: https://dotnet.microsoft.com/download
# macOS: brew install dotnet-sdk
# Linux: https://learn.microsoft.com/en-us/dotnet/core/install/linux

dotnet --version    # verificar
```

### Error: "npm: command not found"

```bash
# Instalar Node.js (incluye npm)
# https://nodejs.org

node -v
npm -v
```

### Error: "Connection refused" en PostgreSQL

```bash
# Verificar si PostgreSQL está corriendo
psql -U postgres -c "SELECT 1"

# Si no está:
# macOS: brew services start postgresql
# Linux: sudo systemctl start postgresql
# Docker: docker start ucbhold-postgres
# Windows: Services → PostgreSQL → Start
```

### Error: "User Secrets not initialized"

```bash
cd Code/Server
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:PostgreSQL" "..."
```

### Error: "Port 4200 already in use" (Angular)

```bash
# Cambiar puerto
ng serve --port 4300

# O matar proceso
# Linux/macOS: lsof -i :4200
# Windows: netstat -ano | findstr :4200
```

### Error: "Port 7001 already in use" (.NET)

```bash
# Cambiar puerto
dotnet run --urls "https://localhost:7002"
```

### Error: "Cannot find module '@angular/...'

```bash
cd Code/Client
npm install
npm list    # verificar todas las dependencias
```

---

## 💡 Tips & Tricks

- ✅ Usar **Rider** para debugging avanzado — breakpoints, watches, etc.
- ✅ Activar **Hot Reload** — `dotnet watch run` en .NET 6+
- ✅ Usar **Postman** o **Insomnia** para testear API
- ✅ Limpiar caché: `npm cache clean --force` y `dotnet clean`
- ✅ Rescanear solución: Rider → File → Invalidate Caches
- 🚫 No commitear `appsettings.*.json` con secretos
- 🚫 No commitear `/node_modules/` o `/bin/obj/`

---

## 🚀 Siguiente Paso

Cuando estés listo para desplegar:
→ Ver [DEPLOYMENT.md](DEPLOYMENT.md) para instrucciones de producción

---
