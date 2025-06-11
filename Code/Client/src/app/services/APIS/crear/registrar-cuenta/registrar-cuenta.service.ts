import { Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Usuario } from '../../../../models/usuario';

@Injectable({
  providedIn: 'root'
})
export class RegistrarCuentaService {
  private apiUrl = environment.apiUrl + '/api/Usuario'; 

  constructor(private http : HttpClient) { }

  registrarCuenta(usuario : Usuario , contrasena : string){
    const envio={
    carnet: usuario.carnet,
    nombre: usuario.nombre,
    apellidoPaterno: usuario.apellido_paterno,
    apellidoMaterno: usuario.apellido_materno,
    email: usuario.correo,
    contrasena: contrasena,
    nombreCarrera: usuario.carrera,
    telefono: usuario.telefono,
    telefonoReferencia: usuario.telefono_referencia,
    nombreReferencia: usuario.nombre_referencia,
    emailReferencia: usuario.email_referencia
    } 

    return this.http.post(this.apiUrl, envio)

  }



}
