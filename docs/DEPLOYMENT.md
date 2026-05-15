# Deployment

Production topology: 2 VMs on Oracle Cloud Always Free.

---

## ✦ Architecture

```
                       Internet
                          │
                          ▼
               ┌─────────────────────────┐
               │   VM #1 (App Server)    │
               │   Ubuntu 22.04          │
               │                         │
               │  nginx :80/:443         │  ← reverse proxy
               │     ↓                   │
               │  .NET 8 :5000 (internal)│  ← systemd service
               └───────────┬─────────────┘
                           │ TCP :5432
               ┌───────────▼─────────────┐
               │   VM #2 (DB Server)     │
               │   PostgreSQL 14 :5432   │
               │   IMT_Reservas DB       │
               └─────────────────────────┘
```

Request flow: `Client → nginx (80/443) → .NET (5000) → PostgreSQL (5432)`

---

## ✦ VM #2 — PostgreSQL

### Install

```bash
sudo apt update && sudo apt install -y postgresql postgresql-contrib
sudo systemctl enable --now postgresql
```

### Enable remote access

`/etc/postgresql/14/main/postgresql.conf`:
```ini
listen_addresses = '*'
```

`/etc/postgresql/14/main/pg_hba.conf` — add at end:
```
host    IMT_Reservas    imt_user    <IP_VM1>/32    md5
```

```bash
sudo systemctl restart postgresql
```

### Create user and database

```sql
-- as postgres user
CREATE USER imt_user WITH PASSWORD '<STRONG_PASSWORD>';
CREATE DATABASE "IMT_Reservas" OWNER imt_user;
GRANT ALL PRIVILEGES ON DATABASE "IMT_Reservas" TO imt_user;
```

### Load schema

```bash
psql -U imt_user -d IMT_Reservas -h localhost -f DataBase/database.ddl
psql -U imt_user -d IMT_Reservas -h localhost -c "\dt"   # 15 tables
```

### Oracle Cloud firewall — VM #2

| Protocol | Port | Source | Description |
|---|---|---|---|
| TCP | 5432 | `<IP_VM1>/32` | PostgreSQL from VM #1 only |

---

## ✦ VM #1 — Backend + Frontend + nginx

### Install dependencies

```bash
sudo apt update && sudo apt install -y nginx

# .NET runtime
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update && sudo apt install -y aspnetcore-runtime-8.0

dotnet --version   # 8.0.x
nginx -v           # nginx/1.18.x
```

### Prepare directories

```bash
sudo mkdir -p /var/www/imt-reservas /var/www/imt-frontend
sudo chown ubuntu:ubuntu /var/www/imt-reservas /var/www/imt-frontend
```

### Build and deploy — Backend

```bash
# Local machine
cd Code/Server
dotnet publish -c Release -o ./publish
scp -r ./publish/* ubuntu@<IP_VM1>:/var/www/imt-reservas/
```

### Build and deploy — Frontend

```bash
# Local machine
cd Code/Client
ng build --configuration production --base-href /
scp -r dist/imt_reservas.client/* ubuntu@<IP_VM1>:/var/www/imt-frontend/
```

### Environment file

`/etc/imt-reservas.env` on VM #1:

```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://localhost:5000
ConnectionStrings__PostgreSQL=Host=<IP_VM2>;Port=5432;Database=IMT_Reservas;Username=imt_user;Password=<PASSWORD>;Pooling=true;MinPoolSize=2;MaxPoolSize=20
AllowedOrigins__0=http://<IP_VM1>
AllowedOrigins__1=https://<IP_VM1>
```

```bash
sudo chmod 600 /etc/imt-reservas.env
sudo chown root:root /etc/imt-reservas.env
```

### systemd service

`/etc/systemd/system/imt-reservas.service`:

```ini
[Unit]
Description=IMT Reservas API (.NET 8)
After=network.target

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
LimitNOFILE=65535

[Install]
WantedBy=multi-user.target
```

```bash
sudo systemctl daemon-reload
sudo systemctl enable --now imt-reservas
sudo systemctl status imt-reservas
```

### nginx

`/etc/nginx/sites-available/imt-reservas`:

```nginx
upstream api_backend {
    server localhost:5000;
    keepalive 32;
}

server {
    listen 80 default_server;
    listen [::]:80 default_server;
    server_name _;

    root /var/www/imt-frontend;
    index index.html;

    access_log /var/log/nginx/imt-reservas-access.log;
    error_log  /var/log/nginx/imt-reservas-error.log;

    # Angular SPA
    location / {
        try_files $uri $uri/ /index.html;
        expires 1h;
        add_header Cache-Control "public, immutable";
    }

    # Static assets — long cache
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
        proxy_connect_timeout 60s;
        proxy_send_timeout    60s;
        proxy_read_timeout    60s;
    }
}
```

```bash
sudo nginx -t
sudo ln -s /etc/nginx/sites-available/imt-reservas /etc/nginx/sites-enabled/
sudo rm -f /etc/nginx/sites-enabled/default
sudo systemctl reload nginx
```

### Oracle Cloud firewall — VM #1

| Protocol | Port | Source | Description |
|---|---|---|---|
| TCP | 80 | `0.0.0.0/0` | HTTP public |
| TCP | 443 | `0.0.0.0/0` | HTTPS public |

---

## ✦ Logs

```bash
# API — live
sudo journalctl -u imt-reservas -f

# API — last 100 lines
sudo journalctl -u imt-reservas -n 100

# nginx
sudo tail -f /var/log/nginx/imt-reservas-access.log
sudo tail -f /var/log/nginx/imt-reservas-error.log
```

---

## ✦ Update

```bash
# 1. Build locally
cd Code/Server && dotnet publish -c Release -o ./publish
cd Code/Client && ng build --configuration production

# 2. Stop service
ssh ubuntu@<IP_VM1> "sudo systemctl stop imt-reservas"

# 3. Copy files
scp -r Code/Server/publish/*         ubuntu@<IP_VM1>:/var/www/imt-reservas/
scp -r Code/Client/dist/imt_reservas.client/* ubuntu@<IP_VM1>:/var/www/imt-frontend/

# 4. Restart
ssh ubuntu@<IP_VM1> "sudo systemctl start imt-reservas && sudo systemctl reload nginx"
```

---

## ✦ Troubleshooting

**Service won't start**
```bash
sudo systemctl status imt-reservas
sudo journalctl -u imt-reservas -n 50
```

**PostgreSQL connection failing** — from VM #1:
```bash
psql -U imt_user -d IMT_Reservas -h <IP_VM2> -c "SELECT version();"
```

**nginx 502 Bad Gateway**
```bash
sudo netstat -tlnp | grep 5000   # verify .NET is listening
sudo journalctl -u imt-reservas | tail -20
sudo systemctl restart imt-reservas
```
