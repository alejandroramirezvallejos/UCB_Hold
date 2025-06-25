# Historias de Usuario

## Módulo Usuarios

### HU1 - Creación y Registro de Usuario desde Administración

#### Descripción
Como administrador, quiero registrar nuevos usuarios con sus datos personales y académicos para permitirles acceder y utilizar el sistema de préstamos.

#### Criterios de Aceptación Funcionales
- El sistema permite crear un usuario registrando: carnet de identidad, nombre, apellido paterno, apellido materno, email institucional (@ucb.edu.bo), contraseña, carrera, teléfono, rol (Estudiante/Administrador) y datos de referencia opcionales (nombre, teléfono y email de referencia).
- Si existe un usuario con el mismo carnet o email, el sistema rechaza la creación.
- Validaciones:
  - Email debe tener formato válido (@ucb.edu.bo).
  - Ningún campo obligatorio vacío.
  - Longitud máxima de 50 caracteres en todos los campos (excepto teléfonos: 6-10 caracteres, y teléfono principal: 8-10 caracteres).
- Datos de entrada: Carnet, nombre, apellidos, carrera, email, teléfono, datos de referencia.
- Datos de salida: Confirmación de creación o mensaje de error específico.

#### Excepciones y Validaciones (ubicación: `src/Shared/Exceptions/`)
- ErrorCarnetInvalido.cs
- ErrorCarnetRequerido.cs
- ErrorEmailInvalido.cs
- ErrorCampoRequerido.cs
- ErrorUsuarioYaExiste.cs
- ErrorLongitudMaxima.cs
- ErrorTelefonoInvalido.cs

#### Evidencia en Código y Tests (Backend)
- Controlador: `src/Presentations/Controllers/UsuarioController.cs`
- Servicio: `src/Application/Services/Implementations/UsuarioService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IUsuarioService.cs`
- Request DTO: `src/Application/Request DTOs/Usuario/CrearUsuarioComando.cs`
- Response DTO: `src/Application/Response DTOs/UsuarioDto.cs`
- Repositorio: `src/Infrastructure/Repositories/UsuarioRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IUsuarioRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorCarnetInvalido.cs`, `src/Shared/Exceptions/ErrorCarnetRequerido.cs`, `src/Shared/Exceptions/ErrorEmailInvalido.cs`, `src/Shared/Exceptions/ErrorCampoRequerido.cs`, `src/Shared/Exceptions/ErrorUsuarioYaExiste.cs`, `src/Shared/Exceptions/ErrorLongitudMaxima.cs`, `src/Shared/Exceptions/ErrorTelefonoInvalido.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/UsuarioControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/UsuarioServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/UsuarioRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/usuario/registro/registro.component.ts`
- Servicio: `Client/src/app/services/APIS/usuario/usuarios-api.service.ts`
- Modelo: `Client/src/app/models/usuario.ts`
- Test de componente: `Client/src/app/componentes/usuario/registro/registro.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/usuario/usuarios-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Robustez: Pruebas unitarias y de integración cubren los flujos principales y alternativos, incluyendo validaciones y manejo de excepciones. El sistema valida exhaustivamente los datos de entrada y muestra mensajes claros ante errores. 
- Experiencia de usuario: La interfaz es intuitiva, con validaciones en tiempo real y retroalimentación visual inmediata ante errores o éxito. Los mensajes de error son comprensibles y útiles.
- Seguridad: Las contraseñas nunca se exponen en los DTOs ni en las respuestas del backend. 
- Auditoría: Los usuarios solo se eliminan de forma lógica, nunca física, para mantener la trazabilidad.

---

### HU2 - Obtención de Usuarios

#### Descripción
Como administrador, quiero obtener la lista de todos los usuarios registrados para poder visualizar y gestionar los usuarios del sistema.

#### Criterios de Aceptación Funcionales
- El sistema retorna la lista completa de usuarios activos (no eliminados).
- Si no hay usuarios registrados, retorna mensaje: "No se encontraron usuarios".
- Datos de salida: Lista de usuarios con datos no sensibles.

#### Excepciones y Validaciones
- ErrorUsuarioNoEncontrado.cs

#### Evidencia en Código y Tests (Backend)
- Controlador: `src/Presentations/Controllers/UsuarioController.cs`
- Servicio: `src/Application/Services/Implementations/UsuarioService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IUsuarioService.cs`
- Response DTO: `src/Application/Response DTOs/UsuarioDto.cs`
- Repositorio: `src/Infrastructure/Repositories/UsuarioRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IUsuarioRepository.cs`
- Excepción: `src/Shared/Exceptions/ErrorUsuarioNoEncontrado.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/UsuarioControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/UsuarioServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/UsuarioRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/usuarios/usuarios.component.ts`
- Servicio: `Client/src/app/services/APIS/usuario/usuarios-api.service.ts`
- Modelo: `Client/src/app/models/usuario.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/usuarios/usuarios.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/usuario/usuarios-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Robustez: Pruebas unitarias y de integración aseguran la correcta obtención de usuarios y el manejo adecuado de excepciones cuando no existan registros. El backend filtra correctamente los usuarios eliminados lógicamente.
- Experiencia de usuario: Listados claros, navegación fluida y acciones accesibles para editar o eliminar usuarios. Mensajes informativos si no hay usuarios.
- Seguridad: Los DTOs nunca exponen contraseñas ni datos sensibles. El backend protege los endpoints para que solo usuarios autorizados accedan a la información.
- Auditoría: Los usuarios solo se eliminan de forma lógica, nunca física.

---

### HU3 - Actualización de Perfil de Usuario

#### Descripción
Como administrador o usuario registrado, quiero actualizar mis datos personales para mantener mi información actualizada en el sistema.

#### Criterios de Aceptación Funcionales
- El sistema permite actualizar los datos propios de un usuario existente por carnet.
- Si no se ha iniciado sesión, el sistema rechaza la actualización.
- Validaciones:
  - El carnet no puede ser modificado.
  - El email debe tener formato válido si se proporciona.
- Datos de entrada: Cualquier campo a actualizar.
- Datos de salida: Confirmación de actualización o mensaje de error.

#### Excepciones y Validaciones
- ErrorCarnetInvalido.cs
- ErrorEmailInvalido.cs
- ErrorUsuarioNoEncontrado.cs

#### Evidencia en Código y Tests (Backend)
- Controlador: `src/Presentations/Controllers/UsuarioController.cs`
- Servicio: `src/Application/Services/Implementations/UsuarioService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IUsuarioService.cs`
- Request DTO: `src/Application/Request DTOs/Usuario/ActualizarUsuarioComando.cs`
- Response DTO: `src/Application/Response DTOs/UsuarioDto.cs`
- Repositorio: `src/Infrastructure/Repositories/UsuarioRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IUsuarioRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorCarnetInvalido.cs`, `src/Shared/Exceptions/ErrorEmailInvalido.cs`, `src/Shared/Exceptions/ErrorUsuarioNoEncontrado.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/UsuarioControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/UsuarioServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/UsuarioRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/usuario/perfil/perfil.component.ts`
- Servicio: `Client/src/app/services/APIS/usuario/usuarios-api.service.ts`
- Modelo: `Client/src/app/models/usuario.ts`
- Test de componente: `Client/src/app/componentes/usuario/perfil/perfil.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/usuario/usuarios-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Robustez: Pruebas unitarias y de integración validan la actualización, incluyendo validaciones de formato y manejo de errores. 
- Experiencia de usuario: Formularios amigables y mensajes claros ante errores o éxito. Cambios reflejados de inmediato en la interfaz. 
- Seguridad: Los DTOs no exponen datos sensibles. 
- Auditoría: Los usuarios solo se eliminan de forma lógica, nunca física.

---

### HU4 - Actualización de Usuario en modo Administrador

#### Descripción
Como administrador, quiero actualizar los datos de los usuarios para mantener la información actualizada en el sistema.

#### Criterios de Aceptación Funcionales
- El sistema permite actualizar los datos de un usuario existente por carnet (botón interno).
- Validaciones:
  - El carnet no puede ser modificado.
  - El email debe tener formato válido si se proporciona.
  - El teléfono debe estar entre 8 y 10 caracteres.
- Datos de entrada: Cualquier campo a actualizar, incluido el rol.
- Datos de salida: Confirmación de actualización o mensaje de error.

#### Excepciones y Validaciones
- ErrorCarnetInvalido.cs
- ErrorEmailInvalido.cs
- ErrorTelefonoInvalido.cs
- ErrorUsuarioNoEncontrado.cs

#### Evidencia en Código y Tests (Backend)
- Controlador: `src/Presentations/Controllers/UsuarioController.cs`
- Servicio: `src/Application/Services/Implementations/UsuarioService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IUsuarioService.cs`
- Request DTO: `src/Application/Request DTOs/Usuario/ActualizarUsuarioComando.cs`
- Response DTO: `src/Application/Response DTOs/UsuarioDto.cs`
- Repositorio: `src/Infrastructure/Repositories/UsuarioRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IUsuarioRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorCarnetInvalido.cs`, `src/Shared/Exceptions/ErrorEmailInvalido.cs`, `src/Shared/Exceptions/ErrorTelefonoInvalido.cs`, `src/Shared/Exceptions/ErrorUsuarioNoEncontrado.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/UsuarioControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/UsuarioServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/UsuarioRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/usuarios/usuarios.component.ts`
- Servicio: `Client/src/app/services/APIS/usuario/usuarios-api.service.ts`
- Modelo: `Client/src/app/models/usuario.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/usuarios/usuarios.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/usuario/usuarios-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Robustez: Pruebas unitarias y de integración aseguran la actualización correcta y el manejo de errores, especialmente en validaciones de campos y roles. Excepciones controladas y mensajes claros. 
- Experiencia de usuario: Interfaz clara para edición y retroalimentación visual. Acceso rápido a la edición y confirmación de cambios.
- Seguridad: Los DTOs no exponen contraseñas ni datos sensibles. 
- Auditoría: Los usuarios solo se eliminan de forma lógica, nunca física.

---

### HU5 - Eliminación Lógica de Usuario

#### Descripción
Como administrador, quiero eliminar usuarios del sistema para gestionar los registros de usuarios que ya no deben tener acceso.

#### Criterios de Aceptación Funcionales
- El sistema permite eliminar lógicamente un usuario por carnet (botón interno).
- Datos de entrada: Carnet del usuario a eliminar.
- Datos de salida: Confirmación de eliminación o mensaje de error.

#### Excepciones y Validaciones
- ErrorCarnetInvalido.cs
- ErrorUsuarioNoEncontrado.cs

#### Evidencia en Código y Tests (Backend)
- Controlador: `src/Presentations/Controllers/UsuarioController.cs`
- Servicio: `src/Application/Services/Implementations/UsuarioService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IUsuarioService.cs`
- Request DTO: `src/Application/Request DTOs/Usuario/EliminarUsuarioComando.cs`
- Response DTO: `src/Application/Response DTOs/UsuarioDto.cs`
- Repositorio: `src/Infrastructure/Repositories/UsuarioRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IUsuarioRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorCarnetInvalido.cs`, `src/Shared/Exceptions/ErrorUsuarioNoEncontrado.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/UsuarioControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/UsuarioServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/UsuarioRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/usuarios/usuarios.component.ts`
- Servicio: `Client/src/app/services/APIS/usuario/usuarios-api.service.ts`
- Modelo: `Client/src/app/models/usuario.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/usuarios/usuarios.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/usuario/usuarios-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Robustez: Pruebas unitarias y de integración validan la eliminación lógica y el manejo de errores, asegurando que no se pierdan datos críticos. Excepciones bien gestionadas.
- Experiencia de usuario: Confirmaciones antes de eliminar, mensajes claros de éxito o error y actualización inmediata del listado de usuarios.
- Seguridad: Solo usuarios autorizados pueden eliminar usuarios. El backend nunca elimina físicamente los datos, solo los marca como eliminados.
- Auditoría: Los usuarios solo se eliminan de forma lógica, nunca física.

---

## Módulo Préstamos

### HU6 - Solicitar Préstamo

#### Descripción
Como usuario, quiero solicitar préstamos de equipos para usarlos en mis actividades curriculares por un periodo de tiempo limitado.

#### Criterios de Aceptación Funcionales
- El sistema permite un préstamo associando equipos a un usuario.
- Datos de entrada: cantidad de equipos, grupo equipo, fecha inicio y devolución esperada (máximo 1 año), carnet del usuario.
- El usuario debe firmar un contrato de préstamo.
- Datos de salida: Confirmación de creación o mensaje de error.

#### Excepciones y Validaciones
- ErrorFechaInvalida.cs
- ErrorPrestamoYaExiste.cs
- ErrorUsuarioNoEncontrado.cs
- ErrorEquipoNoDisponible.cs

#### Evidencia en Código y Tests (Backend)
- Controlador: `src/Presentations/Controllers/PrestamoController.cs`
- Servicio: `src/Application/Services/Implementations/PrestamoService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IPrestamoService.cs`
- Request DTO: `src/Application/Request DTOs/Prestamo/CrearPrestamoComando.cs`
- Response DTO: `src/Application/Response DTOs/PrestamoDto.cs`
- Repositorio: `src/Infrastructure/Repositories/PrestamoRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IPrestamoRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorFechaInvalida.cs`, `src/Shared/Exceptions/ErrorPrestamoYaExiste.cs`, `src/Shared/Exceptions/ErrorUsuarioNoEncontrado.cs`, `src/Shared/Exceptions/ErrorEquipoNoDisponible.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/PrestamoControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/PrestamoServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/PrestamoRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/cliente_modulo/solicitud/solicitud.component.ts`
- Servicio: `Client/src/app/services/APIS/prestamo/prestamos-api.service.ts`
- Modelo: `Client/src/app/models/prestamo.ts`
- Test de componente: `Client/src/app/componentes/cliente_modulo/solicitud/solicitud.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/prestamo/prestamos-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Robustez: Pruebas unitarias y de integración validan la solicitud, manejo de fechas, disponibilidad y errores. Excepciones controladas y mensajes claros ante cualquier fallo. 
- Experiencia de usuario: Formularios claros, validaciones en tiempo real y confirmación visual de la solicitud. El usuario recibe notificaciones del estado de su solicitud.
- Seguridad: El backend valida que el usuario esté autenticado para solicitar préstamos. Los DTOs no exponen datos sensibles. 
- Auditoría: Los préstamos solo se eliminan de forma lógica, nunca física.

---

### HU7 - Obtención de Préstamos

#### Descripción
Como administrador, quiero obtener la lista de todos los préstamos registrados para monitorear y gestionar los préstamos activos e históricos.

#### Criterios de Aceptación Funcionales
- El sistema retorna la lista completa de préstamos, con filtro por estado (Pendiente, Rechazado, Aprobado, Activo, Finalizado, Cancelado).
- Si no hay préstamos registrados, retorna mensaje: "No hay préstamos disponibles".
- Datos de salida: Lista de préstamos con toda su información relacionada.

#### Excepciones y Validaciones
- ErrorPrestamoNoEncontrado.cs

#### Evidencia en Código y Tests (Backend)
- Controlador: `src/Presentations/Controllers/PrestamoController.cs`
- Servicio: `src/Application/Services/Implementations/PrestamoService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IPrestamoService.cs`
- Response DTO: `src/Application/Response DTOs/PrestamoDto.cs`
- Repositorio: `src/Infrastructure/Repositories/PrestamoRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IPrestamoRepository.cs`
- Excepción: `src/Shared/Exceptions/ErrorPrestamoNoEncontrado.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/PrestamoControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/PrestamoServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/PrestamoRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/prestamos/prestamos.component.ts`
- Servicio: `Client/src/app/services/APIS/prestamo/prestamos-api.service.ts`
- Modelo: `Client/src/app/models/prestamo.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/prestamos/prestamos.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/prestamo/prestamos-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Robustez: Pruebas unitarias y de integración aseguran la correcta obtención y filtrado de préstamos, así como el manejo de errores y excepciones.
- Experiencia de usuario: Listados organizados, filtros accesibles y mensajes informativos si no hay préstamos.
- Seguridad: Solo usuarios autorizados pueden ver los préstamos. Los DTOs no exponen datos sensibles.
- Auditoría: Los préstamos solo se eliminan de forma lógica, nunca física.

---

### HU8 - Eliminación Lógica de Préstamo

#### Descripción
Como administrador, quiero eliminar préstamos del sistema para corregir registros erróneos o cancelar préstamos.

#### Criterios de Aceptación Funcionales
- El sistema permite eliminar lógicamente un préstamo (botón junto al préstamo).
- Datos de entrada: ID del préstamo a eliminar.
- Datos de salida: Confirmación de eliminación o mensaje de error.

#### Excepciones y Validaciones
- ErrorPrestamoNoEncontrado.cs

#### Evidencia en Código y Tests (Backend)
- Controlador: `src/Presentations/Controllers/PrestamoController.cs`
- Servicio: `src/Application/Services/Implementations/PrestamoService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IPrestamoService.cs`
- Request DTO: `src/Application/Request DTOs/Prestamo/EliminarPrestamoComando.cs`
- Response DTO: `src/Application/Response DTOs/PrestamoDto.cs`
- Repositorio: `src/Infrastructure/Repositories/PrestamoRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IPrestamoRepository.cs`
- Excepción: `src/Shared/Exceptions/ErrorPrestamoNoEncontrado.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/PrestamoControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/PrestamoServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/PrestamoRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/prestamos/prestamos.component.ts`
- Servicio: `Client/src/app/services/APIS/prestamo/prestamos-api.service.ts`
- Modelo: `Client/src/app/models/prestamo.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/prestamos/prestamos.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/prestamo/prestamos-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Robustez: Pruebas unitarias y de integración validan la eliminación lógica y el manejo de errores, asegurando la integridad de los datos y el control de excepciones. 
- Experiencia de usuario: Confirmaciones previas, mensajes claros y actualización inmediata del listado de préstamos. 
- Seguridad: Solo usuarios autorizados pueden eliminar préstamos. El backend nunca elimina físicamente los datos, solo los marca como eliminados.
- Auditoría: Los préstamos solo se eliminan de forma lógica, nunca física.

---

## Módulo Equipos y Grupos de Equipos

### HU9 - Gestión de Equipos

#### Descripción
Como administrador, quiero gestionar los equipos disponibles para préstamo para mantener un inventario actualizado.

#### Criterios de Aceptación Funcionales
- El sistema permite registrar, consultar, actualizar y eliminar lógicamente equipos.
- Los datos obligatorios son: Grupo de Equipo, Código UCB, Número de Serie, Ubicación y Nombre del Gavetero.
- Como datos opcionales se incluyen: Costo de Referencia, Tiempo Máximo de Préstamo (en días) y Descripción de la Procedencia.

#### Excepciones y Validaciones
- ErrorEquipoNoEncontrado.cs
- ErrorCampoRequerido.cs
- ErrorCategoriaNoEncontrada.cs

#### Evidencia en Código y Tests (Backend)
- Controlador: `src/Presentations/Controllers/EquipoController.cs`
- Servicio: `src/Application/Services/Implementations/EquipoService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IEquipoService.cs`
- Request DTO: `src/Application/Request DTOs/Equipo/CrearEquipoComando.cs`
- Response DTO: `src/Application/Response DTOs/EquipoDto.cs`
- Repositorio: `src/Infrastructure/Repositories/EquipoRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IEquipoRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorEquipoNoEncontrado.cs`, `src/Shared/Exceptions/ErrorCampoRequerido.cs`, `src/Shared/Exceptions/ErrorCategoriaNoEncontrada.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/EquipoControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/EquipoServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/EquipoRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/equipos/equipos.component.ts`
- Servicio: `Client/src/app/services/APIS/equipo/equipos-api.service.ts`
- Modelo: `Client/src/app/models/equipo.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/equipos/equipos.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/equipo/equipos-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Robustez: Pruebas unitarias y de integración validan la gestión de equipos, incluyendo validaciones de campos y manejo de excepciones. 
- Experiencia de usuario: Formularios intuitivos, validaciones en tiempo real y retroalimentación visual. 
- Seguridad: Solo usuarios autorizados pueden gestionar equipos. Los DTOs no exponen datos sensibles. 
- Auditoría: Los equipos solo se eliminan de forma lógica, nunca física.

---

### HU10 - Gestión de Grupos de Equipos

#### Descripción
Como administrador, quiero gestionar grupos de equipos para clasificar y organizar los equipos por modelos y tipos similares.

#### Criterios de Aceptación Funcionales
- El sistema permite crear, consultar, actualizar y eliminar lógicamente grupos de equipos.
- Datos obligatorios: Nombre, Modelo, Marca, Categoría, Descripción, equipo, URL Imagen. (URL Data Sheet es opcional).

#### Excepciones y Validaciones
- ErrorGrupoEquipoNoEncontrado.cs
- ErrorCampoRequerido.cs
- ErrorCategoriaNoEncontrada.cs

#### Evidencia en Código y Tests (Backend)
- Controlador: `src/Presentations/Controllers/GrupoEquipoController.cs`
- Servicio: `src/Application/Services/Implementations/GrupoEquipoService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IGrupoEquipoService.cs`
- Request DTO: `src/Application/Request DTOs/GrupoEquipo/CrearGrupoEquipoComando.cs`
- Response DTO: `src/Application/Response DTOs/GrupoEquipoDto.cs`
- Repositorio: `src/Infrastructure/Repositories/GrupoEquipoRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IGrupoEquipoRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorGrupoEquipoNoEncontrado.cs`, `src/Shared/Exceptions/ErrorCampoRequerido.cs`, `src/Shared/Exceptions/ErrorCategoriaNoEncontrada.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/GrupoEquipoControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/GrupoEquipoServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/GrupoEquipoRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/grupos_equipos/grupos-equipos.component.ts`
- Servicio: `Client/src/app/services/APIS/grupo-equipo/grupos-equipos-api.service.ts`
- Modelo: `Client/src/app/models/grupo_equipo.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/grupos_equipos/grupos-equipos.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/grupo-equipo/grupos-equipos-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Robustez: Pruebas unitarias y de integración validan la gestión de grupos de equipos, manejo de relaciones y excepciones. El sistema previene inconsistencias y maneja errores de forma controlada.
- Experiencia de usuario: Interfaz clara para crear, editar y eliminar grupos con navegación sencilla.
- Seguridad: Solo usuarios autorizados pueden gestionar grupos de equipos. Los DTOs no exponen datos sensibles. 
- Auditoría: Los grupos de equipos solo se eliminan de forma lógica, nunca física.

---
