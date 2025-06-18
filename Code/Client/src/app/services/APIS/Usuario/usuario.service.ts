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

  registrarCuenta(usuario : Usuario , contrasena : string){
    const envio={
    Carnet: usuario.carnet,
    Nombre: usuario.nombre,
    ApellidoPaterno: usuario.apellido_paterno,
    ApellidoMaterno: usuario.apellido_materno,
    Email: usuario.correo,
    Contrasena: contrasena,
    NombreCarrera: usuario.carrera,
    Telefono: usuario.telefono,
    TelefonoReferencia: usuario.telefono_referencia,
    NombreReferencia: usuario.nombre_referencia,
    EmailReferencia: usuario.email_referencia
    } 

    return this.http.post(this.apiUrl, envio)

  }


  iniciarsesion(correo: string, contraseña: string) {
    const api =  `${this.apiUrl + '/iniciarSesion'}?email=${correo}&contrasena=${contraseña}`;
    return this.http.get<any>(api).pipe(
      map(item=>({
        id : item.Carnet,
        carnet: item.Carnet,
        nombre: item.Nombre,
        apellido_materno: item.ApellidoMaterno,
        apellido_paterno: item.ApellidoPaterno,
        rol: item.Rol,
        carrera: item.CarreraNombre,
        correo: item.Email,
        telefono: item.Telefono,
        nombre_referencia: item.NombreReferencia,
        telefono_referencia: item.TelefonoReferencia,
        email_referencia: item.EmailReferencia
      })));
  }

  actualizarUsuario(usuario: Usuario) {
    const envio = {
      Carnet: usuario.carnet,
      Nombre: usuario.nombre,
      ApellidoPaterno: usuario.apellido_paterno,
      ApellidoMaterno: usuario.apellido_materno,
      Email: usuario.correo,
      NombreCarrera: usuario.carrera,
      Telefono: usuario.telefono,
      TelefonoReferencia: usuario.telefono_referencia,
      NombreReferencia: usuario.nombre_referencia,
      EmailReferencia: usuario.email_referencia,
    };

    return this.http.put<Usuario>(`${this.apiUrl}/${usuario.id}`, envio);

  }


  obtenerUsuarios() {
    return this.http.get<any[]>(this.apiUrl).pipe(
      map(data => data.map(item => ({
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
        carrera_Id: item.CarreraId,
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
      NombreCarrera: usuario.carrera,
      Telefono: usuario.telefono,
      TelefonoReferencia: usuario.telefono_referencia,
      NombreReferencia: usuario.nombre_referencia,
      EmailReferencia: usuario.email_referencia
    };

    return this.http.put<Usuario>(`${this.apiUrl}/${usuario.id}`, envio);
  }

  eliminarUsuario(id: string) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  
    


 
}
