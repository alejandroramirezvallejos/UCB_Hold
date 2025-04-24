import { Component } from '@angular/core';
import { PantallaMainComponent } from './pantalla-main/pantalla-main.component';
import { NavbarComponent } from './navbar/navbar.component';
import { ObjetoComponent } from './clic_objeto/objeto.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [PantallaMainComponent, NavbarComponent, ObjetoComponent],
})
export class AppComponent {
  eleccionObjeto = '';
  eleccionNavbar = '';

  getitem(selecteditem: string) {
    this.eleccionObjeto = selecteditem;
    this.eleccionNavbar = ''; // Resetear navegación
  }

  getitemNavbar(selecteditem: string) {
    this.eleccionNavbar = selecteditem;

    if (selecteditem === 'home') {
      this.eleccionObjeto = ''; // Resetear objeto al volver a home
    }
  }
}
