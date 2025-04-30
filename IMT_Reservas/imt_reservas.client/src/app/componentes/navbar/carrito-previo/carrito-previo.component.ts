import { CommonModule } from '@angular/common';
import { Component, Input ,Output , EventEmitter} from '@angular/core';
import { CarritoService } from '../../../services/carrito/carrito.service';
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
  carritoItems: { [key: string]: CarritoItem } = {
    '1': { name: 'Producto 1', price: 100, quantity: 2 },
    '2': { name: 'Producto 2', price: 200, quantity: 1 }
  };

  // Alterna la visibilidad del carrito (por ejemplo, al pulsar la "X")
  toggleCarrito() {
    this.showCarritoevent.emit(!this.showCarrito);
  }

  // Incrementa la cantidad de un producto
  increaseQuantity(itemKey: string) {
    if (this.carritoItems[itemKey]) {
      this.carritoItems[itemKey].quantity++;
    }
  }

  // Decrementa la cantidad; si llega a 1, se elimina el producto (opcional)
  decreaseQuantity(itemKey: string) {
    if (this.carritoItems[itemKey]) {
      if (this.carritoItems[itemKey].quantity > 1) {
        this.carritoItems[itemKey].quantity--;
      } else {
        delete this.carritoItems[itemKey];
      }
    }
  }

  // Lógica para confirmar la reserva
  confirmReserva() {
    console.log('¡Reserva confirmada!');
    // Aquí puedes agregar la lógica para proceder con la reserva, por ejemplo,
    // llamar a un servicio o redirigir a otra ruta.
  }
}
