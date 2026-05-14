# API Reference

Backend `.NET 8` expone endpoints REST en `/api/{Controller}`. Todos los responses siguen el esquema `Response<T>` (ver [ARCHITECTURE.md](ARCHITECTURE.md#estructura-de-respuesta)).

Swagger disponible en Development: `https://localhost:{puerto}/swagger`.

---

## Formato de respuesta

```json
{
  "Status": 200,
  "Value": { "Id": 1, "Nombre": "..." },
  "Errors": [],
  "ValidationErrors": [],
  "SuccessMessage": null
}
```

En caso de error:
```json
{
  "Status": 400,
  "Value": null,
  "Errors": ["Email ya existe"],
  "ValidationErrors": [],
  "SuccessMessage": null
}
```

---

## Endpoints

### Usuario `/api/Usuario`

| Método | Ruta | Body | Respuesta |
|--------|------|------|-----------|
| GET | `/` | — | `Response<List<UsuarioDto>>` |
| GET | `/{carnet}` | — | `Response<UsuarioDto>` |
| POST | `/` | `UsuarioDto` | `201 + Response<UsuarioDto>` |
| PUT | `/{carnet}` | `UsuarioDto` | `Response<UsuarioDto>` |
| DELETE | `/{carnet}` | — | `204 No Content` |
| POST | `/login` | `{Email, Contrasena}` | `Response<UsuarioDto>` |

### Prestamo `/api/Prestamo`

| Método | Ruta | Body | Respuesta |
|--------|------|------|-----------|
| GET | `/` | — | `Response<List<PrestamoDto>>` |
| GET | `/{id}` | — | `Response<PrestamoDto>` |
| GET | `/historial?carnetUsuario=X&estadoPrestamo=Y` | — | `Response<List<PrestamoDto>>` |
| GET | `/contrato/{prestamoId}` | — | `Response<{contrato}>` |
| POST | `/` | `PrestamoDto` (con `Contrato` HTML opcional) | `201 + Response<PrestamoDto>` |
| PUT | `/{id}` | `PrestamoDto` | `Response<PrestamoDto>` |
| PUT | `/{id}/estado` | `"aprobado"` (string) | `Response<PrestamoDto>` |
| DELETE | `/{id}` | — | `204 No Content` |

**Estados válidos en `/estado`:** `pendiente`, `aprobado`, `activo`, `rechazado`, `finalizado`, `cancelado`. Transiciones validadas por `EstadoPrestamoState` ([ARCHITECTURE.md](ARCHITECTURE.md#state-transiciones-de-estadoprestamo)).

### Contrato `/api/Contrato`

| Método | Ruta | Body | Respuesta |
|--------|------|------|-----------|
| POST | `/crear` | `multipart/form-data` (Archivo + PrestamoId) | `Response<ContratoDto>` |
| GET | `/{prestamoId}` | — | `Response<ContratoDto>` |
| DELETE | `/{prestamoId}` | — | `204 No Content` |

### Equipo `/api/Equipo`

CRUD estándar: `GET`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}`.

### Catálogos

Todos siguen patrón CRUD estándar:
- `/api/Carrera`
- `/api/Categoria`
- `/api/GrupoEquipo`
- `/api/Gavetero`
- `/api/Mueble`
- `/api/Accesorio`
- `/api/Componente`
- `/api/EmpresaMantenimiento`
- `/api/Mantenimiento`

### Carrito `/api/Carrito`

| Método | Ruta | Body | Respuesta |
|--------|------|------|-----------|
| POST | `/disponibilidadEquipos` | `CarritoDto` (`FechaInicio`, `FechaFin`, `ArrayIds`) | `Response<List<CarritoDto>>` con disponibilidad por día |

---

## Códigos de error comunes

| Status | Significado | Causas típicas |
|--------|-------------|----------------|
| 400 | Bad Request | Validación de negocio (fechas inválidas, transición no permitida) |
| 401 | Unauthorized | Login con credenciales inválidas o faltantes |
| 404 | Not Found | ID no existe, recurso eliminado |
| 409 | Conflict | Duplicado de PK (carnet/email), restricción de DB violada |
| 500 | Server Error | Error no controlado — chequear logs del backend |

---

## Ejemplo: flujo de login + creación de préstamo

```bash
# 1. Login
curl -X POST https://localhost:7001/api/Usuario/login \
  -H "Content-Type: application/json" \
  -d '{"Email":"alumno@ucb.edu.bo","Contrasena":"password"}'

# 2. Crear préstamo (sin contrato, mismo día)
curl -X POST https://localhost:7001/api/Prestamo \
  -H "Content-Type: application/json" \
  -d '{
    "CarnetUsuario": "12345678",
    "FechaPrestamoEsperada": "2026-05-15T10:00:00",
    "FechaDevolucionEsperada": "2026-05-15T18:00:00",
    "GrupoEquipoId": [3, 5],
    "Observacion": "Práctica de laboratorio"
  }'

# 3. Cambiar estado a aprobado
curl -X PUT https://localhost:7001/api/Prestamo/42/estado \
  -H "Content-Type: application/json" \
  -d '"aprobado"'
```
