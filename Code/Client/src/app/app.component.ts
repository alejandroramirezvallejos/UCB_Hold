import { Component, signal, WritableSignal } from '@angular/core';
import { Router, RouterEvent, NavigationStart, NavigationEnd, NavigationCancel, NavigationError, RouterOutlet } from '@angular/router';
import { NavbarComponent } from './componentes/navbar/navbar.component';
import { PantallaCargaComponent } from './componentes/pantallas_avisos/pantalla-carga/pantalla-carga.component';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [NavbarComponent, RouterOutlet, PantallaCargaComponent, CommonModule],
})
export class AppComponent {
  cargando: WritableSignal<boolean> = signal(false);

  constructor(private router: Router) {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationStart) {
        this.cargando.set(true);
      } else if (
        event instanceof NavigationEnd ||
        event instanceof NavigationCancel ||
        event instanceof NavigationError
      ) {
        this.cargando.set(false);
      }
    });
  }
}
