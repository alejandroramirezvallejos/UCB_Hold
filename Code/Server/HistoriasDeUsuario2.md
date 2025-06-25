## Módulo Categorías

### HU11 - Gestión de Categorías

#### Descripción
Como administrador del sistema, quiero gestionar categorías para clasificar los grupos de equipos según su tipo y uso.

#### Criterios de Aceptación Funcionales
- El sistema permite crear una categoría si el nombre tiene más de 3 y menos de 50 caracteres, consultar, actualizar y eliminar lógicamente categorías de equipos.

#### Excepciones y Validaciones (ubicación: `src/Shared/Exceptions/`)
- ErrorCampoRequerido.cs
- ErrorCategoriaNoEncontrada.cs
- ErrorLongitudMaxima.cs

#### Evidencia en Código y Tests (Backend)
- Controlador: `src/Presentations/Controllers/CategoriaController.cs`
- Servicio: `src/Application/Services/Implementations/CategoriaService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/ICategoriaService.cs`
- Request DTO: `src/Application/Request DTOs/Categoria/CrearCategoriaComando.cs`
- Response DTO: `src/Application/Response DTOs/CategoriaDto.cs`
- Repositorio: `src/Infrastructure/Repositories/CategoriaRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/ICategoriaRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorCampoRequerido.cs`, `src/Shared/Exceptions/ErrorCategoriaNoEncontrada.cs`, `src/Shared/Exceptions/ErrorLongitudMaxima.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/CategoriaControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/CategoriaServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/CategoriaRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/categorias/categorias.component.ts`
- Servicio: `Client/src/app/services/APIS/categoria/categorias-api.service.ts`
- Modelo: `Client/src/app/models/categoria.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/categorias/categorias.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/categoria/categorias-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- **Robustez:** El backend implementa pruebas unitarias y de integración para todos los flujos principales y alternativos, incluyendo manejo de excepciones y validaciones de nombre. Los tests cubren creación, consulta, actualización y eliminación lógica. Las excepciones personalizadas aseguran mensajes claros ante errores. El sistema nunca elimina categorías físicamente, solo de manera lógica.
- **Experiencia de usuario:** La interfaz permite gestionar categorías de forma intuitiva, con formularios claros y validaciones en tiempo real. Los mensajes de error y éxito son específicos y comprensibles. El usuario recibe retroalimentación inmediata ante acciones exitosas o fallidas.
- **Seguridad:** Los DTOs solo exponen los datos necesarios, nunca información sensible. El backend valida los datos antes de procesarlos.
- **Auditoría:** Se aplica eliminación es lógica para preservar la trazabilidad de los datos.

---

## Módulo Accesorios

### HU12 - Gestión de Accesorios

#### Descripción
Como administrador del sistema, quiero gestionar los accesorios de equipos para mantener un registro de los complementos disponibles para préstamo.

#### Criterios de Aceptación Funcionales
- El sistema permite crear, consultar, actualizar y eliminar lógicamente accesorios.
- Al crear: nombre, modelo, tipo, código IMT (debe ser válido y pertenecer a un equipo), todos obligatorios. Descripción, precio y URL data sheet son opcionales.

#### Excepciones y Validaciones (ubicación: `src/Shared/Exceptions/`)
- ErrorCampoRequerido.cs
- ErrorEquipoNoEncontrado.cs
- ErrorLongitudMaxima.cs

#### Evidencia en Código y Tests (Backend)
- Controlador: `src/Presentations/Controllers/AccesorioController.cs`
- Servicio: `src/Application/Services/Implementations/AccesorioService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IAccesorioService.cs`
- Request DTO: `src/Application/Request DTOs/Accesorio/CrearAccesorioComando.cs`
- Response DTO: `src/Application/Response DTOs/AccesorioDto.cs`
- Repositorio: `src/Infrastructure/Repositories/AccesorioRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IAccesorioRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorCampoRequerido.cs`, `src/Shared/Exceptions/ErrorEquipoNoEncontrado.cs`, `src/Shared/Exceptions/ErrorLongitudMaxima.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/AccesorioControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/AccesorioServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/AccesorioRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/accesorios/accesorios.component.ts`
- Servicio: `Client/src/app/services/APIS/accesorio/accesorios-api.service.ts`
- Modelo: `Client/src/app/models/accesorio.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/accesorios/accesorios.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/accesorio/accesorios-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- **Robustez:** El backend cuenta con pruebas unitarias y de integración para todos los flujos, incluyendo validaciones de campos obligatorios, existencia de equipo y longitud máxima. Las excepciones personalizadas aseguran mensajes claros y controlados. La eliminación es lógica, nunca física.
- **Experiencia de usuario:** Formularios claros y validaciones en tiempo real en el frontend. Mensajes de error y éxito inmediatos y comprensibles. El usuario puede gestionar accesorios de forma sencilla y rápida.
- **Seguridad:** Los DTOs solo exponen información relevante, nunca datos sensibles. El backend valida la pertenencia del accesorio a un equipo existente.
- **Auditoría:** La eliminación lógica permite mantener la trazabilidad de los accesorios.

---

## Módulo Componentes

### HU13 - Gestión de Componentes

#### Descripción
Como administrador del sistema, quiero gestionar componentes de equipos para llevar un registro detallado de las partes que componen cada equipo.

#### Criterios de Aceptación Funcionales
- El sistema permite crear, consultar, actualizar y eliminar lógicamente componentes.
- Al crear: nombre, modelo, tipo, código IMT del equipo (obligatorio), descripción, precio de referencia y URL data sheet (opcionales).

#### Excepciones y Validaciones (ubicación: `src/Shared/Exceptions/`)
- ErrorCampoRequerido.cs
- ErrorEquipoNoEncontrado.cs
- ErrorLongitudMaxima.cs

#### Evidencia en Código y Tests (Backend)
- Controlador: `src/Presentations/Controllers/ComponenteController.cs`
- Servicio: `src/Application/Services/Implementations/ComponenteService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IComponenteService.cs`
- Request DTO: `src/Application/Request DTOs/Componente/CrearComponenteComando.cs`
- Response DTO: `src/Application/Response DTOs/ComponenteDto.cs`
- Repositorio: `src/Infrastructure/Repositories/ComponenteRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IComponenteRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorCampoRequerido.cs`, `src/Shared/Exceptions/ErrorEquipoNoEncontrado.cs`, `src/Shared/Exceptions/ErrorLongitudMaxima.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/ComponenteControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/ComponenteServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/ComponenteRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/componentes/componentes.component.ts`
- Servicio: `Client/src/app/services/APIS/componente/componentes-api.service.ts`
- Modelo: `Client/src/app/models/componente.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/componentes/componentes.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/componente/componentes-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- **Robustez:** Pruebas unitarias y de integración cubren todos los flujos, incluyendo validaciones de campos, existencia de equipo y manejo de excepciones. La eliminación es lógica, nunca física.
- **Experiencia de usuario:** La gestión de componentes es intuitiva, con formularios claros y validaciones en tiempo real. Mensajes de error y éxito inmediatos y específicos.
- **Seguridad:** Los DTOs solo exponen información relevante, nunca datos sensibles. El backend valida la relación con el equipo.
- **Auditoría:** La eliminación lógica mantiene la trazabilidad de los componentes.

---

## Módulo Mantenimientos

### HU14 - Gestión de Mantenimientos

#### Descripción
Como administrador del sistema, quiero registrar y consultar los mantenimientos realizados a los equipos para llevar un histórico de intervenciones y estado de los equipos.

#### Criterios de Aceptación Funcionales
- El sistema permite crear, consultar y eliminar lógicamente registros de mantenimiento.
- Al crear: empresa de mantenimiento, fecha de inicio, fecha de finalización, equipos asociados (con tipo y descripción específica), costo (opcional, puede ser 0), descripción (opcional).

#### Excepciones y Validaciones (ubicación: `src/Shared/Exceptions/`)
- ErrorCampoRequerido.cs
- ErrorEquipoNoEncontrado.cs
- ErrorLongitudMaxima.cs

#### Evidencia en Código y Tests (Backend)
- Controlador: `src/Presentations/Controllers/MantenimientoController.cs`
- Servicio: `src/Application/Services/Implementations/MantenimientoService.cs`
- Interfaz de Servicio: `src/Application/Services/Interfaces/IMantenimientoService.cs`
- Request DTO: `src/Application/Request DTOs/Mantenimiento/CrearMantenimientoComando.cs`
- Response DTO: `src/Application/Response DTOs/MantenimientoDto.cs`
- Repositorio: `src/Infrastructure/Repositories/MantenimientoRepository.cs`
- Interfaz de Repositorio: `src/Infrastructure/Repositories/IMantenimientoRepository.cs`
- Excepciones: `src/Shared/Exceptions/ErrorCampoRequerido.cs`, `src/Shared/Exceptions/ErrorEquipoNoEncontrado.cs`, `src/Shared/Exceptions/ErrorLongitudMaxima.cs`
- Test de Controlador: `Tests/ControllerTests/Implementations/MantenimientoControllerTest.cs`
- Test de Servicio: `Tests/ServiceTests/Implementations/MantenimientoServiceTest.cs`
- Test de Repositorio: `Tests/RepositoryTests/Implementations/MantenimientoRepositoryTest.cs`

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/mantenimientos/mantenimientos.component.ts`
- Servicio: `Client/src/app/services/APIS/mantenimiento/mantenimientos-api.service.ts`
- Modelo: `Client/src/app/models/mantenimiento.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/mantenimientos/mantenimientos.component.spec.ts`
- Test de servicio: `Client/src/app/services/APIS/mantenimiento/mantenimientos-api.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- **Robustez:** El backend implementa pruebas unitarias y de integración para todos los flujos, validando campos requeridos, existencia de equipos y manejo de excepciones. La eliminación es lógica, nunca física. Los tests aseguran que los errores sean controlados y los mensajes claros.
- **Experiencia de usuario:** Formularios claros y validaciones en tiempo real en el frontend. Mensajes de error y éxito inmediatos y comprensibles. El usuario puede consultar y registrar mantenimientos de forma sencilla.
- **Seguridad:** Los DTOs solo exponen información relevante, nunca datos sensibles. El backend valida la relación con los equipos y la empresa de mantenimiento.
- **Auditoría:** La eliminación lógica mantiene la trazabilidad de los mantenimientos.

---

## Módulo Administración

### HU15 - Panel de Administración

#### Descripción:

Como administrador del sistema, quiero acceder a un panel centralizado que me permita gestionar todas las entidades del sistema (prestamos, carreas usuarios, categorias, componentes, empresa de manteniimento, equipos, gaverteros, grupo de equipos, mantenimientos, muebles, accesoios ) para facilitar la administración general.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe mostrar un panel con acceso a todas las funcionalidades administrativas.
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

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/admin_modulo/administrador/administrador.component.ts`
- Test de componente: `Client/src/app/componentes/admin_modulo/administrador/administrador.component.spec.ts`

---

## Módulo Catálogo y Búsqueda de Equipos

### HU16 - Visualización de Catálogo de Equipos

#### Descripción:

Como usuario del sistema, quiero ver un catálogo de los equipos disponibles para préstamo, con imágenes, detalles y comentarios para seleccionar los que necesito.

#### Criterios de Aceptación Funcionales:

- **Comportamiento Principal:** El sistema debe mostrar un listado visual de todos los grupos de equipos disponibles.
- **Flujos Alternativos:** Si no hay equipos disponibles, se debe mostrar un mensaje apropiado.
- **Datos de Salida:** Catálogo con imágenes, nombres, modelos y descripciones de los equipos.

#### Criterios de Aceptación No Funcionales
- **Usabilidad:** El catálogo debe ser atractivo y fácil de navegar.

#### Evidencia en Código:

- **Archivos relevantes:** Componentes en la carpeta `cliente_modulo/catalogo`, servicio `src/Client/app/services/equipo-catalogo.service.ts`
- **Test:** `Client/src/app/componentes/catalogo/catalogo.component.spec.ts` (pruebas unitarias de visualización de catálogo), `Client/src/app/services/equipo-catalogo.service.spec.ts` (pruebas de servicio de catálogo de equipos)

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/cliente_modulo/catalogo/catalogo.component.ts`
- Servicio: `Client/src/app/services/equipo-catalogo.service.ts`
- Modelo: `Client/src/app/models/grupo_equipo.ts`
- Test de componente: `Client/src/app/componentes/cliente_modulo/catalogo/catalogo.component.spec.ts`
- Test de servicio: `Client/src/app/services/equipo-catalogo.service.spec.ts`

---

### HU17 - Búsqueda y Visualización de Detalles de Equipo

#### Descripción
Como usuario del sistema, quiero buscar equipos por filtros y ver los detalles completos de cada equipo para tomar decisiones informadas sobre los préstamos.

#### Criterios de Aceptación Funcionales
- El sistema permite buscar equipos por categoría y nombre del grupo equipo.
- Al seleccionar un equipo, se muestran todos sus detalles: título, descripción, imagen y comentarios.
- Si no hay resultados, se muestra un mensaje apropiado.

#### Excepciones y Validaciones (ubicación: `src/Shared/Exceptions/`)
- ErrorGrupoEquipoNoEncontrado.cs
- ErrorEquipoNoEncontrado.cs

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/cliente_modulo/catalogo/`, `Client/src/app/componentes/equipo/detalle-equipo/`
- Servicios: `Client/src/app/services/equipo-catalogo.service.ts`, `Client/src/app/services/buscador/buscador.service.ts`
- Test de catálogo: `Client/src/app/componentes/catalogo/catalogo.component.spec.ts`
- Test de detalles: `Client/src/app/componentes/equipo/detalle-equipo/detalle-equipo.component.spec.ts`
- Test de servicio: `Client/src/app/services/equipo-catalogo.service.spec.ts`, `Client/src/app/services/buscador/buscador.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Usabilidad: filtros y detalles claros, navegación sencilla.

---

### HU18 - Búsqueda de Equipos

#### Descripción
Como usuario del sistema, quiero utilizar filtros básicos en la búsqueda de equipos para encontrar lo que necesito.

#### Criterios de Aceptación Funcionales
- El sistema permite buscar equipos por categoría y nombre del grupo equipo.
- Si no hay resultados, se debe mostrar un mensaje apropiado.
- Datos de entrada: Criterios de búsqueda (categoría, nombre del grupo equipo).
- Datos de salida: Lista filtrada de grupos de equipos que cumplan con ambos criterios.

#### Excepciones y Validaciones (ubicación: `src/Shared/Exceptions/`)
- ErrorGrupoEquipoNoEncontrado.cs

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/cliente_modulo/buscador/buscador.component.ts`
- Servicio: `Client/src/app/services/buscador/buscador.service.ts`
- Modelo: `Client/src/app/models/grupo_equipo.ts`
- Test de componente: `Client/src/app/componentes/cliente_modulo/buscador/buscador.component.spec.ts`
- Test de servicio: `Client/src/app/services/buscador/buscador.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- Usabilidad: Los filtros deben ser intuitivos y fáciles de usar.

---

### HU19 - Visualización de Detalles de Equipo

#### Descripción
Como usuario del sistema, quiero ver el título, la descripción, la imagen y comentarios para evaluar si cumple con mis necesidades.

#### Criterios de Aceptación Funcionales
- Al seleccionar un equipo del catálogo, el sistema debe mostrar todos sus detalles.
- Datos de salida: Información detallada del equipo, incluyendo comentarios, descripción e imagen.

#### Excepciones y Validaciones (ubicación: `src/Shared/Exceptions/`)
- ErrorEquipoNoEncontrado.cs

#### Evidencia en Código y Tests (Frontend)
- Componente: `Client/src/app/componentes/equipo/detalle-equipo/detalle-equipo.component.ts`
- Servicio: `Client/src/app/services/equipo-catalogo.service.ts`
- Modelo: `Client/src/app/models/equipo.ts`
- Test de componente: `Client/src/app/componentes/equipo/detalle-equipo/detalle-equipo.component.spec.ts`
- Test de servicio: `Client/src/app/services/equipo-catalogo.service.spec.ts`

#### Criterios de Aceptación No Funcionales
- **Usabilidad:** La información debe presentarse de forma clara y organizada. El usuario puede acceder fácilmente a los detalles desde el catálogo y navegar entre equipos sin recargar la página.
- **Robustez:** Existen pruebas unitarias en los componentes y servicios del frontend para asegurar la correcta visualización y manejo de errores en la obtención de detalles. El backend retorna mensajes claros ante errores como equipo no encontrado.
- **Seguridad:** Solo se exponen datos públicos del equipo, nunca información sensible.

---
