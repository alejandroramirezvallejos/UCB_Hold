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


  agregarproducto(id : number , nombre :string | null , link : string , marca : string , modelo : string ,precio : number , cantidadMax : number) {
    nombre = nombre 
    if (nombre == '' || nombre == undefined) {
      return; 
    }


    const fechaLocal = new Date();
    const año = fechaLocal.getFullYear();
    const mes = (fechaLocal.getMonth() + 1).toString().padStart(2, '0');
    const día = fechaLocal.getDate().toString().padStart(2, '0');
    const fechaISO = `${año}-${mes}-${día}`;


   
    if (!this.carrito[id]) {
      this.carrito[id] = { nombre,modelo,marca ,cantidad: 1 ,fecha_inicio : fechaISO ,fecha_final : null , imagen : link , precio , cantidadMax : cantidadMax };
      this.cantidadtotal++;
    }
    else if(this.carrito[id].cantidad < this.carrito[id].cantidadMax) {
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

  vaciarcarrito(){
    this.carrito={};
    this.cantidadtotal=0; 
  }

  preciototal() : number{
    let total : number =0; 
    for(const clave in this.carrito){
      total=total +this.carrito[clave].precio *this.carrito[clave].cantidad;
    }
    return total;
  }

 editarcantidad(key: number, numero: number) {

  if(this.carrito[key].cantidad < numero) {
    this.cantidadtotal = this.cantidadtotal + (numero - this.carrito[key].cantidad);
    this.carrito[key].cantidad = numero;
  }
  else if(this.carrito[key].cantidad > numero) {
    this.cantidadtotal = this.cantidadtotal - (this.carrito[key].cantidad - numero);
    this.carrito[key].cantidad = numero;
  }
  if (this.carrito[key].cantidad <= 0) {
    delete this.carrito[key];
  }
}


  obtenerfechainicio(){
    const items = Object.values(this.carrito)
    if (items.length > 0) {
      return items[0].fecha_inicio; 
    }
    else{
      return null; 
    }

  }

    obtenerfechafinal(){
    const items = Object.values(this.carrito)
    if (items.length > 0) {
      return items[0].fecha_final; 
    }
    else{
      return null; 
    }

  }

}
