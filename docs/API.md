# API Reference

Backend `.NET 8` expone endpoints REST en `/api/{Controller}`.

Swagger disponible en Development: `https://localhost:{puerto}/swagger`

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

Error:
```json
{
  "Status": 400,
  "Value": null,
  "Errors": ["Mensaje de error"],
  "ValidationErrors": [],
  "SuccessMessage": null
}
```

---

## Endpoints

### Usuario `/api/Usuario`

| Método | Ruta | Body / Params | Respuesta |
|--------|------|---------------|-----------|
| GET | `/` | — | `200 Response<List<UsuarioDto>>` |
| GET | `/{carnet}` | route: carnet (string) | `200 Response<UsuarioDto>` \| `404` |
| POST | `/` | body: `UsuarioDto` | `201 Response<UsuarioDto>` \| `400` |
| PUT | `/{carnet}` | route: carnet, body: `UsuarioDto` | `200 Response<UsuarioDto>` \| `400` |
| DELETE | `/{carnet}` | route: carnet (string) | `204 No Content` \| `400` |
| POST | `/login` | body: `{ Email, Contrasena }` | `200 Response<UsuarioDto>` \| `401 Unauthorized` |

---

### Prestamo `/api/Prestamo`

| Método | Ruta | Body / Params | Respuesta |
|--------|------|---------------|-----------|
| GET | `/` | — | `200 Response<List<PrestamoDto>>` |
| GET | `/{id}` | route: id (int) | `200 Response<PrestamoDto>` \| `404` |
| GET | `/historial` | query: `carnetUsuario` (string), `estadoPrestamo` (string) | `200 Response<List<PrestamoDto>>` \| `400` |
| GET | `/contrato/{prestamoId}` | route: prestamoId (int) | `200 Response<{ contrato: string }>` \| `404` |
| POST | `/` | body: `PrestamoDto` (campo `Contrato` HTML opcional) | `201 Response<PrestamoDto>` \| `400` |
| PUT | `/{id}` | route: id, body: `PrestamoDto` | `200 Response<PrestamoDto>` \| `400` |
| PUT | `/{id}/estado` | route: id, body: string (estado) | `200 Response<PrestamoDto>` \| `400` |
| DELETE | `/{id}` | route: id (int) | `204 No Content` \| `400` |

**Estados válidos en `/estado`:** `pendiente`, `aprobado`, `activo`, `rechazado`, `finalizado`, `cancelado`

Transiciones validadas por `EstadoPrestamoState`.

---

### Contrato `/api/Contrato`

| Método | Ruta | Body / Params | Respuesta |
|--------|------|---------------|-----------|
| POST | `/crear` | form: `Archivo` (IFormFile) + `PrestamoId` (int) | `200 Response<object>` \| `400` |
| GET | `/{prestamoId}` | route: prestamoId (int) | `200 Response<object>` \| `404` |
| DELETE | `/{prestamoId}` | route: prestamoId (int) | `204 No Content` \| `400` |

---

### Equipo `/api/Equipo`

| Método | Ruta | Body / Params | Respuesta |
|--------|------|---------------|-----------|
| GET | `/` | — | `200 Response<List<EquipoDto>>` |
| GET | `/{id}` | route: id (int) | `200 Response<EquipoDto>` \| `404` |
| POST | `/` | body: `EquipoDto` | `201 Response<EquipoDto>` \| `400` |
| PUT | `/{id}` | route: id, body: `EquipoDto` | `200 Response<EquipoDto>` \| `400` |
| DELETE | `/{id}` | route: id (int) | `204 No Content` \| `400` |

---

### GrupoEquipo `/api/GrupoEquipo`

| Método | Ruta | Body / Params | Respuesta |
|--------|------|---------------|-----------|
| GET | `/` | — | `200 Response<List<GrupoEquipoDto>>` |
| GET | `/buscar` | query: `nombre` (string, opcional), `categoria` (string, opcional) | `200 Response<List<GrupoEquipoDto>>` \| `400` |
| GET | `/{id}` | route: id (int) | `200 Response<GrupoEquipoDto>` \| `404` |
| POST | `/` | body: `GrupoEquipoDto` | `201 Response<GrupoEquipoDto>` \| `400` |
| PUT | `/{id}` | route: id, body: `GrupoEquipoDto` | `200 Response<GrupoEquipoDto>` \| `400` |
| DELETE | `/{id}` | route: id (int) | `204 No Content` \| `400` |

---

### Carrito `/api/Carrito`

| Método | Ruta | Body / Params | Respuesta |
|--------|------|---------------|-----------|
| POST | `/disponibilidadEquipos` | body: `CarritoDto` (`FechaInicio`, `FechaFin`, `ArrayIds`) | `200 Response<List<CarritoDto>>` con disponibilidad por día |

`CantidadDisponible` por grupo por día = `grupo.Cantidad - prestamos_activos_en_esa_fecha`.

---

### Catálogos — CRUD estándar

Todos los catálogos exponen `GET /`, `GET /{id}`, `POST /`, `PUT /{id}`, `DELETE /{id}` con el mismo patrón de respuesta.

#### Carrera `/api/Carrera`

| Método | Ruta | Body | Respuesta |
|--------|------|------|-----------|
| GET | `/` | — | `200 Response<List<CarreraDto>>` |
| GET | `/{id}` | — | `200 Response<CarreraDto>` \| `404` |
| POST | `/` | body: `CarreraDto` | `201 Response<CarreraDto>` \| `400` |
| PUT | `/{id}` | body: `CarreraDto` | `200 Response<CarreraDto>` \| `400` |
| DELETE | `/{id}` | — | `204 No Content` \| `400` |

#### Categoria `/api/Categoria`

| Método | Ruta | Body | Respuesta |
|--------|------|------|-----------|
| GET | `/` | — | `200 Response<List<CategoriaDto>>` |
| GET | `/{id}` | — | `200 Response<CategoriaDto>` \| `404` |
| POST | `/` | body: `CategoriaDto` | `201 Response<CategoriaDto>` \| `400` |
| PUT | `/{id}` | body: `CategoriaDto` | `200 Response<CategoriaDto>` \| `400` |
| DELETE | `/{id}` | — | `204 No Content` \| `400` |

#### Gavetero `/api/Gavetero`

| Método | Ruta | Body | Respuesta |
|--------|------|------|-----------|
| GET | `/` | — | `200 Response<List<GaveteroDto>>` |
| GET | `/{id}` | — | `200 Response<GaveteroDto>` \| `404` |
| POST | `/` | body: `GaveteroDto` | `201 Response<GaveteroDto>` \| `400` |
| PUT | `/{id}` | body: `GaveteroDto` | `200 Response<GaveteroDto>` \| `400` |
| DELETE | `/{id}` | — | `204 No Content` \| `400` |

#### Mueble `/api/Mueble`

| Método | Ruta | Body | Respuesta |
|--------|------|------|-----------|
| GET | `/` | — | `200 Response<List<MuebleDto>>` |
| GET | `/{id}` | — | `200 Response<MuebleDto>` \| `404` |
| POST | `/` | body: `MuebleDto` | `201 Response<MuebleDto>` \| `400` |
| PUT | `/{id}` | body: `MuebleDto` | `200 Response<MuebleDto>` \| `400` |
| DELETE | `/{id}` | — | `204 No Content` \| `400` |

#### Accesorio `/api/Accesorio`

| Método | Ruta | Body | Respuesta |
|--------|------|------|-----------|
| GET | `/` | — | `200 Response<List<AccesorioDto>>` |
| GET | `/{id}` | — | `200 Response<AccesorioDto>` \| `404` |
| POST | `/` | body: `AccesorioDto` | `201 Response<AccesorioDto>` \| `400` |
| PUT | `/{id}` | body: `AccesorioDto` | `200 Response<AccesorioDto>` \| `400` |
| DELETE | `/{id}` | — | `204 No Content` \| `400` |

#### Componente `/api/Componente`

| Método | Ruta | Body | Respuesta |
|--------|------|------|-----------|
| GET | `/` | — | `200 Response<List<ComponenteDto>>` |
| GET | `/{id}` | — | `200 Response<ComponenteDto>` \| `404` |
| POST | `/` | body: `ComponenteDto` | `201 Response<ComponenteDto>` \| `400` |
| PUT | `/{id}` | body: `ComponenteDto` | `200 Response<ComponenteDto>` \| `400` |
| DELETE | `/{id}` | — | `204 No Content` \| `400` |

#### EmpresaMantenimiento `/api/EmpresaMantenimiento`

| Método | Ruta | Body | Respuesta |
|--------|------|------|-----------|
| GET | `/` | — | `200 Response<List<EmpresaMantenimientoDto>>` |
| GET | `/{id}` | — | `200 Response<EmpresaMantenimientoDto>` \| `404` |
| POST | `/` | body: `EmpresaMantenimientoDto` | `201 Response<EmpresaMantenimientoDto>` \| `400` |
| PUT | `/{id}` | body: `EmpresaMantenimientoDto` | `200 Response<EmpresaMantenimientoDto>` \| `400` |
| DELETE | `/{id}` | — | `204 No Content` \| `400` |

#### Mantenimiento `/api/Mantenimiento`

| Método | Ruta | Body | Respuesta |
|--------|------|------|-----------|
| GET | `/` | — | `200 Response<List<MantenimientoDto>>` |
| GET | `/{id}` | — | `200 Response<MantenimientoDto>` \| `404` |
| POST | `/` | body: `MantenimientoDto` | `201 Response<MantenimientoDto>` \| `400` |
| PUT | `/{id}` | body: `MantenimientoDto` | `200 Response<MantenimientoDto>` \| `400` |
| DELETE | `/{id}` | — | `204 No Content` \| `400` |

---

