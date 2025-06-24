# Historias de Usuario

### HU1 Creación de Usuario

#### Descripción:

Como administrador del sistema, quiero registrar nuevos usuarios con sus datos personales y académicos para permitirles acceder y utilizar el sistema de préstamos.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema permite crear un usuario registrando sus datos personales (carnet, nombre, apellido paterno, apellido materno, email, contraseña, carrera, teléfono, rol, datos de referencia).
- **Flujos Alternativos:** Si existe un usuario con el mismo carnet o email, el sistema rechaza la creación.
- **Validaciones:**
  - El email debe tener un formato válido.
  - Ningún campo obligatorio puede estar vacío (carnet, nombre, apellidos, email, contraseña, carrera, teléfono, rol).
  - Validación de longitud y formato para los campos según reglas de negocio.
- **Datos de Entrada:** Carnet, nombre, apellido paterno, apellido materno, carrera, rol, email, teléfono, teléfono de referencia, nombre de referencia, email de referencia.
- **Datos de Salida:** Confirmación de creación exitosa o mensaje de error específico.

#### Evidencia en Código y Tests:

- **Controlador:** `src/Presentations/Controllers/UsuarioController.cs`
- **Comando de creación:** `src/Presentations/Commands/Usuario/CrearUsuarioComando.cs`
- **DTO:** `src/Application/DTOs/UsuarioDto.cs`
- **Test de Controlador:** `Tests/ControllerTests/Implementations/UsuarioControllerTest.cs`
- **Test de Servicio:** `Tests/ServiceTests/Implementations/UsuarioServiceTest.cs`
- **Test de Repositorio:** `Tests/RepositoryTests/Implementations/UsuarioRepositoryTest.cs`

#### Criterios de Aceptación No Funcionales:

- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar la funcionalidad principal de cada historia de usuario.
- **Control de Excepciones:** Se debe manejar adecuadamente errores específicos relacionados con cada historia de usuario, utilizando las clases de excepción disponibles en la carpeta `Shared/Exceptions`.
- **Usabilidad:** La interfaz debe ser intuitiva y accesible para todos los usuarios, con validaciones en tiempo real donde sea aplicable.

---

### HU2 Obtención de Usuarios

#### Descripción:

Como administrador del sistema, quiero obtener la lista de todos los usuarios registrados para poder visualizar y gestionar los usuarios del sistema.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema retorna la lista completa de usuarios activos.
- **Flujos Alternativos:** Si no hay usuarios registrados, retorna una lista vacía.
- **Datos de Salida:** Lista de usuarios con sus datos no sensibles.

#### Evidencia en Código y Tests:

- **Controlador:** `src/Presentations/Controllers/UsuarioController.cs`
- **Servicio:** `src/Application/Services/Implementations/UsuarioService.cs`
- **DTO:** `src/Application/DTOs/UsuarioDto.cs`
- **Test de Controlador:** `Tests/ControllerTests/Implementations/UsuarioControllerTest.cs`
- **Test de Servicio:** `Tests/ServiceTests/Implementations/UsuarioServiceTest.cs`
- **Test de Repositorio:** `Tests/RepositoryTests/Implementations/UsuarioRepositoryTest.cs`

#### Criterios de Aceptación No Funcionales:

- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar la funcionalidad principal de cada historia de usuario.
- **Control de Excepciones:** Se debe manejar adecuadamente errores específicos relacionados con cada historia de usuario, utilizando las clases de excepción disponibles en la carpeta `Shared/Exceptions`.
- **Usabilidad:** La interfaz debe ser intuitiva y accesible para todos los usuarios, con validaciones en tiempo real donde sea aplicable.

---

### HU3 Actualización de Usuario

#### Descripción:

Como administrador o usuario registrado, quiero actualizar mis datos personales para mantener mi información actualizada en el sistema.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema permite actualizar los datos de un usuario existente por carnet.
- **Flujos Alternativos:** Si no existe un usuario con el carnet especificado, el sistema rechaza la actualización.
- **Validaciones:**
  - El carnet no puede ser modificado.
  - El email debe tener un formato válido si se proporciona.
- **Datos de Entrada:** Carnet (obligatorio), y cualquier otro campo a actualizar.
- **Datos de Salida:** Confirmación de actualización exitosa o mensaje de error específico.

#### Evidencia en Código y Tests:

- **Controlador:** `src/Presentations/Controllers/UsuarioController.cs`
- **Servicio:** `src/Application/Services/Implementations/UsuarioService.cs`
- **DTO:** `src/Application/DTOs/UsuarioDto.cs`
- **Test de Controlador:** `Tests/ControllerTests/Implementations/UsuarioControllerTest.cs`
- **Test de Servicio:** `Tests/ServiceTests/Implementations/UsuarioServiceTest.cs`
- **Test de Repositorio:** `Tests/RepositoryTests/Implementations/UsuarioRepositoryTest.cs`

#### Criterios de Aceptación No Funcionales:

- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar la funcionalidad principal de cada historia de usuario.
- **Control de Excepciones:** Se debe manejar adecuadamente errores específicos relacionados con cada historia de usuario, utilizando las clases de excepción disponibles en la carpeta `Shared/Exceptions`.
- **Usabilidad:** La interfaz debe ser intuitiva y accesible para todos los usuarios, con validaciones en tiempo real donde sea aplicable.

---

### HU4 Eliminación Lógica de Usuario

#### Descripción:

Como administrador del sistema, quiero eliminar usuarios del sistema para gestionar los registros de usuarios que ya no deben tener acceso.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema permite eliminar lógicamente un usuario por carnet.
- **Flujos Alternativos:** Si no existe un usuario con el carnet especificado, el sistema informa que no se encontró.
- **Datos de Entrada:** Carnet del usuario a eliminar.
- **Datos de Salida:** Confirmación de eliminación exitosa o mensaje de error específico.

#### Evidencia en Código y Tests:

- **Controlador:** `src/Presentations/Controllers/UsuarioController.cs`
- **Servicio:** `src/Application/Services/Implementations/UsuarioService.cs`
- **DTO:** `src/Application/DTOs/UsuarioDto.cs`
- **Test de Controlador:** `Tests/ControllerTests/Implementations/UsuarioControllerTest.cs`
- **Test de Servicio:** `Tests/ServiceTests/Implementations/UsuarioServiceTest.cs`
- **Test de Repositorio:** `Tests/RepositoryTests/Implementations/UsuarioRepositoryTest.cs`

#### Criterios de Aceptación No Funcionales:

- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar la funcionalidad principal de cada historia de usuario.
- **Control de Excepciones:** Se debe manejar adecuadamente errores específicos relacionados con cada historia de usuario, utilizando las clases de excepción disponibles en la carpeta `Shared/Exceptions`.
- **Usabilidad:** La interfaz debe ser intuitiva y accesible para todos los usuarios, con validaciones en tiempo real donde sea aplicable.

---

### HU6 Creación de Préstamo

#### Descripción:

Como administrador del sistema, quiero registrar nuevos préstamos de equipos a usuarios para llevar un control de los equipos prestados.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema permite crear un préstamo asociando grupos de equipos a un usuario.
- **Datos de Entrada:** IDs de grupos de equipo, fechas esperadas, observaciones, carnet de usuario, contrato opcional.
- **Datos de Salida:** Confirmación de creación exitosa o mensaje de error específico.

#### Evidencia en Código y Tests:

- **Controlador:** `src/Presentations/Controllers/PrestamoController.cs`
- **Comando de creación:** `src/Presentations/Commands/Prestamo/CrearPrestamoComando.cs`
- **Servicio:** `src/Application/Services/Implementations/PrestamoService.cs` (método `CrearPrestamo(CrearPrestamoComando comando)`)
- **DTO:** `src/Application/DTOs/PrestamoDto.cs`
- **Test de Controlador:** `Tests/ControllerTests/Implementations/PrestamoControllerTest.cs`
- **Test de Servicio:** `Tests/ServiceTests/Implementations/PrestamoServiceTest.cs`
- **Test de Repositorio:** `Tests/RepositoryTests/Implementations/PrestamoRepositoryTest.cs`

#### Criterios de Aceptación No Funcionales:

- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar la funcionalidad principal de cada historia de usuario.
- **Control de Excepciones:** Se debe manejar adecuadamente errores específicos relacionados con cada historia de usuario, utilizando las clases de excepción disponibles en la carpeta `Shared/Exceptions`.
- **Usabilidad:** La interfaz debe ser intuitiva y accesible para todos los usuarios, con validaciones en tiempo real donde sea aplicable.

---

### HU7 Obtención de Préstamos

#### Descripción:

Como administrador del sistema, quiero obtener la lista de todos los préstamos registrados para poder monitorear y gestionar los préstamos activos e históricos.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema retorna la lista completa de préstamos.
- **Flujos Alternativos:** Si no hay préstamos registrados, retorna una lista vacía.
- **Datos de Salida:** Lista de préstamos con toda su información relacionada.

#### Evidencia en Código y Tests:

- **Controlador:** `src/Presentations/Controllers/PrestamoController.cs`
- **Servicio:** `src/Application/Services/Implementations/PrestamoService.cs`
- **DTO:** `src/Application/DTOs/PrestamoDto.cs`
- **Test de Controlador:** `Tests/ControllerTests/Implementations/PrestamoControllerTest.cs`
- **Test de Servicio:** `Tests/ServiceTests/Implementations/PrestamoServiceTest.cs`
- **Test de Repositorio:** `Tests/RepositoryTests/Implementations/PrestamoRepositoryTest.cs`

#### Criterios de Aceptación No Funcionales:

- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar la funcionalidad principal de cada historia de usuario.
- **Control de Excepciones:** Se debe manejar adecuadamente errores específicos relacionados con cada historia de usuario, utilizando las clases de excepción disponibles en la carpeta `Shared/Exceptions`.
- **Usabilidad:** La interfaz debe ser intuitiva y accesible para todos los usuarios, con validaciones en tiempo real donde sea aplicable.

---

### HU8 Eliminación Lógica de Préstamo

#### Descripción:

Como administrador del sistema, quiero eliminar préstamos del sistema para corregir registros erróneos o cancelar préstamos.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema permite eliminar lógicamente un préstamo por su ID.
- **Datos de Entrada:** ID del préstamo a eliminar.
- **Datos de Salida:** Confirmación de eliminación exitosa o mensaje de error específico.

#### Evidencia en Código y Tests:

- **Controlador:** `src/Presentations/Controllers/PrestamoController.cs`
- **Servicio:** `src/Application/Services/Implementations/PrestamoService.cs`
- **DTO:** `src/Application/DTOs/PrestamoDto.cs`
- **Test de Controlador:** `Tests/ControllerTests/Implementations/PrestamoControllerTest.cs`
- **Test de Servicio:** `Tests/ServiceTests/Implementations/PrestamoServiceTest.cs`
- **Test de Repositorio:** `Tests/RepositoryTests/Implementations/PrestamoRepositoryTest.cs`

#### Criterios de Aceptación No Funcionales:

- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar la funcionalidad principal de cada historia de usuario.
- **Control de Excepciones:** Se debe manejar adecuadamente errores específicos relacionados con cada historia de usuario, utilizando las clases de excepción disponibles en la carpeta `Shared/Exceptions`.
- **Usabilidad:** La interfaz debe ser intuitiva y accesible para todos los usuarios, con validaciones en tiempo real donde sea aplicable.

---

### HU10 Gestión de Equipos

#### Descripción:

Como administrador del sistema, quiero gestionar los equipos disponibles para préstamo para mantener un inventario actualizado.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema permite registrar, consultar, actualizar y eliminar lógicamente equipos.

#### Evidencia en Código y Tests:

- **Controlador:** `src/Presentations/Controllers/EquipoController.cs`
- **Servicio:** `src/Application/Services/Implementations/EquipoService.cs`
- **Test:** `Tests/ControllerTests/Implementations/EquipoControllerTest.cs`

#### Criterios de Aceptación No Funcionales:

- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar la funcionalidad principal de cada historia de usuario.
- **Control de Excepciones:** Se debe manejar adecuadamente errores específicos relacionados con cada historia de usuario, utilizando las clases de excepción disponibles en la carpeta `Shared/Exceptions`.
- **Usabilidad:** La interfaz debe ser intuitiva y accesible para todos los usuarios, con validaciones en tiempo real donde sea aplicable.

---

### HU11 Gestión de Grupos de Equipos

#### Descripción:

Como administrador del sistema, quiero gestionar grupos de equipos para clasificar y organizar los equipos por modelos y tipos similares.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema permite crear, consultar, actualizar y eliminar lógicamente grupos de equipos.

#### Evidencia en Código y Tests:

- **Controlador:** `src/Presentations/Controllers/GrupoEquipoController.cs`
- **Servicio:** `src/Application/Services/Implementations/GrupoEquipoService.cs`
- **DTO:** `src/Application/DTOs/GrupoEquipoDto.cs`
- **Test de Controlador:** `Tests/ControllerTests/Implementations/GrupoEquipoControllerTest.cs`
- **Test de Servicio:** `Tests/ServiceTests/Implementations/GrupoEquipoServiceTest.cs`
- **Test de Repositorio:** `Tests/RepositoryTests/Implementations/GrupoEquipoRepositoryTest.cs`

#### Criterios de Aceptación No Funcionales:

- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar la funcionalidad principal de cada historia de usuario.
- **Control de Excepciones:** Se debe manejar adecuadamente errores específicos relacionados con cada historia de usuario, utilizando las clases de excepción disponibles en la carpeta `Shared/Exceptions`.
- **Usabilidad:** La interfaz debe ser intuitiva y accesible para todos los usuarios, con validaciones en tiempo real donde sea aplicable.

---

### HU12 Gestión de Categorías

#### Descripción:

Como administrador del sistema, quiero gestionar categorías para clasificar los grupos de equipos según su tipo y uso.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema permite crear, consultar, actualizar y eliminar lógicamente categorías de equipos.

#### Evidencia en Código y Tests:

- **Controlador:** `src/Presentations/Controllers/CategoriaController.cs`
- **Servicio:** `src/Application/Services/Implementations/CategoriaService.cs`
- **DTO:** `src/Application/DTOs/CategoriaDto.cs`
- **Test de Controlador:** `Tests/ControllerTests/Implementations/CategoriaControllerTest.cs`
- **Test de Servicio:** `Tests/ServiceTests/Implementations/CategoriaServiceTest.cs`
- **Test de Repositorio:** `Tests/RepositoryTests/Implementations/CategoriaRepositoryTest.cs`

#### Criterios de Aceptación No Funcionales:

- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar la funcionalidad principal de cada historia de usuario.
- **Control de Excepciones:** Se debe manejar adecuadamente errores específicos relacionados con cada historia de usuario, utilizando las clases de excepción disponibles en la carpeta `Shared/Exceptions`.
- **Usabilidad:** La interfaz debe ser intuitiva y accesible para todos los usuarios, con validaciones en tiempo real donde sea aplicable.

---

### HU13 Gestión de Accesorios

#### Descripción:

Como administrador del sistema, quiero gestionar los accesorios de equipos para mantener un registro de los complementos disponibles para préstamo.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema permite crear, consultar, actualizar y eliminar lógicamente accesorios.

#### Evidencia en Código y Tests:

- **Controlador:** `src/Presentations/Controllers/AccesorioController.cs`
- **Servicio:** `src/Application/Services/Implementations/AccesorioService.cs`
- **DTO:** `src/Application/DTOs/AccesorioDto.cs`
- **Test de Controlador:** `Tests/ControllerTests/Implementations/AccesorioControllerTest.cs`
- **Test de Servicio:** `Tests/ServiceTests/Implementations/AccesorioServiceTest.cs`
- **Test de Repositorio:** `Tests/RepositoryTests/Implementations/AccesorioRepositoryTest.cs`

#### Criterios de Aceptación No Funcionales:

- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar la funcionalidad principal de cada historia de usuario.
- **Control de Excepciones:** Se debe manejar adecuadamente errores específicos relacionados con cada historia de usuario, utilizando las clases de excepción disponibles en la carpeta `Shared/Exceptions`.
- **Usabilidad:** La interfaz debe ser intuitiva y accesible para todos los usuarios, con validaciones en tiempo real donde sea aplicable.

---

### HU14 Gestión de Componentes

#### Descripción:

Como administrador del sistema, quiero gestionar componentes de equipos para llevar un registro detallado de las partes que componen cada equipo.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema permite crear, consultar, actualizar y eliminar lógicamente componentes.

#### Evidencia en Código y Tests:

- **Controlador:** `src/Presentations/Controllers/ComponenteController.cs`
- **Servicio:** `src/Application/Services/Implementations/ComponenteService.cs`
- **DTO:** `src/Application/DTOs/ComponenteDto.cs`
- **Test de Controlador:** `Tests/ControllerTests/Implementations/ComponenteControllerTest.cs`
- **Test de Servicio:** `Tests/ServiceTests/Implementations/ComponenteServiceTest.cs`
- **Test de Repositorio:** `Tests/RepositoryTests/Implementations/ComponenteRepositoryTest.cs`

#### Criterios de Aceptación No Funcionales:

- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar la funcionalidad principal de cada historia de usuario.
- **Control de Excepciones:** Se debe manejar adecuadamente errores específicos relacionados con cada historia de usuario, utilizando las clases de excepción disponibles en la carpeta `Shared/Exceptions`.
- **Usabilidad:** La interfaz debe ser intuitiva y accesible para todos los usuarios, con validaciones en tiempo real donde sea aplicable.

---

### HU15 Gestión de Mantenimientos

#### Descripción:

Como administrador del sistema, quiero registrar y consultar los mantenimientos realizados a los equipos para llevar un histórico de intervenciones y estado de los equipos.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema permite crear, consultar, actualizar y eliminar lógicamente registros de mantenimiento.

#### Evidencia en Código y Tests:

- **Controlador:** `src/Presentations/Controllers/MantenimientoController.cs`
- **Servicio:** `src/Application/Services/Implementations/MantenimientoService.cs`
- **DTO:** `src/Application/DTOs/MantenimientoDto.cs`
- **Test de Controlador:** `Tests/ControllerTests/Implementations/MantenimientoControllerTest.cs`
- **Test de Servicio:** `Tests/ServiceTests/Implementations/MantenimientoServiceTest.cs`
- **Test de Repositorio:** `Tests/RepositoryTests/Implementations/MantenimientoRepositoryTest.cs`

#### Criterios de Aceptación No Funcionales:

- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar la funcionalidad principal de cada historia de usuario.
- **Control de Excepciones:** Se debe manejar adecuadamente errores específicos relacionados con cada historia de usuario, utilizando las clases de excepción disponibles en la carpeta `Shared/Exceptions`.
- **Usabilidad:** La interfaz debe ser intuitiva y accesible para todos los usuarios, con validaciones en tiempo real donde sea aplicable.

---

### HU16 Panel de Administración

#### Descripción:

Como administrador del sistema, quiero acceder a un panel centralizado que me permita gestionar todas las entidades del sistema (usuarios, préstamos, equipos, etc.) para facilitar la administración general.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe mostrar un panel con acceso a todas las funcionalidades administrativas (usuarios, préstamos, equipos, grupos de equipos, categorías, accesorios, componentes, mantenimientos).
- **Flujos Alternativos:** Solo usuarios con rol de administrador pueden acceder.
- **Datos de Salida:** Panel con menús de navegación a todas las funcionalidades administrativas.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** La interfaz debe ser intuitiva y organizada por categorías lógicas.
- **Seguridad:** Solo usuarios autenticados con rol de administrador pueden acceder.

#### Evidencia en Código:

- **Archivos relevantes:** Componentes en la carpeta `admin_modulo/administrador` (panel), `admin_modulo/usuarios`, `admin_modulo/prestamos`, `admin_modulo/equipos`, `admin_modulo/grupos_equipos`, `admin_modulo/categorias`, `admin_modulo/accesorios`, `admin_modulo/componentes`, `admin_modulo/mantenimientos`
- **Test de Controlador:** `Tests/ControllerTests/Implementations/UsuarioControllerTest.cs`, `Tests/ControllerTests/Implementations/PrestamoControllerTest.cs`, `Tests/ControllerTests/Implementations/EquipoControllerTest.cs`, `Tests/ControllerTests/Implementations/GrupoEquipoControllerTest.cs`, `Tests/ControllerTests/Implementations/CategoriaControllerTest.cs`, `Tests/ControllerTests/Implementations/AccesorioControllerTest.cs`, `Tests/ControllerTests/Implementations/ComponenteControllerTest.cs`, `Tests/ControllerTests/Implementations/MantenimientoControllerTest.cs`
- **Test de Servicio:** `Tests/ServiceTests/Implementations/UsuarioServiceTest.cs`, `Tests/ServiceTests/Implementations/PrestamoServiceTest.cs`, `Tests/ServiceTests/Implementations/EquipoServiceTest.cs`, `Tests/ServiceTests/Implementations/GrupoEquipoServiceTest.cs`, `Tests/ServiceTests/Implementations/CategoriaServiceTest.cs`, `Tests/ServiceTests/Implementations/AccesorioServiceTest.cs`, `Tests/ServiceTests/Implementations/ComponenteServiceTest.cs`, `Tests/ServiceTests/Implementations/MantenimientoServiceTest.cs`
- **Test de Repositorio:** `Tests/RepositoryTests/Implementations/UsuarioRepositoryTest.cs`, `Tests/RepositoryTests/Implementations/PrestamoRepositoryTest.cs`, `Tests/RepositoryTests/Implementations/EquipoRepositoryTest.cs`, `Tests/RepositoryTests/Implementations/GrupoEquipoRepositoryTest.cs`, `Tests/RepositoryTests/Implementations/CategoriaRepositoryTest.cs`, `Tests/RepositoryTests/Implementations/AccesorioRepositoryTest.cs`, `Tests/RepositoryTests/Implementations/ComponenteRepositoryTest.cs`, `Tests/RepositoryTests/Implementations/MantenimientoRepositoryTest.cs`

---

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

- **Archivos relevantes:** Componentes en la carpeta `cliente_modulo/catalogo`, servicio `src/Client/app/services/equipo-catalogo.service.ts`
- **Test:** `Client/src/app/componentes/catalogo/catalogo.component.spec.ts` (pruebas unitarias de visualización de catálogo), `Client/src/app/services/equipo-catalogo.service.spec.ts` (pruebas de servicio de catálogo de equipos)

---

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
- **Test:** `Client/src/app/componentes/mantenimientos/mantenimientos.component.spec.ts` (pruebas unitarias de gestión de mantenimientos), `Client/src/app/services/mantenimientos.service.spec.ts` (pruebas de servicio de mantenimientos)

---

### HU19 Búsqueda Avanzada de Equipos

#### Descripción:

Como usuario del sistema, quiero utilizar filtros básicos en la búsqueda de equipos para encontrar lo que necesito.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema permite buscar equipos por categoría y producto.
- **Flujos Alternativos:** Si no hay resultados, se debe mostrar un mensaje apropiado.
- **Datos de Entrada:** Criterios de búsqueda (categoría, producto).
- **Datos de Salida:** Lista filtrada de equipos que cumplen con los criterios.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** Los filtros deben ser intuitivos y fáciles de usar.

#### Evidencia en Código:

- **Archivos relevantes:** Servicios en la carpeta `src/Client/app/services/buscador`, componentes relacionados en `cliente_modulo/buscador`
- **Test:** `Client/src/app/services/buscador/buscador.service.spec.ts` (pruebas unitarias básicas de creación del servicio)

---

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
- **Test:** `Client/src/app/componentes/equipo/detalle-equipo/detalle-equipo.component.spec.ts` (pruebas unitarias de visualización de detalles), `Client/src/app/services/equipo-catalogo.service.spec.ts` (pruebas de servicio de detalles de equipo)

---

### HU21 Solicitud de Préstamo desde Interfaz

#### Descripción:

Como usuario del sistema, quiero completar el proceso de solicitud de préstamo de equipos a través de una interfaz básica para reservar los recursos que necesito.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema permite crear nuevos préstamos utilizando datos del carrito y el carnet del usuario.
- **Flujos Alternativos:** No se implementa verificación de disponibilidad ni manejo de errores específicos.
- **Datos de Entrada:** Equipos seleccionados, carnet de usuario, contrato opcional.
- **Datos de Salida:** Confirmación de creación de préstamo.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** El proceso debe ser sencillo y directo.

#### Evidencia en Código:

- **Archivos relevantes:** Servicio `Client/src/app/services/APIS/prestamo/prestamos-api.service.ts`

---

### HU22 Visualización del Historial de Préstamos

#### Descripción:

Como usuario del sistema, quiero ver mi historial completo de préstamos para llevar un seguimiento de mis solicitudes pasadas y actuales.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema permite visualizar diferentes estados de préstamos (Activo, Aprobado, Pendiente, Rechazado, Finalizado, Cancelado).
- **Flujos Alternativos:** Si no hay préstamos registrados, se debe mostrar un mensaje apropiado.
- **Datos de Salida:** Lista de estados de préstamos disponibles.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** Los estados deben mostrarse organizados y ser accesibles mediante clics.

#### Evidencia en Código:

- **Archivos relevantes:** Componente `Client/src/app/componentes/usuario/historial/historial.component.ts`

---

### HU23 Control de Acceso Basado en Roles

#### Descripción:

Como administrador del sistema, quiero que las funcionalidades estén restringidas según el rol del usuario para mantener la seguridad y el control del sistema.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema incluye navegación básica pero no implementa restricciones específicas basadas en roles.
- **Flujos Alternativos:** No se implementa lógica para redirigir o notificar en caso de acceso no autorizado.
- **Datos de Entrada:** Ninguno.
- **Datos de Salida:** Navegación básica sin restricciones.

#### Criterios de Aceptación No Funcionales:
- **Seguridad:** Las restricciones de acceso deben ser verificadas en cada solicitud para evitar accesos no autorizados.

---

### HU24 Sistema de Notificaciones

#### Descripción:
Como usuario del sistema, quiero recibir y gestionar notificaciones relacionadas con mis préstamos para estar informado sobre eventos importantes.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema permite:
  - Crear notificaciones.
  - Obtener notificaciones por usuario.
  - Eliminar notificaciones.
  - Marcar notificaciones como leídas.
  - Verificar si hay notificaciones no leídas.
- **Flujos Alternativos:**
  - Las notificaciones pueden ser generadas manualmente o automáticamente.
  - Si no hay notificaciones, se retorna una lista vacía.

#### Datos de Entrada:
- Carnet de usuario.
- Título y contenido de la notificación.
- Identificador de notificación.

#### Datos de Salida:
- Confirmación de creación, eliminación o marcado como leído.
- Listado de notificaciones.
- Estado de notificaciones no leídas.

#### Evidencia en Código y Tests:

- **Controlador:** `src/Presentations/Controllers/NotificacionController.cs`
- **Servicio:** `src/Application/Services/Implementations/NotificacionService.cs`
- **Test de Controlador:** `Tests/ControllerTests/Implementations/NotificacionControllerTest.cs`
- **Test de Servicio:** `Tests/ServiceTests/Implementations/NotificacionServiceTest.cs`
- **Test de Repositorio:** `Tests/RepositoryTests/Implementations/NotificacionRepositoryTest.cs`

#### Criterios de Aceptación No Funcionales:
- **Usabilidad:** Los mensajes deben ser claros y accesibles, con opciones para marcar como leídos o eliminar.
- **Escalabilidad:** El sistema debe manejar grandes volúmenes de notificaciones sin degradar el rendimiento.

---

### HU25 Navegación y Experiencia de Usuario

#### Descripción:

Como usuario del sistema, quiero una interfaz de navegación intuitiva con una barra lateral y superior para acceder fácilmente a todas las funcionalidades disponibles según mi rol.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe proporcionar barras de navegación claras que muestren todas las opciones disponibles según el rol del usuario.
- **Flujos Alternativos:** El menú debe adaptarse dinámicamente según el rol del usuario logueado.
- **Datos de Salida:** Menú de navegación con enlaces a todas las funcionalidades permitidas.

#### Criterios de Aceptación No Funcionales:

- **Usabilidad:** La navegación debe ser intuitiva, con iconos y etiquetas descriptivas.

#### Evidencia en Código:

- **Archivos relevantes:** `navbar.component.ts`

---

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

- **Archivos relevantes:** `mostrarerror.component.ts`

---

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
- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar la creación, edición y eliminación de grupos de equipos.
- **Control de Excepciones:** Se debe manejar adecuadamente errores como campos obligatorios vacíos o formatos de datos inválidos.

#### Evidencia en Código:

- **Controlador:** `GrupoEquipoController.cs`
- **Test:** `GrupoEquipoControllerTest.cs` (pruebas de obtención de grupos de equipos).

---

### HU28 Gestión de Imágenes de Grupos de Equipos

#### Descripción:

Como administrador del sistema, quiero cargar, modificar y eliminar imágenes para los grupos de equipos para permitir a los usuarios identificar visualmente los equipos disponibles.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** No se implementa la funcionalidad para gestionar imágenes de grupos de equipos.
- **Flujos Alternativos:** No se maneja lógica para errores en la carga de imágenes.
- **Validaciones:**
  - Solo se permiten formatos de imagen comunes (JPG, PNG, etc.).
- **Datos de Entrada:** Archivos de imagen seleccionados por el usuario.
- **Datos de Salida:** No se generan confirmaciones ni previsualizaciones de imágenes.

#### Criterios de Aceptación No Funcionales:
- **Usabilidad:** La interfaz debe permitir una carga intuitiva de imágenes con previsualización antes de confirmar.
- **Accesibilidad:** Los botones y opciones deben ser claramente visibles y accesibles para usuarios con discapacidades visuales.
- **Compatibilidad:** La funcionalidad debe ser compatible con navegadores modernos y dispositivos móviles.
- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar la carga de imágenes y la previsualización.
- **Control de Excepciones:** Se debe manejar adecuadamente errores como formatos de imagen no soportados o fallos en la carga.

---

### HU29 Visualización de Datos en Tablas Dinámicas

#### Descripción:

Como usuario del sistema, quiero ver los datos en tablas interactivas que me permitan ordenar, filtrar y paginar la información para encontrar fácilmente lo que necesito.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe presentar datos tabulares con funcionalidades de ordenamiento, filtrado y paginación.
- **Flujos Alternativos:** Si no hay datos que mostrar, se debe presentar un mensaje claro.
- **Datos de Entrada:** Criterios de filtrado y ordenamiento definidos por el usuario.
- **Datos de Salida:** Datos organizados según las preferencias del usuario.

#### Criterios de Aceptación No Funcionales:
- **Usabilidad:** Las tablas deben incluir opciones claras para ordenar y filtrar datos.
- **Accesibilidad:** Los encabezados de las tablas deben ser etiquetados correctamente para lectores de pantalla.
- **Pruebas Unitarias:** Se deben realizar pruebas unitarias para validar el ordenamiento, filtrado y paginación de datos.
- **Control de Excepciones:** Se debe manejar adecuadamente errores como datos faltantes o criterios de filtrado inválidos.
