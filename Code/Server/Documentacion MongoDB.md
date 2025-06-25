# MongoDB

# Módulo Comentarios

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

# Módulo Contratos y Préstamos

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

# Módulo Notificaciones

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
