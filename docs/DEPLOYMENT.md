# Despliegue 

Topología productiva sobre 2 VMs en Oracle Cloud Always Free.

---

## Arquitectura

```
                    Internet
                       │
                       ▼
              ┌─────────────────┐
              │   VM #1 (App)   │
              │                 │
              │  ┌───────────┐  │
              │  │  nginx    │  │  ←─ reverse proxy :80/:443
              │  │           │  │     sirve Angular estático
              │  └─────┬─────┘  │     proxy /api/* → backend
              │        │        │
              │        ▼        │
              │  ┌───────────┐  │
              │  │ .NET API  │  │  ←─ ASPNETCORE_ENVIRONMENT=Production
              │  │  :5000    │  │
              │  └─────┬─────┘  │
              └────────┼────────┘
                       │
                       ▼
              ┌─────────────────┐
              │   VM #2 (DB)    │
              │   PostgreSQL    │
              │   :5432         │
              └─────────────────┘
```

Frontend llama `http://<IP_VM1>/api/...` — nginx escucha en :80 y hace proxy al backend en :5000.

---

## VM #2 — PostgreSQL (configurar primero)

### 1. Instalar

```bash
sudo apt update
sudo apt install -y postgresql postgresql-contrib
```

### 2. Acceso remoto

`/etc/postgresql/14/main/postgresql.conf`:
```
listen_addresses = '*'
```

`/etc/postgresql/14/main/pg_hba.conf` (agregar línea):
```
host    IMT_Reservas    imt_user    <IP_VM1>/32    md5
```

```bash
sudo systemctl restart postgresql
```

### 3. Crear usuario y base de datos

```bash
sudo -u postgres psql
```
```sql
CREATE USER imt_user WITH PASSWORD '<password>';
CREATE DATABASE "IMT_Reservas" OWNER imt_user;
\q
```

### 4. Cargar schema

```bash
psql -U imt_user -d IMT_Reservas -h localhost -f DataBase/database.ddl
```

### 5. Firewall Oracle Cloud

- VM #2: abrir puerto `5432` **solo** desde la IP de VM #1 (Security List → Ingress Rule)
- VM #1: abrir puertos `80` y `443` desde `0.0.0.0/0`

---

## VM #1 — Backend + Frontend

### 1. Instalar dependencias

```bash
sudo apt update
sudo apt install -y nginx
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update && sudo apt install -y aspnetcore-runtime-8.0
```

### 2. Publicar backend

En la máquina de desarrollo:
```bash
cd Code/Server
dotnet publish -c Release -o ./publish
scp -r ./publish/* ubuntu@<IP_VM1>:/var/www/imt-reservas/
```

### 3. Build y publicar frontend

```bash
cd Code/Client
ng build --configuration production --base-href /
scp -r dist/* ubuntu@<IP_VM1>:/var/www/imt-frontend/
```

### 4. Variables de entorno

Crear `/etc/imt-reservas.env` en VM #1:
```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://localhost:5000
ConnectionStrings__PostgreSQL=Host=<IP_VM2>;Port=5432;Database=IMT_Reservas;Username=imt_user;Password=<password>;Pooling=true;MinPoolSize=2;MaxPoolSize=20
AllowedOrigins__0=http://<IP_VM1>
```

> `MinPoolSize=2;MaxPoolSize=20` — pre-calienta conexiones y limita footprint de memoria.

### 5. Servicio systemd

`/etc/systemd/system/imt-reservas.service`:
```ini
[Unit]
Description=IMT Reservas API
After=network.target

[Service]
WorkingDirectory=/var/www/imt-reservas
ExecStart=/usr/bin/dotnet /var/www/imt-reservas/IMT_Reservas.Server.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=imt-reservas
User=www-data
EnvironmentFile=/etc/imt-reservas.env

[Install]
WantedBy=multi-user.target
```

```bash
sudo systemctl daemon-reload
sudo systemctl enable imt-reservas
sudo systemctl start imt-reservas
sudo systemctl status imt-reservas
```

### 6. nginx

`/etc/nginx/sites-available/imt-reservas`:
```nginx
server {
    listen 80 default_server;
    server_name _;

    root /var/www/imt-frontend;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /api/ {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

```bash
sudo ln -s /etc/nginx/sites-available/imt-reservas /etc/nginx/sites-enabled/
sudo rm -f /etc/nginx/sites-enabled/default
sudo nginx -t
sudo systemctl reload nginx
```

---

## Logs en producción

`appsettings.Production.json` usa `LogLevel: Warning` — solo warnings y errores. Logs van a `journalctl`:

```bash
sudo journalctl -u imt-reservas -f               # tail en vivo
sudo journalctl -u imt-reservas --since today     # solo hoy
sudo journalctl -u imt-reservas -n 100            # últimas 100 líneas
```

Swagger está deshabilitado en producción (`app.Environment.IsDevelopment()` en `Program.cs`).

---

## Actualización

```bash
# En máquina de desarrollo — compilar
cd Code/Server && dotnet publish -c Release -o ./publish
cd ../Client && ng build --configuration production

# En VM #1 — desplegar
sudo systemctl stop imt-reservas

scp -r Code/Server/publish/* ubuntu@<IP_VM1>:/var/www/imt-reservas/
scp -r Code/Client/dist/*   ubuntu@<IP_VM1>:/var/www/imt-frontend/

sudo systemctl start imt-reservas
sudo systemctl reload nginx
```

Verificar:
```bash
sudo systemctl status imt-reservas
curl -s http://localhost:5000/api/GrupoEquipo | head -c 200
```
