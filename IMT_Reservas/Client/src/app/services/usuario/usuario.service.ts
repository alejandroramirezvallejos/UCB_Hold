import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UsuarioService {
    carnet? : string; 
    nombre? : string;
    apellido_materno? : string ; 
    apellido_paterno? : string;
    rol? : string; 
    carrera_Id? : number;
    correo? : string;
    telefono? : string;
    nombre_referencia? : string; 
    telefono_referencia? : string;
    email_referencia? : string;
  constructor() { 

  }


  //TODO: MODIFICAR ESTO CUANDOE ESTE LISTO 
  iniciarsesion(nombre : string , contraseña : string ) {
    this.nombre=nombre;
    this.carnet=contraseña; 

  }
  vacio() : boolean{
    if(!this.nombre || this.nombre.trim() ==="" || this.nombre==""){
      return  true;
    }
    return false;
  }

}
