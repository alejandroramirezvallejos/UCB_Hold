// navbar.component.ts
import { Component, Output, EventEmitter, signal, WritableSignal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CarritoService } from '../../services/carrito/carrito.service';
import { CarritoPrevioComponent } from './carrito-previo/carrito-previo.component';
import { CommonModule } from '@angular/common';
import { BuscadorService } from '../../services/buscador/buscador.service';
import { Router } from '@angular/router';
import { UsuarioPrevioComponent } from './usuario-previo/usuario-previo.component';
@Component({
  selector: 'app-navbar',
  standalone: true,
<<<<<<< HEAD
  imports: [RouterModule, CarritoPrevioComponent, CommonModule],
=======
  imports: [RouterModule , CarritoPrevioComponent , CommonModule, UsuarioPrevioComponent],
>>>>>>> 74ac4c51b29fde7745201bbfc1d86401ca8e36bf
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})


export class NavbarComponent {
<<<<<<< HEAD
  @Output() seleccion = new EventEmitter<string>();
  showUserMenu = false;
  showCarrito = false;
  constructor(
    private carrito: CarritoService,
    private buscador: BuscadorService,
    private router: Router
  ) {}
=======
 
  showUserMenu : WritableSignal<boolean> = signal(false);
  showCarrito: WritableSignal<boolean> = signal(false);
  constructor(private carrito : CarritoService, private buscador : BuscadorService , private router : Router) { }

>>>>>>> 74ac4c51b29fde7745201bbfc1d86401ca8e36bf

  botonhome() {
    this.buscador.reiniciar();
    this.router.navigate(['/home']);
  }


  toggleUserMenu() {
    this.showUserMenu.set(!this.showUserMenu());
  }



  totalproductos(): number {
    return this.carrito.obtenertotal();
  }

<<<<<<< HEAD
  // Función para alternar la visualización del carrito
  mostrarcarrito() {
    this.showCarrito = !this.showCarrito;
=======
  mostrarcarrito() {
    this.showCarrito.set(!this.showCarrito());
>>>>>>> 74ac4c51b29fde7745201bbfc1d86401ca8e36bf
  }
}
