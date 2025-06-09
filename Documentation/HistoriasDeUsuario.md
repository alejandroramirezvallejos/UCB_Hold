# Historias de Usuario

## Módulo: Gestión de Usuarios

### HU1 Creación de Usuario

#### Descripción:

Como administrador del sistema, quiero registrar nuevos usuarios con sus datos personales y académicos para permitirles acceder y utilizar el sistema de préstamos.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir crear un usuario registrando todos sus datos personales (carnet, nombre, apellidos, email, contraseña).
- **Flujos Alternativos:** Si existe un usuario con el mismo carnet o email, el sistema debe rechazar la creación.
- **Validaciones:**
  - El email debe tener un formato válido.
  - Ningún campo obligatorio puede estar vacío.
- **Datos de Entrada:** Carnet, nombre, apellido paterno, apellido materno, email, contraseña, nombre de carrera, teléfono, y opcionalmente datos de referencia.
- **Datos de Salida:** Confirmación de creación exitosa o mensaje de error específico.

#### Criterios de Aceptación No Funcionales:

- **Seguridad:** La contraseña debe almacenarse de forma segura.

#### Evidencia en Código:

- **Archivos relevantes:** `UsuarioController.cs`, `Usuario.cs`
- **Funciones/métodos clave:** `CrearUsuario(UsuarioRequestDto dto)`

### HU2 Obtención de Usuarios

#### Descripción:

Como administrador del sistema, quiero obtener la lista de todos los usuarios registrados para poder visualizar y gestionar los usuarios del sistema.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe retornar la lista completa de usuarios activos.
- **Flujos Alternativos:** Si no hay usuarios registrados, se debe retornar una lista vacía.
- **Datos de Salida:** Lista de usuarios con todos sus datos no sensibles.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** La respuesta debe ser rápida incluso con un gran número de usuarios.

#### Evidencia en Código:

- **Archivos relevantes:** `UsuarioController.cs`
- **Funciones/métodos clave:** `ObtenerUsuarios()`

### HU3 Actualización de Usuario

#### Descripción:

Como administrador o usuario registrado, quiero actualizar mis datos personales para mantener mi información actualizada en el sistema.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir actualizar los datos de un usuario existente.
- **Flujos Alternativos:** Si no existe un usuario con el carnet especificado, el sistema debe rechazar la actualización.
- **Validaciones:**
  - El carnet no puede ser modificado (es el identificador).
  - El email debe tener un formato válido si se proporciona.
- **Datos de Entrada:** Carnet (obligatorio), y cualquier otro campo que se desee actualizar.
- **Datos de Salida:** Confirmación de actualización exitosa o mensaje de error específico.

#### Criterios de Aceptación No Funcionales:

- **Seguridad:** Solo el propio usuario o un administrador puede actualizar los datos.

#### Evidencia en Código:

- **Archivos relevantes:** `UsuarioController.cs`
- **Funciones/métodos clave:** `ActualizarUsuario(string carnet, ActualizarUsuarioRequestDto dto)`

### HU4 Eliminación Lógica de Usuario

#### Descripción:

Como administrador del sistema, quiero eliminar usuarios del sistema para gestionar los registros de usuarios que ya no deben tener acceso.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir eliminar un usuario existente por su carnet.
- **Flujos Alternativos:** Si no existe un usuario con el carnet especificado, el sistema debe informar que no se encontró.
- **Datos de Entrada:** Carnet del usuario a eliminar.
- **Datos de Salida:** Confirmación de eliminación exitosa o mensaje de error específico.

#### Criterios de Aceptación No Funcionales:

- **Seguridad:** Solo un administrador puede eliminar usuarios.

#### Evidencia en Código:

- **Archivos relevantes:** `UsuarioController.cs`
- **Funciones/métodos clave:** `EliminarUsuario(string carnet)`

### HU5 Inicio de Sesión

#### Descripción:

Como usuario registrado, quiero iniciar sesión en el sistema utilizando mi email y contraseña para acceder a las funcionalidades según mi rol.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe validar las credenciales y permitir el acceso si son correctas.
- **Flujos Alternativos:** Si las credenciales son incorrectas, el sistema debe rechazar el acceso.
- **Datos de Entrada:** Email y contraseña.
- **Datos de Salida:** Confirmación de acceso exitoso o mensaje de error.

#### Criterios de Aceptación No Funcionales:

- **Seguridad:** La contraseña debe enmascararse por defecto mostrando caracteres ocultos (•••••). El sistema debe proporcionar un botón que, al ser presionado, permita al usuario alternar entre la visualización y el ocultamiento de la contraseña. Este comportamiento debe ser consistente en todas las interfaces de inicio de sesión y registro de la aplicación.

#### Evidencia en Código:

- **Archivos relevantes:** `iniciar-sesion.component.ts`, `usuario.service.ts`
- **Funciones/métodos clave:** `login()`, `iniciarsesion()`

---

## Módulo: Gestión de Préstamos

### HU6 Creación de Préstamo

#### Descripción:

Como administrador del sistema, quiero registrar nuevos préstamos de equipos a usuarios para llevar un control de los equipos prestados.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir crear un préstamo asociando grupos de equipos a un usuario.
- **Flujos Alternativos:** Si los grupos de equipo no están disponibles o el usuario no existe, el sistema debe rechazar la creación.
- **Validaciones:**
  - La fecha de préstamo esperada no puede ser en el pasado.
  - La fecha de devolución esperada debe ser posterior a la fecha de préstamo esperada.
  - Se debe especificar al menos un grupo de equipo.
  - No se pueden repetir grupos de equipo en el mismo préstamo.
- **Datos de Entrada:** IDs de grupos de equipo, fechas esperadas, observaciones, carnet de usuario, contrato opcional.
- **Datos de Salida:** Confirmación de creación exitosa o mensaje de error específico.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** El sistema debe manejar múltiples solicitudes de préstamo simultáneamente.

#### Evidencia en Código:

- **Archivos relevantes:** `PrestamoController.cs`, `Prestamo.cs`
- **Funciones/métodos clave:** `CrearPrestamo(PrestamoRequestDto dto)`

### HU7 Obtención de Préstamos

#### Descripción:

Como administrador del sistema, quiero obtener la lista de todos los préstamos registrados para poder monitorear y gestionar los préstamos activos e históricos.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe retornar la lista completa de préstamos.
- **Flujos Alternativos:** Si no hay préstamos registrados, se debe retornar una lista vacía.
- **Datos de Salida:** Lista de préstamos con toda su información relacionada.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** La respuesta debe ser rápida incluso con un gran número de préstamos.

#### Evidencia en Código:

- **Archivos relevantes:** `PrestamoController.cs`
- **Funciones/métodos clave:** `ObtenerPrestamos()`

### HU8 Eliminación Lógica de Préstamo

#### Descripción:

Como administrador del sistema, quiero eliminar préstamos del sistema para corregir registros erróneos o cancelar préstamos.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir eliminar un préstamo existente por su ID.
- **Flujos Alternativos:** Si no existe un préstamo con el ID especificado, el sistema debe informar que no se encontró.
- **Validaciones:** El ID debe ser un número positivo.
- **Datos de Entrada:** ID del préstamo a eliminar.
- **Datos de Salida:** Confirmación de eliminación exitosa o mensaje de error específico.

#### Criterios de Aceptación No Funcionales:

- **Seguridad:** Solo un administrador puede eliminar préstamos.

#### Evidencia en Código:

- **Archivos relevantes:** `PrestamoController.cs`
- **Funciones/métodos clave:** `EliminarPrestamo(int id)`

### HU9 Consulta de Préstamo por ID

#### Descripción:

Como administrador o usuario registrado, quiero consultar los detalles de un préstamo específico para verificar su estado y datos.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir obtener la información completa de un préstamo por su ID.
- **Flujos Alternativos:** Si no existe un préstamo con el ID especificado, el sistema debe informar que no se encontró.
- **Validaciones:** El ID debe ser un número positivo.
- **Datos de Entrada:** ID del préstamo a consultar.
- **Datos de Salida:** Información completa del préstamo o mensaje de error.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** La consulta debe ser rápida para proporcionar una buena experiencia de usuario.

#### Evidencia en Código:

- **Archivos relevantes:** `PrestamoController.cs`
- **Funciones/métodos clave:** `ObtenerPrestamoPorId(int id)`

---

## Módulo: Gestión de Equipos

### HU10 Gestión de Equipos

#### Descripción:

Como administrador del sistema, quiero gestionar los equipos disponibles para préstamo para mantener un inventario actualizado.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir registrar, consultar, actualizar y eliminar lógicamente equipos con sus detalles.
- **Validaciones:**
  - El código IMT es obligatorio.
  - El código UCB es opcional.
  - El ID de grupo de equipo debe ser válido.
- **Datos de Entrada:** ID de grupo de equipo, códigos, estado, ubicación, información de costos, etc.
- **Datos de Salida:** Confirmación de operación exitosa o mensaje de error específico.

#### Criterios de Aceptación No Funcionales:

#### Evidencia en Código:

- **Archivos relevantes:** `EquipoController.cs`, `Equipo.cs`

---

## Módulo: Gestión de Grupos de Equipos

### HU11 Gestión de Grupos de Equipos

#### Descripción:

Como administrador del sistema, quiero gestionar grupos de equipos para clasificar y organizar los equipos por modelos y tipos similares.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir crear, consultar, actualizar y eliminar lógicamente grupos de equipos.
- **Validaciones:**
  - El nombre, modelo y marca son obligatorios.
  - La categoría asociada debe existir.
  - La cantidad debe ser un número natural.
- **Datos de Entrada:** Nombre, modelo, marca, URL de imagen, cantidad, ID de categoría, descripción.
- **Datos de Salida:** Confirmación de operación exitosa o mensaje de error específico.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** La gestión debe ser eficiente para soportar un número elevado de equipos.

#### Evidencia en Código:

- **Archivos relevantes:** `GrupoEquipoController.cs`, `GrupoEquipo.cs`

---

## Módulo: Gestión de Categorías

### HU12 Gestión de Categorías

#### Descripción:

Como administrador del sistema, quiero gestionar categorías para clasificar los grupos de equipos según su tipo y uso.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir crear, consultar, actualizar y eliminar lógicamente categorías de equipos.
- **Validaciones:** El nombre de la categoría es obligatorio y debe ser único.
- **Datos de Entrada:** Nombre de la categoría.
- **Datos de Salida:** Confirmación de operación exitosa o mensaje de error específico.

#### Criterios de Aceptación No Funcionales:

#### Evidencia en Código:

- **Archivos relevantes:** `CategoriaController.cs`, `Categoria.cs`

---

## Módulo: Gestión de Accesorios

### HU13 Gestión de Accesorios

#### Descripción:

Como administrador del sistema, quiero gestionar los accesorios de equipos para mantener un registro de los complementos disponibles para préstamo.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir crear, consultar, actualizar y eliminar lógicamente accesorios.
- **Validaciones:** La descripción del accesorio es obligatoria.
- **Datos de Entrada:** Descripción, cantidad, estado.
- **Datos de Salida:** Confirmación de operación exitosa o mensaje de error específico.

#### Criterios de Aceptación No Funcionales:

#### Evidencia en Código:

- **Archivos relevantes:** `AccesorioController.cs`, `Accesorio.cs`

---

## Módulo: Gestión de Componentes

### HU14 Gestión de Componentes

#### Descripción:

Como administrador del sistema, quiero gestionar componentes de equipos para llevar un registro detallado de las partes que componen cada equipo.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir crear, consultar, actualizar y eliminar lógicamente componentes.
- **Validaciones:** La descripción del componente es obligatoria.
- **Datos de Entrada:** Descripción, tipo, especificaciones técnicas.
- **Datos de Salida:** Confirmación de operación exitosa o mensaje de error específico.

#### Criterios de Aceptación No Funcionales:

#### Evidencia en Código:

- **Archivos relevantes:** `ComponenteController.cs`, `Componente.cs`

---

## Módulo: Gestión de Mantenimientos

### HU15 Gestión de Mantenimientos

#### Historia de Usuario:

Como administrador del sistema, quiero registrar y consultar los mantenimientos realizados a los equipos para llevar un histórico de intervenciones y estado de los equipos.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir crear, consultar, actualizar y eliminar lógicamente registros de mantenimiento.
- **Validaciones:** El equipo asociado y la fecha del mantenimiento son obligatorios.
- **Datos de Entrada:** ID de equipo, fecha, tipo de mantenimiento, empresa, costo, observaciones.
- **Datos de Salida:** Confirmación de operación exitosa o mensaje de error específico.

#### Criterios de Aceptación No Funcionales:

#### Evidencia en Código:

- **Archivos relevantes:** `MantenimientoController.cs`, `Mantenimiento.cs`, `DetalleMantenimiento.cs`

---

## Módulo: Interfaz de Administración

### HU16 Panel de Administración

#### Descripción:

Como administrador del sistema, quiero acceder a un panel centralizado que me permita gestionar todas las entidades del sistema (usuarios, préstamos, equipos, etc.) para facilitar la administración general.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe mostrar un panel con acceso a todas las funcionalidades administrativas (//TODO: Agregar que funcionalidades).
- **Flujos Alternativos:** Solo usuarios con rol de administrador pueden acceder.
- **Datos de Salida:** Panel con menús de navegación a todas las funcionalidades administrativas.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** La interfaz debe ser intuitiva y organizada por categorías lógicas.
- **Seguridad:** Solo usuarios autenticados con rol de administrador pueden acceder.

#### Evidencia en Código:

- **Archivos relevantes:** Componentes en la carpeta `admin_modulo/administrador`

---

## Módulo: Gestión de Inventario

### HU17 Visualización de Catálogo de Equipos

#### Descripción:

Como usuario del sistema, quiero ver un catálogo de los equipos disponibles para préstamo, con imágenes y detalles, para seleccionar los que necesito.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe mostrar un listado visual de todos los grupos de equipos disponibles.
- **Flujos Alternativos:** Si no hay equipos disponibles, se debe mostrar un mensaje apropiado.
- **Datos de Salida:** Catálogo con imágenes, nombres, modelos y descripciones de los equipos.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** Las imágenes deben cargarse optimizadas para una visualización rápida.
- **Usabilidad:** El catálogo debe ser atractivo y fácil de navegar.

#### Evidencia en Código:

- **Archivos relevantes:** Componentes en la carpeta `cliente_modulo`, servicio `producto.service.ts`

### HU18 Gestión del Mantenimiento de Equipos

#### Descripción:

Como administrador del sistema, quiero registrar y consultar el historial de mantenimiento de cada equipo a través de una interfaz gráfica para mantener un seguimiento actualizado del estado de los equipos.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** La interfaz debe permitir registrar nuevos mantenimientos y consultar el historial completo.
- **Flujos Alternativos:** Si el equipo no existe o está dado de baja, se debe mostrar una advertencia.
- **Datos de Entrada:** ID de equipo, fecha, tipo de mantenimiento, empresa, costos, observaciones.
- **Datos de Salida:** Confirmación del registro o historial completo de mantenimientos de un equipo.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** Los formularios deben ser claros y permitir seleccionar fechas mediante calendarios.
- **Rendimiento:** La carga del historial debe ser rápida incluso con muchos registros.

#### Evidencia en Código:

- **Archivos relevantes:** Componentes en la carpeta `admin_modulo/mantenimientos`

---

## Módulo: Catálogo y Búsqueda

### HU19 Búsqueda Avanzada de Equipos

#### Descripción:

Como usuario del sistema, quiero utilizar filtros avanzados en la búsqueda de equipos para encontrar exactamente lo que necesito.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir buscar equipos por categoría, marca, modelo y disponibilidad.
- **Flujos Alternativos:** Si no hay resultados, se debe sugerir alternativas similares.
- **Datos de Entrada:** Criterios de búsqueda (categoría, marca, modelo, disponibilidad).
- **Datos de Salida:** Lista filtrada de equipos que cumplen con los criterios.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** La búsqueda debe ser rápida y responder en tiempo real.
- **Usabilidad:** Los filtros deben ser intuitivos y fáciles de usar.

#### Evidencia en Código:

- **Archivos relevantes:** Servicios en la carpeta `services/buscador`, componentes relacionados en `cliente_modulo`

### HU20 Visualización de Detalles de Equipo

#### Descripción:

Como usuario del sistema, quiero ver todos los detalles técnicos e información de un equipo específico para evaluar si cumple con mis necesidades.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** Al seleccionar un equipo del catálogo, el sistema debe mostrar todos sus detalles.
- **Flujos Alternativos:** Si el equipo ya no está disponible, se debe mostrar alternativas similares.
- **Datos de Salida:** Información detallada del equipo, incluyendo especificaciones técnicas, disponibilidad y condiciones de préstamo.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** La información debe presentarse de forma clara y organizada.
- **Rendimiento:** La carga de detalles e imágenes debe ser rápida.

#### Evidencia en Código:

- **Archivos relevantes:** Componentes en la carpeta `cliente_modulo`, servicios relacionados

---

## Módulo: Proceso de Préstamo

### HU21 Solicitud de Préstamo desde Interfaz

#### Descripción:

Como usuario del sistema, quiero completar el proceso de solicitud de préstamo de equipos a través de una interfaz intuitiva para reservar los recursos que necesito.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe guiar al usuario a través del proceso completo de solicitud de préstamo.
- **Flujos Alternativos:** Si hay problemas con la disponibilidad durante el proceso, el sistema debe notificarlo.
- **Validaciones:** Como verificación de fechas y disponibilidad.
- **Datos de Entrada:** Equipos seleccionados, fechas de préstamo y devolución, observaciones.
- **Datos de Salida:** Confirmación de solicitud exitosa o errores específicos.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** Proceso paso a paso con indicaciones claras.
- **Seguridad:** Solo usuarios autenticados pueden realizar solicitudes.
- **Rendimiento:** El proceso debe ser fluido sin retrasos significativos.

#### Evidencia en Código:

- **Archivos relevantes:** Componentes en la carpeta `cliente_modulo`, servicios relacionados con carrito y envío
- **Tests relacionados:** No se encontraron tests específicos.

### HU22 Visualización del Historial de Préstamos

#### Descripción:

Como usuario del sistema, quiero ver mi historial completo de préstamos para llevar un seguimiento de mis solicitudes pasadas y actuales.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe mostrar todos los préstamos del usuario, tanto activos como históricos.
- **Flujos Alternativos:** Si no hay préstamos registrados, se debe mostrar un mensaje apropiado.
- **Datos de Salida:** Lista de préstamos con fechas, equipos, estado y observaciones.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** Los préstamos deben mostrarse organizados cronológicamente.
- **Rendimiento:** La carga del historial debe ser eficiente.

#### Evidencia en Código:

- **Archivos relevantes:** Componentes relacionados en las carpetas `cliente_modulo` y `admin_modulo/prestamos`

---

## Módulo: Roles y Seguridad

### HU23 Control de Acceso Basado en Roles

#### Descripción:

Como administrador del sistema, quiero que las funcionalidades estén restringidas según el rol del usuario para mantener la seguridad y el control del sistema.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir o denegar acceso a funcionalidades específicas según el rol del usuario autenticado.
- **Flujos Alternativos:** Si un usuario intenta acceder a una funcionalidad no autorizada, debe ser redirigido o notificado.
- **Datos de Entrada:** Rol del usuario autenticado.
- **Datos de Salida:** Acceso permitido/denegado a funcionalidades específicas.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** El sistema solo debe mostrar opciones a las que el usuario tiene acceso.

#### Evidencia en Código:

- **Archivos relevantes:** `UsuarioController.cs` (comprobación de roles), (//TODO: Especifica que componentes de navegación y autenticación).

---

## Módulo: Notificaciones

### HU24 Sistema de Notificaciones para Préstamos

#### Descripción:

Como usuario del sistema, quiero recibir notificaciones sobre el estado de mis préstamos para estar informado sobre fechas de vencimiento y cambios de estado.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe mostrar notificaciones relevantes sobre préstamos activos y próximos a vencer.
- **Flujos Alternativos:** Las notificaciones deben marcarse como leídas una vez revisadas.
- **Datos de Salida:** Notificaciones con detalles del préstamo y acciones requeridas.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** Las notificaciones deben ser claras y destacadas en la interfaz.
- **Rendimiento:** El sistema de notificaciones no debe afectar el rendimiento general.

#### Evidencia en Código:

- **Archivos relevantes:** (//TODO: Agregar cuando lo tengamos)

---

## Módulo: Cliente

### HU25 Navegación y Experiencia de Usuario

#### Descripción:

Como usuario del sistema, quiero una interfaz de navegación intuitiva con una barra lateral y superior para acceder fácilmente a todas las funcionalidades disponibles según mi rol.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe proporcionar barras de navegación claras que muestren todas las opciones disponibles según el rol del usuario.
- **Flujos Alternativos:** El menú debe adaptarse dinámicamente según el rol del usuario logueado.
- **Datos de Salida:** Menú de navegación con enlaces a todas las funcionalidades permitidas.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** La navegación debe ser intuitiva, con iconos y etiquetas descriptivas.
- **Rendimiento:** La navegación debe ser fluida sin demoras al cambiar entre secciones.
- **Compatibilidad:** El diseño debe ser responsivo y funcionar en diferentes tamaños de pantalla.

#### Evidencia en Código:

- **Archivos relevantes:** Componentes en las carpetas `navbar` y `sidebard`

### HU26 Visualización de Errores en la Interfaz

#### Descripción:

Como usuario del sistema, quiero recibir notificaciones claras de errores cuando ocurran problemas durante mi interacción para entender qué ha fallado y cómo proceder.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe mostrar mensajes de error descriptivos cuando ocurran fallos.
- **Flujos Alternativos:** Los errores deben mostrarse con mensajes apropiados.
- **Datos de Salida:** Mensajes de error con información sobre el problema.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** Los mensajes de error deben ser claros, visibles y no intrusivos.
- **Accesibilidad:** Los mensajes deben ser perceptibles para todos los usuarios.

#### Evidencia en Código:

- **Archivos relevantes:** Componente en la carpeta `mostrarerror`

---

## Módulo: Gestión de Grupos de Equipos

### HU27 Administración de Grupos de Equipos en Interfaz

#### Descripción:

Como administrador del sistema, quiero gestionar los grupos de equipos a través de una interfaz gráfica para mantener organizado el inventario por tipos y modelos de equipos.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** La interfaz debe permitir crear, editar, eliminar lógicamente y consultar grupos de equipos con todos sus detalles.
- **Flujos Alternativos:** Si un grupo tiene equipos asociados, el sistema debe advertir antes de su eliminación.
- **Validaciones:**
  - El nombre, modelo y marca son obligatorios.
  - La URL de imagen debe ser válida.
  - La cantidad debe ser un número positivo.
- **Datos de Entrada:** Formularios para ingresar todos los atributos del grupo de equipos.
- **Datos de Salida:** Confirmación visual de las operaciones y listado actualizado de grupos.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** Formularios intuitivos con validaciones en tiempo real.
- **Rendimiento:** La carga de imágenes y datos debe ser eficiente.

#### Evidencia en Código:

- **Archivos relevantes:** Componentes en la carpeta `admin_modulo/grupos_equipos`

### HU28 Gestión de Imágenes de Grupos de Equipos

#### Descripción:

Como administrador del sistema, quiero cargar, modificar y eliminar imágenes para los grupos de equipos para permitir a los usuarios identificar visualmente los equipos disponibles.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir subir imágenes para grupos de equipos y asociarlas correctamente.
- **Flujos Alternativos:** Si la carga falla, el sistema debe mostrar un error claro.
- **Validaciones:**
  - Solo se permiten formatos de imagen comunes (JPG, PNG, etc.).
- **Datos de Entrada:** Archivos de imagen seleccionados por el usuario.
- **Datos de Salida:** Confirmación de carga exitosa y previsualización de la imagen.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** Las imágenes deben optimizarse para carga rápida.
- **Almacenamiento:** El sistema debe gestionar eficientemente el espacio de almacenamiento.

#### Evidencia en Código:

- **Archivos relevantes:** Servicios y componentes relacionados con la gestión de imágenes (//TODO: Especificar cuales son)

---

## Módulo: Gestión de Tablas y Datos

### HU29 Visualización de Datos en Tablas Dinámicas

#### Descripción:

Como usuario del sistema, quiero ver los datos en tablas interactivas que me permitan ordenar, filtrar y paginar la información para encontrar fácilmente lo que necesito.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe presentar datos tabulares con funcionalidades de ordenamiento, filtrado y paginación.
- **Flujos Alternativos:** Si no hay datos que mostrar, se debe presentar un mensaje claro.
- **Datos de Entrada:** Criterios de filtrado y ordenamiento definidos por el usuario.
- **Datos de Salida:** Datos organizados según las preferencias del usuario.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** Las operaciones de ordenamiento y filtrado deben ser rápidas incluso con grandes conjuntos de datos.
- **Usabilidad:** La interfaz de tabla debe ser intuitiva y fácil de utilizar.

#### Evidencia en Código:

- **Archivos relevantes:** Componentes en la carpeta `admin_modulo/tablas`

---

## Módulo: Carrito de Préstamos

### HU30 Gestión del Carrito de Préstamos

#### Descripción:

Como usuario del sistema, quiero añadir varios equipos a un carrito de préstamo y gestionar su contenido para preparar una solicitud consolidada de múltiples equipos.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir añadir equipos a un carrito temporal, visualizar su contenido, modificar cantidades y eliminar ítems.
- **Flujos Alternativos:** El usuario debe poder vaciar el carrito completo con una sola acción.
- **Validaciones:** El sistema debe verificar la disponibilidad real de los equipos antes de procesarlos.
- **Datos de Entrada:** Equipos seleccionados del catálogo.
- **Datos de Salida:** Lista actualizada de equipos en el carrito con cantidad y detalles.

#### Criterios de Aceptación No Funcionales:

- **Persistencia:** El contenido del carrito debe mantenerse durante la sesión del usuario.
- **Rendimiento:** Las operaciones de actualización del carrito deben ser inmediatas.

#### Evidencia en Código:

- **Archivos relevantes:** Servicio `carrito.service.ts` y componentes relacionados

### HU31 Procesamiento de Solicitud desde Carrito

#### Descripción:

Como usuario del sistema, quiero finalizar mi selección de equipos en el carrito y enviar la solicitud de préstamo para iniciar el proceso formal de préstamo.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir al usuario revisar el contenido final del carrito, ingresar fechas de préstamo y devolución, y enviar la solicitud.
- **Flujos Alternativos:** Si algún equipo ya no está disponible al momento de procesar la solicitud, el sistema debe notificarlo.
- **Datos de Entrada:** Contenido del carrito, fechas seleccionadas, observaciones.
- **Datos de Salida:** Confirmación de solicitud procesada o errores detallados.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** El proceso debe ser guiado y claro, con confirmaciones en cada paso.
- **Seguridad:** La solicitud debe procesarse solo si el usuario está autenticado.

#### Evidencia en Código:

- **Archivos relevantes:** Servicio `enviar.service.ts`, (//TODO: especificar componentes relacionados con el procesamiento de carrito)

---

## Módulo: Gestión de Datos y Validaciones

### HU32 Validación de Datos de Entrada

#### Descripción:

Como usuario del sistema, quiero recibir feedback inmediato sobre la validez de los datos que ingreso en formularios para corregir errores antes de enviar la información.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe validar en tiempo real los datos ingresados en formularios y mostrar errores o confirmaciones.
- **Flujos Alternativos:** Los formularios no deben permitir el envío si contienen errores de validación.
- **Validaciones:** Cada tipo de campo debe tener sus validaciones específicas (emails, fechas, números, etc.).
- **Datos de Entrada:** Datos ingresados por el usuario en formularios.
- **Datos de Salida:** Indicadores visuales de validación (mensajes).

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** Los mensajes de error deben ser claros y aparecer cerca del campo correspondiente.
- **Accesibilidad:** Los errores de validación deben ser perceptibles para todos los usuarios.

#### Evidencia en Código:

- **Archivos relevantes:** (//TODO: Componentes con formularios en diversos módulos)

---

## Módulo: Estados y Flujos de Préstamo

### HU33 Gestión de Estados de Préstamo

#### Descripción:

Como administrador del sistema, quiero gestionar el ciclo de vida completo de los préstamos a través de diferentes estados (solicitado, aprobado, en progreso, devuelto, rechazado, etc.) para llevar un control eficiente del proceso.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir cambiar el estado de un préstamo siguiendo un flujo predefinido.
- **Flujos Alternativos:** Ciertos cambios de estado deben estar restringidos según reglas específicas (por ejemplo, un préstamo rechazado no puede pasar a aprobado).
- **Validaciones:** Solo se pueden realizar cambios de estado válidos según el estado actual.
- **Datos de Entrada:** ID del préstamo y el nuevo estado.
- **Datos de Salida:** Confirmación del cambio de estado o mensaje de error si la transición no es válida.

#### Criterios de Aceptación No Funcionales:

- **Trazabilidad:** Todos los cambios de estado deben quedar registrados con fecha y usuario responsable.
- **Seguridad:** Solo usuarios con permisos específicos pueden realizar ciertos cambios de estado.

#### Evidencia en Código:

- **Archivos relevantes:** `Prestamo.cs` (propiedad EstadoPrestamo), `PrestamoController.cs`

### HU34 Registro de Devoluciones de Equipos

#### Descripción:

Como administrador del sistema, quiero registrar la devolución de equipos prestados para actualizar su disponibilidad en el inventario y completar el ciclo del préstamo.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir registrar la fecha real de devolución y observaciones sobre el estado devuelto.
- **Flujos Alternativos:** Si el equipo es devuelto con daños, se debe poder registrar esta información para seguimiento.
- **Validaciones:**
  - La fecha de devolución no puede ser anterior a la fecha de préstamo.
  - El préstamo debe estar en estado "en progreso" para poder registrar su devolución.
- **Datos de Entrada:** ID del préstamo, fecha de devolución, observaciones, estado de los equipos devueltos.
- **Datos de Salida:** Confirmación de devolución registrada y actualización del estado del préstamo.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** La interfaz debe facilitar la inspección rápida de los equipos devueltos.
- **Rendimiento:** La actualización del inventario debe ser inmediata tras confirmar la devolución.

#### Evidencia en Código:

- **Archivos relevantes:** `Prestamo.cs` (propiedades FechaDevolucion y observaciones), `PrestamoController.cs`

---

## Módulo: Gestión de Contratos y Documentos

### HU35 Generación y Almacenamiento de Contratos de Préstamo

#### Descripción:

Como administrador del sistema, quiero generar automáticamente contratos de préstamo y almacenarlos en el sistema para tener respaldo legal de cada operación.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe generar un documento de contrato a partir de una plantilla con los datos del préstamo.
- **Flujos Alternativos:** El contrato debe poder ser regenerado si hay cambios en el préstamo antes de su aprobación.
- **Validaciones:** El contrato debe incluir todos los datos legales necesarios y las firmas requeridas.
- **Datos de Entrada:** ID del préstamo para el que se genera el contrato.
- **Datos de Salida:** Documento del contrato en formato PDF y enlace de almacenamiento.

#### Criterios de Aceptación No Funcionales:

- **Seguridad:** Los contratos deben almacenarse de forma segura y con acceso restringido.

#### Evidencia en Código:

- **Archivos relevantes:** `Prestamo.cs` (propiedad Contrato), `PrestamoController.cs` (manejo de archivos)

### HU36 Visualización y Descarga de Contratos

#### Descripción::

Como usuario del sistema, quiero visualizar y descargar los contratos de mis préstamos para tener constancia de las condiciones acordadas.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir visualizar y descargar los contratos asociados a los préstamos del usuario.
- **Flujos Alternativos:** Si un contrato no está disponible, se debe mostrar un mensaje claro.
- **Validaciones:** Solo el usuario propietario y los administradores pueden acceder a un contrato específico.
- **Datos de Salida:** Visualización en navegador o descarga del archivo PDF del contrato.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** La descarga debe iniciarse inmediatamente despues que el usuario descargue el contrato.

#### Evidencia en Código:

- **Archivos relevantes:** (//TODO: Componentes relacionados con visualización de documentos, servicios de descarga de archivos)

---

## Módulo: Disponibilidad de Equipos

### HU37 Verificación de Disponibilidad de Equipos

#### Descripción:

Como usuario del sistema, quiero verificar la disponibilidad de equipos en fechas específicas para planificar mis solicitudes de préstamo.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe mostrar qué equipos están disponibles en un rango de fechas seleccionado.
- **Flujos Alternativos:** Se debe mostrar cuándo estará disponible un equipo actualmente prestado.
- **Validaciones:** Las fechas de búsqueda deben ser válidas (no en el pasado, fecha fin posterior a fecha inicio).
- **Datos de Entrada:** Rango de fechas, tipo de equipo (opcional), categoría (opcional).
- **Datos de Salida:** Lista de equipos disponibles con sus detalles.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** La búsqueda de disponibilidad debe ser rápida incluso con muchos préstamos activos.
- **Usabilidad:** Se debe visualizar claramente la disponibilidad mediante un calendario o esquema temporal.

#### Evidencia en Código:

- **Archivos relevantes:** Servicios relacionados con la verificación de disponibilidad, componentes de búsqueda
- **Tests relacionados:** No se encontraron tests específicos.

### HU38 Reserva Temporal de Equipos

#### Descripción:

Como usuario del sistema, quiero reservar temporalmente equipos mientras completo el proceso de solicitud para asegurar que no sean asignados a otra persona mientras finalizo mi trámite.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe bloquear temporalmente la disponibilidad de equipos seleccionados durante el proceso de solicitud.
- **Flujos Alternativos:** La reserva debe liberarse automáticamente después de un tiempo de inactividad o si el usuario cancela.
- **Validaciones:** Solo se pueden reservar equipos que estén disponibles en ese momento.
- **Datos de Entrada:** IDs de equipos a reservar, duración de la reserva temporal.
- **Datos de Salida:** Confirmación de reserva temporal exitosa con tiempo límite.

#### Criterios de Aceptación No Funcionales:

- **Concurrencia:** El sistema debe manejar correctamente reservas simultáneas del mismo equipo.
- **Rendimiento:** El proceso de reserva debe ser inmediato para evitar conflictos.

#### Evidencia en Código:

- **Archivos relevantes:** (//TODO: Especificar Servicios relacionados con la gestión del carrito y reserva temporal)

---

## Módulo: Gestión de Estados de Equipos

### HU39 Control de Estado de Equipos

#### Descripción:

Como administrador del sistema, quiero gestionar el estado físico y operativo de los equipos (nuevo, bueno, regular, malo, fuera de servicio) para mantener actualizado el inventario y planificar mantenimientos.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir actualizar el estado de cada equipo y registrar observaciones.
- **Flujos Alternativos:** Al marcar un equipo como "fuera de servicio", debe quedar automáticamente no disponible para préstamos.
- **Validaciones:** Los cambios de estado deben incluir justificación o comentarios.
- **Datos de Entrada:** ID del equipo, nuevo estado, observaciones.
- **Datos de Salida:** Confirmación de actualización y registro del cambio en el historial.

#### Criterios de Aceptación No Funcionales:

- **Trazabilidad:** Se debe mantener un historial completo de los cambios de estado de cada equipo.

#### Evidencia en Código:

- **Archivos relevantes:** `Equipo.cs` (propiedad EstadoEquipo), `EquipoController.cs`

### HU40 Registro de Incidentes con Equipos

#### Descripción:

Como usuario del sistema, quiero reportar problemas o incidentes con los equipos prestados para alertar al personal responsable sobre posibles desperfectos.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir registrar incidentes asociados a equipos específicos.
- **Flujos Alternativos:** Se debe notificar automáticamente al personal responsable cuando se registra un incidente grave.
- **Validaciones:** El reporte debe incluir descripción detallada y opcionalmente imágenes.
- **Datos de Entrada:** ID del equipo, tipo de incidente, descripción, imágenes (opcionales).
- **Datos de Salida:** Confirmación de registro de incidente con número de seguimiento.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** El formulario debe ser sencillo y accesible desde la interfaz de usuario.
- **Seguridad:** Se debe identificar correctamente al usuario que reporta el incidente.

#### Evidencia en Código:

- **Archivos relevantes:** (//TODO: Componentes y servicios relacionados con el reporte de incidentes)

---

## Módulo: Reportes y Auditoría

### HU41 Generación de Reportes Personalizados

#### Descripción:

Como administrador del sistema, quiero generar reportes personalizados sobre préstamos, equipos y usuarios para analizar el uso del sistema y tomar decisiones informadas.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir generar reportes personalizados.
- **Flujos Alternativos:** Los reportes deben poder guardarse para uso futuro.
- **Validaciones:** Los filtros y parámetros seleccionados deben ser coherentes entre sí.
- **Datos de Entrada:** Tipo de reporte, rango de fechas, filtros específicos, formato de salida.
- **Datos de Salida:** Reporte generado según los parámetros especificados.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** La generación de reportes debe ser eficiente.
- **Flexibilidad:** El sistema de reportes debe ser extensible para añadir nuevos tipos en el futuro.

#### Evidencia en Código:

- **Archivos relevantes:** (//TODO: Componentes y servicios relacionados con la generación de reportes)

---

## Módulo: Experiencia de Usuario Avanzada

### HU42 Accesibilidad y Modo Oscuro

#### Descripción:

Como usuario del sistema, quiero tener opciones de accesibilidad y modo oscuro para adaptar la interfaz a mis necesidades y preferencias visuales.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir alternar entre modo claro y oscuro y ajustar opciones de accesibilidad.
- **Flujos Alternativos:** Las preferencias deben recordarse para futuras sesiones del mismo usuario.
- **Datos de Entrada:** Preferencias de visualización seleccionadas por el usuario.
- **Datos de Salida:** Interfaz adaptada según las preferencias seleccionadas.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** Los controles de accesibilidad deben ser fáciles de encontrar y utilizar.
- **Compatibilidad:** Las adaptaciones deben funcionar correctamente en diferentes navegadores.
- **Rendimiento:** Los cambios visuales deben aplicarse sin recargar la página completa.

#### Evidencia en Código:

- **Archivos relevantes:** (//TODO: Componentes y servicios relacionados con la configuración de la interfaz de usuario)

---

## Módulo: Experiencia de Usuario en Reserva de Equipos

### HU43 Exploración del Catálogo de Equipos Disponibles

#### Descripción:

Como usuario normal del sistema, quiero navegar por un catálogo visual de equipos disponibles para préstamo para identificar y seleccionar los recursos que necesito para mis actividades académicas.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe presentar un listado visualmente atractivo de los equipos disponibles con imágenes y nombres.
- **Flujos Alternativos:** Si no hay equipos disponibles en una categoría, se debe mostrar un mensaje explicativo.
- **Datos de Salida:** Listado de equipos con sus principales características y disponibilidad.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** Las imágenes deben cargarse de forma optimizada para no retrasar la navegación.
- **Usabilidad:** El catálogo debe organizarse por categorías o tipos de equipos para facilitar la búsqueda.
- **Compatibilidad:** El catálogo debe visualizarse correctamente en dispositivos móviles y de escritorio.

#### Evidencia en Código:

- **Archivos relevantes:** Componente `lista-objetos`, `pantalla-main` en la carpeta `cliente_modulo`

### HU44 Visualización Detallada de un Equipo

#### Descripción:

Como usuario normal del sistema, quiero ver la información detallada de un equipo específico para evaluar si cumple con mis necesidades técnicas y requisitos antes de añadirlo a mi carrito de préstamo.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** Al hacer clic en un equipo del catálogo, el sistema debe mostrar una vista detallada con su nombre, descripcion y disponibilidad.
- **Flujos Alternativos:** Si el equipo está temporalmente no disponible, se debe mostrar cuándo estará disponible nuevamente.
- **Validaciones:** Las fechas de disponibilidad deben actualizarse en tiempo real.
- **Datos de Salida:** Informacion del grupo de equipo

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** La carga de información detallada debe ser rápida.
- **Usabilidad:** La información debe presentarse de forma clara y bien organizada.

#### Evidencia en Código:

- **Archivos relevantes:** Componente `clic_objeto` en la carpeta `cliente_modulo`

### HU45 Adición de Equipos al Carrito de Préstamo

#### Descripción:

Como usuario normal del sistema, quiero añadir equipos a mi carrito de préstamo para ir construyendo mi solicitud con todos los elementos que necesito.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir añadir equipos al carrito desde la vista de catálogo o desde la vista detallada del equipo.
- **Flujos Alternativos:** Si el equipo ya está en el carrito, se debe incrementar su cantidad en lugar de duplicar la entrada.
- **Validaciones:** Solo se pueden añadir equipos que estén disponibles en las fechas seleccionadas.
- **Datos de Entrada:** ID del equipo, cantidad deseada.
- **Datos de Salida:** Confirmación visual de la adición al carrito y actualización del contador de ítems.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** La adición al carrito debe ser inmediata.
- **Usabilidad:** El botón de añadir al carrito debe ser visible y accesible.
- **Persistencia:** Los ítems deben permanecer en el carrito durante toda la sesión del usuario.

#### Evidencia en Código:

- **Archivos relevantes:** Servicio `CarritoService` método `agregarproducto`, componente `carrito`

### HU46 Gestión del Carrito de Préstamo

#### Descripción:

Como usuario normal del sistema, quiero gestionar los equipos en mi carrito de préstamo para ajustar cantidades, eliminar ítems o vaciar completamente el carrito antes de finalizar mi solicitud.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir visualizar todos los ítems del carrito, modificar sus cantidades, eliminar ítems individuales o vaciar el carrito completo.
- **Flujos Alternativos:** Si se elimina el último ítem, se debe mostrar un mensaje indicando que el carrito está vacío.
- **Datos de Entrada:** Acciones del usuario sobre los ítems del carrito (cambio de cantidad, eliminación).
- **Datos de Salida:** Mensaje de confirmacion o de error especifico.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** Las actualizaciones del carrito deben ser inmediatas.
- **Usabilidad:** Las acciones de gestión deben ser intuitivas y accesibles.

#### Evidencia en Código:

- **Archivos relevantes:** Componente `carrito`, métodos `editarcantidad`, `quitarproducto` y `vaciarcarrito` en `CarritoService`

### HU47 Selección de Fechas para el Préstamo

#### Descripción:

Como usuario normal del sistema, quiero especificar las fechas de inicio y fin de mi préstamo para reservar los equipos durante el período exacto en que los necesitaré.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir seleccionar fechas de inicio y finalización del préstamo para todos los equipos en el carrito.
- **Flujos Alternativos:** Si algún equipo no está disponible en el rango de fechas seleccionado, el sistema debe notificarlo.
- **Validaciones:**
  - La fecha de inicio no puede ser anterior a la fecha actual.
  - La fecha de finalización debe ser posterior a la fecha de inicio.
  - El sistema verificará que cada equipo solicitado no tenga reservas previas que se solapen con el período seleccionado.
- **Datos de Entrada:** Fechas de inicio y fin del préstamo.
- **Datos de Salida:** Confirmación de disponibilidad de los equipos en las fechas seleccionadas.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** Los selectores de fecha deben ser intuitivos y permitir fácil navegación por el calendario.
- **Rendimiento:** La verificación de disponibilidad debe ser rápida.

#### Evidencia en Código:

- **Archivos relevantes:** Métodos `onInicioChange` y `onFinChange` en el componente `carrito`

### HU48 Proceso de Confirmación de la Solicitud de Préstamo

#### Descripción:

Como usuario normal del sistema, quiero revisar y confirmar mi solicitud de préstamo para asegurarme de que todos los detalles son correctos antes de enviarla.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe mostrar un resumen completo de la solicitud, incluyendo equipos, cantidades, fechas y condiciones, antes de permitir la confirmación final.
- **Flujos Alternativos:** El usuario debe poder volver atrás para editar su solicitud si detecta algún error.
- **Validaciones:** El sistema debe realizar una comprobación de las reservas existentes para garantizar que el equipo solicitado esté completamente disponible durante todo el período solicitado:
- **Datos de Entrada:** Confirmación del usuario (botón o acción similar).
- **Datos de Salida:** Mensaje de confirmación con número o identificador de la solicitud.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** El resumen debe ser claro y mostrar toda la información relevante.

#### Evidencia en Código:

- **Archivos relevantes:** Componente `formulario` en la carpeta `cliente_modulo`

### HU49 Generación y Firma del Contrato de Préstamo

#### Descripción:

Como usuario normal del sistema, quiero generar, revisar y firmar digitalmente el contrato de préstamo para formalizar mi solicitud de equipos.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe generar automáticamente un documento de contrato con los términos y condiciones del préstamo, permitir su revisión y recoger la firma digital del usuario.
- **Flujos Alternativos:** El usuario debe poder cancelar la firma y volver al proceso en cualquier momento.
- **Validaciones:** La firma no puede estar vacía y debe cumplir con requisitos mínimos de legibilidad.
- **Datos de Entrada:** Firma digital del usuario.
- **Datos de Salida:** Contrato firmado en formato PDF con todos los detalles del préstamo.

#### Criterios de Aceptación No Funcionales:

- **Seguridad:** El proceso de firma debe ser seguro y el documento resultante debe tener validez legal.
- **Rendimiento:** La generación del PDF debe ser rápida y eficiente.
- **Usabilidad:** El área de firma debe ser fácil de usar tanto en dispositivos móviles como de escritorio.

#### Evidencia en Código:

- **Archivos relevantes:** Componente `formulario` y su subcomponente `firma`, uso de librerías como jsPDF y html2canvas

### HU50 Seguimiento del Estado de Solicitudes de Préstamo

#### Descripción:

Como usuario normal del sistema, quiero poder consultar el estado de mis solicitudes de préstamo para saber si han sido aprobadas, rechazadas o están pendientes.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe presentar un listado de todas las solicitudes del usuario con su estado actual (pendiente, aprobada, rechazada, en curso, devuelta).
- **Flujos Alternativos:** Si no hay solicitudes, se debe mostrar un mensaje explicativo.
- **Datos de Salida:** Lista de solicitudes con detalles como fecha, equipos solicitados, estado y acciones disponibles según el estado.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** La carga de información de solicitudes debe ser rápida.
- **Usabilidad:** Los diferentes estados deben distinguirse claramente mediante códigos de color o iconos.

#### Evidencia en Código:

- **Archivos relevantes:** (//TODO: Especificar componentes relacionados con la visualización de historia de préstamos del usuario)

### HU51 Recepción de Notificaciones sobre Préstamos

#### Descripción:

Como usuario normal del sistema, quiero recibir notificaciones sobre cambios en el estado de mis préstamos para estar informado sin necesidad de consultar constantemente el sistema.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe enviar notificaciones cuando: se apruebe/rechace una solicitud, se acerque la fecha de vencimiento, o cuando haya cualquier cambio relevante.
- **Flujos Alternativos:** El usuario debe poder marcar notificaciones como leídas o eliminarlas.
- **Datos de Salida:** Notificaciones con información clara y concisa sobre el evento que las generó.

#### Criterios de Aceptación No Funcionales:

- **Oportunidad:** Las notificaciones deben enviarse en tiempo real o con mínima demora.
- **Accesibilidad:** Las notificaciones deben ser visibles claramente en la interfaz.

#### Evidencia en Código:

- **Archivos relevantes:** (//TODO: Componentes relacionados con el sistema de notificaciones)

### HU52 Cancelación de Solicitudes de Préstamo

#### Descripción:

Como usuario normal del sistema, quiero poder cancelar mis solicitudes de préstamo pendientes cuando ya no los necesite.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir cancelar solicitudes que estén en estado "pendiente" o "aprobada" pero que no hayan iniciado.
- **Flujos Alternativos:** (//TODO: Si la cancelación tiene penalizaciones, el sistema debe informarlo antes de confirmar.)
- **Validaciones:** Solo se pueden cancelar préstamos en estados permitidos y por el mismo usuario que los solicitó.
- **Datos de Entrada:** ID o identificador de la solicitud a cancelar.
- **Datos de Salida:** Confirmación de cancelación exitosa.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** El proceso de cancelación debe ser claro y solicitar confirmación para evitar cancelaciones accidentales.
- **Rendimiento:** La actualización del estado tras la cancelación debe ser inmediata.

#### Evidencia en Código:

- **Archivos relevantes:** (//TODO: Especificar Componentes relacionados con la gestión de préstamos del usuario)

### HU53 Filtrado Avanzado de Equipos

#### Descripción:

Como usuario normal del sistema, quiero filtrar el catálogo de equipos por múltiples criterios para encontrar rápidamente los equipos que más se ajusten a mis necesidades específicas.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe proporcionar opciones de filtrado por categoría, marca, modelo Y disponibilidad.
- **Flujos Alternativos:** Si los filtros aplicados no devuelven resultados, se debe mostrar un mensaje apropiado con sugerencias.
- **Datos de Entrada:** Criterios de filtrado seleccionados por el usuario.
- **Datos de Salida:** Lista filtrada de equipos que cumplen con todos los criterios seleccionados.

#### Criterios de Aceptación No Funcionales:

- **Rendimiento:** El filtrado debe ejecutarse rápidamente incluso con múltiples criterios.
- **Usabilidad:** Los controles de filtro deben ser intuitivos y permitir fácil selección/deselección.

#### Evidencia en Código:

- **Archivos relevantes:** (//TODO: Especificar componentes y servicios relacionados con la búsqueda y filtrado en el módulo cliente)

### HU54 Solicitud de Devolución Anticipada

#### Descripción:

Como usuario normal del sistema, quiero registrar la devolución anticipada de equipos prestados para liberar mi responsabilidad sobre ellos cuando ya no los necesito antes de la fecha acordada.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe permitir al usuario solicitar la devolución anticipada de equipos prestados, programando una fecha y hora para entregarlos.
- **Flujos Alternativos:** Si hay restricciones para devoluciones anticipadas, el sistema debe informarlo claramente.
- **Validaciones:** Solo se pueden devolver anticipadamente préstamos en estado "en curso".
- **Datos de Entrada:** ID del préstamo, fecha y hora propuesta para la devolución.
- **Datos de Salida:** Confirmación de la solicitud de devolución con instrucciones para el proceso físico.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** El proceso debe ser sencillo y claro.
- **Rendimiento:** La actualización del estado debe reflejarse inmediatamente en el sistema.

#### Evidencia en Código:

- **Archivos relevantes:** (//TODO: Especificar componentes relacionados con la gestión de préstamos activos)
