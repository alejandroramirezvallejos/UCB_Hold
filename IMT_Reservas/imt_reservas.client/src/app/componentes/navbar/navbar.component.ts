// navbar.component.ts
import { Component, Output, EventEmitter } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CarritoService } from '../../services/carrito/carrito.service';
import { CarritoPrevioComponent } from './carrito-previo/carrito-previo.component'
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule , CarritoPrevioComponent , CommonModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})

//TODO  : Eliminar lo no necesario
export class NavbarComponent {
  @Output() seleccion = new EventEmitter<string>();
  showUserMenu = false;
  showCarrito = false;
  constructor(private carrito : CarritoService) { }

  obtenershow() {
    this.showCarrito = false;
  }


  toggleUserMenu() {
    this.showUserMenu = !this.showUserMenu;
  }

  seleccionar(item: string) {
    this.seleccion.emit(item);
    this.showUserMenu = false; // Cierra el menú al seleccionar
  }

  totalproductos() : number   {
    return this.carrito.obtenertotal();
  }

  // Función para alternar la visualización del carrito
    mostrarcarrito() {
    this.showCarrito = !this.showCarrito;
  }
}
