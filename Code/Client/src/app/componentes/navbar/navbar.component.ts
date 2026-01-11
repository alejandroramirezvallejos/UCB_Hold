// navbar.component.ts
import { Component, signal, WritableSignal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CarritoService } from '../../services/carrito/carrito.service';
import { CommonModule, Location } from '@angular/common';

import { Router, NavigationEnd } from '@angular/router';
import { UsuarioPrevioComponent } from './usuario-previo/usuario-previo.component';
import { UsuarioService } from '../../services/usuario/usuario.service';
import { filter } from 'rxjs';
import { SidebarService } from '../../services/sidebar.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule  , CommonModule, UsuarioPrevioComponent],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})


export class NavbarComponent {

  showUserMenu : WritableSignal<boolean> = signal(false);
  showAdminSidebarToggle: WritableSignal<boolean> = signal(false);

  showHome: WritableSignal<boolean> = signal(true);
  showBack: WritableSignal<boolean> = signal(true);
  showCart: WritableSignal<boolean> = signal(true);
  showProfile: WritableSignal<boolean> = signal(true);

  constructor(private carrito : CarritoService , private router : Router , private usuario : UsuarioService, private location: Location, private sidebarService: SidebarService) {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      this.updateButtonVisibility(event.urlAfterRedirects || event.url);
    });
  }

  private updateButtonVisibility(url: string) {
    // Normalize URL
    const cleanUrl = url.split('?')[0];

    if (cleanUrl.includes('/Iniciar-Sesion') || cleanUrl.includes('/Registrar-Usuario')) {
      // Login / Register: Hide All
      this.showHome.set(false);
      this.showBack.set(false);
      this.showCart.set(false);
      this.showProfile.set(false);
    } else if (cleanUrl.includes('/home')) {
      // Home Page: No Home, No Back
      this.showHome.set(false);
      this.showBack.set(false);
      this.showCart.set(true);
      this.showProfile.set(true);
      // Rastrear que estamos en modo usuario
      this.previousAdminRoute = false;
    } else if (cleanUrl.includes('/Carrito') || cleanUrl.includes('/Objeto') || cleanUrl.includes('/Formulario')) {
      // Cart / Object / Form: Show Back, Hide Home
      this.showHome.set(false);
      this.showBack.set(true);
      this.showCart.set(true);
      this.showProfile.set(true);
    } else if (cleanUrl.includes('/Perfil') || cleanUrl.includes('/Historial')) {
      // Profile: Show Home, Hide Back
      this.showHome.set(true);
      this.showBack.set(false);
      this.showCart.set(true);
      this.showProfile.set(true);
    } else {
      // Default: Show All
      this.showHome.set(true);
      this.showBack.set(true);
      this.showCart.set(true);
      this.showProfile.set(true);
    }

    // Logic for Admin role
    if (this.usuario.obtenerrol() === 'administrador') {
        // Allow home button for admin too, but keep back button hidden usually?
        // The user only asked for Home button behavior.
        // Actually the code previously hid home for admin.
        // "en perfil deberia aparecer en el navbar el icono de home"
        // If I simply remove the line "this.showHome.set(false);" from the admin block, it will respect the previous logic (set to true for profile).

       this.showBack.set(false);
       this.showCart.set(false);
      // Logic: If on admin route, show sidebar toggle
      if (cleanUrl.includes('/admin')) {
         this.showHome.set(false); // Hide home on admin dashboard itself
         this.showAdminSidebarToggle.set(true);
         // Rastrear que estamos en modo admin
         this.previousAdminRoute = true;
      } else {
         // e.g. Profile
         this.showAdminSidebarToggle.set(false);
         // Ensure home is shown if not in admin route explicitly (e.g. profile)
         // The previous logic already set showHome.
      }
    } else {
      this.showAdminSidebarToggle.set(false);
    }
  }

  private previousAdminRoute: boolean = false;

  botonhome() {
    if (this.usuario.vacio()) {
      this.router.navigate(['/Iniciar-Sesion']);
    } else {
        // Navegar según el contexto previo (si estaba en modo admin o modo usuario)
        if (this.previousAdminRoute) {
            this.router.navigate(['/admin']);
        } else {
            this.router.navigate(['/home']);
        }
    }

  }



  toggleUserMenu() {
      this.showUserMenu.set(!this.showUserMenu());
  }

  toggleSidebar() {
    this.sidebarService.toggle();
  }

  totalproductos(): number {
    return this.carrito.obtenertotal();
  }

  mostrarcarrito() {
    if(this.usuario.vacio()){
      this.router.navigate(['/Iniciar-Sesion']);
    }
    else{
      this.router.navigate(['/Carrito']);
    }

  }

  goBack() {
    this.location.back();
  }

}
