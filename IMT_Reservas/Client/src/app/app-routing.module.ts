import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PantallaMainComponent } from './componentes/cliente_modulo/pantalla-main/pantalla-main.component'; 
import { ObjetoComponent } from './componentes/cliente_modulo/clic_objeto/objeto.component';
import { CarritoComponent } from './componentes/cliente_modulo/carrito/carrito.component';
import { AdministradorComponent } from './componentes/admin_modulo/administrador/administrador.component';
import { FormularioComponent } from './componentes/cliente_modulo/formulario/formulario.component';
import { IniciarSesionComponent } from './componentes/iniciar-sesion/iniciar-sesion.component';
import { HistorialComponent } from './componentes/usuario/historial/historial.component';
import { PerfilComponent } from './componentes/usuario/perfil/perfil.component';
import { AccesoriosTablaComponent } from './componentes/admin_modulo/tablas/accesorios/accesorios-tabla/accesorios-tabla.component';
import { CarrerasTablaComponent } from './componentes/admin_modulo/tablas/carreras/carreras-tabla/carreras-tabla.component';
import { UsuariosTablaComponent } from './componentes/admin_modulo/tablas/usuarios/usuarios-tabla/usuarios-tabla.component';
import { CategoriasTablaComponent } from './componentes/admin_modulo/tablas/categorias/categorias-tabla/categorias-tabla.component';
import { ComponentesTablaComponent } from './componentes/admin_modulo/tablas/componentes/componentes-tabla/componentes-tabla.component';
import { MantenimientosTablaComponent } from './componentes/admin_modulo/tablas/mantenimientos/mantenimientos-tabla/mantenimientos-tabla.component';
import { EmpresasMantenimientoTablaComponent } from './componentes/admin_modulo/tablas/empresas_mantenimiento/empresas-mantenimiento-tabla/empresas-mantenimiento-tabla.component';
import { EquiposTablaComponent } from './componentes/admin_modulo/tablas/equipos/equipos-tabla/equipos-tabla.component';
import { GaveterosTablaComponent } from './componentes/admin_modulo/tablas/gaveteros/gaveteros-tabla/gaveteros-tabla.component';
import { GruposEquiposTablaComponent } from './componentes/admin_modulo/tablas/grupos_equipos/grupos-equipos-tabla/grupos-equipos-tabla.component';
import { MueblesTablaComponent } from './componentes/admin_modulo/tablas/muebles/muebles-tabla/muebles-tabla.component';
import { PrestamosTablaComponent } from './componentes/admin_modulo/tablas/prestamos/prestamos-tabla/prestamos-tabla.component';

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },

  // Ruta principal de administración
  { path: 'admin', component: AdministradorComponent },

  {path: 'admin/accesorios',component:AccesoriosTablaComponent},
  {path:'admin/carreras',component:CarrerasTablaComponent},
  {path:'admin/usuarios',component:UsuariosTablaComponent},
  {path:'admin/categorias',component:CategoriasTablaComponent},
  {path:'admin/componentes',component:ComponentesTablaComponent},
  {path:'admin/mantenimientos',component:MantenimientosTablaComponent},
  {path:'admin/empresas_mantenimiento',component:EmpresasMantenimientoTablaComponent},
  {path:'admin/equipos',component:EquiposTablaComponent},
  {path:'admin/gaveteros',component:GaveterosTablaComponent},
  {path:'admin/grupos_equipos',component:GruposEquiposTablaComponent},
  {path:'admin/muebles',component:MueblesTablaComponent},
  {path:'admin/prestamos',component:PrestamosTablaComponent},

  // RUTAS GENERALES
  { path: 'home', component: PantallaMainComponent },
  { path: 'Objeto/:id', component: ObjetoComponent },
  { path: 'ConfirmarReserva', component: CarritoComponent },
  { path: 'Formulario', component: FormularioComponent },
  { path: 'Iniciar-Sesion', component: IniciarSesionComponent },
  { path: 'Historial', component: HistorialComponent },
 { path: 'Perfil', component: PerfilComponent }

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
