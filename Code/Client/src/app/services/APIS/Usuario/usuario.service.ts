import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Usuario } from '../../../models/usuario';
import { map } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class UsuarioServiceAPI {
 private apiUrl = environment.apiUrl + '/api/Usuario';
  constructor(private http : HttpClient) { }
  registrarCuenta(usuario : Usuario , contrasena : string , rol : string  | null){
    if(!rol) {
      rol = 'Estudiante';
    }
    const envio={
    Carnet: usuario.carnet,
    Nombre: usuario.nombre,
    ApellidoPaterno: usuario.apellido_paterno,
    ApellidoMaterno: usuario.apellido_materno,
    Rol: rol,
    Email: usuario.correo,
    Contrasena: contrasena,
    CarreraNombre: usuario.carrera,
    IdCarrera: 0,
    Telefono: usuario.telefono,
    TelefonoReferencia: usuario.telefono_referencia,
    NombreReferencia: usuario.nombre_referencia,
    EmailReferencia: usuario.email_referencia
    }
    return this.http.post(this.apiUrl, envio)
  }
  iniciarsesion(correo: string, contraseña: string) {
    const api = `${this.apiUrl}/login`;
    const body = { Email: correo, Contrasena: contraseña };
    return this.http.post<any>(api, body).pipe(
      map(response=>({
        id : response.Value.Carnet,
        carnet: response.Value.Carnet,
        nombre: response.Value.Nombre,
        apellido_materno: response.Value.ApellidoMaterno,
        apellido_paterno: response.Value.ApellidoPaterno,
        rol: response.Value.Rol,
        carrera: response.Value.CarreraNombre,
        correo: response.Value.Email,
        telefono: response.Value.Telefono,
        nombre_referencia: response.Value.NombreReferencia,
        telefono_referencia: response.Value.TelefonoReferencia,
        email_referencia: response.Value.EmailReferencia
      })));
  }
  actualizarUsuario(usuario: Usuario) {
    const envio = {
      Carnet: usuario.carnet,
      Nombre: usuario.nombre,
      ApellidoPaterno: usuario.apellido_paterno,
      ApellidoMaterno: usuario.apellido_materno,
      Email: usuario.correo,
      CarreraNombre: usuario.carrera,
      IdCarrera: usuario.carrera_Id || 0,
      Telefono: usuario.telefono,
      TelefonoReferencia: usuario.telefono_referencia,
      NombreReferencia: usuario.nombre_referencia,
      EmailReferencia: usuario.email_referencia,
    };
    return this.http.put<Usuario>(`${this.apiUrl}`, envio);
  }
  obtenerUsuarios() {
    return this.http.get<any>(this.apiUrl).pipe(
      map(response => response.Value.map((item: any) => ({
        id: item.Carnet,
        carnet: item.Carnet,
        nombre: item.Nombre,
        apellido_materno: item.ApellidoMaterno,
        apellido_paterno: item.ApellidoPaterno,
        rol: item.Rol,
        correo: item.Email,
        telefono: item.Telefono,
        telefono_referencia: item.TelefonoReferencia,
        nombre_referencia: item.NombreReferencia,
        email_referencia: item.EmailReferencia,
        carrera_Id: item.IdCarrera,
        carrera: item.CarreraNombre,
      })))
    );
  }
  editarUsuario(usuario: Usuario , entrada: string) {
    let contrasena ;
    if(entrada === '') {
      contrasena = null;
    }
    else{
      contrasena = entrada;
    }
    const envio = {
      Carnet: usuario.carnet,
      Nombre: usuario.nombre,
      ApellidoPaterno: usuario.apellido_paterno,
      ApellidoMaterno: usuario.apellido_materno,
      Email: usuario.correo,
      Contrasena: contrasena,
      Rol: usuario.rol,
      CarreraNombre: usuario.carrera,
      IdCarrera: usuario.carrera_Id || 0,
      Telefono: usuario.telefono,
      TelefonoReferencia: usuario.telefono_referencia,
      NombreReferencia: usuario.nombre_referencia,
      EmailReferencia: usuario.email_referencia
    };
    return this.http.put<Usuario>(`${this.apiUrl}`, envio);
  }
  eliminarUsuario(id: string) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
