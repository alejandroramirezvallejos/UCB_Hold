<div align="center">

# API Reference

Backend `.NET 8` con endpoints REST bajo `/api/{Controller}` y respuestas normalizadas con `Ardalis.Result`.

[Volver al README](../README.md) · [Setup](SETUP.md) · [Base de datos](DATABASE.md)

</div>

---

## <img height="22" src="../Images/readme-icons/response.svg" alt="" /> Formato de Respuesta

### `200 OK`

```json
{
  "Status": 200,
  "Value": { "Id": 1, "Nombre": "Ejemplo" },
  "Errors": [],
  "ValidationErrors": []
}
```

### `201 Created`

```json
{
  "Status": 201,
  "Value": { "Id": 42 }
}
```

### `400 Validation`

```json
{
  "Status": 400,
  "Value": null,
  "Errors": ["Carnet ya existe"],
  "ValidationErrors": []
}
```

### `401 Unauthorized`

```json
{
  "Status": 401,
  "Value": null,
  "Errors": ["Credenciales inválidas"]
}
```

### `404 Not Found`

```json
{
  "Status": 404,
  "Value": null,
  "Errors": ["Not Found"]
}
```

---

## <img height="22" src="../Images/readme-icons/endpoints.svg" alt="" /> Endpoints

### Usuario

| Método   | Ruta                    | Uso                                  |
| -------- | ----------------------- | ------------------------------------ |
| `GET`    | `/api/Usuario`          | Lista usuarios activos.              |
| `GET`    | `/api/Usuario/{carnet}` | Obtiene un usuario por carnet.       |
| `POST`   | `/api/Usuario`          | Crea usuario y hashea contraseña.    |
| `PUT`    | `/api/Usuario/{carnet}` | Actualiza datos del usuario.         |
| `DELETE` | `/api/Usuario/{carnet}` | Elimina lógicamente al usuario.      |
| `POST`   | `/api/Usuario/login`    | Autentica y retorna datos de sesión. |

### Equipo

| Método   | Ruta               | Uso                            |
| -------- | ------------------ | ------------------------------ |
| `GET`    | `/api/Equipo`      | Lista equipos activos.         |
| `GET`    | `/api/Equipo/{id}` | Obtiene equipo por id.         |
| `POST`   | `/api/Equipo`      | Crea una unidad de equipo.     |
| `PUT`    | `/api/Equipo/{id}` | Actualiza una unidad.          |
| `DELETE` | `/api/Equipo/{id}` | Elimina lógicamente la unidad. |

### GrupoEquipo

| Método   | Ruta                    | Uso                           |
| -------- | ----------------------- | ----------------------------- |
| `GET`    | `/api/GrupoEquipo`      | Lista grupos activos.         |
| `GET`    | `/api/GrupoEquipo/{id}` | Obtiene grupo por id.         |
| `POST`   | `/api/GrupoEquipo`      | Crea grupo.                   |
| `PUT`    | `/api/GrupoEquipo/{id}` | Actualiza grupo.              |
| `DELETE` | `/api/GrupoEquipo/{id}` | Elimina lógicamente el grupo. |

### Prestamo

| Método   | Ruta                               | Uso                                  |
| -------- | ---------------------------------- | ------------------------------------ |
| `GET`    | `/api/Prestamo`                    | Lista préstamos para administración. |
| `GET`    | `/api/Prestamo/{id}`               | Obtiene préstamo por id.             |
| `GET`    | `/api/Prestamo/historial/{carnet}` | Historial de un usuario.             |
| `POST`   | `/api/Prestamo`                    | Crea solicitud de préstamo.          |
| `PUT`    | `/api/Prestamo/{id}/estado`        | Cambia estado del préstamo.          |
| `DELETE` | `/api/Prestamo/{id}`               | Elimina lógicamente el préstamo.     |

### Disponibilidad

| Método | Ruta                          | Uso                                                       |
| ------ | ----------------------------- | --------------------------------------------------------- |
| `POST` | `/api/Carrito/disponibilidad` | Calcula unidades disponibles por grupo y rango de fechas. |

### Mantenimiento

| Método   | Ruta                      | Uso                                |
| -------- | ------------------------- | ---------------------------------- |
| `GET`    | `/api/Mantenimiento`      | Lista mantenimientos.              |
| `GET`    | `/api/Mantenimiento/{id}` | Obtiene mantenimiento.             |
| `POST`   | `/api/Mantenimiento`      | Crea mantenimiento.                |
| `PUT`    | `/api/Mantenimiento/{id}` | Actualiza mantenimiento.           |
| `DELETE` | `/api/Mantenimiento/{id}` | Elimina lógicamente mantenimiento. |

### Catálogos

| Controller             | Ruta                        | Operaciones |
| ---------------------- | --------------------------- | ----------- |
| `Categoria`            | `/api/Categoria`            | CRUD        |
| `Carrera`              | `/api/Carrera`              | CRUD        |
| `Accesorio`            | `/api/Accesorio`            | CRUD        |
| `Componente`           | `/api/Componente`           | CRUD        |
| `EmpresaMantenimiento` | `/api/EmpresaMantenimiento` | CRUD        |
| `Mueble`               | `/api/Mueble`               | CRUD        |
| `Gavetero`             | `/api/Gavetero`             | CRUD        |

---

## <img height="22" src="../Images/readme-icons/validation.svg" alt="" /> Reglas de Validación

| Entidad              | Regla                                                                     |
| -------------------- | ------------------------------------------------------------------------- |
| Usuario              | `Carnet` y `Email` requeridos y únicos. `Telefono` único cuando se envía. |
| Contraseña           | Mínimo 8 caracteres, una mayúscula, un número y un carácter especial.     |
| Equipo               | `IdGrupoEquipo` requerido. `CodigoImt` se asigna al crear y no se edita.  |
| EmpresaMantenimiento | `NIT` único cuando se envía.                                              |
| Prestamo             | Usuario, grupo, fecha de préstamo y fecha de devolución son obligatorios. |

Transiciones de préstamo:

```text
pendiente -> aprobado -> activo -> finalizado
    |             |          |
    +-> rechazado +-> cancelado
```

La aprobación revalida conflictos antes de confirmar el préstamo.

---

## <img height="22" src="../Images/readme-icons/health.svg" alt="" /> Health Check

```http
GET /api/health
```

Retorna `200 OK` con `Healthy` cuando la API y la base de datos están operativas.
