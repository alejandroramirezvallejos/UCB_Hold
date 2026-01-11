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
  private currentRole: string = '';

  constructor(private carrito : CarritoService , private router : Router , private usuario : UsuarioService, private location: Location, private sidebarService: SidebarService) {
    // Llamar inmediatamente para establecer estado inicial correcto
    this.currentRole = this.usuario.obtenerrol();
    this.updateButtonVisibility(this.router.url);

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      setTimeout(() => {
        this.currentRole = this.usuario.obtenerrol();
        this.updateButtonVisibility(event.urlAfterRedirects || event.url);
      }, 50);
    });

    // Verificación periódica para asegurar visibilidad correcta del carrito
    setInterval(() => {
      const currentUrl = this.router.url;
      const newRole = this.usuario.obtenerrol();
      const isInLoginRegister = currentUrl.includes('/Iniciar-Sesion') || currentUrl.includes('/Registrar-Usuario');
      const isInAdminMode = newRole === 'administrador' && currentUrl.includes('/admin');

      // Actualizar visibilidad si cambió el rol
      if (this.currentRole !== newRole) {
        this.currentRole = newRole;
        this.updateButtonVisibility(currentUrl);
      }

      // Asegurar que el carrito esté visible cuando corresponde:
      // - Siempre visible excepto en modo admin (/admin) o en login/registro
      if (!isInAdminMode && !isInLoginRegister) {
        if (!this.showCart()) {
          this.showCart.set(true);
        }
      }
    }, 300);
  }
  private updateButtonVisibility(url: string) {
    const cleanUrl = url.split('?')[0];
    const rol = this.usuario.obtenerrol();
    const isAdmin = rol === 'administrador';
    // Un admin en /admin está en modo admin, si está en otras rutas está en modo usuario
    const isInAdminMode = isAdmin && cleanUrl.includes('/admin');

    if (cleanUrl.includes('/Iniciar-Sesion') || cleanUrl.includes('/Registrar-Usuario')) {
      this.showHome.set(false);
      this.showBack.set(false);
      this.showCart.set(false);
      this.showProfile.set(false);
    } else {
        // Mostrar carrito: solo ocultar si está en modo admin (/admin),
        // si es admin pero está en /home, mostrar el carrito
        this.showCart.set(!isInAdminMode);
        this.showProfile.set(true);

        if (cleanUrl.includes('/home')) {
            this.showHome.set(false);
            this.showBack.set(false);
            this.previousAdminRoute = false;
        } else if (cleanUrl.includes('/Carrito') || cleanUrl.includes('/Objeto') || cleanUrl.includes('/Formulario')) {
            this.showHome.set(false);
            this.showBack.set(true);
        } else if (cleanUrl.includes('/Perfil') || cleanUrl.includes('/Historial')) {
            this.showHome.set(true);
            this.showBack.set(false);
        } else {
            // Default
            this.showHome.set(true);
            this.showBack.set(true);
        }
    }

    if (isInAdminMode) {
       this.showBack.set(false);
       this.showHome.set(false);
       this.showAdminSidebarToggle.set(true);
       this.previousAdminRoute = true;
    } else {
      this.showAdminSidebarToggle.set(false);
    }
  }
  private previousAdminRoute: boolean = false;
  botonhome() {
    if (this.usuario.vacio()) {
      this.router.navigate(['/Iniciar-Sesion']);
    } else {
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
