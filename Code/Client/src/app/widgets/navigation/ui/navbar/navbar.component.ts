import {
  Component,
  signal,
  WritableSignal,
  effect,
  HostListener,
  ElementRef,
} from '@angular/core';
import { RouterModule } from '@angular/router';
import { CarritoService } from '@features/cart';
import { CommonModule, Location } from '@angular/common';
import { Router, NavigationEnd } from '@angular/router';
import { UsuarioPrevioComponent } from './usuario-previo/usuario-previo.component';
import { UsuarioService } from '@entities/user';
import { filter } from 'rxjs';
import { SidebarService } from '@widgets/admin-sidebar';
@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, CommonModule, UsuarioPrevioComponent],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})
export class NavbarComponent {
  showUserMenu: WritableSignal<boolean> = signal(false);
  showAdminSidebarToggle: WritableSignal<boolean> = signal(false);
  showHome: WritableSignal<boolean> = signal(true);
  showBack: WritableSignal<boolean> = signal(true);
  showCart: WritableSignal<boolean> = signal(true);
  showProfile: WritableSignal<boolean> = signal(true);
  private readonly currentRole: string = '';

  private previousAdminRoute: boolean = false;

  constructor(
    private readonly carrito: CarritoService,
    private readonly router: Router,
    private readonly usuario: UsuarioService,
    private readonly location: Location,
    private readonly sidebarService: SidebarService,
    private readonly el: ElementRef,
  ) {
    this.updateButtonVisibility(this.router.url);

    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.updateButtonVisibility(event.urlAfterRedirects || event.url);
      });

    effect(() => {
      this.usuario.obtenerRol();

      this.updateButtonVisibility(this.router.url);
    });
  }

  private updateButtonVisibility(url: string) {
    const cleanUrl = url.split('?')[0];
    const rol = this.usuario.obtenerRol();
    const isLoginOrRegister =
      cleanUrl.includes('/Iniciar-Sesion') ||
      cleanUrl.includes('/Registrar-Usuario');

    if (isLoginOrRegister) {
      this.showHome.set(false);
      this.showBack.set(false);
      this.showCart.set(false);
      this.showProfile.set(false);
      this.showAdminSidebarToggle.set(false);
      return;
    }

    const isAdmin = rol === 'administrador';
    const isInAdminMode = isAdmin && cleanUrl.includes('/admin');

    this.showProfile.set(true);

    this.showCart.set(!isInAdminMode);

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
      } else if (
        cleanUrl.includes('/Carrito') ||
        cleanUrl.includes('/Objeto') ||
        cleanUrl.includes('/Formulario')
      ) {
        this.showHome.set(false);
        this.showBack.set(true);
      } else if (
        cleanUrl.includes('/Perfil') ||
        cleanUrl.includes('/Historial')
      ) {
        this.showHome.set(true);
        this.showBack.set(false);
      } else {
        this.showHome.set(true);
        this.showBack.set(true);
      }
    }
  }

  botonhome() {
    if (this.usuario.estaVacio()) {
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
    return this.carrito.obtenerTotal();
  }

  mostrarcarrito() {
    if (this.usuario.estaVacio()) {
      this.router.navigate(['/Iniciar-Sesion']);
    } else {
      this.router.navigate(['/Carrito']);
    }
  }

  goBack() {
    this.location.back();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (this.showUserMenu() && !this.el.nativeElement.contains(event.target)) {
      this.showUserMenu.set(false);
    }
    if (
      this.sidebarService.isOpen() &&
      !this.el.nativeElement.contains(event.target)
    ) {
      this.sidebarService.close();
    }
  }
}
