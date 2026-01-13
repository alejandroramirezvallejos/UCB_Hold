import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { IniciarSesionComponent } from './componentes/usuario/iniciar-sesion/iniciar-sesion.component';

const routes: Routes = [
  { path: '', redirectTo: '/Iniciar-Sesion', pathMatch: 'full' },
  { path: 'Iniciar-Sesion', component: IniciarSesionComponent },
  // Lazy loading - solo carga cuando el usuario navega a estas rutas
  { 
    path: 'admin', 
    loadComponent: () => import('./componentes/admin_modulo/administrador/administrador.component')
      .then(m => m.AdministradorComponent) 
  },
  { 
    path: 'home', 
    loadComponent: () => import('./componentes/cliente_modulo/pantalla-main/pantalla-main.component')
      .then(m => m.PantallaMainComponent) 
  },
  { 
    path: 'Objeto/:id', 
    loadComponent: () => import('./componentes/cliente_modulo/clic_objeto/objeto.component')
      .then(m => m.ObjetoComponent) 
  },
  { 
    path: 'Carrito', 
    loadComponent: () => import('./componentes/cliente_modulo/carrito/carrito.component')
      .then(m => m.CarritoComponent) 
  },
  { 
    path: 'ConfirmarReserva', 
    loadComponent: () => import('./componentes/cliente_modulo/carrito/carrito.component')
      .then(m => m.CarritoComponent) 
  },
  { 
    path: 'Formulario', 
    loadComponent: () => import('./componentes/cliente_modulo/formulario/formulario.component')
      .then(m => m.FormularioComponent) 
  },
  { 
    path: 'Historial', 
    loadComponent: () => import('./componentes/usuario/historial/historial.component')
      .then(m => m.HistorialComponent) 
  },
  { 
    path: 'Perfil', 
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
    loadComponent: () => import('./componentes/admin_modulo/buscador/buscador.component')
      .then(m => m.BuscadorComponent) 
  },
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
