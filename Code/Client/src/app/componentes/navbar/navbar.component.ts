import { Component, signal, WritableSignal, effect } from '@angular/core';
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

  private previousAdminRoute: boolean = false;

  constructor(
    private carrito: CarritoService, 
    private router: Router, 
    private usuario: UsuarioService, 
    private location: Location, 
    private sidebarService: SidebarService
  ) {
    // 1. Ejecutar al iniciar el componente
    this.updateButtonVisibility(this.router.url);

    // 2. Suscribirse SOLO a los cambios de navegación
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      // El evento ya trae la URL final, no necesitamos TIMEOUT
      this.updateButtonVisibility(event.urlAfterRedirects || event.url);
    });

    // 3. Reaccionar a cambios en el usuario (inicio/cierre sesión)
    effect(() => {
      // Esta lectura registra la dependencia de la señal del usuario
      this.usuario.obtenerrol(); 
      // Actualizamos la visibilidad de botones
      this.updateButtonVisibility(this.router.url);
    });
  }
  
  private updateButtonVisibility(url: string) {
    const cleanUrl = url.split('?')[0];
    const rol = this.usuario.obtenerrol(); 
    const isLoginOrRegister = cleanUrl.includes('/Iniciar-Sesion') || cleanUrl.includes('/Registrar-Usuario');
    
    // Lógica más limpia y directa
    if (isLoginOrRegister) {
      this.showHome.set(false);
      this.showBack.set(false);
      this.showCart.set(false);
      this.showProfile.set(false);
      this.showAdminSidebarToggle.set(false); // Asegurar que no se muestre en login
      return; // Salir temprano
    }

    const isAdmin = rol === 'administrador';
    const isInAdminMode = isAdmin && cleanUrl.includes('/admin');

    // Configuración Base
    this.showProfile.set(true);
    
    // Lógica del Carrito
    this.showCart.set(!isInAdminMode);

    // Lógica de Botones de Navegación
    if (isInAdminMode) {
       this.showBack.set(false);
       this.showHome.set(false);
       this.showAdminSidebarToggle.set(true);
       this.previousAdminRoute = true;
    } else {
       this.showAdminSidebarToggle.set(false);
       
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
            // Default para otras páginas de usuario estandar
            this.showHome.set(true);
            this.showBack.set(true);
        }
    }
  }


  
  botonhome() {
    if (this.usuario.vacio()) {
      this.router.navigate(['/Iniciar-Sesion']);
    } else {
      this.router.navigate(['/home']);
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
