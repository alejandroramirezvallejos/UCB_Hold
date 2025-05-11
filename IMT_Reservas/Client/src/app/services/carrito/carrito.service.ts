import { Injectable } from '@angular/core';
import { Carrito } from '../../models/carrito'; 
@Injectable({
  providedIn: 'root'
})
export class CarritoService {
   carrito: Carrito = {

   };

  cantidadtotal: number = 0; 


  constructor() {}


  agregarproducto(id : number , nombre :string , link : string , marca : string , modelo : string ,precio : number) {
    nombre = nombre 
    if (nombre == '' || nombre == undefined) {
      return; 
    }


    const fechaLocal = new Date();
    const año = fechaLocal.getFullYear();
    const mes = (fechaLocal.getMonth() + 1).toString().padStart(2, '0');
    const día = fechaLocal.getDate().toString().padStart(2, '0');
    const fechaISO = `${año}-${mes}-${día}`;


    // cambiar precio
    if (!this.carrito[id]) {
      this.carrito[id] = { nombre,modelo,marca ,cantidad: 1 ,fecha_inicio : fechaISO ,fecha_final : null , imagen : link , precio};
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



  preciototal() : number{
    let total : number =0; 
    for(const clave in this.carrito){
      total=total +this.carrito[clave].precio *this.carrito[clave].cantidad;
    }
    return total;
  }


}
