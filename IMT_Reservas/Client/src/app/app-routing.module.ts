import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PantallaMainComponent } from './componentes/cliente_modulo/pantalla-main/pantalla-main.component'; 
import { ObjetoComponent } from './componentes/cliente_modulo/clic_objeto/objeto.component';
import { CarritoComponent } from './componentes/cliente_modulo/carrito/carrito.component';
import { AdministradorComponent } from './componentes/admin_modulo/administrador/administrador.component';
import { FormularioComponent } from './componentes/cliente_modulo/formulario/formulario.component';
import { IniciarSesionComponent } from './componentes/iniciar-sesion/iniciar-sesion.component';
import { AccesoriosComponent } from './componentes/admin_modulo/accesorios/accesorios/accesorios.component';
import { CarrerasComponent } from './componentes/admin_modulo/carreras/carreras/carreras.component';
import { CategoriasComponent } from './componentes/admin_modulo/categorias/categorias/categorias.component';
import { ComponentesComponent } from './componentes/admin_modulo/componentes/componentes/componentes.component';
import { EmpresasMantenimientoComponent } from './componentes/admin_modulo/empresas_mantenimiento/empresas-mantenimiento/empresas-mantenimiento.component';
import { EquiposComponent } from './componentes/admin_modulo/equipos/equipos/equipos.component';
import { GaveterosComponent } from './componentes/admin_modulo/gaveteros/gaveteros/gaveteros.component';
import { GruposEquiposComponent } from './componentes/admin_modulo/grupos_equipos/grupos-equipos/grupos-equipos.component';
import { MantenimientosComponent } from './componentes/admin_modulo/mantenimientos/mantenimientos/mantenimientos.component';
import { MueblesComponent } from './componentes/admin_modulo/muebles/muebles/muebles.component';
import { PrestamosComponent } from './componentes/admin_modulo/prestamos/prestamos/prestamos.component';
import { UsuariosComponent } from './componentes/admin_modulo/usuarios/usuarios/usuarios.component';
import { AccesoriosCrearComponent } from './componentes/admin_modulo/accesorios/accesorios-crear/accesorios-crear.component';
import { AccesoriosObtenerComponent } from './componentes/admin_modulo/accesorios/accesorios-obtener/accesorios-obtener.component';
import { AccesoriosActualizarComponent } from './componentes/admin_modulo/accesorios/accesorios-actualizar/accesorios-actualizar.component';
import { AccesoriosEliminarComponent } from './componentes/admin_modulo/accesorios/accesorios-eliminar/accesorios-eliminar.component';
import { CarrerasCrearComponent } from './componentes/admin_modulo/carreras/carreras-crear/carreras-crear.component';
import { CarrerasObtenerComponent } from './componentes/admin_modulo/carreras/carreras-obtener/carreras-obtener.component';
import { CarrerasActualizarComponent } from './componentes/admin_modulo/carreras/carreras-actualizar/carreras-actualizar.component';
import { CarrerasEliminarComponent } from './componentes/admin_modulo/carreras/carreras-eliminar/carreras-eliminar.component';
import { CategoriasActualizarComponent } from './componentes/admin_modulo/categorias/categorias-actualizar/categorias-actualizar.component';
import { CategoriasCrearComponent } from './componentes/admin_modulo/categorias/categorias-crear/categorias-crear.component';
import { CategoriasEliminarComponent } from './componentes/admin_modulo/categorias/categorias-eliminar/categorias-eliminar.component';
import { CategoriasObtenerComponent } from './componentes/admin_modulo/categorias/categorias-obtener/categorias-obtener.component';
import { ComponentesActualizarComponent } from './componentes/admin_modulo/componentes/componentes-actualizar/componentes-actualizar.component';
import { ComponentesCrearComponent } from './componentes/admin_modulo/componentes/componentes-crear/componentes-crear.component';
import { ComponentesEliminarComponent } from './componentes/admin_modulo/componentes/componentes-eliminar/componentes-eliminar.component';
import { ComponentesObtenerComponent } from './componentes/admin_modulo/componentes/componentes-obtener/componentes-obtener.component';
import { EmpresasMantenimientoActualizarComponent } from './componentes/admin_modulo/empresas_mantenimiento/empresas-mantenimiento-actualizar/empresas-mantenimiento-actualizar.component';
import { EmpresasMantenimientoCrearComponent } from './componentes/admin_modulo/empresas_mantenimiento/empresas-mantenimiento-crear/empresas-mantenimiento-crear.component';
import { EmpresasMantenimientoEliminarComponent } from './componentes/admin_modulo/empresas_mantenimiento/empresas-mantenimiento-eliminar/empresas-mantenimiento-eliminar.component';
import { EmpresasMantenimientoObtenerComponent } from './componentes/admin_modulo/empresas_mantenimiento/empresas-mantenimiento-obtener/empresas-mantenimiento-obtener.component';
import { EquiposActualizarComponent } from './componentes/admin_modulo/equipos/equipos-actualizar/equipos-actualizar.component';
import { EquiposCrearComponent } from './componentes/admin_modulo/equipos/equipos-crear/equipos-crear.component';
import { EquiposEliminarComponent } from './componentes/admin_modulo/equipos/equipos-eliminar/equipos-eliminar.component';
import { EquiposObtenerComponent } from './componentes/admin_modulo/equipos/equipos-obtener/equipos-obtener.component';
import { GaveterosActualizarComponent } from './componentes/admin_modulo/gaveteros/gaveteros-actualizar/gaveteros-actualizar.component';
import { GaveterosCrearComponent } from './componentes/admin_modulo/gaveteros/gaveteros-crear/gaveteros-crear.component';
import { GaveterosEliminarComponent } from './componentes/admin_modulo/gaveteros/gaveteros-eliminar/gaveteros-eliminar.component';
import { GaveterosObtenerComponent } from './componentes/admin_modulo/gaveteros/gaveteros-obtener/gaveteros-obtener.component';
import { GruposEquiposActualizarComponent } from './componentes/admin_modulo/grupos_equipos/grupos-equipos-actualizar/grupos-equipos-actualizar.component';
import { GruposEquiposCrearComponent } from './componentes/admin_modulo/grupos_equipos/grupos-equipos-crear/grupos-equipos-crear.component';
import { GruposEquiposEliminarComponent } from './componentes/admin_modulo/grupos_equipos/grupos-equipos-eliminar/grupos-equipos-eliminar.component';
import { GruposEquiposObtenerComponent } from './componentes/admin_modulo/grupos_equipos/grupos-equipos-obtener/grupos-equipos-obtener.component';
import { MantenimientosActualizarComponent } from './componentes/admin_modulo/mantenimientos/mantenimientos-actualizar/mantenimientos-actualizar.component';
import { MantenimientosCrearComponent } from './componentes/admin_modulo/mantenimientos/mantenimientos-crear/mantenimientos-crear.component';
import { MantenimientosEliminarComponent } from './componentes/admin_modulo/mantenimientos/mantenimientos-eliminar/mantenimientos-eliminar.component';
import { MantenimientosObtenerComponent } from './componentes/admin_modulo/mantenimientos/mantenimientos-obtener/mantenimientos-obtener.component';
import { MueblesActualizarComponent } from './componentes/admin_modulo/muebles/muebles-actualizar/muebles-actualizar.component';
import { MueblesCrearComponent } from './componentes/admin_modulo/muebles/muebles-crear/muebles-crear.component';
import { MueblesEliminarComponent } from './componentes/admin_modulo/muebles/muebles-eliminar/muebles-eliminar.component';
import { MueblesObtenerComponent } from './componentes/admin_modulo/muebles/muebles-obtener/muebles-obtener.component';
import { PrestamosActualizarComponent } from './componentes/admin_modulo/prestamos/prestamos-actualizar/prestamos-actualizar.component';
import { PrestamosCrearComponent } from './componentes/admin_modulo/prestamos/prestamos-crear/prestamos-crear.component';
import { PrestamosEliminarComponent } from './componentes/admin_modulo/prestamos/prestamos-eliminar/prestamos-eliminar.component';
import { PrestamosObtenerComponent } from './componentes/admin_modulo/prestamos/prestamos-obtener/prestamos-obtener.component';
import { UsuariosActualizarComponent } from './componentes/admin_modulo/usuarios/usuarios-actualizar/usuarios-actualizar.component';
import { UsuariosCrearComponent } from './componentes/admin_modulo/usuarios/usuarios-crear/usuarios-crear.component';
import { UsuariosEliminarComponent } from './componentes/admin_modulo/usuarios/usuarios-eliminar/usuarios-eliminar.component';
import { UsuariosObtenerComponent } from './componentes/admin_modulo/usuarios/usuarios-obtener/usuarios-obtener.component';
import { DetallesPrestamosComponent } from './componentes/admin_modulo/detalles_prestamos/detalles-prestamos/detalles-prestamos.component';
import { DetallesPrestamosCrearComponent } from './componentes/admin_modulo/detalles_prestamos/detalles-prestamos-crear/detalles-prestamos-crear.component';
import { DetallesPrestamosObtenerComponent } from './componentes/admin_modulo/detalles_prestamos/detalles-prestamos-obtener/detalles-prestamos-obtener.component';
import { DetallesPrestamosActualizarComponent } from './componentes/admin_modulo/detalles_prestamos/detalles-prestamos-actualizar/detalles-prestamos-actualizar.component';
import { DetallesPrestamosEliminarComponent } from './componentes/admin_modulo/detalles_prestamos/detalles-prestamos-eliminar/detalles-prestamos-eliminar.component';
import { DetallesMantenimientosComponent } from './componentes/admin_modulo/detalles_mantenimientos/detalles-mantenimientos/detalles-mantenimientos.component';
import { DetallesMantenimientosCrearComponent } from './componentes/admin_modulo/detalles_mantenimientos/detalles-mantenimientos-crear/detalles-mantenimientos-crear.component';
import { DetallesMantenimientosObtenerComponent } from './componentes/admin_modulo/detalles_mantenimientos/detalles-mantenimientos-obtener/detalles-mantenimientos-obtener.component';
import { DetallesMantenimientosActualizarComponent } from './componentes/admin_modulo/detalles_mantenimientos/detalles-mantenimientos-actualizar/detalles-mantenimientos-actualizar.component';
import { DetallesMantenimientosEliminarComponent } from './componentes/admin_modulo/detalles_mantenimientos/detalles-mantenimientos-eliminar/detalles-mantenimientos-eliminar.component';


const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },

  // Ruta principal de administración
  { path: 'admin', component: AdministradorComponent },

  // ACCESORIOS
  { path: 'admin/accesorios', component: AccesoriosComponent },
  { path: 'admin/accesorios/crear', component: AccesoriosCrearComponent },
  { path: 'admin/accesorios/obtener', component: AccesoriosObtenerComponent },
  { path: 'admin/accesorios/actualizar', component: AccesoriosActualizarComponent },
  { path: 'admin/accesorios/eliminar', component: AccesoriosEliminarComponent },

  // CARRERAS
  { path: 'admin/carreras', component: CarrerasComponent },
  { path: 'admin/carreras/crear', component: CarrerasCrearComponent },
  { path: 'admin/carreras/obtener', component: CarrerasObtenerComponent },
  { path: 'admin/carreras/actualizar', component: CarrerasActualizarComponent },
  { path: 'admin/carreras/eliminar', component: CarrerasEliminarComponent },

  // CATEGORÍAS
  { path: 'admin/categorias', component: CategoriasComponent },
  { path: 'admin/categorias/crear', component: CategoriasCrearComponent },
  { path: 'admin/categorias/obtener', component: CategoriasObtenerComponent },
  { path: 'admin/categorias/actualizar', component: CategoriasActualizarComponent },
  { path: 'admin/categorias/eliminar', component: CategoriasEliminarComponent },

  // COMPONENTES
  { path: 'admin/componentes', component: ComponentesComponent },
  { path: 'admin/componentes/crear', component: ComponentesCrearComponent },
  { path: 'admin/componentes/obtener', component: ComponentesObtenerComponent },
  { path: 'admin/componentes/actualizar', component: ComponentesActualizarComponent },
  { path: 'admin/componentes/eliminar', component: ComponentesEliminarComponent },

  // EMPRESAS DE MANTENIMIENTO
  { path: 'admin/empresas_mantenimiento', component: EmpresasMantenimientoComponent },
  { path: 'admin/empresas_mantenimiento/crear', component: EmpresasMantenimientoCrearComponent },
  { path: 'admin/empresas_mantenimiento/obtener', component: EmpresasMantenimientoObtenerComponent },
  { path: 'admin/empresas_mantenimiento/actualizar', component: EmpresasMantenimientoActualizarComponent },
  { path: 'admin/empresas_mantenimiento/eliminar', component: EmpresasMantenimientoEliminarComponent },

  // EQUIPOS
  { path: 'admin/equipos', component: EquiposComponent },
  { path: 'admin/equipos/crear', component: EquiposCrearComponent },
  { path: 'admin/equipos/obtener', component: EquiposObtenerComponent },
  { path: 'admin/equipos/actualizar', component: EquiposActualizarComponent },
  { path: 'admin/equipos/eliminar', component: EquiposEliminarComponent },

  // GAVETEROS
  { path: 'admin/gaveteros', component: GaveterosComponent },
  { path: 'admin/gaveteros/crear', component: GaveterosCrearComponent },
  { path: 'admin/gaveteros/obtener', component: GaveterosObtenerComponent },
  { path: 'admin/gaveteros/actualizar', component: GaveterosActualizarComponent },
  { path: 'admin/gaveteros/eliminar', component: GaveterosEliminarComponent },

  // GRUPOS DE EQUIPOS
  { path: 'admin/grupos_equipos', component: GruposEquiposComponent },
  { path: 'admin/grupos_equipos/crear', component: GruposEquiposCrearComponent },
  { path: 'admin/grupos_equipos/obtener', component: GruposEquiposObtenerComponent },
  { path: 'admin/grupos_equipos/actualizar', component: GruposEquiposActualizarComponent },
  { path: 'admin/grupos_equipos/eliminar', component: GruposEquiposEliminarComponent },

  // MANTENIMIENTOS
  { path: 'admin/mantenimientos', component: MantenimientosComponent },
  { path: 'admin/mantenimientos/crear', component: MantenimientosCrearComponent },
  { path: 'admin/mantenimientos/obtener', component: MantenimientosObtenerComponent },
  { path: 'admin/mantenimientos/actualizar', component: MantenimientosActualizarComponent },
  { path: 'admin/mantenimientos/eliminar', component: MantenimientosEliminarComponent },

  // MUEBLES
  { path: 'admin/muebles', component: MueblesComponent },
  { path: 'admin/muebles/crear', component: MueblesCrearComponent },
  { path: 'admin/muebles/obtener', component: MueblesObtenerComponent },
  { path: 'admin/muebles/actualizar', component: MueblesActualizarComponent },
  { path: 'admin/muebles/eliminar', component: MueblesEliminarComponent },

  // PRÉSTAMOS
  { path: 'admin/prestamos', component: PrestamosComponent },
  { path: 'admin/prestamos/crear', component: PrestamosCrearComponent },
  { path: 'admin/prestamos/obtener', component: PrestamosObtenerComponent },
  { path: 'admin/prestamos/actualizar', component: PrestamosActualizarComponent },
  { path: 'admin/prestamos/eliminar', component: PrestamosEliminarComponent },

  // USUARIOS
  { path: 'admin/usuarios', component: UsuariosComponent },
  { path: 'admin/usuarios/crear', component: UsuariosCrearComponent },
  { path: 'admin/usuarios/obtener', component: UsuariosObtenerComponent },
  { path: 'admin/usuarios/actualizar', component: UsuariosActualizarComponent },
  { path: 'admin/usuarios/eliminar', component: UsuariosEliminarComponent },

  // DETALLES DE PRÉSTAMOS
  { path: 'admin/detalles_prestamos', component: DetallesPrestamosComponent },
  { path: 'admin/detalles_prestamos/crear', component: DetallesPrestamosCrearComponent },
  { path: 'admin/detalles_prestamos/obtener', component: DetallesPrestamosObtenerComponent },
  { path: 'admin/detalles_prestamos/actualizar', component: DetallesPrestamosActualizarComponent },
  { path: 'admin/detalles_prestamos/eliminar', component: DetallesPrestamosEliminarComponent },

  // DETALLES DE MANTENIMIENTOS
  { path: 'admin/detalles_mantenimientos', component: DetallesMantenimientosComponent },
  { path: 'admin/detalles_mantenimientos/crear', component: DetallesMantenimientosCrearComponent },
  { path: 'admin/detalles_mantenimientos/obtener', component: DetallesMantenimientosObtenerComponent },
  { path: 'admin/detalles_mantenimientos/actualizar', component: DetallesMantenimientosActualizarComponent },
  { path: 'admin/detalles_mantenimientos/eliminar', component: DetallesMantenimientosEliminarComponent },

  // RUTAS GENERALES
  { path: 'home', component: PantallaMainComponent },
  { path: 'Objeto/:id', component: ObjetoComponent },
  { path: 'ConfirmarReserva', component: CarritoComponent },
  { path: 'Formulario', component: FormularioComponent },
  { path: 'Iniciar-Sesion', component: IniciarSesionComponent }
 

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }