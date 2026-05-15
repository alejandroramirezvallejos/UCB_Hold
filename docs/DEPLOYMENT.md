# 🚀 Despliegue en Producción

> Topología productiva sobre 2 VMs en Oracle Cloud Always Free | Alta disponibilidad sin costos

```
🏗️  Arquitectura:  VM App + VM Database (separadas)
☁️  Proveedor:     Oracle Cloud (Always Free Tier)
🔒 SSL/TLS:       HTTPS via nginx (opcional)
⚡ Performance:    Connection pooling + reverse proxy
📊 Logging:       journalctl + syslog
🔐 Firewall:      Reglas restrictivas por puerto
```

---

## 📋 Tabla de Contenidos

- [Arquitectura](#arquitectura)
- [VM #2: PostgreSQL](#vm-2--postgresql)
- [VM #1: Backend + Frontend](#vm-1--backend--frontend)
- [Logs en Producción](#logs-en-producción)
- [Actualización](#actualización)
- [Troubleshooting](#troubleshooting)

---

## 🏗️ Arquitectura

```
                        🌐 Internet
                           │
                           ▼
                ┌─────────────────────────┐
                │   VM #1 (App Server)    │
                │   Ubuntu 22.04          │
                │                         │
                │  ┌─────────────────┐    │
                │  │  nginx :80/:443 │    │◄── Reverse Proxy
                │  │  (frontal)      │    │
                │  └────────┬────────┘    │
                │           │              │
                │  ┌────────▼──────────┐   │
                │  │  .NET 8 API       │   │◄── systemd service
                │  │  :5000 (interno)  │   │
                │  │  (ASPNETCORE_ENV) │   │
                │  └────────┬──────────┘   │
                └───────────┼──────────────┘
                            │
                  ┌─────────┴─────────┐
                  │ Conexión TCP      │
                  │ :5432 Firewall    │
                  │ (Solo desde VM#1) │
                  │                   │
                  ▼
        ┌──────────────────────┐
        │  VM #2 (DB Server)   │
        │  Ubuntu 22.04        │
        │                      │
        │ PostgreSQL 14        │
        │ :5432 (escucha)      │
        │ IMT_Reservas DB      │
        │                      │
        └──────────────────────┘
```

**Flujo de Requests:**

1. Cliente `https://IP_VM1/api/Usuario` → nginx (port 80/443)
2. nginx → proxy pass `http://localhost:5000/api/Usuario`
3. .NET → connection pool → PostgreSQL `IP_VM2:5432`
4. Response JSON → nginx → Cliente

---

## 💛 VM #2 — PostgreSQL (Configurar Primero)

### 1️⃣ Instalar PostgreSQL

```bash
sudo apt update
sudo apt install -y postgresql postgresql-contrib
sudo systemctl status postgresql
```

### 2️⃣ Habilitar Acceso Remoto

**Archivo:** `/etc/postgresql/14/main/postgresql.conf`

```bash
sudo nano /etc/postgresql/14/main/postgresql.conf
```

Buscar y descomentar:

```ini
listen_addresses = '*'    # de lo contrario: listen_addresses = 'localhost'
```

**Archivo:** `/etc/postgresql/14/main/pg_hba.conf`

Agregar línea al final:

```
host    IMT_Reservas    imt_user    <IP_VM1>/32    md5
```

Reiniciar PostgreSQL:

```bash
sudo systemctl restart postgresql
```

### 3️⃣ Crear Usuario y Base de Datos

```bash
sudo -u postgres psql
```

**En el prompt psql:**

```sql
CREATE USER imt_user WITH PASSWORD '<PASSWORD_FUERTE>';
CREATE DATABASE "IMT_Reservas" OWNER imt_user;
GRANT ALL PRIVILEGES ON DATABASE "IMT_Reservas" TO imt_user;
\q
```

### 4️⃣ Cargar Schema DDL

Desde tu máquina local (o transferir el archivo):

```bash
psql -U imt_user -d IMT_Reservas -h <IP_VM2> -f DataBase/database.ddl
```

O desde VM #2:

```bash
psql -U imt_user -d IMT_Reservas -h localhost -f DataBase/database.ddl
```

### 5️⃣ Verificar Conexión

```bash
psql -U imt_user -d IMT_Reservas -h localhost -c "\dt"
```

Deberías ver las 15 tablas listadas ✅

### 6️⃣ Configurar Firewall Oracle Cloud (VM #2)

**Security List → Ingress Rules:**

| Protocolo | Puerto | Origen        | Descripción           |
| --------- | ------ | ------------- | --------------------- |
| TCP       | 5432   | `<IP_VM1>/32` | PostgreSQL desde VM#1 |

⚠️ **NO abrir** puerto 5432 desde `0.0.0.0/0` — solo desde VM#1

---

## 💛 VM #1 — Backend + Frontend + nginx

### 1️⃣ Instalar Dependencias

```bash
sudo apt update

# nginx — reverse proxy + static files
sudo apt install -y nginx

# .NET Runtime
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update && sudo apt install -y aspnetcore-runtime-8.0
```

Verificar instalación:

```bash
dotnet --version      # debe mostrar 8.0.x
nginx -v              # debe mostrar nginx/1.18.0
```

### 2️⃣ Crear Directorios de Despliegue

```bash
# Backend
sudo mkdir -p /var/www/imt-reservas
sudo chown ubuntu:ubuntu /var/www/imt-reservas

# Frontend estático
sudo mkdir -p /var/www/imt-frontend
sudo chown ubuntu:ubuntu /var/www/imt-frontend

# Permisos
sudo chmod 755 /var/www/imt-*
```

### 3️⃣ Publicar Backend Localmente

En tu máquina de desarrollo:

```bash
cd Code/Server
dotnet clean
dotnet publish -c Release -o ./publish
```

Copiar a VM #1:

```bash
scp -r ./publish/* ubuntu@<IP_VM1>:/var/www/imt-reservas/
```

### 4️⃣ Build y Publicar Frontend

```bash
cd Code/Client
ng build --configuration production --base-href /
scp -r dist/imt-reservas/* ubuntu@<IP_VM1>:/var/www/imt-frontend/
```

### 5️⃣ Crear Archivo de Variables de Entorno

En VM #1:

```bash
sudo vim /etc/imt-reservas.env
```

Contenido:

```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://localhost:5000
ConnectionStrings__PostgreSQL=Host=<IP_VM2>;Port=5432;Database=IMT_Reservas;Username=imt_user;Password=<PASSWORD>;Pooling=true;MinPoolSize=2;MaxPoolSize=20
AllowedOrigins__0=http://<IP_VM1>
AllowedOrigins__1=https://<IP_VM1>
```

**Permisos:**

```bash
sudo chmod 600 /etc/imt-reservas.env
sudo chown root:root /etc/imt-reservas.env
```

> 💛 **Parámetros:**
>
> - `MinPoolSize=2` — precalienta 2 conexiones (evita cold-start de ~8s)
> - `MaxPoolSize=20` — límite de conexiones abiertas
> - `ASPNETCORE_ENVIRONMENT=Production` — desactiva Swagger

### 6️⃣ Crear Servicio systemd

Archivo: `/etc/systemd/system/imt-reservas.service`

```ini
[Unit]
Description=IMT Reservas API (.NET 8)
After=network.target postgresql.service
Documentation=https://github.com/tuorganizacion/imt-reservas

[Service]
Type=simple
WorkingDirectory=/var/www/imt-reservas
ExecStart=/usr/bin/dotnet /var/www/imt-reservas/IMT_Reservas.Server.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
StandardOutput=journal
StandardError=journal
SyslogIdentifier=imt-reservas
User=www-data
Group=www-data
EnvironmentFile=/etc/imt-reservas.env

# Límites de recursos
LimitNOFILE=65535
LimitNPROC=65535

[Install]
WantedBy=multi-user.target
```

Activar servicio:

```bash
sudo systemctl daemon-reload
sudo systemctl enable imt-reservas
sudo systemctl start imt-reservas
sudo systemctl status imt-reservas
```

### 7️⃣ Configurar nginx

Archivo: `/etc/nginx/sites-available/imt-reservas`

```nginx
upstream api_backend {
    server localhost:5000;
    keepalive 32;
}

server {
    listen 80 default_server;
    listen [::]:80 default_server;
    server_name _;

    # Frontend estático
    root /var/www/imt-frontend;
    index index.html;

    # Logs
    access_log /var/log/nginx/imt-reservas-access.log;
    error_log /var/log/nginx/imt-reservas-error.log;

    # SPA routing (Angular)
    location / {
        try_files $uri $uri/ /index.html;
        expires 1h;
        add_header Cache-Control "public, immutable";
    }

    # Assets estáticos (cache largo)
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 30d;
        add_header Cache-Control "public, immutable";
    }

    # API proxy
    location /api/ {
        proxy_pass http://api_backend;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header Connection "";

        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }

    # Health check (opcional)
    location /health {
        proxy_pass http://api_backend/health;
        access_log off;
    }
}
```

Validar y recargar:

```bash
sudo nginx -t
sudo ln -s /etc/nginx/sites-available/imt-reservas /etc/nginx/sites-enabled/
sudo rm -f /etc/nginx/sites-enabled/default
sudo systemctl reload nginx
```

### 8️⃣ Configurar Firewall Oracle Cloud (VM #1)

**Security List → Ingress Rules:**

| Protocolo | Puerto | Origen      | Descripción               |
| --------- | ------ | ----------- | ------------------------- |
| TCP       | 80     | `0.0.0.0/0` | HTTP público              |
| TCP       | 443    | `0.0.0.0/0` | HTTPS público (si aplica) |

---

## 📊 Logs en Producción

### Ver logs en vivo

```bash
sudo journalctl -u imt-reservas -f              # real-time tail
sudo journalctl -u imt-reservas --since today    # solo hoy
sudo journalctl -u imt-reservas -n 100           # últimas 100 líneas
sudo journalctl -u imt-reservas -S -1h           # últimas 1 hora
```

### Logs de nginx

```bash
sudo tail -f /var/log/nginx/imt-reservas-access.log
sudo tail -f /var/log/nginx/imt-reservas-error.log
```

### Rotación de logs

Automática vía `logrotate` (configurado en `appsettings.Production.json`):

```bash
sudo cat /etc/logrotate.d/nginx
```

> 💛 En Production: `LogLevel: Warning` — solo errores importantes
> Swagger deshabilitado automáticamente (`app.Environment.IsDevelopment()`)

---

## 🔄 Actualización de Código

### Script de Despliegue Automatizado

En tu máquina local, crear `deploy.sh`:

```bash
#!/bin/bash
set -e

VM_IP="<IP_VM1>"
VM_USER="ubuntu"

# 1. Build local
echo "📦 Building backend..."
cd Code/Server && dotnet publish -c Release -o ./publish
echo "📦 Building frontend..."
cd ../Client && ng build --configuration production

# 2. Deploy
echo "🚀 Deploying to VM#1..."
ssh $VM_USER@$VM_IP "sudo systemctl stop imt-reservas"

scp -r Code/Server/publish/* $VM_USER@$VM_IP:/var/www/imt-reservas/
scp -r Code/Client/dist/imt-reservas/* $VM_USER@$VM_IP:/var/www/imt-frontend/

ssh $VM_USER@$VM_IP "sudo systemctl start imt-reservas"
ssh $VM_USER@$VM_IP "sudo systemctl reload nginx"

# 3. Verify
echo "✅ Verifying deployment..."
sleep 5
curl -s http://$VM_IP/api/Usuario | head -c 100
echo ""
```

Usar:

```bash
chmod +x deploy.sh
./deploy.sh
```

### Despliegue manual paso a paso

```bash
# 1. Compilar en desarrollo
cd Code/Server && dotnet publish -c Release -o ./publish
cd Code/Client && ng build --configuration production

# 2. Conectar a VM y detener servicio
ssh ubuntu@<IP_VM1>
sudo systemctl stop imt-reservas

# 3. Desplegar archivos
exit  # salir de SSH
scp -r Code/Server/publish/* ubuntu@<IP_VM1>:/var/www/imt-reservas/
scp -r Code/Client/dist/imt-reservas/* ubuntu@<IP_VM1>:/var/www/imt-frontend/

# 4. Reiniciar servicios
ssh ubuntu@<IP_VM1>
sudo systemctl start imt-reservas
sudo systemctl reload nginx
sudo systemctl status imt-reservas
exit
```

---

## 🔧 Troubleshooting

### Servicio no inicia

```bash
sudo systemctl status imt-reservas        # ver estado
sudo journalctl -u imt-reservas -n 50     # últimos 50 logs
sudo systemctl restart imt-reservas       # reintentar
```

### Conexión a PostgreSQL fallando

```bash
# Desde VM#1, verificar conectividad
psql -U imt_user -d IMT_Reservas -h <IP_VM2> -c "SELECT version();"

# Si falla, revisar PostgreSQL en VM#2
sudo systemctl status postgresql
sudo tail -f /var/log/postgresql/postgresql.log
```

### nginx retornando 502 Bad Gateway

```bash
# Verificar que .NET esté escuchando
sudo netstat -tlnp | grep 5000

# Revisar logs de .NET
sudo journalctl -u imt-reservas | tail -50

# Reiniciar servicio
sudo systemctl restart imt-reservas
```

### Performance lento

```bash
# Verificar pool de conexiones
sudo systemctl status imt-reservas
ps aux | grep dotnet

# Monitorear recursos
top
df -h          # espacio en disco
free -h        # memoria RAM
```

---

## 📈 Monitoreo Recomendado

- ✅ Habilitar alertas en Oracle Cloud Console
- ✅ Logs centralizados (opcional: Papertrail, Loki)
- ✅ Health check endpoint: `GET /api/health`
- ✅ Backups automáticos de PostgreSQL cada 24h
- 🚫 No deshabilitar firewall — más restrictivo es mejor

---
