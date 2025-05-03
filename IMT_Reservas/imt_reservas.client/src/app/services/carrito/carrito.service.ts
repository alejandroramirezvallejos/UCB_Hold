import { Injectable } from '@angular/core';
import { Carrito } from '../../models/carrito'; 
@Injectable({
  providedIn: 'root'
})
export class CarritoService {
   carrito: Carrito = {};

  cantidadtotal: number = 0; 


  constructor() {}


  agregarproducto(id : number , nombre :string , link : string ) {
    if (nombre == '' || nombre == undefined) {
      return; 
    }

    if (!this.carrito[id]) {
      this.carrito[id] = { nombre, cantidad: 1 ,fecha_inicio : null,fecha_final : null , imagen : link};
      this.cantidadtotal++;
    }
    else {
      this.carrito[id].cantidad += 1;
      this.cantidadtotal++; 
    }
  }

  quitarproducto(id: number) {
    if (id in this.carrito) {
      if (this.carrito[id].cantidad > 1) {
        this.carrito[id].cantidad -= 1;
        this.cantidadtotal--;
      }
      else {
        delete this.carrito[id];
        this.cantidadtotal--; 
      }
    }
  }

  sumarproducto(id: number) {
    if (id in this.carrito) {

      this.carrito[id].cantidad++;
      this.cantidadtotal++;
    }

  }


  obtenercarrito() {
    return this.carrito; 
  }

  obtenertotal() {
    return this.cantidadtotal; 
  }

}
