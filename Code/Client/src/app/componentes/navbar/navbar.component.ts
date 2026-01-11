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
    const cleanUrl = url.split('?')[0];
    if (cleanUrl.includes('/Iniciar-Sesion') || cleanUrl.includes('/Registrar-Usuario')) {
      this.showHome.set(false);
      this.showBack.set(false);
      this.showCart.set(false);
      this.showProfile.set(false);
    } else if (cleanUrl.includes('/home')) {
      this.showHome.set(false);
      this.showBack.set(false);
      this.showCart.set(true);
      this.showProfile.set(true);
      this.previousAdminRoute = false;
    } else if (cleanUrl.includes('/Carrito') || cleanUrl.includes('/Objeto') || cleanUrl.includes('/Formulario')) {
      this.showHome.set(false);
      this.showBack.set(true);
      this.showCart.set(true);
      this.showProfile.set(true);
    } else if (cleanUrl.includes('/Perfil') || cleanUrl.includes('/Historial')) {
      this.showHome.set(true);
      this.showBack.set(false);
      this.showCart.set(true);
      this.showProfile.set(true);
    } else {
      this.showHome.set(true);
      this.showBack.set(true);
      this.showCart.set(true);
      this.showProfile.set(true);
    }
    if (this.usuario.obtenerrol() === 'administrador') {
       this.showBack.set(false);
       this.showCart.set(false);
      if (cleanUrl.includes('/admin')) {
         this.showHome.set(false); // Hide home on admin dashboard itself
         this.showAdminSidebarToggle.set(true);
         this.previousAdminRoute = true;
      } else {
         this.showAdminSidebarToggle.set(false);
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
