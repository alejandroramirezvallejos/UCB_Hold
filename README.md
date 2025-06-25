# UCB Hold: Reservas y Gestion de Articulos de Mecatronica

## 1. 🛢️ Modelo Entidad–Relación

![Image](/Images/bd.png)

---

## 2. 𝄜 Tablas, Procedimientos Almacenados, Triggers y Vistas

### Tablas

- `usuarios`, `prestamos`, `detalles_prestamos`, `categorias`, `carreras`, `empresas_mantenimiento`, `mantenimientos`, `detalles_mantenimientos`, `grupos_equipos`, `equipos`, `gaveteros`, `muebles`, `accesorios`, `componentes`.
- Todas incluyen columna `estado_eliminado BOOLEAN DEFAULT FALSE` para borrado lógico.

### Triggers

- **En `equipos`:**
  - **AFTER INSERT/UPDATE/DELETE** sobre relación a `grupos_equipos` → Recalcula `cantidad_equipos` en `grupos_equipos`.
- **En `gaveteros`:**
  - **AFTER INSERT/UPDATE/DELETE** sobre relación a `muebles` → Recalcula `numero_gaveteros` en `muebles`.

### Vistas

- **`vw_equipos_necesitan_mantenimiento`**
- **`vw_ubicaciones_grupos_equipos`**

---

## 3. 🔗 Índices Bien Diseñados y Queries Reescritas

**Usuarios**  
Los índices sobre correo electrónico y estado de eliminación aceleran las búsquedas de usuario activo por su email, clave en operaciones de login y validación. Además, el índice sobre nombre y estado garantiza respuestas rápidas en listados y filtros de usuarios sin cargar filas dadas de baja.

**Prestamos**  
El índice compuesto que abarca fechas de préstamo y devolución, junto con el carnet de usuario y el indicador de eliminación lógica, optimiza las consultas de rangos temporales y el filtrado por cliente. Esto es vital para reportes de retrasos y cadencias de préstamo sin escanear toda la tabla.

**Mantenimientos**  
Los índices que combinan fecha de inicio, fecha final y empresa de mantenimiento, más el flag de borrado, permiten segmentar rápidamente históricos de servicio por compañía y período. De esta forma, los informes de cumplimiento SLA y auditorías temporales se resuelven con mínimas lecturas de disco.

**Grupos_equipos**  
El índice sobre categoría, nombre, modelo, marca y estado lógico mejora la localización de familias de equipos. Con él, las búsquedas de inventario por atributos combinados (por ejemplo, “todos los impresoras HP activas”) devuelven resultados de forma mucho más eficaz.

**Gaveteros**  
Al indexar nombre de gavetero, mueble asociado y estado de eliminación, las operaciones de asignación y consulta de espacio de almacenamiento responden instantáneamente. Esto evita bloqueos costosos cuando múltiples procesos monitorean la distribución de gavetas.

**Equipos**  
El índice que contempla grupo de equipo, código único IMT y estado lógico es fundamental para cualquier unión o filtro de equipos activos. Permite acceder directamente a un equipo en particular o a todos los de un grupo sin escanear la totalidad de la tabla.

**Empresas_mantenimiento**  
Con un índice en nombre y estado se acelera la búsqueda de proveedores activos al generar ordenes de trabajo o seleccionar empresas para cotizaciones, garantizando que sólo se consideren entidades vigentes.

**Detalles_prestamos**  
El índice por identificador de préstamo y estado de eliminación optimiza la obtención de ítems de un préstamo específico, esencial para calcular multas, verificar activos prestados y generar informes de devoluciones.

**Detalles_mantenimientos**  
Indexar por mantenimiento y flag de borrado permite reconstruir con rapidez el histórico detallado de intervenciones de un equipo, lo que simplifica diagnósticos y análisis de fallos.

**Componentes**  
El índice sobre nombre de componente, equipo asociado y estado lógico facilita verificaciones de stock, compatibilidad y asignación de repuestos sin recorrer filas “muertas” o registros obsoletos.

**Categorias**  
Un índice en nombre y estado de eliminación reduce drásticamente el costo de validar unicidad en inserciones y de listar las categorías activas para menús o filtros en la interfaz de usuario.

**Carreras**  
Indexar la columna nombre junto al estado lógico agiliza las consultas para poblar dropdowns o validar inscripciones en la carrera correspondiente, evitando demoras en formularios de alta de usuario.

**Accesorios**  
El índice que agrupa nombre, equipo y estado de eliminación acelera la asociación y revisión de accesorios disponibles para cada equipo, fundamental para operaciones de complemento y preparación de solicitudes de mantenimiento.

## 4. 📈 Análisis de Plan de Ejecución

### Consulta pesada sin indices

![Image](https://github.com/user-attachments/assets/90820cbc-6f9d-4186-8b0d-4777f01e61f9)

### Consulta pesada con indices

![Image](https://github.com/user-attachments/assets/102805fd-879f-4817-845c-df516831b876)

---

## 5. 🚀 Transacciones Funcionales y Niveles de Aislamiento

En todos los procedures tenemos principios ACID con atomicidad y transacciones

- **Nivel de aislamiento**: `SERIALIZABLE`
  - **Justificación**: Garantiza ausencia de lecturas no repetibles y lecturas fantasmas.

---

## Colección Comentarios

### Estructura de documento
```json
{
  "_id": "ObjectId",
  "CarnetUsuario": "string",
  "IdGrupoEquipo": "int",
  "Contenido": "string",
  "Likes": [
    { "CarnetUsuario": "string", "Fecha": "ISODate" }
  ],
  "FechaCreacion": "ISODate",
  "EstadoEliminado": "bool"
}
```

**Nota importante:**
- En la base de datos MongoDB, el campo `Likes` es un array de objetos, donde cada objeto representa un like dado por un usuario (identificado por `CarnetUsuario`) y la fecha en que se dio el like. Esto permite controlar que un usuario no pueda dar más de un like a un mismo comentario (se verifica por `CarnetUsuario` antes de agregar un nuevo like).
- Sin embargo, en la respuesta de la API (ver `ComentarioDto`), el campo `Likes` es un entero que representa únicamente el conteo de likes (es decir, la cantidad de elementos en el array `Likes`), y **no** se retorna el array completo de likes.
- Esta conversión de array a conteo se realiza en el servicio `ComentarioService`, específicamente en el método `MapearFilaADto`, donde se asigna el valor de `Likes` como un entero.
- Ejemplo de consulta equivalente en MongoDB para obtener el conteo de likes de cada comentario:
  ```js
  db.comentarios.find(
    { IdGrupoEquipo: 20, EstadoEliminado: false },
    { Likes: 1 }
  )
  ```

### Inserción
La inserción de un comentario se realiza en la clase `ComentarioRepository`, método `Crear`, mediante la construcción de un documento BSON y el uso de `InsertOne`.

#### En C#
```csharp
coleccion.InsertOne(doc);
```
Clase: ComentarioRepository  
Método: Crear

#### En MongoDB shell
```js
db.comentarios.insertOne({
  CarnetUsuario: "12890061",
  IdGrupoEquipo: 20,
  Contenido: "No suelda bien",
  Likes: [],
  FechaCreacion: new Date(),
  EstadoEliminado: false
})
```

### Consulta
Para obtener comentarios de un grupo:

#### En C#
```csharp
var filtro = Builders<BsonDocument>.Filter.And(
    Builders<BsonDocument>.Filter.Eq("IdGrupoEquipo", idGrupoEquipo),
    Builders<BsonDocument>.Filter.Eq("EstadoEliminado", false)
);
coleccion.Find(filtro).SortByDescending(x => x["FechaCreacion"]);
```
Clase: ComentarioRepository  
Método: ObtenerPorGrupoEquipo

#### En MongoDB shell
```js
db.comentarios.find({ IdGrupoEquipo: 20, EstadoEliminado: false }).sort({ FechaCreacion: -1 })
```

### Eliminación lógica
Para eliminar un comentario (soft delete):

#### En C#
```csharp
coleccion.UpdateOne(filtro, Builders<BsonDocument>.Update.Set("EstadoEliminado", true));
```
Clase: ComentarioRepository  
Método: Eliminar

#### En MongoDB shell
```js
db.comentarios.updateOne({ _id: ObjectId(id), EstadoEliminado: false }, { $set: { EstadoEliminado: true } })
```

### Likes
Agregar un like:

#### En C#
```csharp
coleccion.UpdateOne(filtro, Builders<BsonDocument>.Update.Push("Likes", likeObj));
```
Clase: ComentarioRepository  
Método: AgregarLike

#### En MongoDB shell
```js
db.comentarios.updateOne(
  { _id: ObjectId(id), EstadoEliminado: false },
  { $push: { Likes: { CarnetUsuario: carnet, Fecha: new Date() } } }
)
```
Quitar un like:

#### En C#
```csharp
coleccion.UpdateOne(filtro, Builders<BsonDocument>.Update.PullFilter("Likes", Builders<BsonDocument>.Filter.Eq("CarnetUsuario", comando.CarnetUsuario)));
```
Clase: ComentarioRepository  
Método: QuitarLike

#### En MongoDB shell
```js
db.comentarios.updateOne(
  { _id: ObjectId(id), EstadoEliminado: false },
  { $pull: { Likes: { CarnetUsuario: carnet } } }
)
```

### Justificación de índices
- `_id`: Identificador único por defecto.
- `CarnetUsuario_1_IdGrupoEquipo_1`: Permite búsquedas eficientes por usuario y grupo, útil para filtrar comentarios de un usuario en un grupo de equipos.
- `IdGrupoEquipo_1`: Optimiza la consulta principal de comentarios por grupo.

---

## Colección contratos (MongoDB) y tabla prestamos (PostgreSQL)

### Estructura de documento en MongoDB
```json
{
  "_id": "ObjectId",
  "prestamoId": "int",
  "fileId": "string",
  "EstadoEliminado": "bool"
}
```

### Estructura de tabla en PostgreSQL

| Campo                   | Tipo de dato   | Descripción                                 |
|------------------------|----------------|---------------------------------------------|
| id_prestamo            | int            | Identificador único del préstamo            |
| fecha_solicitud        | timestamp      | Fecha en que se solicitó el préstamo        |
| fecha_prestamo         | timestamp      | Fecha en que se realizó el préstamo         |
| fecha_devolucion_esperada | timestamp   | Fecha esperada de devolución                |
| observacion            | text           | Observaciones adicionales                   |
| estado_prestamo        | text           | Estado actual del préstamo                  |
| carnet                 | text           | Carnet del usuario solicitante              |
| estado_eliminado       | boolean        | Indica si el préstamo fue eliminado         |
| fecha_devolucion       | timestamp      | Fecha real de devolución                    |
| fecha_prestamo_esperada| timestamp      | Fecha esperada para el inicio del préstamo  |
| id_contrato            | text           | Id del contrato en MongoDB (índice único)   |


### Consulta de préstamos

#### En C# (PostgreSQL)
```csharp
const string sql = @"SELECT id_prestamo, fecha_solicitud, fecha_prestamo, fecha_devolucion_esperada, observacion, estado_prestamo, carnet, estado_eliminado, fecha_devolucion, fecha_prestamo_esperada, id_contrato FROM public.prestamos;";
var resultado = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
```
Clase: PrestamoRepository  
Método: ObtenerTodos

### Inserción de préstamo y contrato
La inserción de un préstamo y su contrato se realiza en la clase `PrestamoRepository`, método `Crear`.

#### Ejemplo en C#
```csharp
var fileId = gridFsBucket.UploadFromStream(...);
coleccionContratos.InsertOne({ prestamoId, fileId, EstadoEliminado: false });
var prestamo = new Prestamo { /* ... */ , IdContrato = fileId };
prestamoRepository.Insert(prestamo);
```
Clase: PrestamoRepository  
Método: Crear

#### Ejemplo equivalente en MongoDB shell y SQL
1. Subir archivo a GridFS usando la herramienta de línea de comandos:
   - `mongofiles -d <database> put <archivo>`
2. Insertar la referencia del contrato en la colección de MongoDB:
```js
db.contratos.insertOne({
  prestamoId: 101,
  fileId: "685c1ad76215783d3b996ccc",
  EstadoEliminado: false
})
```
3. Insertar el préstamo en PostgreSQL:
```sql
INSERT INTO public.prestamos (/* campos */) VALUES (/* valores */) RETURNING id_prestamo;
```

### Consulta 
- El servicio consulta la tabla de préstamos en PostgreSQL.
- Si existe `id_contrato`, se puede consultar el contrato en MongoDB.
- El DTO de respuesta (`PrestamoDto`) incluye ambos datos.

#### Ejemplo de consulta de contrato

En C#:
```csharp
var prestamo = prestamoRepository.GetById(id);
if (prestamo != null && prestamo.IdContrato != null) {
    var contrato = coleccionContratos.Find(x => x.FileId == prestamo.IdContrato && !x.EstadoEliminado).FirstOrDefault();
}
```

Traduccion en MongoDB Shell:
```js
if (prestamo && prestamo.id_contrato) {
    var contrato = db.contratos.findOne({ fileId: prestamo.id_contrato, EstadoEliminado: false });
}
```

### Justificación de índices
- `_id`: Único por documento.
- `prestamoId_1`: Único, garantiza un contrato por préstamo y búsquedas rápidas.
- En la tabla `prestamos` de PostgreSQL, el campo `id_contrato` tiene un índice único, lo que garantiza que cada contrato esté asociado a un solo préstamo y no se repitan referencias de contrato.

---

## Colección notificaciones

### Estructura de documento
```json
{
  "_id": "ObjectId",
  "CarnetUsuario": "string",
  "Titulo": "string",
  "Contenido": "string",
  "FechaEnvio": "ISODate",
  "EstadoEliminado": "bool",
  "Leido": "bool"
}
```

### Inserción
La inserción de una notificación se realiza en la clase `NotificacionRepository`, método `Crear`.

#### En C#
```csharp
coleccion.InsertOne(doc);
```
Clase: NotificacionRepository  
Método: Crear

#### En MongoDB shell
```js
db.notificaciones.insertOne({
  CarnetUsuario: carnet,
  Titulo: titulo,
  Contenido: contenido,
  FechaEnvio: new Date(),
  Leido: false,
  EstadoEliminado: false
})
```

### Consulta
Obtener notificaciones de un usuario:

#### En C#
```csharp
coleccion.Find(filtro).SortByDescending(x => x["FechaEnvio"]);
```
Clase: NotificacionRepository  
Método: ObtenerPorUsuario

#### En MongoDB shell
```js
db.notificaciones.find({ CarnetUsuario: carnet, EstadoEliminado: false }).sort({ FechaEnvio: -1 })
```
Verificar si hay no leídas:

#### En C#
```csharp
coleccion.Find(filtro).Limit(1);
```
Clase: NotificacionRepository  
Método: TieneNotificacionesNoLeidas

#### En MongoDB shell
```js
db.notificaciones.find({ CarnetUsuario: carnet, Leido: false, EstadoEliminado: false }).limit(1)
```

### Actualización
Marcar como leída:

#### En C#
```csharp
coleccion.UpdateOne(filtro, Builders<BsonDocument>.Update.Set("Leido", true));
```
Clase: NotificacionRepository  
Método: MarcarComoLeida

#### En MongoDB shell
```js
db.notificaciones.updateOne({ _id: ObjectId(id) }, { $set: { Leido: true } })
```
Eliminar lógicamente:

#### En C#
```csharp
coleccion.UpdateOne(filtro, Builders<BsonDocument>.Update.Set("EstadoEliminado", true));
```
Clase: NotificacionRepository  
Método: Eliminar

#### En MongoDB shell
```js
db.notificaciones.updateOne({ _id: ObjectId(id) }, { $set: { EstadoEliminado: true } })
```

---

## Inserción directa de notificaciones en MongoDB (Simulación de notificaciones automáticas)

### Ejemplo de inserción directa en MongoDB shell
```js
db.notificaciones.insertOne({
  CarnetUsuario: "12345678",
  Titulo: "Notificación automática",
  Contenido: "Esta es una notificación generada automáticamente.",
  FechaEnvio: new Date(),
  Leido: false,
  EstadoEliminado: false
})
```

### Ejemplo de inserción directa en C#
```csharp
var coleccion = mongoDbContext.BaseDeDatos.GetCollection<BsonDocument>("notificaciones");
var notificacion = new BsonDocument {
    { "CarnetUsuario", "12345678" },
    { "Titulo", "Notificación automática" },
    { "Contenido", "Esta es una notificación generada automáticamente." },
    { "FechaEnvio", DateTime.UtcNow },
    { "Leido", false },
    { "EstadoEliminado", false }
};
coleccion.InsertOne(notificacion);
```

---

### ¿Cómo funciona el envío automático de notificaciones en la aplicación?

El frontend, específicamente en el componente del navbar, realiza una consulta cada segundo a las APIs de notificaciones para verificar si existen nuevas notificaciones para el usuario, únicamente si este ha iniciado sesión. De esta manera, cada usuario conectado funciona como un "nodo" que mantiene actualizado tanto el envío como la visualización de notificaciones en tiempo real.

El sistema dispone de endpoints y lógica de servicios que permiten el envío automático de notificaciones relacionadas con préstamos, tales como retrasos, cambios de estado, penalizaciones. Estas notificaciones se generan e insertan automáticamente en la colección `notificaciones` de MongoDB, y el proceso de verificación y actualización se ejecuta también cada segundo desde el navbar.

### Diagrama

```
[Usuario en el navegador]
        |
        | (cada 1 segundo)
        v
[Frontend llama a API]
        |
        v
[NotificacionController]
        |
        v
[NotificacionService]
        |
        v
[NotificacionRepository]
        |
        v
    [MongoDB]
       
```

### Controlador (NotificacionController)
```csharp
[HttpGet("{carnetUsuario}/tiene-no-leidas")]
public IActionResult TieneNoLeidas(string carnetUsuario)
{
    try
    {
        var consulta = new TieneNotificacionesNoLeidasConsulta(carnetUsuario);
        var tiene = servicio.TieneNotificacionesNoLeidas(consulta);
        return Ok(new { tieneNoLeidas = tiene });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message });
    }
}
```

### Servicio (NotificacionService)
```csharp
public bool TieneNotificacionesNoLeidas(TieneNotificacionesNoLeidasConsulta consulta)
{
    return _notificacionRepository.TieneNotificacionesNoLeidas(consulta);
}
```

### Repositorio (NotificacionRepository)
```csharp
public bool TieneNotificacionesNoLeidas(TieneNotificacionesNoLeidasConsulta consulta)
{
    var filtro = Builders<BsonDocument>.Filter.And(
        Builders<BsonDocument>.Filter.Eq("CarnetUsuario", consulta.CarnetUsuario),
        Builders<BsonDocument>.Filter.Eq("Leido", false),
        Builders<BsonDocument>.Filter.Eq("EstadoEliminado", false)
    );
    var coleccion = _mongoDbContext.BaseDeDatos.GetCollection<BsonDocument>("notificaciones");
    return coleccion.Find(filtro).Limit(1).Any();
}
```

### NotificacionController
- `POST /api/notificacion/enviar-retrasos` → Llama a `EnviarNotificacionesRetraso()` en el servicio.
- `POST /api/notificacion/enviar-penalizaciones` → Llama a `EnviarPenalizaciones()` en el servicio.
- `POST /api/notificacion/enviar-estado-prestamo` → Llama a `EnviarEstadoDelPrestamo()` en el servicio.

### NotificacionService
- `EnviarNotificacionesRetraso()`: Busca préstamos retrasados y genera notificaciones automáticas para los usuarios que no devolvieron a tiempo.
- `EnviarPenalizaciones()`: Busca préstamos con penalización y genera notificaciones automáticas.
- `EnviarEstadoDelPrestamo()`: Notifica automáticamente a los usuarios cuando su préstamo es aprobado o rechazado.

**Importante:**
- Antes de crear una notificación automática, el servicio verifica si ya existe una notificación con el mismo título y contenido para el usuario. Si ya existe, **no se vuelve a crear**. Esto evita notificaciones duplicadas.

#### Ejemplo de verificación en C# (NotificacionService):
```csharp
private bool NotificacionYaExiste(string carnet, string titulo, string contenido)
{
    var consulta = new ObtenerNotificacionPorCarnetUsuarioConsulta(carnet);
    var notificaciones = _notificacionRepository.ObtenerPorUsuario(consulta);
    foreach (DataRow fila in notificaciones.Rows)
    {
        if (fila["titulo"].ToString() == titulo && fila["contenido"].ToString() == contenido)
            return true;
    }
    return false;
}
```

## 6. ⬇️ Instalar

npm install signature_pad

npm install jspdf

npm install html2canvas

---

## 7. 👥 Miembros

- [Josue Galo Balbontin Ugarteche](https://github.com/josue-balbontin)
- [Alejandro Ramirez Vallejos](https://github.com/alejandroramirezvallejos)
- [Fernando Terrazas Llanos](https://github.com/FernandoTerrazasLl)
