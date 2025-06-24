import { Injectable } from '@angular/core';

import { Equipos } from '../../models/admin/Equipos';

@Injectable({
  providedIn: 'root'
})
export class MantenimientosServiceEquipos {

    equipo: Map<number, {
        TipoMantenimiento: string;
        DescripcionEquipo : string;
        nombre : string;}>;


  constructor() {
    this.equipo = new Map();
  }
 

  obtenerEquiposMantenimientos() {

    return this.equipo ;
  }

  agregarEquipoMantenimiento(codigo : number , tipo : string , descripcio : string , nombre : string ): void {

    this.equipo.set(codigo, {
      nombre: nombre,
      TipoMantenimiento: tipo,
      DescripcionEquipo: descripcio
    });

  
  }

  validarsiexitecodigo(codigo: number) {
    if(this.equipo.has(codigo)){
      return true
    }
    else{
      return false; 
    }

    }

  quitarequipo(codigo: number): void {
    this.equipo.delete(codigo);
  }

  vaciarEquiposMantenimientos(): void {
    this.equipo.clear();
  }
}







