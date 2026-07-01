import { CommonModule } from '@angular/common';
import { Component, signal, WritableSignal } from '@angular/core';
import {
  NavigationCancel,
  NavigationEnd,
  NavigationError,
  NavigationStart,
  Router,
  RouterOutlet,
} from '@angular/router';
import { NavbarComponent } from '@widgets/navigation';
import { PantallaCargaComponent } from '@shared/ui';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [
    NavbarComponent,
    RouterOutlet,
    PantallaCargaComponent,
    CommonModule,
  ],
})
export class AppComponent {
  cargando: WritableSignal<boolean> = signal(false);

  constructor(private readonly router: Router) {
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
