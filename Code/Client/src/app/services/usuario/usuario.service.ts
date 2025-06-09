import { Injectable } from '@angular/core';
import { Usuario } from '../../models/usuario';
@Injectable({
  providedIn: 'root'
})
export class UsuarioService {
   usuario: Usuario;

  constructor() {
    this.usuario = new Usuario();
  }


  //TODO: MODIFICAR ESTO CUANDOE ESTE LISTO
  iniciarsesion(nombre : string , contraseña : string , admin: string) {
    this.usuario.nombre=nombre;
    this.usuario.carnet=contraseña;
    this.usuario.rol=admin;
  }

  vacio() : boolean{
    if(!this.usuario.nombre || this.usuario.nombre.trim() ==="" || this.usuario.nombre==""){
      return  true;
    }
    return false;
  }

  vaciar(){
    this.usuario = new Usuario();
  }

  // Método para depuración
  obtenerDatosUsuario() {
    return this.usuario;
  }

  obtenerrol(){
    if(this.usuario.rol=="administrador"){
      return "administrador"
    }
    else{
      return "usuario"
    }
  }

}
