import { CommonModule } from '@angular/common';
import { Component, Input ,Output , EventEmitter} from '@angular/core';
import { CarritoService } from '../../../services/carrito/carrito.service';
import {Carrito } from '../../../models/carrito'
interface CarritoItem {
  name: string;
  price: number;
  quantity: number;
}

@Component({
  selector: 'app-carrito-previo',
  standalone: true,
  imports: [CommonModule  ],
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


  constructor(private serviciocarrito: CarritoService) {

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
    console.log('¡Reserva confirmada!');
    // Aquí puedes agregar la lógica para proceder con la reserva, por ejemplo,
    // llamar a un servicio o redirigir a otra ruta.
  }
}
