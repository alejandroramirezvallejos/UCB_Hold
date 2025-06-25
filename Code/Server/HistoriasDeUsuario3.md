## Módulo Préstamos

### HU20 - Aceptar y Rechazar Solicitud de Préstamo desde Interfaz

#### Descripción
Como administrador del sistema, quiero completar el proceso de solicitud de préstamo de equipos a través de una interfaz simple y rápida.

#### Criterios de Aceptación Funcionales
- El sistema permite aceptar y rechazar solicitudes de préstamos con dos botones al lado de la solicitud en la lista de préstamos.
- Datos de entrada: Préstamo seleccionado.
- Datos de salida: Confirmación de préstamo aprobado o rechazado.

#### Excepciones y Validaciones (ubicación: `src/Shared/Exceptions/`)
- ErrorPrestamoNoEncontrado.cs
- ErrorEstadoPrestamoInvalido.cs

#### Evidencia en Código y Tests
- Servicio Frontend: `Client/src/app/services/APIS/prestamo/prestamos-api.service.ts`
- Controlador Backend: `src/Presentations/Controllers/PrestamoController.cs`
- Servicio Backend: `src/Application/Services/Implementations/PrestamoService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IPrestamoService.cs`
- Request DTO: `src/Application/Request DTOs/Prestamo/ActualizarEstadoPrestamoComando.cs`
- Response DTO: `src/Application/Response DTOs/PrestamoDto.cs`
- Repositorio: `src/Infrastructure/Repositories/PrestamoRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IPrestamoRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorPrestamoNoEncontrado.cs`, `src/Shared/Exceptions/ErrorEstadoPrestamoInvalido.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/PrestamoControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/PrestamoServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/PrestamoRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/prestamos/prestamos.component.ts`
- Servicio: `Client/src/app/services/APIS/prestamo/prestamos-api.service.ts`
- Modelo: `Client/src/app/models/PrestamoAgrupados.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/prestamos/prestamos.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/prestamo/prestamos-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Usabilidad: El proceso es sencillo y directo, con botones claros y navegación intuitiva para aceptar o rechazar solicitudes.
- Robustez: Existen pruebas unitarias y de integración en backend y frontend para asegurar el correcto cambio de estado y el manejo de excepciones. Los mensajes de error son claros.
- Seguridad: Solo usuarios administradores pueden realizar esta acción. Los DTOs no exponen datos sensibles.
- Auditoría: Los préstamos solo se eliminan de forma lógica, nunca física.

---

### HU21 - Visualización de Préstamos (Administrador)

#### Descripción
Como administrador del sistema, quiero visualizar los préstamos de los usuarios para llevar un seguimiento de las solicitudes pasadas y actuales.

#### Criterios de Aceptación Funcionales
- El sistema permite visualizar por cada préstamo: usuario (nombre y apellido paterno), carnet, teléfono, equipos, códigos IMT, fecha solicitud, fecha préstamo esperada, fecha devolución esperada, estado y acciones (rechazar, aceptar, ver contrato, eliminar préstamo).
- Se puede filtrar por estado de préstamo: Pendiente, Rechazado, Aprobado, Activo, Finalizado y Cancelado.

#### Excepciones y Validaciones
- ErrorPrestamoNoEncontrado.cs

#### Evidencia en Código y Tests
- Servicio Frontend: `Client/src/app/services/APIS/prestamo/prestamos-api.service.ts`
- Controlador Backend: `src/Presentations/Controllers/PrestamoController.cs`
- Servicio Backend: `src/Application/Services/Implementations/PrestamoService.cs`
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
- Modelo: `Client/src/app/models/PrestamoAgrupados.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/prestamos/prestamos.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/prestamo/prestamos-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Usabilidad: La visualización es clara, permite filtrar fácilmente por estado y las acciones están agrupadas de forma accesible.
- Robustez: Pruebas unitarias y de integración aseguran la correcta visualización, filtrado y manejo de excepciones. Los mensajes de error son claros.
- Seguridad: Solo usuarios administradores pueden acceder a esta vista. Los DTOs no exponen datos sensibles.
- Auditoría: Los préstamos solo se eliminan de forma lógica, nunca física.

---

### HU22 - Visualización del Historial de Préstamos (Usuario)

#### Descripción
Como usuario del sistema, quiero ver mi historial completo de préstamos para llevar un seguimiento de mis solicitudes pasadas y actuales.

#### Criterios de Aceptación Funcionales
- El sistema permite visualizar diferentes estados de préstamos (Activo, Aprobado, Pendiente, Rechazado, Finalizado, Cancelado).
- Si no hay préstamos registrados, se debe mostrar un mensaje apropiado.
- Para cada préstamo se puede visualizar: nombres de los grupos de equipos, nombre y apellido paterno del usuario, carnet, número de préstamo, códigos IMT de los equipos reservados, estado, devolución esperada, y acciones según estado (cancelar, marcar como recogido, devolver, etc.).

#### Excepciones y Validaciones
- ErrorPrestamoNoEncontrado.cs

#### Evidencia en Código y Tests
- Componente Frontend: `Client/src/app/componentes/usuario/historial/historial.component.ts`
- Servicio Backend: `src/Application/Services/Implementations/PrestamoService.cs`
- Controlador Backend: `src/Presentations/Controllers/PrestamoController.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IPrestamoService.cs`
- Response DTO: `src/Application/Response DTOs/PrestamoDto.cs`
- Repositorio: `src/Infrastructure/Repositories/PrestamoRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IPrestamoRepository.cs`
- Excepción: `src/Shared/Exceptions/ErrorPrestamoNoEncontrado.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/PrestamoControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/PrestamoServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/PrestamoRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/usuario/historial/historial.component.ts`
- Servicio: `Client/src/app/services/APIS/prestamo/prestamos-api.service.ts`
- Modelo: `Client/src/app/models/PrestamoAgrupados.ts`
- Test de componente: `Client/src/app/componentes/usuario/historial/historial.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/prestamo/prestamos-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Usabilidad: Los estados se muestran organizados y son accesibles mediante clics. La navegación es sencilla y los mensajes son claros si no hay préstamos.
- Robustez: Pruebas unitarias y de integración aseguran la correcta visualización y manejo de excepciones. Los mensajes de error son claros.
- Seguridad: Solo el usuario autenticado puede ver su historial. Los DTOs no exponen datos sensibles.
- Auditoría: Los préstamos solo se eliminan de forma lógica, nunca física.

---

## Módulo Seguridad y Control de Acceso

### HU23 - Control de Acceso Basado en Roles

#### Descripción
Como administrador del sistema, quiero que las funcionalidades estén restringidas según el rol del usuario para mantener la seguridad y el control del sistema.

#### Criterios de Aceptación Funcionales
- Solo usuarios con rol de administrador pueden acceder a las funciones administrativas.
- Todos los usuarios administradores tienen también acceso al área de cliente.

#### Excepciones y Validaciones
- ErrorAccesoNoAutorizado.cs

#### Evidencia en Código y Tests
- Guardas y servicios de autenticación en: `Client/src/app/services/auth/`, `Client/src/app/guards/`
- Excepción: `src/Shared/Exceptions/ErrorAccesoNoAutorizado.cs`
- Test de servicios: `Client/src/app/services/auth/auth.service.spec.ts`

#### Evidencia en Código y Tests (Frontend)
- Guard: `Client/src/app/guards/`
- Servicio: `Client/src/app/services/auth/auth.service.ts`
- Test de servicio: `Client/src/app/services/auth/auth.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Seguridad: El sistema verifica el rol del usuario antes de mostrar opciones administrativas. Los DTOs no exponen datos sensibles.
- Robustez: Pruebas unitarias aseguran el correcto funcionamiento de los guards y servicios de autenticación.
- Usabilidad: El sistema muestra menús y opciones de acuerdo al rol, facilitando la navegación y evitando confusiones.

---

## Módulo Notificaciones

### HU24 - Sistema de Notificaciones

#### Descripción
Como usuario del sistema, quiero recibir y gestionar notificaciones relacionadas con mis préstamos para estar informado sobre eventos importantes.

#### Criterios de Aceptación Funcionales
- El sistema permite ver la lista de notificaciones (título, contenido, fecha de envío), marcar como leídas, y verificar si hay no leídas.
- Las notificaciones son generadas automáticamente.
- Si no hay notificaciones, se muestra un mensaje apropiado.

#### Excepciones y Validaciones
- ErrorNotificacionNoEncontrada.cs

#### Evidencia en Código y Tests
- Controlador: `src/Presentations/Controllers/NotificacionController.cs`
- Servicio: `src/Application/Services/Implementations/NotificacionService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/INotificacionService.cs`
- Response DTO: `src/Application/Response DTOs/NotificacionDto.cs`
- Repositorio: `src/Infrastructure/Repositories/NotificacionRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/INotificacionRepository.cs`
- Excepción: `src/Shared/Exceptions/ErrorNotificacionNoEncontrada.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/NotificacionControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/NotificacionServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/NotificacionRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/notificaciones/notificaciones.component.ts`
- Servicio: `Client/src/app/services/APIS/Notificacion/notificacion.service.ts`
- Modelo: `Client/src/app/models/Notificacion.ts`
- Test de componente: `Client/src/app/componentes/notificaciones/notificaciones.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/Notificacion/notificacion.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Usabilidad: Mensajes claros y accesibles, con opciones visibles para marcar como leídos y navegación sencilla entre notificaciones.
- Robustez: Pruebas unitarias y de integración aseguran la correcta gestión de notificaciones y el manejo de excepciones.

---

## Módulo Perfil de Usuario

### HU25 - Ver Perfil

#### Descripción
Como usuario del sistema, quiero ver mi perfil para revisar y actualizar mi información personal y de contacto.

#### Criterios de Aceptación Funcionales
- El sistema permite a los usuarios ver su perfil con información personal: Nombre, Carnet, Apellido Paterno, Apellido Materno, Correo, Teléfono, Carrera, Nombre de Referencia, Teléfono de Referencia, Email de Referencia.

#### Excepciones y Validaciones
- ErrorUsuarioNoEncontrado.cs

#### Evidencia en Código y Tests
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

#### Criterios de Aceptación No Funcionales
- Usabilidad: Visualización clara y organizada de los datos personales, con fácil acceso a la información relevante.
- Robustez: Pruebas unitarias aseguran la correcta visualización y manejo de excepciones.
- Seguridad: Solo el usuario autenticado puede ver su perfil. Los DTOs no exponen datos sensibles.

---

## Módulo Administración de Inventario

### HU26 - Administración de Grupos de Equipos en Interfaz

#### Descripción
Como administrador del sistema, quiero gestionar los grupos de equipos a través de una interfaz gráfica para mantener organizado el inventario por tipos y modelos de equipos.

#### Criterios de Aceptación Funcionales
- La interfaz permite crear, editar, eliminar lógicamente y consultar grupos de equipos con todos sus detalles.
- Para cada grupo se puede ver: nombre, modelo, marca, categoría, descripción.
- Si un grupo tiene equipos asociados, el sistema advierte antes de su eliminación.
- Para crear un grupo de equipos, todos los campos son obligatorios: Nombre, Modelo, Marca, Categoría, Descripción, URL Data Sheet (opcional), URL Imagen (obligatorio).
- Al actualizar se puede modificar cualquier campo.

#### Excepciones y Validaciones
- ErrorCampoRequerido.cs
- ErrorGrupoEquipoNoEncontrado.cs
- ErrorCategoriaNoEncontrada.cs

#### Evidencia en Código y Tests
- Controlador: `src/Presentations/Controllers/GrupoEquipoController.cs`
- Servicio: `src/Application/Services/Implementations/GrupoEquipoService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IGrupoEquipoService.cs`
- Request DTO: `src/Application/Request DTOs/GrupoEquipo/CrearGrupoEquipoComando.cs`
- Response DTO: `src/Application/Response DTOs/GrupoEquipoDto.cs`
- Repositorio: `src/Infrastructure/Repositories/GrupoEquipoRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IGrupoEquipoRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorCampoRequerido.cs`, `src/Shared/Exceptions/ErrorGrupoEquipoNoEncontrado.cs`, `src/Shared/Exceptions/ErrorCategoriaNoEncontrada.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/GrupoEquipoControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/GrupoEquipoServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/GrupoEquipoRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/grupo-equipos/grupo-equipos.component.ts`
- Servicio: `Client/src/app/services/APIS/grupoEquipo/grupo-equipo-api.service.ts`
- Modelo: `Client/src/app/models/GrupoEquipo.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/grupo-equipos/grupo-equipos.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/grupoEquipo/grupo-equipo-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Usabilidad: Formularios comprensibles y organización lógica de los campos para facilitar la gestión de grupos de equipos.
- Robustez: Pruebas unitarias aseguran la correcta gestión de grupos de equipos y el manejo de excepciones.

---

### HU27 - Administración de Gaveteros

#### Descripción
Como administrador del sistema, quiero crear, eliminar, editar y ver la lista de los gaveteros.

#### Criterios de Aceptación Funcionales
- Para cada gavetero se puede ver: nombre, tipo, nombre de mueble, longitud, profundidad y altura.
- Para crear un gavetero: nombre, tipo y nombre de mueble son obligatorios (el mueble debe existir previamente).
- Se puede editar cualquier campo.

#### Excepciones y Validaciones
- ErrorCampoRequerido.cs
- ErrorMuebleNoEncontrado.cs
- ErrorLongitudMaxima.cs

#### Evidencia en Código y Tests
- Controlador: `src/Presentations/Controllers/GaveteroController.cs`
- Servicio: `src/Application/Services/Implementations/GaveteroService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IGaveteroService.cs`
- Request DTO: `src/Application/Request DTOs/Gavetero/CrearGaveteroComando.cs`
- Response DTO: `src/Application/Response DTOs/GaveteroDto.cs`
- Repositorio: `src/Infrastructure/Repositories/GaveteroRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IGaveteroRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorCampoRequerido.cs`, `src/Shared/Exceptions/ErrorMuebleNoEncontrado.cs`, `src/Shared/Exceptions/ErrorLongitudMaxima.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/GaveteroControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/GaveteroServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/GaveteroRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/gaveteros/gaveteros.component.ts`
- Servicio: `Client/src/app/services/APIS/gavetero/gavetero-api.service.ts`
- Modelo: `Client/src/app/models/Gavetero.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/gaveteros/gaveteros.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/gavetero/gavetero-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Usabilidad: Formularios comprensibles y organización lógica de los campos para facilitar la gestión de gaveteros.
- Robustez: Pruebas unitarias aseguran la correcta gestión de gaveteros y el manejo de excepciones.

---

### HU28 - Administración de Muebles

#### Descripción
Como administrador del sistema, quiero crear, eliminar, editar y ver la lista de muebles.

#### Criterios de Aceptación Funcionales
- Para cada mueble se puede ver: nombre, tipo, ubicación, costo, número de gaveteros, longitud, profundidad y altura.
- Para crear un mueble: nombre es obligatorio, los demás campos son opcionales (tipo, ubicación, costo, longitud, profundidad y altura).
- Para actualizar un mueble se puede modificar cualquier campo, excepto el número de gaveteros (no se puede cambiar ni al crear ni al actualizar).
- El costo y las dimensiones no pueden ser negativos.

#### Excepciones y Validaciones
- ErrorCampoRequerido.cs
- ErrorLongitudMaxima.cs
- ErrorValorNegativoNoPermitido.cs

#### Evidencia en Código y Tests
- Controlador: `src/Presentations/Controllers/MuebleController.cs`
- Servicio: `src/Application/Services/Implementations/MuebleService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IMuebleService.cs`
- Request DTO: `src/Application/Request DTOs/Mueble/CrearMuebleComando.cs`
- Response DTO: `src/Application/Response DTOs/MuebleDto.cs`
- Repositorio: `src/Infrastructure/Repositories/MuebleRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IMuebleRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorCampoRequerido.cs`, `src/Shared/Exceptions/ErrorLongitudMaxima.cs`, `src/Shared/Exceptions/ErrorValorNegativoNoPermitido.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/MuebleControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/MuebleServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/MuebleRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/muebles/muebles.component.ts`
- Servicio: `Client/src/app/services/APIS/mueble/mueble-api.service.ts`
- Modelo: `Client/src/app/models/Mueble.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/muebles/muebles.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/mueble/mueble-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Usabilidad: Formularios comprensibles y organización lógica de los campos para facilitar la gestión de muebles.
- Robustez: Pruebas unitarias aseguran la correcta gestión de muebles y el manejo de excepciones.

---

### HU29 - Administración de Carreras

#### Descripción
Como administrador del sistema, quiero ver la lista de las carreras, crear, eliminar y editar carreras.

#### Criterios de Aceptación Funcionales
- Al crear o editar solo se pide el nombre.

#### Excepciones y Validaciones
- ErrorCampoRequerido.cs
- ErrorCarreraNoEncontrada.cs

#### Evidencia en Código y Tests
- Controlador: `src/Presentations/Controllers/CarreraController.cs`
- Servicio: `src/Application/Services/Implementations/CarreraService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/ICarreraService.cs`
- Request DTO: `src/Application/Request DTOs/Carrera/CrearCarreraComando.cs`
- Response DTO: `src/Application/Response DTOs/CarreraDto.cs`
- Repositorio: `src/Infrastructure/Repositories/CarreraRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/ICarreraRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorCampoRequerido.cs`, `src/Shared/Exceptions/ErrorCarreraNoEncontrada.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/CarreraControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/CarreraServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/CarreraRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/carreras/carreras.component.ts`
- Servicio: `Client/src/app/services/APIS/carrera/carrera-api.service.ts`
- Modelo: `Client/src/app/models/Carrera.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/carreras/carreras.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/carrera/carrera-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Usabilidad: Interfaz sencilla y lista de carreras fácil de consultar y editar.
- Robustez: Pruebas unitarias aseguran la correcta gestión de carreras y el manejo de excepciones.

---

### HU30 - Administración de Empresas de Mantenimiento

#### Descripción
Como administrador del sistema, quiero ver la lista de empresas de mantenimiento autorizadas, crear, eliminar y editar empresas de mantenimiento.

#### Criterios de Aceptación Funcionales
- Para cada empresa se puede ver: nombre de la empresa, responsable, teléfono y NIT.
- Al crear todos los campos son obligatorios: nombre de la empresa, nombre y apellido del responsable, teléfono, dirección y NIT.
- Se puede editar cualquier campo menos el NIT.

#### Excepciones y Validaciones
- ErrorCampoRequerido.cs
- ErrorEmpresaMantenimientoNoEncontrada.cs
- ErrorLongitudMaxima.cs

#### Evidencia en Código y Tests
- Controlador: `src/Presentations/Controllers/EmpresaMantenimientoController.cs`
- Servicio: `src/Application/Services/Implementations/EmpresaMantenimientoService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IEmpresaMantenimientoService.cs`
- Request DTO: `src/Application/Request DTOs/EmpresaMantenimiento/CrearEmpresaMantenimientoComando.cs`
- Response DTO: `src/Application/Response DTOs/EmpresaMantenimientoDto.cs`
- Repositorio: `src/Infrastructure/Repositories/EmpresaMantenimientoRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IEmpresaMantenimientoRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorCampoRequerido.cs`, `src/Shared/Exceptions/ErrorEmpresaMantenimientoNoEncontrada.cs`, `src/Shared/Exceptions/ErrorLongitudMaxima.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/EmpresaMantenimientoControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/EmpresaMantenimientoServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/EmpresaMantenimientoRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/empresas-mantenimiento/empresas-mantenimiento.component.ts`
- Servicio: `Client/src/app/services/APIS/empresaMantenimiento/empresa-mantenimiento-api.service.ts`
- Modelo: `Client/src/app/models/EmpresaMantenimiento.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/empresas-mantenimiento/empresas-mantenimiento.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/empresaMantenimiento/empresa-mantenimiento-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Usabilidad: Interfaz clara y lista de empresas fácil de consultar y editar.
- Robustez: Pruebas unitarias aseguran la correcta gestión de empresas de mantenimiento y el manejo de excepciones.

---
