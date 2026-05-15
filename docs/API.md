# API Reference

Backend `.NET 8` expone endpoints REST en `/api/{Controller}`.

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

**Estados válidos en `/estado`:** `pendiente`, `aprobado`, `activo`, `rechazado`, `finalizado`, `cancelado`. Transiciones validadas por `EstadoPrestamoState`.

### Contrato `/api/Contrato`

| Método | Ruta | Body | Respuesta |
|--------|------|------|-----------|
| POST | `/crear` | `multipart/form-data` (Archivo + PrestamoId) | `200 OK + Response<object>` |
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
