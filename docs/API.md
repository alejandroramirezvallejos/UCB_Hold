# API Reference

Backend `.NET 8` — REST endpoints under `/api/{Controller}`.
Interactive documentation: [`https://ucbhold.dev/swagger`](https://ucbhold.dev/swagger) (Development only) · local: `https://localhost:7216/swagger`

---

## ✦ Response Format

All responses use the [Ardalis.Result](https://github.com/ardalis/Result) envelope.

**Success `200`**

```json
{
  "Status": 200,
  "Value": { "Id": 1, "Nombre": "Ejemplo" },
  "Errors": [],
  "ValidationErrors": []
}
```

**Created `201`**

```json
{
  "Status": 201,
  "Value": { "Id": 42 }
}
```

**Validation error `400`**

```json
{
  "Status": 400,
  "Value": null,
  "Errors": ["Carnet ya existe"],
  "ValidationErrors": []
}
```

**Not found `404`**

```json
{
  "Status": 404,
  "Value": null,
  "Errors": ["Not Found"]
}
```

**Unauthorized `401`**

```json
{
  "Status": 401,
  "Value": null,
  "Errors": ["Credenciales inválidas"]
}
```

---

## ✦ Endpoints

### Usuario

| Method | Path                        | Description                         |
| ------ | --------------------------- | ----------------------------------- |
| GET    | `/api/Usuario`              | List all active users               |
| GET    | `/api/Usuario/{carnet}`     | Get user by carnet                  |
| POST   | `/api/Usuario`              | Create user (hashes password)       |
| PUT    | `/api/Usuario/{carnet}`     | Update user (password optional)     |
| DELETE | `/api/Usuario/{carnet}`     | Soft-delete user                    |
| POST   | `/api/Usuario/login`        | Authenticate and return user data   |

### Equipo

| Method | Path                        | Description                         |
| ------ | --------------------------- | ----------------------------------- |
| GET    | `/api/Equipo`               | List all active equipment           |
| GET    | `/api/Equipo/{id}`          | Get equipment by id                 |
| POST   | `/api/Equipo`               | Create equipment unit               |
| PUT    | `/api/Equipo/{id}`          | Update equipment unit               |
| DELETE | `/api/Equipo/{id}`          | Soft-delete equipment unit          |

### GrupoEquipo

| Method | Path                            | Description                        |
| ------ | ------------------------------- | ---------------------------------- |
| GET    | `/api/GrupoEquipo`              | List all active groups             |
| GET    | `/api/GrupoEquipo/{id}`         | Get group by id                    |
| POST   | `/api/GrupoEquipo`              | Create group                       |
| PUT    | `/api/GrupoEquipo/{id}`         | Update group                       |
| DELETE | `/api/GrupoEquipo/{id}`         | Soft-delete group                  |

### Prestamo

| Method | Path                                  | Description                              |
| ------ | ------------------------------------- | ---------------------------------------- |
| GET    | `/api/Prestamo`                       | List all loans (admin)                   |
| GET    | `/api/Prestamo/{id}`                  | Get loan by id                           |
| GET    | `/api/Prestamo/historial/{carnet}`    | Loan history for a user                  |
| POST   | `/api/Prestamo`                       | Create loan request                      |
| PUT    | `/api/Prestamo/{id}/estado`           | Transition loan status                   |
| DELETE | `/api/Prestamo/{id}`                  | Soft-delete loan                         |

### Carrito (Availability)

| Method | Path                                  | Description                                          |
| ------ | ------------------------------------- | ---------------------------------------------------- |
| POST   | `/api/Carrito/disponibilidad`         | Get available units per group for a date range       |

### Mantenimiento

| Method | Path                            | Description                        |
| ------ | ------------------------------- | ---------------------------------- |
| GET    | `/api/Mantenimiento`            | List all maintenance records       |
| GET    | `/api/Mantenimiento/{id}`       | Get maintenance record             |
| POST   | `/api/Mantenimiento`            | Create maintenance record          |
| PUT    | `/api/Mantenimiento/{id}`       | Update maintenance record          |
| DELETE | `/api/Mantenimiento/{id}`       | Soft-delete maintenance record     |

### Catalogs

| Controller             | Path                           | Operations |
| ---------------------- | ------------------------------ | ---------- |
| `Categoria`            | `/api/Categoria`               | CRUD       |
| `Carrera`              | `/api/Carrera`                 | CRUD       |
| `Accesorio`            | `/api/Accesorio`               | CRUD       |
| `Componente`           | `/api/Componente`              | CRUD       |
| `EmpresaMantenimiento` | `/api/EmpresaMantenimiento`    | CRUD       |
| `Mueble`               | `/api/Mueble`                  | CRUD       |
| `Gavetero`             | `/api/Gavetero`                | CRUD       |

---

## ✦ Validation Rules

### Usuario

| Field        | Rule                                                                  |
| ------------ | --------------------------------------------------------------------- |
| `Carnet`     | Required. Must be unique across all users.                            |
| `Email`      | Required. Valid email format. Must be unique across all users.        |
| `Telefono`   | Optional. Must be unique if provided.                                 |
| `Contrasena` | Required on Create. Min 8 chars. Must include: uppercase, number, special character (`!@#$%^&*`). |

**Password validation errors:**
- `"Contraseña requerida"` — empty on create
- `"Contraseña debe tener al menos una mayúscula"`
- `"Contraseña debe tener al menos un número"`
- `"Contraseña debe tener al menos un carácter especial"`

**Uniqueness errors:**
- `"Carnet ya existe"`
- `"Email ya existe"`
- `"Teléfono ya registrado"`

### Equipo

| Field           | Rule                                               |
| --------------- | -------------------------------------------------- |
| `IdGrupoEquipo` | Required. Referenced group must exist.             |
| `EstadoEquipo`  | Required. One of: `operativo`, `parcialmente_operativo`, `inoperativo`. |
| `CodigoImt`     | Auto-assigned on Create (max + 1). Read-only — updates are ignored. |

**Validation errors:**
- `"Grupo equipo no existe"`

### EmpresaMantenimiento

| Field  | Rule                                   |
| ------ | -------------------------------------- |
| `NIT`  | Optional. Must be unique if provided.  |

**Validation errors:**
- `"NIT ya registrado"`

### Prestamo

| Field                    | Rule                                                      |
| ------------------------ | --------------------------------------------------------- |
| `CarnetUsuario`          | Required. Referenced user must exist.                     |
| `GrupoEquipoId`          | Required. At least one group. Each group must exist.      |
| `FechaPrestamoEsperada`  | Required.                                                 |
| `FechaDevolucionEsperada`| Required. Must be after start date.                       |

**Availability rule:** an equipment unit is blocked only when an existing loan for the same unit is in state `aprobado` or `activo` with overlapping dates. `pendiente` loans do **not** block availability.

**Status transitions:**

```
pendiente ──► aprobado ──► activo ──► finalizado
    │              │          │
    └──► rechazado └──► cancelado ◄──┘
```

All terminal states (`finalizado`, `rechazado`, `cancelado`) cannot transition further.

Approval (`pendiente → aprobado`) re-validates that the assigned equipment has no conflicts at the moment of approval.

---

## ✦ Health Check

```
GET /api/health
```

Returns `200 OK` with `"Healthy"` when the backend and database connection are operational.
