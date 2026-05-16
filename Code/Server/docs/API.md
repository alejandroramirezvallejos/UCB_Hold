# API Reference

Base URL: `https://localhost:7001/api`

## Formato de Respuesta

Errores de validación y errores generales se devuelven en:

```json
{
  "Status": 400,
  "Errors": ["mensaje de error"],
  "ValidationErrors": [{ "Identifier": "campo", "ErrorMessage": "..." }]
}
```

Respuestas exitosas de lista/detalle devuelven el objeto directamente (sin envoltura `Value`).  
La única excepción es `GET /Prestamo/contrato/{id}` que devuelve `{ "contrato": "<html>" }`.

## Autenticación

No hay JWT. El login devuelve el DTO del usuario directamente. El frontend guarda el estado en el cliente (localStorage / signal).

## Endpoints

### Usuario
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/Usuario` | Listar todos los usuarios |
| GET | `/Usuario/{carnet}` | Obtener usuario por carnet |
| POST | `/Usuario` | Crear usuario (contraseña requerida) |
| PUT | `/Usuario/{carnet}` | Actualizar usuario (contraseña opcional) |
| DELETE | `/Usuario/{carnet}` | Eliminar usuario (soft-delete) |
| POST | `/Usuario/login` | Login — body: `{ Email, Contrasena }` |

### Prestamo
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/Prestamo` | Listar préstamos |
| GET | `/Prestamo/{id}` | Obtener préstamo |
| POST | `/Prestamo` | Crear préstamo + asignar equipos + guardar contrato |
| PUT | `/Prestamo/{id}` | Actualizar préstamo |
| PUT | `/Prestamo/{id}/estado` | Cambiar estado — body: `{ "EstadoPrestamo": "aprobado" }` |
| DELETE | `/Prestamo/{id}` | Eliminar préstamo |
| GET | `/Prestamo/historial` | Historial por usuario — query: `carnetUsuario`, `estadoPrestamo` |
| GET | `/Prestamo/contrato/{id}` | Obtener HTML del contrato — respuesta: `{ "contrato": "..." }` |

**Estados válidos y transiciones:**
```
Pendiente  → Aprobado, Rechazado, Cancelado
Aprobado   → Activo, Cancelado
Activo     → Finalizado, Cancelado
Rechazado  → (terminal)
Finalizado → (terminal)
Cancelado  → (terminal)
```

### Equipo / GrupoEquipo / Carrito
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET/POST/PUT/DELETE | `/Equipo` | CRUD equipos |
| GET/POST/PUT/DELETE | `/GrupoEquipo` | CRUD grupos de equipo |
| POST | `/Carrito/disponibilidad` | Calcular disponibilidad — body: `CarritoDto` |

### Catálogos
| Recurso | Ruta base |
|---------|-----------|
| Carrera | `/Carrera` |
| Categoria | `/Categoria` |
| Mueble | `/Mueble` |
| Gavetero | `/Gavetero` |
| Accesorio | `/Accesorio` |
| Componente | `/Componente` |
| EmpresaMantenimiento | `/EmpresaMantenimiento` |
| Mantenimiento | `/Mantenimiento` |

Todos admiten `GET`, `GET/{id}`, `POST`, `PUT/{id}`, `DELETE/{id}`.

### Salud
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/health` | Health check |
