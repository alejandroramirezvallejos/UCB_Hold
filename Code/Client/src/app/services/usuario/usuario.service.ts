import { Injectable, signal, WritableSignal } from '@angular/core';
import { Usuario } from '../../models/usuario';
@Injectable({
  providedIn: 'root'
})
export class UsuarioService {
  private usuarioSignal: WritableSignal<Usuario> = signal(new Usuario());

  constructor() {
  }


  iniciarsesion(usuario: Usuario) {
    this.usuarioSignal.set(usuario);
  }
  

  vacio(): boolean {
    const u = this.usuarioSignal();
    if (!u.nombre || u.nombre.trim() === "" || u.nombre === "") {
      return true;
    }
    return false;
  }

  vaciar() {
    this.usuarioSignal.set(new Usuario());
  }


  obtenerDatosUsuario() {
    return this.usuarioSignal();
  }
  

  obtenerrol() {
    const u = this.usuarioSignal();
    if (u.rol == "administrador") {
      return "administrador";
    } else {
      return "usuario";
    }
  }


  obtenercarnet() {
    const u = this.usuarioSignal();
    if (u.carnet) {
      return u.carnet;
    } else {
      return "";
    }
  }


}
