import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PantallaMainComponent } from './pantalla-main/pantalla-main.component'; 
import { ObjetoComponent } from './clic_objeto/objeto.component';


const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: PantallaMainComponent },
  { path: 'Objeto/:nombre', component: ObjetoComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
