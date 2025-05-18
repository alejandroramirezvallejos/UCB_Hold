import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UsuarioService {
    id? : number;
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
  iniciarsesion(nombre : string , contraseña : string , admin: string) {
    this.nombre=nombre;
    this.carnet=contraseña;
    this.rol=admin;
  }

  vacio() : boolean{
    if(!this.nombre || this.nombre.trim() ==="" || this.nombre==""){
      return  true;
    }
    return false;
  }

  vaciar(){
    this.id = undefined;
    this.carnet = "";
    this.nombre = "";
    this.apellido_materno = "";
    this.apellido_paterno = "";
    this.rol = "";
    this.carrera_Id = undefined; // Para números, puedes usar null o undefined según tu lógica
    this.correo = "";
    this.telefono = "";
    this.nombre_referencia = "";
    this.telefono_referencia = "";
    this.email_referencia = "";
  }

  // Método para depuración
  obtenerDatosUsuario() {
    return {
      id: this.id,
      carnet: this.carnet,
      nombre: this.nombre,
      apellido_paterno: this.apellido_paterno,
      apellido_materno: this.apellido_materno,
      rol: this.rol,
      carrera_Id: this.carrera_Id,
      correo: this.correo,
      telefono: this.telefono,
      nombre_referencia: this.nombre_referencia,
      telefono_referencia: this.telefono_referencia,
      email_referencia: this.email_referencia
    };
  }

  obtenerrol(){
    if(this.rol=="administrador"){
      return "administrador"
    }
    else{
      return "usuario"
    }
  }

}
