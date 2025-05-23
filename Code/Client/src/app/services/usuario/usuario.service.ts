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

  prueba(){
    this.nombre= "josue Balbontin ";
    this.carnet ="11111";

  }

  iniciarsesion() {


  }


}
