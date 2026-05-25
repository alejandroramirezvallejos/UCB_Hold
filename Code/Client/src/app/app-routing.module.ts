import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { IniciarSesionComponent } from './componentes/usuario/iniciar-sesion/iniciar-sesion.component';
import { PantallaMainComponent } from './componentes/cliente_modulo/pantalla-main/pantalla-main.component';
import { ObjetoComponent } from './componentes/cliente_modulo/clic_objeto/objeto.component';
import { CarritoComponent } from './componentes/cliente_modulo/carrito/carrito.component';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

const routes: Routes = [
  { path: '', redirectTo: '/Iniciar-Sesion', pathMatch: 'full' },
  { path: 'Iniciar-Sesion', component: IniciarSesionComponent },
  { path: 'home',            component: PantallaMainComponent, canActivate: [authGuard] },
  { path: 'Objeto/:id',      component: ObjetoComponent,       canActivate: [authGuard] },
  { path: 'Carrito',         component: CarritoComponent,      canActivate: [authGuard] },
  { path: 'ConfirmarReserva', component: CarritoComponent,     canActivate: [authGuard] },
  {
    path: 'admin',
    canActivate: [adminGuard],
    loadComponent: () => import('./componentes/admin_modulo/administrador/administrador.component')
      .then(m => m.AdministradorComponent)
  },
  {
    path: 'Formulario',
    canActivate: [authGuard],
    loadComponent: () => import('./componentes/cliente_modulo/formulario/formulario.component')
      .then(m => m.FormularioComponent)
  },
  {
    path: 'Historial',
    canActivate: [authGuard],
    loadComponent: () => import('./componentes/usuario/historial/historial.component')
      .then(m => m.HistorialComponent)
  },
  {
    path: 'Perfil',
    canActivate: [authGuard],
    loadComponent: () => import('./componentes/usuario/perfil/perfil.component')
      .then(m => m.PerfilComponent)
  },
  {
    path: 'Registrar-Usuario',
    loadComponent: () => import('./componentes/usuario/registrar-usuario/registrar-usuario.component')
      .then(m => m.RegistrarUsuarioComponent)
  },
  {
    path: 'pruebas',
    canActivate: [adminGuard],
    loadComponent: () => import('./componentes/admin_modulo/buscador/buscador.component')
      .then(m => m.BuscadorComponent)
  },
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
