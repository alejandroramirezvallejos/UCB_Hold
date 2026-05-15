# ⭐ API Reference

> Backend `.NET 8` expone endpoints REST en `/api/{Controller}` con documentación interactiva en Swagger

```
🌐 Development:   https://localhost:{puerto}/swagger
📡 Production:    https://{domain}/api
🔐 Auth:          Bearer token en headers
```

---

## 📋 Tabla de Contenidos

- [Formato de Respuesta](#formato-de-respuesta)
- [Endpoints Principales](#endpoints-principales)
  - [Usuario](#usuario)
  - [Préstamo](#préstamo)
  - [Contrato](#contrato)
  - [Equipo & GrupoEquipo](#equipo--grupoequipo)
  - [Carrito](#carrito)
- [Catálogos](#catálogos)

---

## 💛 Formato de Respuesta

## 💛 Formato de Respuesta

**Success (2xx)**

```json
{
  "Status": 200,
  "Value": { "Id": 1, "Nombre": "Ejemplo" },
  "Errors": [],
  "ValidationErrors": [],
  "SuccessMessage": "Operación completada"
}
```

**Error (4xx/5xx)**

```json
{
  "Status": 400,
  "Value": null,
  "Errors": ["Descripción del error"],
  "ValidationErrors": [{ "Campo": "Email", "Mensaje": "Formato inválido" }],
  "SuccessMessage": null
}
```

---

## 🚀 Endpoints Principales

### 👤 Usuario `/api/Usuario`

Gestión de usuarios, autenticación y control de acceso.

| Método   | Ruta        | Descripción                    | Respuesta                           |
| -------- | ----------- | ------------------------------ | ----------------------------------- |
| `GET`    | `/`         | Listar todos los usuarios      | `200 Response<List<UsuarioDto>>`    |
| `GET`    | `/{carnet}` | Obtener usuario por carnet     | `200 Response<UsuarioDto>` \| `404` |
| `POST`   | `/`         | Crear nuevo usuario            | `201 Response<UsuarioDto>` \| `400` |
| `PUT`    | `/{carnet}` | Actualizar usuario             | `200 Response<UsuarioDto>` \| `400` |
| `DELETE` | `/{carnet}` | Eliminar usuario (soft delete) | `204 No Content` \| `400`           |
| `POST`   | `/login`    | Autenticación                  | `200 Response<UsuarioDto>` \| `401` |

**Body para `/login`:**

```json
{ "Email": "user@ucb.edu.bo", "Contrasena": "***" }
```

---

### 📦 Préstamo `/api/Prestamo`

Gestión completa del ciclo de préstamos de equipos.

| Método   | Ruta                     | Descripción           | Parámetros                               | Respuesta                            |
| -------- | ------------------------ | --------------------- | ---------------------------------------- | ------------------------------------ |
| `GET`    | `/`                      | Listar préstamos      | —                                        | `200 Response<List<PrestamoDto>>`    |
| `GET`    | `/{id}`                  | Obtener préstamo      | route: `id`                              | `200 Response<PrestamoDto>` \| `404` |
| `GET`    | `/historial`             | Historial por usuario | query: `carnetUsuario`, `estadoPrestamo` | `200 Response<List<PrestamoDto>>`    |
| `GET`    | `/contrato/{prestamoId}` | Descargar contrato    | route: `prestamoId`                      | `200 Response<object>` \| `404`      |
| `POST`   | `/`                      | Crear nuevo préstamo  | body: `PrestamoDto`                      | `201 Response<PrestamoDto>`          |
| `PUT`    | `/{id}`                  | Actualizar préstamo   | body: `PrestamoDto`                      | `200 Response<PrestamoDto>`          |
| `PUT`    | `/{id}/estado`           | Cambiar estado        | body: `string`                           | `200 Response<PrestamoDto>`          |
| `DELETE` | `/{id}`                  | Cancelar préstamo     | —                                        | `204 No Content`                     |

**Estados válidos:** `pendiente` → `aprobado` → `activo` → `finalizado` | `rechazado` | `cancelado`

> ⭐ Transiciones validadas automáticamente por `EstadoPrestamoState`

---

### 📋 Contrato `/api/Contrato`

Gestión de contratos HTML para préstamos.

| Método   | Ruta            | Descripción       | Parámetros                     | Respuesta              |
| -------- | --------------- | ----------------- | ------------------------------ | ---------------------- |
| `POST`   | `/crear`        | Generar contrato  | form: `Archivo` + `PrestamoId` | `200 Response<object>` |
| `GET`    | `/{prestamoId}` | Obtener contrato  | route: `prestamoId`            | `200 Response<object>` |
| `DELETE` | `/{prestamoId}` | Eliminar contrato | route: `prestamoId`            | `204 No Content`       |

---

### ⚙️ Equipo & GrupoEquipo

**Equipo `/api/Equipo`** - Gestión de equipos individuales

| Método   | Ruta    | Descripción              | Respuesta                          |
| -------- | ------- | ------------------------ | ---------------------------------- |
| `GET`    | `/`     | Listar todos los equipos | `200 Response<List<EquipoDto>>`    |
| `GET`    | `/{id}` | Obtener equipo por ID    | `200 Response<EquipoDto>` \| `404` |
| `POST`   | `/`     | Crear nuevo equipo       | `201 Response<EquipoDto>` \| `400` |
| `PUT`    | `/{id}` | Actualizar equipo        | `200 Response<EquipoDto>` \| `400` |
| `DELETE` | `/{id}` | Eliminar equipo (soft)   | `204 No Content`                   |

**GrupoEquipo `/api/GrupoEquipo`** - Agrupaciones de equipos por categoría

| Método   | Ruta      | Descripción      | Parámetros                   | Respuesta                            |
| -------- | --------- | ---------------- | ---------------------------- | ------------------------------------ |
| `GET`    | `/`       | Listar grupos    | —                            | `200 Response<List<GrupoEquipoDto>>` |
| `GET`    | `/buscar` | Buscar grupos    | query: `nombre`, `categoria` | `200 Response<List<GrupoEquipoDto>>` |
| `GET`    | `/{id}`   | Obtener grupo    | route: `id`                  | `200 Response<GrupoEquipoDto>`       |
| `POST`   | `/`       | Crear grupo      | body: `GrupoEquipoDto`       | `201 Response<GrupoEquipoDto>`       |
| `PUT`    | `/{id}`   | Actualizar grupo | body: `GrupoEquipoDto`       | `200 Response<GrupoEquipoDto>`       |
| `DELETE` | `/{id}`   | Eliminar grupo   | —                            | `204 No Content`                     |

---

### 🛒 Carrito `/api/Carrito`

Consultar disponibilidad de equipos por rango de fechas.

| Método | Ruta                     | Descripción              | Respuesta                        |
| ------ | ------------------------ | ------------------------ | -------------------------------- |
| `POST` | `/disponibilidadEquipos` | Verificar disponibilidad | `200 Response<List<CarritoDto>>` |

**Body requerido:**

```json
{
  "FechaInicio": "2024-05-20",
  "FechaFin": "2024-05-25",
  "ArrayIds": [1, 2, 3]
}
```

> 💛 `CantidadDisponible` = `Cantidad_total - prestamos_activos_en_ese_rango`

---

## 📚 Catálogos

Todos los catálogos exponen CRUD estándar: `GET /`, `GET /{id}`, `POST /`, `PUT /{id}`, `DELETE /{id}`

| Catálogo                | Endpoint                    | Descripción                |
| ----------------------- | --------------------------- | -------------------------- |
| 🏢 Carrera              | `/api/Carrera`              | Carreras profesionales     |
| 📂 Categoría            | `/api/Categoria`            | Categorías de equipos      |
| 📦 Gavetero             | `/api/Gavetero`             | Gavetas/compartimentos     |
| 🪑 Mueble               | `/api/Mueble`               | Muebles de almacenamiento  |
| 🔌 Accesorio            | `/api/Accesorio`            | Accesorios complementarios |
| ⚙️ Componente           | `/api/Componente`           | Componentes internos       |
| 🏭 EmpresaMantenimiento | `/api/EmpresaMantenimiento` | Empresas de mantenimiento  |
| 🔧 Mantenimiento        | `/api/Mantenimiento`        | Registros de mantenimiento |

---

## ✨ Notas Importantes

- ✅ Todos los endpoints retornan un objeto `Response<T>` estándar
- 🔒 Autenticación requerida con `Bearer token` en header `Authorization`
- 🚫 Soft delete habilitado: registros marcados con `estado_eliminado = true` pero no eliminados
- 📊 Paginación disponible en endpoints GET mediante query params `page`, `pageSize`
- ⚡ Rate limiting: 100 requests/minuto por IP
- 🌐 CORS configurado para desarrollo y producción
- 📝 Todas las respuestas de error incluyen detalles de validación

---
