import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CarritoService {
  id_productos: number = 0;
  nombre: string = '';




  constructor(id : number , name : string) {
    this.id_productos = id;
    this.nombre = name;

  }





}
