import { CommonModule } from '@angular/common';
import { Component, Output , EventEmitter, Input, ɵunwrapWritableSignal, WritableSignal, signal} from '@angular/core';
import { CarritoService } from '../../../services/carrito/carrito.service';
import {Carrito } from '../../../models/carrito'
import { Router } from '@angular/router';
import { UsuarioService } from '../../../services/usuario/usuario.service';


@Component({
  selector: 'app-carrito-previo',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './carrito-previo.component.html',
  styleUrls: ['./carrito-previo.component.css']
})
export class CarritoPrevioComponent {

  @Input() showCarritoevent  : WritableSignal<boolean> = signal(true) ;
  carritoItems: Carrito = {};

  constructor(private serviciocarrito: CarritoService , private router : Router , private usuario : UsuarioService) {

    this.carritoItems = serviciocarrito.obtenercarrito();
  };


  increaseQuantity(itemKey: string) {
    this.serviciocarrito.sumarproducto(Number(itemKey));
  }

  decreaseQuantity(itemKey: string) {
    if (this.carritoItems[Number(itemKey)]) {
      this.serviciocarrito.quitarproducto(Number(itemKey));
    }
  }

  confirmReserva() {
    if (this.carritoItems == null || Object.keys(this.carritoItems).length === 0) {

    }
    else if (this.usuario.vacio()){
      this.router.navigate(['/Iniciar-Sesion']);
    }
    else {
      this.showCarritoevent.set(false);
      this.router.navigate(['/ConfirmarReserva']);
    }

  }
}
