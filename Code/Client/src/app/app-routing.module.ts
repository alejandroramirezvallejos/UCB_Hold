import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PantallaMainComponent } from './componentes/cliente_modulo/pantalla-main/pantalla-main.component'; 
import { ObjetoComponent } from './componentes/cliente_modulo/clic_objeto/objeto.component';
import { CarritoComponent } from './componentes/cliente_modulo/carrito/carrito.component';
import { AdministradorComponent } from './componentes/admin_modulo/administrador/administrador.component';
import { FormularioComponent } from './componentes/cliente_modulo/formulario/formulario.component';
import { IniciarSesionComponent } from './componentes/usuario/iniciar-sesion/iniciar-sesion.component';
import { HistorialComponent } from './componentes/usuario/historial/historial.component';
import { PerfilComponent } from './componentes/usuario/perfil/perfil.component';
import { RegistrarUsuarioComponent } from './componentes/usuario/registrar-usuario/registrar-usuario.component';
import { PantallaCargaComponent } from './componentes/pantalla-carga/pantalla-carga.component';



const routes: Routes = [
  { path: '', redirectTo: '/Iniciar-Sesion', pathMatch: 'full' },

  // Ruta principal de administración
  { path: 'admin', component: AdministradorComponent },

  // RUTAS GENERALES
  { path: 'home', component: PantallaMainComponent },
  { path: 'Objeto/:id', component: ObjetoComponent },
  { path: 'Carrito', component: CarritoComponent },
  { path: 'ConfirmarReserva', component: CarritoComponent },
  { path: 'Formulario', component: FormularioComponent },
  { path: 'Iniciar-Sesion', component: IniciarSesionComponent },
  { path: 'Historial', component: HistorialComponent },
  { path: 'Perfil', component: PerfilComponent },
  { path: 'Registrar-Usuario', component: RegistrarUsuarioComponent },
  { path: 'cargando', component: PantallaCargaComponent },



];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
