## 1. AccesorioControllerTest
- **GetAccesorios_ConDatos_RetornaOk**: Verifica que el endpoint retorna correctamente una lista de accesorios cuando existen datos en la base. Se asegura que el tipo y la cantidad de elementos sean los esperados.
- **GetAccesorios_SinDatos_RetornaOkVacia**: Valida que la respuesta sea una lista vacía cuando no hay accesorios registrados, comprobando que el endpoint maneja correctamente la ausencia de datos.
- **GetAccesorios_ServicioError_RetornaBadRequest**: Evalúa el manejo de errores del servicio simulando una excepción, esperando que el endpoint retorne un BadRequest con el mensaje de error adecuado.
- **CrearAccesorio_Valido_RetornaCreated**: Comprueba que la creación de un accesorio válido retorna un Created, verificando que el controlador procese correctamente la petición y delegue al servicio.

## 2. CarreraControllerTest
- **GetCarreras_ConDatos_RetornaOk**: Verifica la obtención de carreras con datos existentes, asegurando que el endpoint retorna la lista completa y correcta de carreras.
- **GetCarreras_SinDatos_RetornaOkVacia**: Valida la respuesta vacía cuando no hay carreras, comprobando que el endpoint maneja correctamente la ausencia de registros.
- **GetCarreras_ServicioError_RetornaBadRequest**: Evalúa el manejo de errores del servicio lanzando una excepción, esperando que el endpoint retorne un BadRequest.
- **CrearCarrera_Valido_RetornaCreated**: Comprueba la creación exitosa de una carrera, verificando que el controlador procese correctamente la petición y delegue al servicio.

## 3. CategoriaControllerTest
- **GetCategorias_ConDatos_RetornaOk**: Verifica la obtención de categorías con datos, asegurando que el endpoint retorna la lista completa y correcta de categorías.
- **GetCategorias_SinDatos_RetornaOkVacia**: Valida la respuesta vacía cuando no hay categorías, comprobando que el endpoint maneja correctamente la ausencia de registros.
- **GetCategorias_ServicioError_RetornaBadRequest**: Evalúa el manejo de errores del servicio lanzando una excepción, esperando que el endpoint retorne un BadRequest.
- **CrearCategoria_Valida_RetornaCreated**: Comprueba la creación exitosa de una categoría, verificando que el controlador procese correctamente la petición y delegue al servicio.

## 4. ComentarioControllerTest
- **CrearComentario_Valido_RetornaCreated**: Verifica la creación exitosa de un comentario, asegurando que el controlador retorne un Created cuando los datos son válidos.
- **CrearComentario_Invalido_RetornaBadRequest**: Evalúa múltiples casos de error en la creación de comentarios (campos inválidos, referencias inexistentes, longitudes incorrectas, etc.), esperando siempre un BadRequest.
- **CrearComentario_ServicioError_RetornaError500**: Valida el manejo de errores generales del servicio simulando una excepción inesperada, esperando que el endpoint retorne un error 500 (ObjectResult).

## 5. EquipoControllerTest
- **GetEquipos_ConDatos_RetornaOk**: Verifica la obtención de equipos con datos.
- **GetEquipos_SinDatos_RetornaOkVacia**: Valida la respuesta vacía.
- **GetEquipos_ServicioError_RetornaBadRequest**: Evalúa el manejo de errores del servicio.
- **CrearEquipo_Valido_RetornaCreated**: Comprueba la creación exitosa de un equipo.
- **CrearEquipo_Invalido_RetornaBadRequest**: Evalúa múltiples casos de error en la creación de equipos (campos requeridos, valores negativos, etc.).
- **CrearEquipo_RegistroExistente_RetornaConflict**: Verifica el manejo de conflictos por registros duplicados.
- **CrearEquipo_ServicioError_RetornaError500**: Valida el manejo de errores generales del servicio.
- **ActualizarEquipo_Valido_RetornaOk**: Comprueba la actualización exitosa de un equipo.
- **ActualizarEquipo_Invalido_RetornaBadRequest**: Evalúa errores en la actualización de equipos.

## 6. UsuarioControllerTest
- **GetUsuarios_ConDatos_RetornaOk**: Verifica la obtención de usuarios con datos.
- **CrearUsuario_Valido_RetornaOk**: Comprueba la creación exitosa de un usuario.
- **ActualizarUsuario_Valido_RetornaOk**: Valida la actualización de un usuario existente.
- **EliminarUsuario_Valido_RetornaOk**: Evalúa la eliminación exitosa de un usuario.
- **IniciarSesion_Valido_RetornaOkConUsuario**: Verifica el inicio de sesión exitoso y la obtención del usuario.
- **IniciarSesion_Invalido_RetornaOkNulo**: Comprueba que un inicio de sesión inválido retorna un resultado nulo.

## 7. ComponenteControllerTest
- **GetComponentes_ConDatos_RetornaOk**: Verifica que el endpoint retorna correctamente una lista de componentes cuando existen datos.
- **GetComponentes_SinDatos_RetornaOkVacia**: Valida que la respuesta sea una lista vacía cuando no hay componentes.
- **GetComponentes_ServicioError_RetornaBadRequest**: Evalúa el manejo de errores del servicio, esperando un BadRequest.
- **CrearComponente_Valido_RetornaCreated**: Comprueba que la creación de un componente válido retorna un Created.

## 8. EmpresaMantenimientoControllerTest
- **GetEmpresas_ConDatos_RetornaOk**: Verifica la obtención de empresas de mantenimiento con datos existentes.
- **GetEmpresas_SinDatos_RetornaOkVacia**: Valida la respuesta vacía cuando no hay empresas.
- **GetEmpresas_ServicioError_RetornaBadRequest**: Evalúa el manejo de errores del servicio.

## 9. GaveteroControllerTest
- **GetGaveteros_ConDatos_RetornaOk**: Verifica la obtención de gaveteros con datos.
- **GetGaveteros_SinDatos_RetornaOkVacia**: Valida la respuesta vacía.
- **GetGaveteros_ServicioError_RetornaBadRequest**: Evalúa el manejo de errores del servicio.
- **CrearGavetero_Valido_RetornaCreated**: Comprueba la creación exitosa de un gavetero.

## 10. GrupoEquipoControllerTest
- **GetGruposEquipos_ConDatos_RetornaOk**: Verifica la obtención de grupos de equipos con datos.
- **GetGruposEquipos_SinDatos_RetornaOkVacia**: Valida la respuesta vacía.
- **GetGruposEquipos_ServicioError_RetornaBadRequest**: Evalúa el manejo de errores del servicio.

## 11. MantenimientoControllerTest
- **GetMantenimientos_ConDatos_RetornaOk**: Verifica la obtención de mantenimientos con datos.
- **GetMantenimientos_SinDatos_RetornaOkVacia**: Valida la respuesta vacía.
- **GetMantenimientos_ServicioError_RetornaBadRequest**: Evalúa el manejo de errores del servicio.

## 12. MuebleControllerTest
- **GetMuebles_ConDatos_RetornaOk**: Verifica que el endpoint retorna correctamente una lista de muebles cuando existen datos en la base. Se asegura que el tipo y la cantidad de elementos sean los esperados.
- **GetMuebles_SinDatos_RetornaOkVacia**: Valida que la respuesta sea una lista vacía cuando no hay muebles registrados, comprobando que el endpoint maneja correctamente la ausencia de datos.
- **GetMuebles_ServicioError_RetornaBadRequest**: Evalúa el manejo de errores del servicio simulando una excepción, esperando que el endpoint retorne un BadRequest con el mensaje de error adecuado.
- **CrearMueble_Valido_RetornaCreated**: Comprueba que la creación de un mueble válido retorna un Created, verificando que el controlador procese correctamente la petición y delegue al servicio.

## 13. NotificacionControllerTest
- **CrearNotificacion_Valido_RetornaOk**: Verifica que la creación de una notificación válida retorna un Ok, asegurando que el controlador procese correctamente la petición y delegue al servicio.
- **ObtenerNotificacionesPorUsuario_ConDatos_RetornaOk**: Valida que el endpoint retorna correctamente una lista de notificaciones para un usuario específico, comprobando la correcta obtención y filtrado de datos.
- **EliminarNotificacion_Valido_RetornaNoContent**: Comprueba que la eliminación de una notificación existente retorna un NoContent, asegurando que el controlador procese correctamente la petición de borrado.
- **MarcarComoLeida_Valido_RetornaOk**: Verifica que una notificación pueda marcarse como leída correctamente, comprobando que el endpoint procese la acción y retorne un Ok.

## 14. PrestamoControllerTest
- **GetPrestamos_ConDatos_RetornaOk**: Verifica que el endpoint retorna correctamente una lista de préstamos cuando existen datos en la base. Se asegura que el tipo y la cantidad de elementos sean los esperados.
- **GetPrestamos_SinDatos_RetornaOkVacia**: Valida que la respuesta sea una lista vacía cuando no hay préstamos registrados, comprobando que el endpoint maneja correctamente la ausencia de datos.
- **GetPrestamos_ServicioError_RetornaError500**: Evalúa el manejo de errores del servicio simulando una excepción, esperando que el endpoint retorne un error 500 (ObjectResult) con el mensaje de error adecuado.
- **CrearPrestamo_Valido_RetornaOk**: Comprueba que la creación de un préstamo válido retorna un Ok, verificando que el controlador procese correctamente la petición y delegue al servicio.
