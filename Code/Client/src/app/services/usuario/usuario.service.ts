import { Injectable, signal, WritableSignal } from '@angular/core';
import { Usuario } from '../../models/usuario';

const STORED_USER_KEY = 'ucbhold_user';

@Injectable({
  providedIn: 'root'
})
export class UsuarioService {
  private readonly usuarioSignal: WritableSignal<Usuario> = signal(new Usuario());

  constructor() {
    const storedJson = localStorage.getItem(STORED_USER_KEY);
    if (storedJson) {
      try {
        this.usuarioSignal.set(JSON.parse(storedJson));
      } catch {
        localStorage.removeItem(STORED_USER_KEY);
      }
    }
  }

  iniciarsesion(usuario: Usuario) {
    localStorage.setItem(STORED_USER_KEY, JSON.stringify(usuario));
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
    localStorage.removeItem(STORED_USER_KEY);
    this.usuarioSignal.set(new Usuario());
  }


  obtenerDatosUsuario() {
    return this.usuarioSignal();
  }

  actualizarDatos(usuario: Usuario) {
    this.usuarioSignal.set(usuario);
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
