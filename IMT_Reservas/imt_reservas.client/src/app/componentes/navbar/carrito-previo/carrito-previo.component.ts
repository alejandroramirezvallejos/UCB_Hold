import { CommonModule } from '@angular/common';
import { Component, Output , EventEmitter} from '@angular/core';
import { CarritoService } from '../../../services/carrito/carrito.service';
import {Carrito } from '../../../models/carrito'
import { Router } from '@angular/router';
interface CarritoItem {
  name: string;
  price: number;
  quantity: number;
}

@Component({
  selector: 'app-carrito-previo',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './carrito-previo.component.html',
  styleUrls: ['./carrito-previo.component.css']
})
export class CarritoPrevioComponent {
  // Para probar, inicializamos en true.  
  // En producción deberás controlar su valor desde un componente padre o servicio.
  showCarrito: boolean = true;
  @Output() showCarritoevent = new EventEmitter<boolean>();
  // Diccionario de productos (ejemplo)
  carritoItems: Carrito = {};


  constructor(private serviciocarrito: CarritoService , private router : Router) {

    this.carritoItems = serviciocarrito.obtenercarrito();
  };




  // Alterna la visibilidad del carrito (por ejemplo, al pulsar la "X")
  toggleCarrito() {
    this.showCarritoevent.emit(!this.showCarrito);
  }

  // Incrementa la cantidad de un producto
  increaseQuantity(itemKey: string) {
    this.serviciocarrito.sumarproducto(Number(itemKey));
  }

  // Decrementa la cantidad; si llega a 1, se elimina el producto (opcional)
  decreaseQuantity(itemKey: string) {
    if (this.carritoItems[Number(itemKey)]) {
      this.serviciocarrito.quitarproducto(Number(itemKey));
    }
  }

  // Lógica para confirmar la reserva
  confirmReserva() {
    if (this.carritoItems == null || Object.keys(this.carritoItems).length === 0) {

    }
    else {
      this.toggleCarrito();
      this.router.navigate(['/ConfirmarReserva']);
    }

  }
}
