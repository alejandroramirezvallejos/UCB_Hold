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


  
  iniciarsesion(usuario : Usuario ) {
    this.usuario = usuario;

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

  obtenercarnet(){
    if(this.usuario.carnet){
      return this.usuario.carnet;
    }
    else{
      return "";
    }
  }

}
