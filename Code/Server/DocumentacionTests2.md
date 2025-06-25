## 1. AccesorioRepositoryTest
- **Crear_LlamaExecuteSpNR_ConParametrosCorrectos**: Verifica que al crear un accesorio se invoque el procedimiento almacenado correspondiente con todos los parámetros requeridos y valores correctos.
- **ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable**: Comprueba que la obtención de todos los accesorios llama a la función adecuada en la base de datos y retorna el DataTable esperado, validando la correcta integración con la capa de datos.
- **Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos**: Valida que la actualización de un accesorio invoque el procedimiento almacenado con los parámetros correctos, asegurando que los cambios se reflejan en la base de datos.
- **Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos**: Evalúa que la eliminación de un accesorio invoque el procedimiento almacenado con el parámetro correcto, garantizando la eliminación lógica o física según la lógica del repositorio.

## 2. CarreraRepositoryTest
- **Crear_LlamaExecuteSpNR_ConParametrosCorrectos**: Verifica la creación de una carrera y la llamada al procedimiento almacenado con los parámetros correctos, asegurando la persistencia de los datos.
- **ObtenerTodas_LlamaEjecutarFuncion_YRetornaDataTable**: Comprueba la obtención de todas las carreras y el retorno del DataTable esperado, validando la consulta y el mapeo de resultados.
- **Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos**: Valida la actualización de una carrera y la llamada al procedimiento almacenado, asegurando la modificación de los datos.
- **Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos**: Evalúa la eliminación de una carrera y la llamada al procedimiento almacenado, comprobando la correcta eliminación lógica o física.

## 3. CategoriaRepositoryTest
- **Crear_LlamaExecuteSpNR_ConParametrosCorrectos**: Verifica la creación de una categoría y la llamada al procedimiento almacenado, asegurando la correcta inserción de datos.
- **ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable**: Comprueba la obtención de todas las categorías y el retorno del DataTable esperado, validando la consulta y el mapeo de resultados.
- **Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos**: Valida la actualización de una categoría y la llamada al procedimiento almacenado, asegurando la modificación de los datos.
- **Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos**: Evalúa la eliminación de una categoría y la llamada al procedimiento almacenado, comprobando la correcta eliminación lógica o física.

## 4. ComentarioRepositoryTest
- **Crear_LlamaAInsertOne**: Verifica que la creación de un comentario en MongoDB invoque el método InsertOne correctamente, asegurando la persistencia del documento.
- **Eliminar_LlamaAUpdateOne**: Comprueba que la eliminación lógica de un comentario invoque el método UpdateOne en la colección de MongoDB, validando el cambio de estado del documento.

## 5. ComponenteRepositoryTest
- **Crear_LlamaExecuteSpNR_ConParametrosCorrectos**: Verifica la creación de un componente y la llamada al procedimiento almacenado, asegurando la correcta inserción de datos.
- **ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable**: Comprueba la obtención de todos los componentes y el retorno del DataTable esperado, validando la consulta y el mapeo de resultados.
- **Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos**: Valida la actualización de un componente y la llamada al procedimiento almacenado, asegurando la modificación de los datos.
- **Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos**: Evalúa la eliminación de un componente y la llamada al procedimiento almacenado, comprobando la correcta eliminación lógica o física.

## 6. EmpresaMantenimientoRepositoryTest
- **Crear_LlamaExecuteSpNR_ConParametrosCorrectos**: Verifica que la creación de una empresa de mantenimiento invoque el procedimiento almacenado con los parámetros correctos, asegurando la persistencia de los datos.
- **ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable**: Comprueba que la obtención de todas las empresas de mantenimiento retorna el DataTable esperado, validando la consulta y el mapeo de resultados.
- **Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos**: Valida que la actualización invoque el procedimiento almacenado con los parámetros correctos, asegurando la modificación de los datos.
- **Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos**: Evalúa que la eliminación invoque el procedimiento almacenado con el parámetro correcto, comprobando la correcta eliminación lógica o física.

## 7. EquipoRepositoryTest
- **Crear_LlamaExecuteSpNR_ConParametrosCorrectos**: Verifica la creación de un equipo y la llamada al procedimiento almacenado, asegurando la correcta inserción de datos.
- **ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable**: Comprueba la obtención de todos los equipos y el retorno del DataTable esperado, validando la consulta y el mapeo de resultados.
- **Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos**: Valida la actualización de un equipo y la llamada al procedimiento almacenado, asegurando la modificación de los datos.
- **Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos**: Evalúa la eliminación de un equipo y la llamada al procedimiento almacenado, comprobando la correcta eliminación lógica o física.

## 8. GaveteroRepositoryTest
- **Crear_LlamaExecuteSpNR_ConParametrosCorrectos**: Verifica la creación de un gavetero y la llamada al procedimiento almacenado, asegurando la correcta inserción de datos.
- **ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable**: Comprueba la obtención de todos los gaveteros y el retorno del DataTable esperado, validando la consulta y el mapeo de resultados.
- **Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos**: Valida la actualización de un gavetero y la llamada al procedimiento almacenado, asegurando la modificación de los datos.
- **Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos**: Evalúa la eliminación de un gavetero y la llamada al procedimiento almacenado, comprobando la correcta eliminación lógica o física.

## 9. GrupoEquipoRepositoryTest
- **Crear_LlamaExecuteSpNR_ConParametrosCorrectos**: Verifica la creación de un grupo de equipo y la llamada al procedimiento almacenado, asegurando la correcta inserción de datos.
- **ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable**: Comprueba la obtención de todos los grupos de equipos y el retorno del DataTable esperado, validando la consulta y el mapeo de resultados.
- **ObtenerPorId_LlamaEjecutarFuncion_YRetornaDataTable**: Valida la obtención de un grupo de equipo por ID y el retorno del DataTable, asegurando la correcta consulta por identificador.
- **ObtenerPorNombreYCategoria_LlamaEjecutarFuncion_YRetornaDataTable**: Evalúa la obtención de grupos de equipos por nombre y categoría, comprobando la correcta aplicación de filtros en la consulta.

## 10. MantenimientoRepositoryTest
- **Crear_LlamaExecuteSpNR_ConParametrosCorrectos**: Verifica la creación de un mantenimiento y la llamada al procedimiento almacenado, asegurando la correcta inserción de datos.
- **ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable**: Comprueba la obtención de todos los mantenimientos y el retorno del DataTable esperado, validando la consulta y el mapeo de resultados.
- **Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos**: Evalúa la eliminación de un mantenimiento y la llamada al procedimiento almacenado, comprobando la correcta eliminación lógica o física.
- **Repositorio_CuandoHayExcepcion_LanzaExcepcion**: Valida que el repositorio lanza una excepción personalizada cuando ocurre un error en la capa de datos, asegurando el manejo adecuado de errores.

## 11. MuebleRepositoryTest
- **Crear_LlamaExecuteSpNR_ConParametrosCorrectos**: Verifica la creación de un mueble y la llamada al procedimiento almacenado, asegurando la correcta inserción de datos.
- **ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable**: Comprueba la obtención de todos los muebles y el retorno del DataTable esperado, validando la consulta y el mapeo de resultados.
- **Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos**: Valida la actualización de un mueble y la llamada al procedimiento almacenado, asegurando la modificación de los datos.
- **Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos**: Evalúa la eliminación de un mueble y la llamada al procedimiento almacenado, comprobando la correcta eliminación lógica o física.

## 12. NotificacionRepositoryTest
- **Crear_LlamaAInsertOne**: Verifica que la creación de una notificación en MongoDB invoque el método InsertOne correctamente, asegurando la persistencia del documento.
- **Eliminar_LlamaAUpdateOne**: Comprueba que la eliminación lógica de una notificación invoque el método UpdateOne en la colección de MongoDB, validando el cambio de estado del documento.

## 13. PrestamoRepositoryTest
- **Crear_LlamaExecuteSpNR_ConParametrosCorrectos**: Verifica la creación de un préstamo y la llamada al procedimiento almacenado, asegurando la correcta inserción de datos.
- **ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable**: Comprueba la obtención de todos los préstamos y el retorno del DataTable esperado, validando la consulta y el mapeo de resultados.

## 14. UsuarioRepositoryTest
- **Crear_LlamaExecuteSpNR_ConParametrosCorrectos**: Verifica la creación de un usuario y la llamada al procedimiento almacenado con los parámetros correctos, asegurando la persistencia de los datos.
- **ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable**: Comprueba la obtención de todos los usuarios y el retorno del DataTable esperado, validando la consulta y el mapeo de resultados.
- **Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos**: Valida la actualización de un usuario y la llamada al procedimiento almacenado, asegurando la modificación de los datos.
- **Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos**: Evalúa la eliminación de un usuario y la llamada al procedimiento almacenado, comprobando la correcta eliminación lógica o física.
- **Repositorio_CuandoHayExcepcion_LanzaExcepcion**: Valida que el repositorio lanza una excepción personalizada cuando ocurre un error en la capa de datos, asegurando el manejo adecuado de errores.
