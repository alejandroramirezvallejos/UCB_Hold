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



}
