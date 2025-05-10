import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PantallaMainComponent } from './componentes/cliente_modulo/pantalla-main/pantalla-main.component'; 
import { ObjetoComponent } from './componentes/cliente_modulo/clic_objeto/objeto.component';
import { CarritoComponent } from './componentes/cliente_modulo/carrito/carrito.component';
import { AdministradorComponent } from './componentes/admin_modulo/administrador/administrador.component';


const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: PantallaMainComponent },
  { path: 'Objeto/:id', component: ObjetoComponent },
  { path: 'ConfirmarReserva', component: CarritoComponent },
  { path: 'Admin', component: AdministradorComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
